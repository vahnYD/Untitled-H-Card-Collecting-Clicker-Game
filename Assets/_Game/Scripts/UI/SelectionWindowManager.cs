/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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
        [SerializeField] private Transform _firstCardSpot = null;

        private List<CardInstance> _selectedCards = new List<CardInstance>();
        private int _selectionAmount = 0;
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
        }
        #endregion
        
        #region Methods
        public async UniTask<CardInstance[]> SelectCards(ICardList cardPool, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            await SelectCardsEnumerator(cardPool, cdReduction);
            //TODO
            return _selectedCards.ToArray();
        }

        public async UniTask<CardInstance[]> SelectCardsByName(ICardList cardPool, string name, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            await SelectCardsByNameEnumerator(FilterCardListByName(cardPool, name), cdReduction);
            //TODO
            return _selectedCards.ToArray();
        }

        public async UniTask<CardInstance[]> SelectCardsByType(ICardList cardPool, Card.CardType type, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            await SelectCardsByTypeEnumerator(FilterCardListByType(cardPool, type), cdReduction);
            //TODO
            return _selectedCards.ToArray();
        }

        public async UniTask<CardInstance[]> SelectCardsByRarity(ICardList cardPool, Card.CardRarity rarity, int amount = 1, bool cdReduction = false)
        {
            if(amount is 0) return null;
            _selectedCards.Clear();
            _selectionAmount = amount;
            await SelectCardsByRarityEnumerator(FilterCardListByRarity(cardPool, rarity), cdReduction);
            //TODO
            return _selectedCards.ToArray();
        }

        private ICardList FilterCardListByName(ICardList cardList, string name) => FilterCardList(cardList, Card.SearchableProperties.Name, name);
        private ICardList FilterCardListByType(ICardList cardList, Card.CardType type) => FilterCardList(cardList, Card.SearchableProperties.Type, type: type);
        private ICardList FilterCardListByRarity(ICardList cardList, Card.CardRarity rarity) => FilterCardList(cardList, Card.SearchableProperties.Rarity, rarity: rarity);
        private ICardList FilterCardList(ICardList cardList, Card.SearchableProperties property, string name = "", Card.CardType type = Card.CardType.Allsexual, Card.CardRarity rarity = Card.CardRarity.Common)
        {
            ICardList output = new Deck();

            //TODO
            return output;
        }

        private void Populate(ICardList cardList)
        {

        }

        private void SelectCard()
        {

        }

        private void DeselectCard()
        {

        }
        #endregion

        //! selection methods await coroutine finish. Coroutine runs till the Confirm button in the selection window is pressed
        private IEnumerator SelectCardsEnumerator(ICardList cardList, bool isCooldownReduction = false)
        {
            yield return null;
        }

        private IEnumerator SelectCardsByNameEnumerator(ICardList cardList, bool isCooldownReduction = false)
        {
            yield return null;
        }

        private IEnumerator SelectCardsByTypeEnumerator(ICardList cardList, bool isCooldownReduction = false)
        {
            yield return null;
        }

        private IEnumerator SelectCardsByRarityEnumerator(ICardList cardList, bool isCooldownReduction = false)
        {
            yield return null;
        }
    }
}
