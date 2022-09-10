/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Extensions;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class DeckScreenInventory : MonoBehaviour
    {
        #region Properties
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private IntValue _lewdPointValueObj = null;
        [SerializeField] private Transform _scrollviewContentObj = null;
        [SerializeField] private Transform[] _columnTransforms = new Transform[4];
        [SerializeField] private float _heightOffset = 182f;
        private int[] _columnCardCount = new int[4];
        private Dictionary<Transform, int> _displayedCards = new Dictionary<Transform, int>();
        private List<int> _unfilledIndeces = new List<int>();
        private GameManager _gameManager = null;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_cardObjectPrefab is null || _lewdPointValueObj is null || _scrollviewContentObj is null)
                Debug.LogWarning("DeckScreenInventory.cs is missing Object References.");
            #endif
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            CatchLewdPointChange(0);
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if(_lewdPointValueObj != null)
            #endif
                _lewdPointValueObj.ValueChangedEvent += CatchLewdPointChange;
            CatchLewdPointChange(0);
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if(_lewdPointValueObj != null)
            #endif
                _lewdPointValueObj.ValueChangedEvent -= CatchLewdPointChange;
        }
        #endregion
        
        #region Methods
        private void CatchLewdPointChange(int newVal)
        {
            if(_gameManager is null) return;
            Populate();
        }

        private void Populate()
        {
            List<CardInstance> ownedCards = _gameManager.GetOwnedCards();
            if(ownedCards.Count is 0) return;

            List<int> removedIndeces = new List<int>();
            bool cardsWereRemoved = RemoveExtraCards(ownedCards, ref removedIndeces);

            CardInstance[] carry = new CardInstance[ownedCards.Count];
            ownedCards.CopyTo(carry);
            foreach(CardInstance card in carry) if(_displayedCards.Where(x=> x.Key.GetComponent<CardObject>().CardInstanceRef == card).Count() > 0) ownedCards.Remove(card);

            foreach(CardInstance card in ownedCards)
            {
                int columnIndex = 0;
                int rowIndex = 0;

                if(cardsWereRemoved && removedIndeces.Count > 0)
                {
                    int index = removedIndeces.First();
                    removedIndeces.Remove(removedIndeces.First());
                    columnIndex = index % _columnTransforms.Length;
                    rowIndex = (index - (index % _columnTransforms.Length))/_columnTransforms.Length;
                }
                else if(_unfilledIndeces.Count > 0)
                {
                    int index = _unfilledIndeces.First();
                    _unfilledIndeces.Remove(_unfilledIndeces.First());
                    columnIndex = index % _columnTransforms.Length;
                    rowIndex = (index - (index % _columnTransforms.Length))/_columnTransforms.Length;
                }
                else
                {
                    int index = _displayedCards.Count;
                    columnIndex = (index % _columnTransforms.Length)+1;
                    rowIndex = (index - (index % _columnTransforms.Length))/_columnTransforms.Length;
                    if(columnIndex is 0) rowIndex++;
                }

                GameObject cardObject = Instantiate(_cardObjectPrefab, _columnTransforms[columnIndex]);
                cardObject.transform.localPosition = Vector2.zero;
                cardObject.GetComponent<CardObject>().Initialise(card);

                cardObject.transform.localPosition = new Vector2(cardObject.transform.localPosition.x, (-1) * _heightOffset * rowIndex);
                _displayedCards.Add(cardObject.transform, columnIndex * rowIndex);
                _columnCardCount[columnIndex]++;
            }
            int tallestColumnCount = 0;
            for(int i = 0; i < _columnCardCount.Length; i++)
                if(i > tallestColumnCount) tallestColumnCount = i;
            _scrollviewContentObj.GetComponent<RectTransform>().sizeDelta = new Vector2(_scrollviewContentObj.GetComponent<RectTransform>().sizeDelta.x, _heightOffset * tallestColumnCount);

            //! needs reworking to actually move cards to fill spots instead of leaving them empty till new cards get added
            if(removedIndeces.Count > 0)
                foreach(int i in removedIndeces)
                    _unfilledIndeces.Add(i);
        }

        private bool RemoveExtraCards(List<CardInstance> ownedCards, ref List<int> removedIndeces)
        {
            bool cardsWereRemoved = false;
            int removedCardsAmount = 0;

            List<CardInstance> displayedCards = _displayedCards.Keys.Select(x => x.GetComponent<CardObject>().CardInstanceRef).ToList<CardInstance>();

            foreach(CardInstance card in displayedCards)
            {
                if(!ownedCards.Contains(card))
                {
                    KeyValuePair<Transform, int> cardObject = _displayedCards.Where(x => x.Key.GetComponent<CardObject>().CardInstanceRef == card).FirstOrDefault();
                    _displayedCards.Remove(cardObject.Key);
                    removedIndeces.Add(cardObject.Value);
                    _columnCardCount[cardObject.Value % _columnTransforms.Length]--;
                    Destroy(cardObject.Key.gameObject);
                    removedCardsAmount++;
                    cardsWereRemoved = true;
                }
            }

            return cardsWereRemoved;
        }
        #endregion
    }
}
