/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using _Game.Scripts.Cards;
using _Game.Scripts.Extensions;

namespace _Game.Scripts.UI
{
    public class HandDisplay : MonoBehaviour
    {
        #region Properties
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private BoolValue _drawIsBlockedFlag = null;
        [SerializeField] private HandDisplayCardPopUp _cardInspectWindowHandler = null;
        [SerializeField] private RectTransform _handTransform = null;
        [SerializeField] private Transform _centerCardSpotTransform = null;
        [SerializeField] private Button _drawButton = null;
        [SerializeField] private TMPro.TMP_Text _cooldownText = null;
        [SerializeField] private Button _inspectGraveButton = null;
        [SerializeField] private Canvas _rootCanvas = null;
        private Dictionary<RectTransform, int> _displayedCards = new Dictionary<RectTransform, int>();
        private GameManager _gameManager;
        private int _drawCooldown;
        private bool _cooldownCoroutineRunning = false;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_cardObjectPrefab is null || _drawIsBlockedFlag is null || _cardInspectWindowHandler is null || _handTransform is null || _centerCardSpotTransform is null || _drawButton is null || _cooldownText is null || _inspectGraveButton is null || _rootCanvas is null)
                Debug.LogWarning("HandDisplay.cs is missing Object References.");
            #endif
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _drawCooldown = _gameManager.GameSettingsRef.DrawCooldownInSec;
            CatchHandSizeChange(0);
        }

        private void OnEnable()
        {
            _drawIsBlockedFlag.ValueChangedEvent += CatchDrawBlock;
            GameManager.HandsizeChangedEvent += Populate;
            _drawButton?.onClick.AddListener(delegate {DrawCard();});
            _inspectGraveButton?.onClick.AddListener(delegate {InspectGrave();});
        }

        private void OnDisable()
        {
            _drawIsBlockedFlag.ValueChangedEvent -= CatchDrawBlock;
            GameManager.HandsizeChangedEvent -= Populate;
            _drawButton?.onClick.RemoveListener(delegate {DrawCard();});
            _inspectGraveButton?.onClick.RemoveListener(delegate {InspectGrave();});

            if(_cooldownCoroutineRunning)
            {
                StopCoroutine(DrawCooldownTextCoroutine());
                _cooldownText.text = "";
            }
        }
        #endregion
        
        #region Methods
        private void CatchHandSizeChange(int newVal)
        {
            if(_gameManager is null) return;
            Populate();
        }

        private async void Populate(int newVal = 0)
        {
            List<CardInstance> cardsInHand = _gameManager.GetHandList();
            if(cardsInHand.Count is 0 && _displayedCards.Count is 0) return;

            //Trigger despawn for any cards that shouldnt be there anymore
            (bool, List<int>) output = await RemoveExtraCards(cardsInHand);
            List<int> removedIndeces = output.Item2;
            bool cardsWereRemoved = output.Item1;

            //Move the remaining cards back together
            FillEmptyIndeces(removedIndeces);

            //remove cards form list to be spawned that are already being displayed
            CardInstance[] carry = new CardInstance[cardsInHand.Count];
            cardsInHand.CopyTo(carry);
            foreach(CardInstance card in carry)
            {
                if(_displayedCards.Where(displayedCard => displayedCard.Key.GetComponent<CardObject_Hand>().CardInstanceRef.CardArt == card.CardArt).Count() > 0)
                    cardsInHand.Remove(card);
            }

            //spawn new cards from the right and adjust individual card position and spacing based on new hand size
            foreach(CardInstance card in cardsInHand)
            {
                GameObject cardObject = Instantiate(_cardObjectPrefab, _centerCardSpotTransform);
                CardObject_Hand cardObjComp = cardObject.GetComponent<CardObject_Hand>();
                RectTransform cardTransform = cardObject.GetComponent<RectTransform>();
                cardObjComp.Initialise( card, this, _rootCanvas.scaleFactor, () => OnHoverStart(cardTransform), () => OnHoverEnd(cardTransform) );
                _displayedCards.Add(cardTransform, (_displayedCards.Count is 0) ? 1 : _displayedCards.Values.Max<int>()+1);
                await cardObjComp.Spawn();
                AdjustCardSpacing();
            }
        }

        ///<summary>
        ///Removes any card objects that arent supposed to be shown anymore, and saves the index of their position.
        ///</summary>
        ///<param name="cardsInHand">List of CardInstance Objects that is the List of cards that are supposed to be displayed.</param>
        ///<returns>Returns a Tupple. The first value being a bool, true if cards were removed, false if not. The second value being a List of ints containing index positions of removed cards if there were any.</returns>
        private async Task<(bool, List<int>)> RemoveExtraCards(List<CardInstance> cardsInHand)
        {
            bool cardsWereRemoved = false;
            List<int> removedIndeces = new List<int>();

            List<CardInstance> displayedCards = _displayedCards.Keys.Select(cardObject => cardObject.GetComponent<CardObject_Hand>().CardInstanceRef).ToList();

            foreach(CardInstance card in displayedCards)
            {
                if(!cardsInHand.Contains(card))
                {
                    KeyValuePair<RectTransform, int> cardObject = _displayedCards.Where(cardObject => cardObject.Key.GetComponent<CardObject_Hand>().CardInstanceRef.CardArt == card.CardArt).FirstOrDefault();
                    _displayedCards.Remove(cardObject.Key);
                    removedIndeces.Add(cardObject.Value);
                    await cardObject.Key.GetComponent<CardObject_Hand>().Despawn();
                    cardsWereRemoved = true;
                }
            }

            return (cardsWereRemoved, removedIndeces);
        }

        private void FillEmptyIndeces(List<int> emptyIndeces)
        {
            _displayedCards.OrderBy(pair => pair.Value);

            for(int i = 0; i < emptyIndeces.Count; i++)
            {
                List<KeyValuePair<RectTransform, int>> higherIndexCards = _displayedCards.Where(pair => pair.Value > emptyIndeces[i]-i).ToList();
                foreach(KeyValuePair<RectTransform, int> pair in higherIndexCards)
                {
                    _displayedCards[pair.Key] = pair.Value-1;
                }
            }
            
            AdjustCardSpacing();
        }

        private void OnHoverStart(RectTransform hoveredCard)
        {
            if(_displayedCards.Count is < 6)
            {
                hoveredCard.anchoredPosition += new Vector2(0, 5);
                return;
            }

            int indexHovered = _displayedCards[hoveredCard];

            //adjust spacing left and right of hovered index by varying amounts based on distance to hovered card
            foreach(KeyValuePair<RectTransform, int> cardObject in _displayedCards)
            {
                int indexDelta = cardObject.Value - indexHovered;

                //TODO find good curve for spacing
            }
        }

        private void OnHoverEnd(RectTransform hoveredCard) => AdjustCardSpacing();

        private void AdjustCardOrder()
        {
            //TODO
            

        }

        private void AdjustCardSpacing()
        {
            if(_displayedCards.Count is 0) return;

            bool centerIsInteger = _displayedCards.Count % 2 > 0;
            float center = (centerIsInteger) ? Mathf.CeilToInt(_displayedCards.Count / 2) : _displayedCards.Count / 2 + 0.5f;

            float maxWidth = _handTransform.sizeDelta.x;
            float halvedWidth = maxWidth / 2;

            int usedCardAmount = Mathf.Max(5, _displayedCards.Count);
            int usedCardAmountHalved = Mathf.FloorToInt(usedCardAmount / 2);

            float cardDistance = halvedWidth / usedCardAmountHalved;

            //TODO needs to be adjusted to account for card drag n drop

            _displayedCards.OrderBy(pair => pair.Value);
            foreach(KeyValuePair<RectTransform, int> pair in _displayedCards)
            {
                CardObject_Hand cardObj = pair.Key.GetComponent<CardObject_Hand>();
                if(cardObj.isBeingDragged != true)
                {
                    pair.Key.DOLocalMove(new Vector2((pair.Value - center) * cardDistance, 0), 0.1f);
                    cardObj.SetPositionOffSetX((pair.Value - center) * cardDistance, cardDistance/2);
                    pair.Key.SetSiblingIndex(pair.Value);
                }
            }
        }

        public void CardClicked(CardInstance card)
        {
            _cardInspectWindowHandler.DisplayCard(card);
        }

        private void DrawCard()
        {
            _gameManager.DrawCard(3);
        }

        private void CatchDrawBlock(bool newVal)
        {
            if(newVal)
            {
                _drawButton.interactable = false;
                StartCoroutine(DrawCooldownTextCoroutine());
                return;
            }

            _drawButton.interactable = true;
        }

        private void InspectGrave()
        {
            _gameManager.ViewCards(GameManager.CardGameStates.Grave);
        }
        #endregion

        private IEnumerator DrawCooldownTextCoroutine()
        {
            _cooldownCoroutineRunning = true;
            TimeSpan cd;
            for(int i = _drawCooldown; i <= 0; i--)
            {
                cd = TimeSpan.FromSeconds(i);
                _cooldownText.text = cd.Hours + ":" + cd.Minutes + ":" + cd.Seconds;
                yield return new WaitForSeconds(1f);
            }
            _cooldownText.text = "";
            _cooldownCoroutineRunning = false;
        }
    }
}
