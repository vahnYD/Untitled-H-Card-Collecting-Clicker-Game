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
using Cysharp.Threading.Tasks;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class SelectionWindowManager : MonoBehaviour
    {
        
        //! Need to make selection uncanclable to work around ability limitations

        public static SelectionWindowManager Instance {get; private set;}
        #region Properties
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private Transform _selectionWindowTransform = null;
        [SerializeField] private Transform _scrollViewContentTransform = null;
        [SerializeField] private Button _selectionConfirmButton = null;
        [SerializeField] private Button _selectionCancelButton = null;
        [SerializeField] private Transform _firstCardSpot = null;

        private List<Transform> _displayedCards = new List<Transform>();
        private int _cardsPerRow = 5;
        private int _cardXOffset = 290;
        private int _cardYOffset = -295;
        private List<CardInstance> _selectedCards = new List<CardInstance>();
        private int _selectionAmount = 0;
        private bool _isSelecting = false;
        private bool _wasCancelled = false;
        private bool _isCooldownRelated = false;
        private bool _cdReductionIsFlat = true;
        private float _cdReductionAmount = 0f;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            #if UNITY_EDITOR
            if(_cardObjectPrefab is null || _selectionWindowTransform is null || _selectionConfirmButton is null || _selectionCancelButton is null || _firstCardSpot is null)
                Debug.LogWarning("SelectionWindowManager.cs is missing Object References.");
        
            
            if(_selectionCancelButton != null && _selectionConfirmButton != null)
            {
            #endif
                _selectionConfirmButton.onClick.AddListener(delegate {ConfirmSelection();});
                _selectionCancelButton.onClick.AddListener(delegate {CancelSelection();});
            #if UNITY_EDITOR
            }

            if(_selectionWindowTransform != null)
            #endif
                _selectionWindowTransform.gameObject.SetActive(false);
        }
        #endregion
        
        #region Methods
        ///<summary>
        ///Opens the selection window and returns an array of CardInstance Objects if exactly the specified <paramref name="amount"> of cards were selected, otherwise returns null.
        ///Fails if the specified <paramref name="amount"> is 0.
        ///Fails if the given <paramref name="cardPool"> is empty.
        ///</summary>
        ///<param name="cardPool">ICardList Interface Object to select from.</param>
        ///<param name="amount">Optional. Amount of cards that need to be selected as int. Defaults to 1</param>
        ///<param name="cdReduction">Optional. Bool flag to set if the selection is cooldown reduction related and needs to show cooldown in the selection window. Defaults to false.</param>
        ///<param name="cdReductionIsFlat">Optional. Bool flag to set if the cooldown reduction is additive or multiplicative. true for additive, false for multiplicative. Defaults to true.</param>
        ///<param name="cooldownReductionAmount">Optional. Amount by which the cooldown will be reduced as a float. Defaults to 0f.</param>
        public async UniTask<CardInstance[]> SelectCards(ICardList cardPool, int amount = 1, bool cdReduction = false, bool cdReductionIsFlat = true, float cooldownReductionAmount = 0f)
        {
            if(amount is 0) return null;
            if(cardPool.isEmpty()) return null;

            _selectedCards.Clear();

            _selectionAmount = amount;
            _isCooldownRelated = cdReduction;
            _cdReductionIsFlat = cdReductionIsFlat;
            _cdReductionAmount = cooldownReductionAmount;

            _selectionConfirmButton.gameObject.SetActive(true);
            await SelectCardsEnumerator(cardPool);

            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        ///<summary>
        ///Opens the selection window and returns an array of CardInstance Objects if exactly the specified <paramref name="amount"> of cards were selected, otherwise returns null.
        ///Filters the <paramref name="cardPool"> by the given <paramref name="name">.
        ///Fails if the specified <paramref name="amount"> is 0.
        ///Fails if the given <paramref name="cardPool"> is empty after filtering by <paramref name="name">.
        ///</summary>
        ///<param name="cardPool">ICardList Interface Object to select from.</param>
        ///<param name="name">Name by which to filter the card list by as string.</param>
        ///<param name="amount">Optional. Amount of cards that need to be selected as int. Defaults to 1</param>
        ///<param name="cdReduction">Optional. Bool flag to set if the selection is cooldown reduction related and needs to show cooldown in the selection window. Defaults to false.</param>
        ///<param name="cdReductionIsFlat">Optional. Bool flag to set if the cooldown reduction is additive or multiplicative. true for additive, false for multiplicative. Defaults to true.</param>
        ///<param name="cooldownReductionAmount">Optional. Amount by which the cooldown will be reduced as a float. Defaults to 0f.</param>
        public async UniTask<CardInstance[]> SelectCardsByName(ICardList cardPool, string name, int amount = 1, bool cdReduction = false, bool cdReductionIsFlat = true, float cooldownReductionAmount = 0f)
        {
            if(amount is 0) return null;

            _selectedCards.Clear();

            _selectionAmount = amount;
            _isCooldownRelated = cdReduction;
            _cdReductionIsFlat = cdReductionIsFlat;
            _cdReductionAmount = cooldownReductionAmount;

            ICardList filteredCardPool = FilterCardList(cardPool, Card.SearchableProperties.Name, name: name);
            if(filteredCardPool.isEmpty()) return null;

            _selectionConfirmButton.gameObject.SetActive(true);
            await SelectCardsEnumerator(filteredCardPool);

            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        ///<summary>
        ///Opens the selection window and returns an array of CardInstance Objects if exactly the specified <paramref name="amount"> of cards were selected, otherwise returns null.
        ///Filters the <paramref name="cardPool"> by the given <paramref name="type">.
        ///Fails if the specified <paramref name="amount"> is 0.
        ///Fails if the given <paramref name="cardPool"> is empty after filtering by <paramref name="type">.
        ///</summary>
        ///<param name="cardPool">ICardList Interface Object to select from.</param>
        ///<param name="type">Card.CardType by which to filter the card list.</param>
        ///<param name="amount">Optional. Amount of cards that need to be selected as int. Defaults to 1</param>
        ///<param name="cdReduction">Optional. Bool flag to set if the selection is cooldown reduction related and needs to show cooldown in the selection window. Defaults to false.</param>
        ///<param name="cdReductionIsFlat">Optional. Bool flag to set if the cooldown reduction is additive or multiplicative. true for additive, false for multiplicative. Defaults to true.</param>
        ///<param name="cooldownReductionAmount">Optional. Amount by which the cooldown will be reduced as a float. Defaults to 0f.</param>
        public async UniTask<CardInstance[]> SelectCardsByType(ICardList cardPool, Card.CardType type, int amount = 1, bool cdReduction = false, bool cdReductionIsFlat = true, float cooldownReductionAmount = 0f)
        {
            if(amount is 0) return null;

            _selectedCards.Clear();

            _selectionAmount = amount;
            _isCooldownRelated = cdReduction;
            _cdReductionIsFlat = cdReductionIsFlat;
            _cdReductionAmount = cooldownReductionAmount;

            ICardList filteredCardPool = FilterCardList(cardPool, Card.SearchableProperties.Type, type: type);
            if(filteredCardPool.isEmpty()) return null;

            _selectionConfirmButton.gameObject.SetActive(true);
            await SelectCardsEnumerator(filteredCardPool);

            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        ///<summary>
        ///Opens the selection window and returns an array of CardInstance Objects if exactly the specified <paramref name="amount"> of cards were selected, otherwise returns null.
        ///Filters the <paramref name="cardPool"> by the given <paramref name="rarity">.
        ///Fails if the specified <paramref name="amount"> is 0.
        ///Fails if the given <paramref name="cardPool"> is empty after filtering by <paramref name="rarity">.
        ///</summary>
        ///<param name="cardPool">ICardList Interface Object to select from.</param>
        ///<param name="rarity">Card.CardRarity by which to filter the card list.</param>
        ///<param name="amount">Optional. Amount of cards that need to be selected as int. Defaults to 1</param>
        ///<param name="cdReduction">Optional. Bool flag to set if the selection is cooldown reduction related and needs to show cooldown in the selection window. Defaults to false.</param>
        ///<param name="cdReductionIsFlat">Optional. Bool flag to set if the cooldown reduction is additive or multiplicative. true for additive, false for multiplicative. Defaults to true.</param>
        ///<param name="cooldownReductionAmount">Optional. Amount by which the cooldown will be reduced as a float. Defaults to 0f.</param>
        public async UniTask<CardInstance[]> SelectCardsByRarity(ICardList cardPool, Card.CardRarity rarity, int amount = 1, bool cdReduction = false, bool cdReductionIsFlat = true, float cooldownReductionAmount = 0f)
        {
            if(amount is 0) return null;

            _selectedCards.Clear();

            _selectionAmount = amount;
            _isCooldownRelated = cdReduction;
            _cdReductionIsFlat = cdReductionIsFlat;
            _cdReductionAmount = cooldownReductionAmount;

            ICardList filteredCardPool = FilterCardList(cardPool, Card.SearchableProperties.Rarity, rarity: rarity);
            if(filteredCardPool.isEmpty()) return null;

            _selectionConfirmButton.gameObject.SetActive(true);
            await SelectCardsEnumerator(filteredCardPool);

            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        ///<summary>
        ///Displays the give card list, but hides the confirm button.
        ///Intended for viewing.
        ///</summary>
        ///<property name="cardPool">ICardList Interface Object to display.</property>
        public async void ViewCards(ICardList cardPool)
        {
            if(cardPool.GetCardList().Count is 0) return;

            _selectionConfirmButton.gameObject.SetActive(false);
            await SelectCardsEnumerator(cardPool);
            _selectionConfirmButton.gameObject.SetActive(true);
        }

        ///<summary>
        ///Filters the given ICardList Interface Instance by the given <paramref name="property">.
        ///</summary>
        ///<param name="cardList">The ICardList Interface Instance to filter.</param>
        ///<param name="property">The Card.SearchableProperties enum value by which to filter.</param>
        ///<param name="name">Optional. The name by which to filter as string. Default to an empty string.</param>
        ///<param name="type">Optional. The Card.CardType enum value by which to filter. Defaults to Card.CardType.Allsexual.</param>
        ///<param name="rarity">Optional. The Card.CardRarity enum value by which to filter. Default to Card.CardRarity.Common.</param>
        private ICardList FilterCardList(ICardList cardList, Card.SearchableProperties property, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
        {
            ICardList output = new Deck();

            //Adds every CardInstance from the ICardList Interface Instance to the output Instance where the property is the same as the given value
            switch(property)
            {
                case Card.SearchableProperties.Name:
                    foreach(CardInstance card in cardList.GetCardList())
                    {
                        if(card.Name == name) output.AddCard(card);
                    }
                    break;
                
                case Card.SearchableProperties.Type:
                    foreach(CardInstance card in cardList.GetCardList())
                    {
                        if(card.CardRef.Type == type) output.AddCard(card);
                    }
                    break;

                case Card.SearchableProperties.Rarity:
                    foreach(CardInstance card in cardList.GetCardList())
                    {
                        if(card.CardRef.Rarity == rarity) output.AddCard(card);
                    }
                    break;
            }
            return output;
        }

        ///<summary>
        ///Populates the Selection Window by spawning CardObject_Selection Instances for each CardInstance in the give <paramref name="cardList">.
        ///</summary>
        ///<param name="cardList">ICardList Interface Instance to populate the window with.</param>
        private void Populate(ICardList cardList)
        {
            List<CardInstance> cards = cardList.GetCardList();

            int columnIndex = 0;
            int rowIndex = 0;

            foreach(CardInstance card in cards)
            {
                GameObject cardObject = Instantiate(_cardObjectPrefab, _firstCardSpot);
                cardObject.transform.localPosition = Vector2.zero;
                cardObject.GetComponent<CardObject_Selection>().Initialise(card, _isCooldownRelated, _cdReductionIsFlat, _cdReductionAmount);

                cardObject.transform.localPosition = new Vector2(columnIndex * _cardXOffset, rowIndex * _cardYOffset);

                columnIndex++;
                if(columnIndex >= _cardsPerRow)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }

            _scrollViewContentTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(_scrollViewContentTransform.GetComponent<RectTransform>().sizeDelta.x, _cardYOffset * rowIndex);
        }

        ///<summary>
        ///De-/Selects the given <paramref name="card"> based on wether or not it was de-/selected prior.
        ///</summary>
        ///<param name="card">CardInstance of the card that was clicked.</param>
        ///<returns>Returns true if the card is now selected, false if it is now deselected.</returns>
        public bool CardClicked(CardInstance card)
        {
            if(_selectedCards.Contains(card))
            {
                DeselectCard(card);
                return false;
            }
            SelectCard(card);
            return true;
        }
        private void SelectCard(CardInstance card)
        {
            _selectedCards.Add(card);
            if(_selectedCards.Count == _selectionAmount) _selectionConfirmButton.interactable = true;
            else _selectionConfirmButton.interactable = false;
        }

        private void DeselectCard(CardInstance card)
        {
            _selectedCards.Remove(card);
            if(_selectedCards.Count == _selectionAmount) _selectionConfirmButton.interactable = true;
            else _selectionConfirmButton.interactable = false;
        }

        private void ConfirmSelection()
        {
            _wasCancelled = false;
            _isSelecting = false;
        }

        private void CancelSelection()
        {
            _wasCancelled = true;
            _isSelecting = false;
        }
        #endregion

        // selection methods await coroutine finish. Coroutine runs till the Confirm button in the selection window is pressed
        private IEnumerator SelectCardsEnumerator(ICardList cardList)
        {
            //populates the selection window
            Populate(cardList);

            _isSelecting = true;
            _selectionConfirmButton.interactable = false;
            _selectionWindowTransform.gameObject.SetActive(true);

            //waits until either the cancel or confirm button was pressed
            yield return new WaitUntil(() => _isSelecting is false);

            _selectionWindowTransform.gameObject.SetActive(false);

            //destroys all the card objects after the window was closed
            List<CardInstance> displayedCards = _displayedCards.Select(x => x.GetComponent<CardObject>().CardInstanceRef).ToList();
            foreach(CardInstance card in displayedCards)
            {
                Transform cardObject = _displayedCards.Where(x => x.GetComponent<CardObject>().CardInstanceRef.CardArt == card.CardArt).FirstOrDefault();
                _displayedCards.Remove(cardObject);
                Destroy(cardObject.gameObject);
            }
        }
    }
}
