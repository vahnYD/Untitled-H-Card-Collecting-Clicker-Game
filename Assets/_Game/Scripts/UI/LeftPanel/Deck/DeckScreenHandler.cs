/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Linq;
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
        [SerializeField] private Button _applyBtn = null;
        [SerializeField] private Button _addBtn = null;
        [SerializeField] private Button _removeBtn = null;
        private GameManager _gameManager = null;
        private GameSettingsScriptableObject _gameSettings = null;
        private Dictionary<Transform, int> _displayedCards = new Dictionary<Transform, int>();
        private List<CardInstance> _selectedCards = new List<CardInstance>();
        private KeyValuePair<CardInstance, bool> _displayedSelectedCard;

        private bool _applyAllowed = false;
        private bool _modifyAllowed = true;
        private int _currentDeckSize = 10;
        private int _amountOfRareCardsInSelection = 0;
        private int _amountOfVRareCardsInSelection = 0;
        private int _amountOfSpecialCardsInSelection = 0;
        #endregion

        public event Action<int, int, int, int> SelectedCardsChangeEvent;

        #region Unity Event Functions
        #if UNITY_EDITOR
        private void Awake()
        {
            if(_cardObjectDeckPrefab is null || _cardArtImageObj is null || _abilityText is null || _abilityTextScrollViewContentObj is null || _cardObjSpawnTransform is null || _applyBtn is null || _addBtn is null || _removeBtn is null)
                Debug.LogWarning("DeckScreenHandler.cs is missing Object References.");
        }
        #endif

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gameSettings = _gameManager.GameSettingsRef;
            _currentDeckSize = _gameSettings.DefaultDeckSize;
            #if UNITY_EDITOR
            if(_removeBtn != null && _addBtn != null && _applyBtn != null)
            {
            #endif
                _removeBtn.interactable = false;
                _addBtn.interactable = false;
                _applyBtn.interactable = false;
            #if UNITY_EDITOR
            }
            #endif
        }
        #endregion
        
        #region Deck CardObject Spawning
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
                cardObject.GetComponent<CardObject_Deck>().Initialise(card, this, true, () => ClickHandling(card));
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
        #endregion

        #region Deck Editing
        public void ClickHandling(CardInstance card)
        {
            _cardArtImageObj.sprite = card.CardArt;
            _abilityText.text = card.AbilityText;
            bool state = _displayedCards.Keys.ToList().Exists(x => x.GetComponent<CardObject_Deck>().CardInstanceRef.CardArt == card.CardArt);
            _displayedSelectedCard = new KeyValuePair<CardInstance, bool>(card, state);
            if(state)
            {
                _addBtn.interactable = false;
                _removeBtn.interactable = true;
            }
            else
            {
                _removeBtn.interactable = false;
                _addBtn.interactable = true;
            }
        }

        public void AddCard() => SelectCard(_displayedSelectedCard.Key);
        public void RemoveCard() => DeselectCard(_displayedSelectedCard.Key);
        private void DeselectCard(CardInstance card) => SelectCard(card, true);
        private void SelectCard(CardInstance card, bool isRemoval = false)
        {
            //Dynamically Check increasing deck size limitation
            if(card.CardRef.Rarity is Card.CardRarity.Rare)
            {
                _amountOfRareCardsInSelection += (isRemoval) ? -1 : 1;
            }
            else if(card.CardRef.Rarity is Card.CardRarity.VeryRare)
            {
                _amountOfVRareCardsInSelection += (isRemoval) ? -1 : 1;
                _currentDeckSize += (isRemoval) ? -_gameSettings.DeckSizeIncreasePerVeryRare : _gameSettings.DeckSizeIncreasePerVeryRare;
            }
            else if(card.CardRef.Rarity is Card.CardRarity.Special)
            {
                _amountOfSpecialCardsInSelection += (isRemoval) ? -1 : 1;
                _currentDeckSize += (isRemoval) ? -_gameSettings.DeckSizeIncreasePerSpecial : _gameSettings.DeckSizeIncreasePerSpecial;
            }

            SelectedCardsChangeEvent?.Invoke(_selectedCards.Count, _amountOfRareCardsInSelection, _amountOfVRareCardsInSelection, _amountOfSpecialCardsInSelection);

            if(!isRemoval)_selectedCards.Add(card);
            else _selectedCards.Remove(card);
            Populate(_selectedCards.Count);

            //Check if apply is allowed
            if(_selectedCards.Count == _currentDeckSize && _amountOfVRareCardsInSelection !> _gameSettings.MaximumVRareCardsAllowedInDeckAtBase && _amountOfSpecialCardsInSelection !> _gameSettings.MaximumSpecialCardsAllowedInDeckAtBase && _gameManager.HandSize is 0 && _gameManager.GraveSize is 0)
            {
                _applyAllowed = true;
                _applyBtn.interactable = true;
            }
            else
            {
                _applyAllowed = false;
                _applyBtn.interactable = false;
            }
        }

        public void Apply()
        {
            if(!_applyAllowed) return;

            _gameManager.OverwriteDecklist(_selectedCards);
        }
        #endregion
    }
}
