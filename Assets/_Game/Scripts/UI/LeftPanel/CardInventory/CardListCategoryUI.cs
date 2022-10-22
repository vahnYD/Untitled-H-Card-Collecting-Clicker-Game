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
using _Game.Scripts.Extensions;

namespace _Game.Scripts.UI
{
    public class CardListCategoryUI : MonoBehaviour
    {
        #region Properties
        [SerializeField] private Card.CardType _type;
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private IntValue _lewdPointObj = null;
        [SerializeField] private Scrollbar _scrollbar = null;
        [SerializeField] private RectTransform _scrollviewContent = null;
        [SerializeField] private Button _buttonRight = null;
        private ScrollViewButtonPress _buttonRightPressCheck = null;
        private bool _scrollingRight = false;
        [SerializeField] private Button _buttonLeft = null;
        private ScrollViewButtonPress _buttonLeftPressCheck = null;
        private bool _scrollingLeft = false;
        [SerializeField] private TMP_Text _typeStrengthText = null;
        [SerializeField] private Transform _firstCardSpot = null;
        [SerializeField] private Transform _offsetTransform = null;
        private float _cardSpotOffset = 0;
        private GameManager _gameManager = null;
        private Dictionary<Transform, int> _displayedCards = new Dictionary<Transform, int>();
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_cardObjectPrefab is null || _lewdPointObj is null || _scrollbar is null || _scrollviewContent is null || _buttonRight is null || _buttonLeft is null || _typeStrengthText is null || _firstCardSpot is null || _offsetTransform is null)
                Debug.LogWarning("CardListCategory.cs of type " + _type + " is missing Object References.");
            
            if(_firstCardSpot != null && _offsetTransform != null)
            #endif
                _cardSpotOffset= _firstCardSpot.transform.localPosition.x - _offsetTransform.localPosition.x;

            #if UNITY_EDITOR
            if(_buttonRight != null && _buttonLeft != null)
            {
            #endif
                _buttonLeftPressCheck = _buttonLeft.GetComponent<ScrollViewButtonPress>();
                _buttonRightPressCheck = _buttonRight.GetComponent<ScrollViewButtonPress>();
            #if UNITY_EDITOR
            }
            #endif
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            CatchLewdPointChange(0);
            CatchScrollbarValueChange(_scrollbar.value);
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if(_lewdPointObj != null && _buttonLeftPressCheck != null && _buttonRightPressCheck != null && _scrollbar != null)
            {
            #endif
                _lewdPointObj.ValueChangedEvent += CatchLewdPointChange;
                _buttonLeftPressCheck.PressStateChangedEvent += ScrollLeft;
                _buttonRightPressCheck.PressStateChangedEvent += ScrollRight;
                _scrollbar.onValueChanged.AddListener((float val) => CatchScrollbarValueChange(val));
            #if UNITY_EDITOR
            }
            #endif
            CatchLewdPointChange(0);
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if(_lewdPointObj != null && _buttonLeftPressCheck != null && _buttonRightPressCheck != null && _scrollbar != null)
            {
            #endif
                _lewdPointObj.ValueChangedEvent -= CatchLewdPointChange;
                _buttonLeftPressCheck.PressStateChangedEvent -= ScrollLeft;
                _buttonRightPressCheck.PressStateChangedEvent -= ScrollRight;
                _scrollbar.onValueChanged.RemoveListener((float val) => CatchScrollbarValueChange(val));
            #if UNITY_EDITOR
            }
            #endif
        }
        #endregion
        
        #region Methods
        private void CatchLewdPointChange(int newVal)
        {
            if(_gameManager is null) return;
            Populate();
            //checks for game manager state to get around an OnEnable null exception.
        }

        private void CatchScrollbarValueChange(float newVal)
        {
            if(newVal >= 1) _buttonRight.gameObject.SetActive(false);
            else if(newVal <= 0) _buttonLeft.gameObject.SetActive(false);
            else
            {
                _buttonRight.gameObject.SetActive(true);
                _buttonLeft.gameObject.SetActive(true);
            }
        }

        ///<summary>
        ///Updates the displayed card objects, deleting wrong ones and spawning missing ones.
        ///</summary>
        private void Populate()
        {
            List<CardInstance> cards = _gameManager.GetOwnedCardsOfType(_type);
            if(cards.Count is 0) return;

            //Removing displayed cards that arent contained in the cart list anymore and saving their positions.
            List<int> removedIndeces = new List<int>();
            bool cardsWereRemoved = RemoveExtraCards(cards, ref removedIndeces);

            //Adding up the strength of all cards of the type to display it and removing any cards from the list of cards that are already being displayed
            int strength = 0;
            CardInstance[] carry = new CardInstance[cards.Count];
            cards.CopyTo(carry);
            foreach(CardInstance card in carry)
            {
                strength += card.CardRef.Strength;
                if(_displayedCards.Where(x=>x.Key.GetComponent<CardObject>().CardInstanceRef.CardArt == card.CardArt).Count() > 0) cards.Remove(card);
            }
            _typeStrengthText.text = strength.ToString();

            //instantiating the new card objects
            //using the places of cards that were removed with priority of applicable
            foreach(CardInstance card in cards)
            {
                GameObject cardObject = Instantiate(_cardObjectPrefab, _firstCardSpot);
                cardObject.transform.localPosition = Vector2.zero;
                int offsetMult = 0;
                if(cardsWereRemoved && removedIndeces.Count > 0)
                {
                    offsetMult = removedIndeces.First();
                    removedIndeces.Remove(removedIndeces.First());
                }
                else offsetMult = _displayedCards.Count;
                cardObject.transform.localPosition = new Vector2((_cardSpotOffset + cardObject.GetComponent<RectTransform>().sizeDelta.x/2) * offsetMult, cardObject.transform.localPosition.y);
                cardObject.GetComponent<CardObject>().Initialise(card, true, true);
                _displayedCards.Add(cardObject.transform, offsetMult);
            }

            //filling any positions that are left empty because not enough new cards needed spawning to fill them all
            //by moving the cards after the unfilled positions backwards
            if(removedIndeces.Count > 0)
                FillEmptyIndeces(removedIndeces);

            //adjusting the scroll view content object size to fit the amount of cards actually spawned
            RectTransform _firstCardSpotRect = _firstCardSpot.GetComponent<RectTransform>();
            _scrollviewContent.sizeDelta = new Vector2( (_firstCardSpotRect.sizeDelta.x + (2*(_cardSpotOffset-(_firstCardSpotRect.sizeDelta.x/2))))*_displayedCards.Count, _scrollviewContent.sizeDelta.y);
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
            //sorts the list of displayed cards by their indeces
            List<KeyValuePair<Transform, int>> sortedDisplayedCards = _displayedCards.ToList();
            sortedDisplayedCards.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            emptyIndeces.Sort((i, j) => i.CompareTo(j));

            //moves all cards with positions after the first empty indeces back by 1 for each empty index
            foreach(int i in emptyIndeces)
            {
                for(int j = i; j < sortedDisplayedCards.Count; j++)
                {
                    KeyValuePair<Transform, int> pair = sortedDisplayedCards[j];
                    pair.Key.localPosition = new Vector2(pair.Key.localPosition.x - (_cardSpotOffset + pair.Key.GetComponent<RectTransform>().sizeDelta.x), pair.Key.localPosition.y);
                    _displayedCards[pair.Key]--;
                }
            }
        }

        private void ScrollRight(bool newVal)
        {
            if(!newVal) _scrollingRight = false;
            else if(newVal && (_scrollingLeft || _scrollingRight)) _scrollingRight = true;
            else
            {
                _scrollingRight = true;
                StartCoroutine(ScrollingCoroutine());
            }
        }

        private void ScrollLeft(bool newVal)
        {
            if(!newVal) _scrollingLeft = false;
            else if(newVal && (_scrollingLeft || _scrollingRight)) _scrollingLeft = true;
            else
            {
                _scrollingLeft = true;
                StartCoroutine(ScrollingCoroutine());
            }
        }


        #endregion

        private IEnumerator ScrollingCoroutine()
        {
            for(;;)
            {
                yield return new WaitForSecondsRealtime(0.05f);
                if(_scrollingLeft) _scrollbar.value -= 0.1f;
                if(_scrollingRight) _scrollbar.value += 0.1f;
                if(!_scrollingLeft && !_scrollingRight)break;
            }
        }
    }
}
