/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class HandDisplay : MonoBehaviour
    {
        #region Properties
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private Transform _cardInspectWindowTransform = null;
        [SerializeField] private RectTransform _handTransform = null;
        [SerializeField] private Transform _centerCardSpotTransform = null;
        private Dictionary<Transform, int> _displayedCards = new Dictionary<Transform, int>();
        private GameManager _gameManager;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            CatchHandSizeChange(0);
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {

        }
        #endregion
        
        #region Methods
        private void CatchHandSizeChange(int newVal)
        {
            if(_gameManager is null) return;
            Populate();
        }

        private async void Populate()
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
                cardObjComp.Initialise(card, this);
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
                    KeyValuePair<Transform, int> cardObject = _displayedCards.Where(cardObject => cardObject.Key.GetComponent<CardObject_Hand>().CardInstanceRef.CardArt == card.CardArt).FirstOrDefault();
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
                List<KeyValuePair<Transform, int>> higherIndexCards = _displayedCards.Where(pair => pair.Value > emptyIndeces[i]-i).ToList();
                foreach(KeyValuePair<Transform, int> pair in higherIndexCards)
                {
                    _displayedCards[pair.Key] = pair.Value-1;
                }
            }
            
            AdjustCardSpacing();
        }

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

            //TODO needs to be tweened rather then set

            _displayedCards.OrderBy(pair => pair.Value);
            foreach(KeyValuePair<Transform, int> pair in _displayedCards)
            {
                pair.Key.GetComponent<RectTransform>().DOLocalMoveX((pair.Value - center) * cardDistance, 0.1f);
                pair.Key.GetComponent<CardObject_Hand>().SetPositionOffSetX((pair.Value - center) * cardDistance);
                pair.Key.SetSiblingIndex(pair.Value);
            }
        }

        public void CardClicked(CardInstance card)
        {

        }
        #endregion
    }
}
