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
using _Game.Scripts.Extensions;

namespace _Game.Scripts.UI
{
    public class DeckScreenHandler : MonoBehaviour
    {
        //TODO dynamic deck size stuff needs to be adjusted for burning medal effects
        
        #region Properties
        [SerializeField] private GameObject _cardObjectDeckPrefab = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _abilityText = null;
        [SerializeField] private Transform _abilityTextScrollViewContentObj = null;
        [SerializeField] private Transform _cardObjSpawnTransform = null;
        [SerializeField] private Transform _deckDisplayScrollViewContentObj = null;
        [SerializeField] private Button _applyBtn = null;
        [SerializeField] private Button _addBtn = null;
        [SerializeField] private Button _removeBtn = null;
        [SerializeField] private BoolValue _modifyIsBlocked = null;
        private GameManager _gameManager = null;
        private GameSettingsScriptableObject _gameSettings = null;
        private Dictionary<Transform, int> _displayedCards = new Dictionary<Transform, int>();
        private List<CardInstance> _selectedCards = new List<CardInstance>();
        private KeyValuePair<CardInstance, bool> _displayedSelectedCard;

        private bool _applyAllowed = false;
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
            if(_cardObjectDeckPrefab is null || _cardArtImageObj is null || _abilityText is null || _abilityTextScrollViewContentObj is null || _cardObjSpawnTransform is null || _deckDisplayScrollViewContentObj is null || _applyBtn is null || _addBtn is null || _removeBtn is null || _modifyIsBlocked is null)
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
        ///<summary>
        ///Updates the deck portion of the deck tab with the selected cards by removing unselected cards and spawning newly selected cards.
        ///</summary>
        private void Populate(int newVal)
        {
            if(_displayedCards.Count is 0 && newVal is 0) return;

            List<CardInstance> deck = new List<CardInstance>();
            CardInstance[] carry1 = new CardInstance[_selectedCards.Count];
            _selectedCards.CopyTo(carry1);
            deck = carry1.ToList();

            //removes any unselected card objects and saves their position
            List<int> removedIndeces = new List<int>();
            bool cardsWereRemoved = CheckForRemoval(deck, ref removedIndeces);
            if(!cardsWereRemoved && (newVal is 0 || newVal == _displayedCards.Count)) return;

            //removes already displayed cards from the list of cards to spawn
            CardInstance[] carry2 = new CardInstance[deck.Count];
            deck.CopyTo(carry2);
            foreach(CardInstance card in carry2)
                if(_displayedCards.Where(x=>x.Key.GetComponent<CardObject_Deck>().CardInstanceRef.CardArt == card.CardArt).Count() > 0) deck.Remove(card);

            //spawn any newly selected cards
            //position priority for spots where cards were previously removed
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

            //fill any left over empty spots when there were not enough newly spawned cards by moving the cards past the empty positions
            if(removedIndeces.Count > 0)
                FillEmptyIndeces(removedIndeces);

            //Update the size of the scroll view content object to fit the amount of displayed cards
            RectTransform firstCardSpot = _cardObjSpawnTransform.GetComponent<RectTransform>();
            RectTransform scrollViewContent = _deckDisplayScrollViewContentObj.GetComponent<RectTransform>();
            scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, firstCardSpot.sizeDelta.y * _selectedCards.Count);
        }

        ///<summary>
        ///Removes any card objects that arent supposed to be shown anymore, and saves the index of their position.
        ///</summary>
        ///<param name="deck">List of CardInstance Objects that is the List of cards that are supposed to be displayed.</param>
        ///<param name="removedIndeces">List of ints as reference to save the indeces of positions of any removed cards to.</param>
        ///<returns>Returns true if any cards were removed, false if there were no cards that needed removing.</returns>
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

        ///<summary>
        ///Fills any empty positions by moving cards with higher indeces backwards to fill them.
        ///</summary>
        ///<param name="emptyIndeces">List of empty Indeces as int.</param>
        private void FillEmptyIndeces(List<int> emptyIndeces)
        {
            //sorts the list of displayed cards by their indeces
            List<KeyValuePair<Transform, int>> sortedDisplayedCards = _displayedCards.ToList();
            sortedDisplayedCards.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            emptyIndeces.Sort((i, j) => i.CompareTo(j));

            //moves all cards with positions after the first empty index backwards by 1 for each empty index
            foreach(int i in emptyIndeces)
            {
                for(int j = i; j < sortedDisplayedCards.Count; j++)
                {
                    KeyValuePair<Transform, int> pair = sortedDisplayedCards[j];
                    pair.Key.localPosition = new Vector2(0, pair.Key.localPosition.y + pair.Key.GetComponent<RectTransform>().sizeDelta.y);
                    _displayedCards[pair.Key]--;
                }
            }
        }
        #endregion

        #region Deck Editing
        ///<summary>
        ///Displays the clicked card and link the de-/selection buttons to it.
        ///</summary>
        ///<param name="card">CardInstance of the card to display.</param>
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


        public void AddCard()
        {
            SelectCard(_displayedSelectedCard.Key);
        }
        public void RemoveCard()
        {
            DeselectCard(_displayedSelectedCard.Key);
        }

        private void DeselectCard(CardInstance card) => SelectCard(card, true);
        private void SelectCard(CardInstance card, bool isRemoval = false)
        {
            if(isRemoval)
            {
                _removeBtn.interactable = false;
                _addBtn.interactable = true;
            }
            else
            {
                _addBtn.interactable = false;
                _removeBtn.interactable = true;
            }

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

            if(!isRemoval)_selectedCards.Add(card);
            else _selectedCards.Remove(card);

            SelectedCardsChangeEvent?.Invoke(_selectedCards.Count, _amountOfRareCardsInSelection, _amountOfVRareCardsInSelection, _amountOfSpecialCardsInSelection);

            Populate(_selectedCards.Count);

            //Check if apply is allowed
            if(_selectedCards.Count == _currentDeckSize && _amountOfVRareCardsInSelection <= _gameSettings.MaximumVRareCardsAllowedInDeckAtBase && _amountOfSpecialCardsInSelection <= _gameSettings.MaximumSpecialCardsAllowedInDeckAtBase && _modifyIsBlocked.Value == false)
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

        ///<summary>
        ///Applies the selected cards to the deck object on the GameManager.
        ///Fails if applyAllowed flag is false.
        ///</summary>
        public void Apply()
        {
            if(!_applyAllowed) return;

            _gameManager.OverwriteDecklist(_selectedCards);
        }
        #endregion
    }
}
