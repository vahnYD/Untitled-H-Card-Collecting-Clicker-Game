/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using UnityEngine;
using TMPro;
using BreakInfinity;
using _Game.Scripts.Extensions;

namespace _Game.Scripts.UI
{
    public class GachaCurrencyUpdater : MonoBehaviour
    {
        #region Properties
        [SerializeField] private IntValue _crystalAmount = null;
        [SerializeField] private IntValue _starAmount = null;
        [SerializeField] private ModifiableBigDoubleValue _singleRollCost = null;
        [SerializeField] private ModifiableBigDoubleValue _tenRollCost = null;
        [SerializeField] private TMP_Text _crystalText = null;
        [SerializeField] private TMP_Text _starText = null;
        [SerializeField] private TMP_Text _singleRollCostText = null;
        [SerializeField] private TMP_Text _tenRollCostText = null;
        #endregion

        #region Unity Event Functions
        private void Start()
        {
            #if UNITY_EDITOR
            if(_crystalAmount is null || _starAmount is null || _singleRollCost is null || _tenRollCost is null || _crystalText is null || _starText is null || _singleRollCostText is null || _tenRollCostText is null)
                Debug.LogWarning("GachaCurrencyUpdate.cs is missing Object References.");
            #endif

            #if UNITY_EDITOR
            if(_crystalAmount != null && _crystalText != null)
            #endif
                _crystalText.text = _crystalAmount.Value.ToString();

            #if UNITY_EDITOR
            if(_starAmount != null && _starText != null)
            #endif
                _starText.text = _starAmount.Value.ToString();

            #if UNITY_EDITOR
            if(_singleRollCost != null && _singleRollCostText != null)
            #endif
                _singleRollCostText.text = (_singleRollCost.ModifiedValue > 999999999) ? _singleRollCost.ModifiedValue.ToString("E4") : _singleRollCost.ModifiedValue.ToString("F0");

            #if UNITY_EDITOR
            if(_tenRollCostText != null && _tenRollCost != null)
            #endif
                _tenRollCostText.text = (_tenRollCost.ModifiedValue > 999999999) ? _tenRollCost.ModifiedValue.ToString("E4") : _tenRollCost.ModifiedValue.ToString("F0");
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if(_crystalAmount != null)
            #endif
                _crystalAmount.ValueChangedEvent += UpdateCrystalText;

            #if UNITY_EDITOR
            if(_starAmount != null)
            #endif
                _starAmount.ValueChangedEvent += UpdateStarText;

            #if UNITY_EDITOR
            if(_singleRollCost != null)
            #endif
                _singleRollCost.ModifiedValueChangedEvent += UpdateSingleRollGachaCost;

            #if UNITY_EDITOR
            if(_tenRollCost != null)
            #endif
                _tenRollCost.ModifiedValueChangedEvent += UpdateTenRollGachaCost;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if(_crystalAmount != null)
            #endif
                _crystalAmount.ValueChangedEvent -= UpdateCrystalText;

            #if UNITY_EDITOR
            if(_starAmount != null)
            #endif
                _starAmount.ValueChangedEvent -= UpdateStarText;

            #if UNITY_EDITOR
            if(_singleRollCost != null)
            #endif
                _singleRollCost.ModifiedValueChangedEvent -= UpdateSingleRollGachaCost;

            #if UNITY_EDITOR
            if(_tenRollCost != null)
            #endif
                _tenRollCost.ModifiedValueChangedEvent -= UpdateTenRollGachaCost;
        }
        #endregion
        
        #region Methods
        private void UpdateCrystalText(int newVal) => _crystalText.text = newVal.ToString();

        private void UpdateStarText(int newVal) => _starText.text = newVal.ToString();

        private void UpdateSingleRollGachaCost(BigDouble newVal) => _singleRollCostText.text = (newVal > 999999999) ? newVal.ToString("E4") : newVal.ToString("F0");

        private void UpdateTenRollGachaCost(BigDouble newVal) => _tenRollCostText.text = (newVal > 999999999) ? newVal.ToString("E4") : newVal.ToString("F0");
        #endregion
    }
}
