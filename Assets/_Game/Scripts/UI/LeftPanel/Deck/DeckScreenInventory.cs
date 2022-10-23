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
        [SerializeField] private DeckScreenHandler _deckScreenHandler = null;
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private IntValue _lewdPointValueObj = null;
        [SerializeField] private Transform _scrollviewContentObj = null;
        [SerializeField] private Transform[] _columnTransforms = new Transform[4];
        [SerializeField] private float _heightOffset = 182f;
        private int[] _columnCardCount = new int[4];
        private Dictionary<Transform, int> _displayedCards = new Dictionary<Transform, int>();
        private GameManager _gameManager = null;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_deckScreenHandler is null || _cardObjectPrefab is null || _lewdPointValueObj is null || _scrollviewContentObj is null)
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

        ///<summary>
        ///Updates the inventory portion of the deck tab. Deletes card objects that arent owned anymore
        ///and spawns new objects for cards that aren't being displayed yet.
        ///</summary>
        private void Populate()
        {
            List<CardInstance> ownedCards = _gameManager.GetOwnedCards().Where(x => x.HasAbility).ToList();
            if(ownedCards.Count is 0) return;

            //removes any displayed cards that arent owned anymore and saves their positions
            List<int> removedIndeces = new List<int>();
            bool cardsWereRemoved = RemoveExtraCards(ownedCards, ref removedIndeces);

            //removes already displayed cards from the list of cards to be spawned
            CardInstance[] carry = new CardInstance[ownedCards.Count];
            ownedCards.CopyTo(carry);
            foreach(CardInstance card in carry)
            {
                if(_displayedCards.Where(x=> x.Key.GetComponent<CardObject>().CardInstanceRef.CardArt == card.CardArt).Count() > 0) ownedCards.Remove(card);
            }

            //spawns any newly owned cards
            //position priority is given to spots were previously displayed cards were deleted in
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
                else
                {
                    int index = _displayedCards.Count;
                    columnIndex = index % _columnTransforms.Length;
                    rowIndex = (index - (index % _columnTransforms.Length))/_columnTransforms.Length;
                    if(columnIndex is 0 && _displayedCards.Count > 3) rowIndex++;
                }

                GameObject cardObject = Instantiate(_cardObjectPrefab, _columnTransforms[columnIndex]);
                cardObject.transform.localPosition = Vector2.zero;
                cardObject.GetComponent<CardObject>().Initialise(card, true, clickExecute: () => _deckScreenHandler.ClickHandling(card));

                cardObject.transform.localPosition = new Vector2(cardObject.transform.localPosition.x, (-1) * (-_heightOffset) * rowIndex);
                _displayedCards.Add(cardObject.transform, columnIndex * rowIndex);
                _columnCardCount[columnIndex] += 1;
            }

            //fills any leftover empty positions by moving card objects in positions past them backwards
            if(removedIndeces.Count > 0)
                FillEmptyIndeces(removedIndeces);

            //adjusts the scroll view content object size to fit the amount of cards being displayed
            int tallestColumnCount = 0;
            for(int i = 0; i < _columnCardCount.Length; i++)
                if(tallestColumnCount < _columnCardCount[i]) tallestColumnCount = _columnCardCount[i];
            _scrollviewContentObj.GetComponent<RectTransform>().sizeDelta = new Vector2(_scrollviewContentObj.GetComponent<RectTransform>().sizeDelta.x, _heightOffset * tallestColumnCount);
        }

        ///<summary>
        ///Removes any card objects that arent supposed to be shown anymore, and saves the index of their position.
        ///</summary>
        ///<param name="ownedCards">List of CardInstance Objects that is the List of cards that are supposed to be displayed.</param>
        ///<param name="removedIndeces">List of ints as reference to save the indeces of positions of any removed cards to.</param>
        ///<returns>Returns true if any cards were removed, false if there were no cards that needed removing.</returns>
        private bool RemoveExtraCards(List<CardInstance> ownedCards, ref List<int> removedIndeces)
        {
            bool cardsWereRemoved = false;

            List<CardInstance> displayedCards = _displayedCards.Keys.Select(x => x.GetComponent<CardObject>().CardInstanceRef).ToList<CardInstance>();

            foreach(CardInstance card in displayedCards)
            {
                if(!ownedCards.Contains(card))
                {
                    KeyValuePair<Transform, int> cardObject = _displayedCards.Where(x => x.Key.GetComponent<CardObject>().CardInstanceRef.CardArt == card.CardArt).FirstOrDefault();
                    _displayedCards.Remove(cardObject.Key);
                    removedIndeces.Add(cardObject.Value);
                    _columnCardCount[cardObject.Value % _columnTransforms.Length]--;
                    Destroy(cardObject.Key.gameObject);
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
            //sorts the displayed cards by the index of their positions
            List<KeyValuePair<Transform, int>> sortedDisplayedCards = _displayedCards.ToList();
            sortedDisplayedCards.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            emptyIndeces.Sort((i, j) => i.CompareTo(j));

            //moves all cards after the first empty position backwards by 1 for every empty empty position
            foreach(int i in emptyIndeces)
            {
                for(int j = i; i < sortedDisplayedCards.Count; j++)
                {
                    KeyValuePair<Transform, int> pair = sortedDisplayedCards[j];
                    int columnIndex = pair.Value % _columnTransforms.Length;
                    int rowIndex = (pair.Value - (pair.Value % _columnTransforms.Length)) / _columnTransforms.Length;
                    _columnCardCount[columnIndex]--;
                    columnIndex--;
                    if(columnIndex < 0)
                    {
                        columnIndex = _columnTransforms.Length - 1;
                        rowIndex--;
                    }
                    pair.Key.SetParent(_columnTransforms[columnIndex]);
                    pair.Key.localPosition = new Vector2(0, _heightOffset * rowIndex);
                    _columnCardCount[columnIndex]++;
                    _displayedCards[pair.Key] = columnIndex * rowIndex;
                }
            }
        }
        #endregion
    }
}
