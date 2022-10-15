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
        public static SelectionWindowManager Instance {get; private set;}
        #region Properties
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private Transform _selectionWindowTransform = null;
        [SerializeField] private Button _selectionConfirmButton = null;
        [SerializeField] private Button _selectionCancelButton = null;
        [SerializeField] private Transform _firstCardSpot = null;

        private List<Transform> _displayedCards = new List<Transform>();
        private int _cardsPerRow = 4;
        private List<CardInstance> _selectedCards = new List<CardInstance>();
        private int _selectionAmount = 0;
        private bool _isSelecting = false;
        private bool _wasCancelled = false;
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
            #endif

            //TODO Cards Per Row Dynamic Calculation
        }
        #endregion
        
        #region Methods
        public async UniTask<CardInstance[]> SelectCards(ICardList cardPool, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            if(cardPool.isEmpty()) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            await SelectCardsEnumerator(cardPool, cdReduction);
            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        public async UniTask<CardInstance[]> SelectCardsByName(ICardList cardPool, string name, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            ICardList filteredCardPool = FilterCardList(cardPool, Card.SearchableProperties.Name, name: name);
            if(filteredCardPool.isEmpty()) return null;
            await SelectCardsEnumerator(filteredCardPool, cdReduction);
            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        public async UniTask<CardInstance[]> SelectCardsByType(ICardList cardPool, Card.CardType type, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            ICardList filteredCardPool = FilterCardList(cardPool, Card.SearchableProperties.Type, type: type);
            if(filteredCardPool.isEmpty()) return null;
            await SelectCardsEnumerator(filteredCardPool, cdReduction);
            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        public async UniTask<CardInstance[]> SelectCardsByRarity(ICardList cardPool, Card.CardRarity rarity, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            ICardList filteredCardPool = FilterCardList(cardPool, Card.SearchableProperties.Rarity, rarity: rarity);
            if(filteredCardPool.isEmpty()) return null;
            await SelectCardsEnumerator(filteredCardPool, cdReduction);
            return (_wasCancelled) ? null : _selectedCards.ToArray();
        }

        private ICardList FilterCardList(ICardList cardList, Card.SearchableProperties property, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
        {
            ICardList output = new Deck();

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

        private void Populate(ICardList cardList, bool isCooldownReduction)
        {

        }

        private void SelectCard()
        {

        }

        private void DeselectCard()
        {

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
        private IEnumerator SelectCardsEnumerator(ICardList cardList, bool isCooldownReduction = false)
        {
            Populate(cardList, isCooldownReduction);
            _isSelecting = true;
            _selectionConfirmButton.interactable = false;
            _selectionWindowTransform.gameObject.SetActive(true);
            while(_isSelecting)
            {
                yield return null;
            }
            _selectionWindowTransform.gameObject.SetActive(false);
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
