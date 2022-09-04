/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _Game.Scripts.Extensions;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class GachaPullUiManager : MonoBehaviour
    {
        public static GachaPullUiManager Instance;
        #region Properties
        [SerializeField] private Button _singlePullButton = null;
        [SerializeField] private Button _tenPullButton = null;
        [SerializeField] private BoolValue _reachedFirstRoundCapFlag = null;
        [SerializeField] private BoolValue _tenPullDisabledFlag = null;
        [SerializeField] private LongValue _coinAmount = null;
        [SerializeField] private IntValue _singlePullCost = null;
        [SerializeField] private IntValue _tenPullCost = null;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            if(Instance != null && Instance != this) Destroy(this);
            else Instance = this;

            #if UNITY_EDITOR
            if(_singlePullButton is null || _tenPullButton is null || _reachedFirstRoundCapFlag is null || _tenPullDisabledFlag is null || _coinAmount is null || _singlePullCost is null || _tenPullCost is null)
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
            if(newVal > _singlePullCost.Value && !_reachedFirstRoundCapFlag.Value) _singlePullButton.interactable = true;
            else _singlePullButton.interactable = false;

            if(newVal > _tenPullCost.Value && !_reachedFirstRoundCapFlag.Value && !_tenPullDisabledFlag) _tenPullButton.interactable = true;
            else _tenPullButton.interactable = false;
        }

        private void CatchFirstRoundCapFlag(bool newVal)
        {
            _singlePullButton.interactable = !newVal;
            if(!_tenPullDisabledFlag.Value && !newVal) _tenPullButton.interactable = true;
            else _tenPullButton.interactable = false; 
        }

        private void CatchTenPullDisabledFlag(bool newVal)
        {
            if(!_reachedFirstRoundCapFlag.Value && !newVal) _tenPullButton.interactable = true;
            else _tenPullButton.interactable = false;
        }

        public void SingleGachaPull() => GachaPull(false);
        public void TenGachaPull() => GachaPull(true);

        private void GachaPull(bool isTenPull) => GachaAnimationTrigger(GameManager.Instance.GachaPull(isTenPull));

        private void GachaAnimationTrigger(Dictionary<CardInstance, bool> pulledCards)
        {
            //TODO
        }
        #endregion
    }
}
