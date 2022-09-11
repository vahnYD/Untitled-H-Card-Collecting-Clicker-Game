/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class DeckScreenHandler : MonoBehaviour
    {
        #region Properties
        [SerializeField] private GameObject _cardObjectDeckPrefab = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _abilityText = null;
        [SerializeField] private Transform _abilityTextScrollViewContentObj = null;
        [SerializeField] private Transform _cardObjSpawnTransform = null;
        private GameManager _gameManager = null;
        private GameSettingsScriptableObject _gameSettings = null;
        private Dictionary<Transform, int> _displayedCards = new Dictionary<Transform, int>();
        private List<CardInstance> _selectedCards = new List<CardInstance>();

        private bool _applyAllowed = false;
        private bool _modifyAllowed = true;
        private int _currentDeckSize = 10;
        private int _amountOfVRareCardsInSelection = 0;
        private int _amountOfSpecialCardsInSelection = 0;
        #endregion

        #region Unity Event Functions
        #if UNITY_EDITOR
        private void Awake()
        {
            if(_cardObjectDeckPrefab is null || _cardArtImageObj is null || _abilityText is null || _abilityTextScrollViewContentObj is null || _cardObjSpawnTransform is null)
                Debug.LogWarning("DeckScreenHandler.cs is missing Object References.");
        }
        #endif

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gameSettings = _gameManager.GameSettingsRef;
            _currentDeckSize = _gameSettings.DefaultDeckSize;
        }

        private void OnEnable()
        {
            GameManager.GraveSizeChangedEvent += CatchGraveSizeChange;
        }

        private void OnDisable()
        {
            GameManager.GraveSizeChangedEvent -= CatchGraveSizeChange;
        }
        #endregion
        
        #region Methods
        private void CatchGraveSizeChange(int newVal)
        {
            _modifyAllowed = (newVal > 0) ? false : true;
        }

        private void Populate(int newVal)
        {
            if(_displayedCards.Count is 0 && newVal is 0) return;

            List<CardInstance> deck = _selectedCards;
            List<int> removedIndeces = new List<int>();
            bool cardsWereRemoved = CheckForRemoval(deck, ref removedIndeces);
            if(!cardsWereRemoved && (newVal is 0 || newVal == _displayedCards.Count)) return;

            CardInstance[] carry = new CardInstance[deck.Count];
            deck.CopyTo(carry);
            foreach(CardInstance card in carry)
                if(_displayedCards.Where(x=>x.Key.GetComponent<CardObject_Deck>().CardInstanceRef.CardArt == card.CardArt).Count() > 0) deck.Remove(card);

            foreach(CardInstance card in deck)
            {
                GameObject cardObject = Instantiate(_cardObjectDeckPrefab, _cardObjSpawnTransform);
                cardObject.transform.localPosition = Vector2.zero;

                int offsetMult = 0;
                if(cardsWereRemoved && removedIndeces.Count > 0)
                {
                    offsetMult = removedIndeces.First();
                    removedIndeces.Remove(removedIndeces.First());
                }
                else offsetMult = _displayedCards.Count;

                cardObject.transform.localPosition = new Vector2(0, -(cardObject.GetComponent<RectTransform>().sizeDelta.y * offsetMult));
                cardObject.GetComponent<CardObject_Deck>().Initialise(card, this);
                _displayedCards.Add(cardObject.transform, offsetMult);
            }

            if(removedIndeces.Count > 0)
                FillEmptyIndeces(removedIndeces);
        }

        private bool CheckForRemoval(List<CardInstance> deck, ref List<int> removedIndeces)
        {
            bool cardsWereRemoved = false;

            List<CardInstance> displayedCards = _displayedCards.Keys.Select(x => x.GetComponent<CardObject_Deck>().CardInstanceRef).ToList<CardInstance>();

            foreach(CardInstance card in displayedCards)
            {
                if(!deck.Contains(card))
                {
                    KeyValuePair<Transform, int> pair = _displayedCards.Where(x=>x.Key.GetComponent<CardObject_Deck>().CardInstanceRef.CardArt == card.CardArt).FirstOrDefault();
                    _displayedCards.Remove(pair.Key);
                    removedIndeces.Add(pair.Value);
                    Destroy(pair.Key.gameObject);
                    cardsWereRemoved = true;
                }
            }

            return cardsWereRemoved;
        }

        private void FillEmptyIndeces(List<int> emptyIndeces)
        {
            List<KeyValuePair<Transform, int>> sortedDisplayedCards = _displayedCards.ToList();
            sortedDisplayedCards.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            emptyIndeces.Sort((i, j) => i.CompareTo(j));

            foreach(int i in emptyIndeces)
            {
                for(int j = i; j < sortedDisplayedCards.Count; j++)
                {
                    KeyValuePair<Transform, int> pair = sortedDisplayedCards[j];
                    pair.Key.localPosition = new Vector2(0, pair.Key.localPosition.y - pair.Key.GetComponent<RectTransform>().sizeDelta.y);
                    _displayedCards[pair.Key]--;
                }
            }
        }

        public void Click(CardInstance card)
        {
            _cardArtImageObj.sprite = card.CardArt;
            _abilityText.text = card.AbilityText;
        }

        private void SelectCard(CardInstance card)
        {
            //TODO Dynamically Check increasing deck size limitation

            _selectedCards.Add(card);
            Populate(_selectedCards.Count);

            //TODO Check if apply is allowed
        }

        public void Apply()
        {
            if(!_applyAllowed) return;

            foreach(CardInstance card in _selectedCards) _gameManager.AddCardToDeck(card);
        }
        #endregion
    }
}
