/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using _Game.Scripts.Extensions;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class GachaPullUiManager : MonoBehaviour
    {
        public static GachaPullUiManager Instance;
        #region Properties
        [SerializeField] private GameObject _cardObjectPrefab = null;
        [SerializeField] private Transform _gachaPullWindowObj = null;
        [SerializeField] private Button _singlePullButton = null;
        [SerializeField] private Button _tenPullButton = null;
        [SerializeField] private Button _closeGachaPullsButton = null;
        [SerializeField] private BoolValue _reachedFirstRoundCapFlag = null;
        [SerializeField] private BoolValue _tenPullDisabledFlag = null;
        [SerializeField] private LongValue _coinAmount = null;
        [SerializeField] private IntValue _singlePullCost = null;
        [SerializeField] private IntValue _tenPullCost = null;
        [SerializeField] private GachaPullDuplicateAnimationHandler[] _displayTenGachaPullsCoordinates;
        [SerializeField] private GachaPullDuplicateAnimationHandler _displaySingleGachaPullCoordinate;
        [SerializeField] private float _singlePullScaleMult = 2f;
        private bool _buttonGachaLock = false;
        private List<Transform> _displayedGachaPulls = new List<Transform>();
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            if(Instance != null && Instance != this) Destroy(this);
            else Instance = this;

            #if UNITY_EDITOR
            if(_gachaPullWindowObj != null)
            #endif
                _gachaPullWindowObj.gameObject.SetActive(false);

            #if UNITY_EDITOR
            if(_cardObjectPrefab is null || _gachaPullWindowObj is null || _singlePullButton is null || _tenPullButton is null || _closeGachaPullsButton is null || _reachedFirstRoundCapFlag is null || _tenPullDisabledFlag is null || _coinAmount is null || _singlePullCost is null || _tenPullCost is null)
                Debug.LogWarning("GachaPullUiManager.cs is missing Object References.");
            #endif

            if(
                #if UNITY_EDITOR
                _tenPullDisabledFlag != null &&
                #endif
                _tenPullDisabledFlag
            ) _tenPullButton.interactable = false;

            #if UNITY_EDITOR
            if(_reachedFirstRoundCapFlag != null && _tenPullDisabledFlag != null && _singlePullCost != null && _tenPullCost != null && _coinAmount != null && _singlePullButton != null && _tenPullButton != null)
            #endif
                CatchCoinAmountChange(_coinAmount.Value);
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if(_coinAmount != null)
            #endif
                _coinAmount.ValueChangedEvent += CatchCoinAmountChange;

            #if UNITY_EDITOR
            if(_reachedFirstRoundCapFlag != null)
            #endif
                _reachedFirstRoundCapFlag.ValueChangedEvent += CatchFirstRoundCapFlag;

            #if UNITY_EDITOR
            if(_tenPullDisabledFlag != null)
            #endif
                _tenPullDisabledFlag.ValueChangedEvent += CatchTenPullDisabledFlag;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if(_coinAmount != null)
            #endif
                _coinAmount.ValueChangedEvent -= CatchCoinAmountChange;

            #if UNITY_EDITOR
            if(_reachedFirstRoundCapFlag != null)
            #endif
                _reachedFirstRoundCapFlag.ValueChangedEvent -= CatchFirstRoundCapFlag;

            #if UNITY_EDITOR
            if(_tenPullDisabledFlag != null)
            #endif
                _tenPullDisabledFlag.ValueChangedEvent -= CatchTenPullDisabledFlag;
        }
        #endregion
        
        #region Methods
        private void CatchCoinAmountChange(long newVal)
        {
            if(newVal >= _singlePullCost.Value && !_reachedFirstRoundCapFlag.Value && !_buttonGachaLock) _singlePullButton.interactable = true;
            else _singlePullButton.interactable = false;

            if(newVal >= _tenPullCost.Value && !_reachedFirstRoundCapFlag.Value && !_tenPullDisabledFlag && !_buttonGachaLock) _tenPullButton.interactable = true;
            else _tenPullButton.interactable = false;
        }

        private void CatchFirstRoundCapFlag(bool newVal)
        {
            if(_coinAmount.Value > _singlePullCost.Value && !newVal && !_buttonGachaLock) _singlePullButton.interactable = true;
            else _singlePullButton.interactable = false;
            if(_coinAmount.Value > _tenPullCost.Value && !_tenPullDisabledFlag.Value && !newVal && !_buttonGachaLock) _tenPullButton.interactable = true;
            else _tenPullButton.interactable = false; 
        }

        private void CatchTenPullDisabledFlag(bool newVal)
        {
            if(_coinAmount.Value > _tenPullCost.Value && !_reachedFirstRoundCapFlag.Value && !newVal && !_buttonGachaLock) _tenPullButton.interactable = true;
            else _tenPullButton.interactable = false;
        }

        public void SingleGachaPull() => GachaPull(false);
        public void TenGachaPull() => GachaPull(true);

        private void GachaPull(bool isTenPull) => GachaAnimationTrigger(GameManager.Instance.GachaPull(isTenPull), isTenPull);

        private void GachaAnimationTrigger(Dictionary<CardInstance, bool> pulledCards, bool isTenPull)
        {
            _buttonGachaLock = true;
            _singlePullButton.interactable = false;
            _tenPullButton.interactable = false;
            _closeGachaPullsButton.interactable = false;

            if(!isTenPull)
            {
                GameObject cardObj = Instantiate(_cardObjectPrefab, _displaySingleGachaPullCoordinate.transform);
                _displayedGachaPulls.Add(cardObj.transform);
                _displayedGachaPulls[0].localScale = new Vector3(1,1,1);
                _displayedGachaPulls[0].localPosition = Vector3.zero;
                bool isDupe = false;
                Card.CardRarity rarity = Card.CardRarity.Common;
                foreach(KeyValuePair<CardInstance, bool> pair in pulledCards)
                {
                    cardObj.GetComponent<CardObject>().Initialise(pair.Key);
                    rarity = pair.Key.CardRef.Rarity;
                    isDupe = pair.Value;
                }
                _gachaPullWindowObj.gameObject.SetActive(true);
                if(isDupe) _displaySingleGachaPullCoordinate.Duplicate(rarity);
                _closeGachaPullsButton.interactable = true;
                return;
            }

            foreach(KeyValuePair<CardInstance, bool> pair in pulledCards)
            {
                GameObject cardObj = Instantiate(_cardObjectPrefab, _gachaPullWindowObj);
                cardObj.transform.localPosition = Vector3.zero;
                _displayedGachaPulls.Add(cardObj.transform);
                cardObj.GetComponent<CardObject>().Initialise(pair.Key);
            }
            _gachaPullWindowObj.gameObject.SetActive(true);
            StartCoroutine(GachaAnimCoroutine(pulledCards));
        }

        public void GachaCloseBtn()
        {
            _gachaPullWindowObj.gameObject.SetActive(false);
            for(int i = _displayedGachaPulls.Count - 1; i >= 0; i--)
            {
                Transform ob = _displayedGachaPulls[i];
                _displayedGachaPulls.Remove(ob);
                Destroy(ob.gameObject);
            }
            foreach(GachaPullDuplicateAnimationHandler handler in _displayTenGachaPullsCoordinates) handler.Reset();
            _buttonGachaLock = false;
            if(_coinAmount.Value > _singlePullCost.Value && !_reachedFirstRoundCapFlag.Value) _singlePullButton.interactable = true;
            if(_coinAmount.Value > _tenPullCost.Value && !_reachedFirstRoundCapFlag.Value && !_tenPullDisabledFlag.Value) _tenPullButton.interactable = true;
        }

        private void TriggerIfDupe(bool isDupe, int index, Card.CardRarity rarity)
        {
            if(isDupe) _displayTenGachaPullsCoordinates[index].Duplicate(rarity);
        }
        #endregion

        private IEnumerator GachaAnimCoroutine(Dictionary<CardInstance, bool> pulledCards)
        {
            int counter = 0;
            foreach(Transform obj in _displayedGachaPulls)
            {
                CardInstance card = obj.gameObject.GetComponent<CardObject>().CardInstanceRef;
                Card.CardRarity rarity = card.CardRef.Rarity;
                bool isDupe = pulledCards[card];
                obj.SetParent(_displayTenGachaPullsCoordinates[counter].transform);
                obj.DOLocalMove(Vector3.zero, 0.2f).OnComplete(()=>TriggerIfDupe(isDupe, counter, rarity)).timeScale=1;
                counter++;
                yield return new WaitForSecondsRealtime(0.1f);
            }
            yield return new WaitForSecondsRealtime(0.2f);
            _closeGachaPullsButton.interactable = true;
        }
    }
}
