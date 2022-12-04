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
    public class CurrencyUpdater : MonoBehaviour
    {
        #region Properties
        [SerializeField] private BigDoubleValue _coinAmount = null;
        [SerializeField] private LongValue _soulAmount = null;
        [SerializeField] private TMP_Text _coinDisplay = null;
        [SerializeField] private TMP_Text _soulDisplay = null;
        [SerializeField] private TMP_Text _soulPercent = null;
        [SerializeField, Range(0, 10)] private int _decimalAmount;
        private GameManager _gameManager;
        private long _maxSouls;
        #endregion

        #region Unity Event Functions
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _maxSouls = _gameManager.GameSettingsRef.MaxSoulsAtBase;
            #if UNITY_EDITOR
            if(_coinAmount is null || _soulAmount is null || _coinDisplay is null || _soulDisplay is null)
                Debug.LogWarning("CurrencyUpdate.cs is missing Object References.");
            #endif
            
            #if UNITY_EDITOR
            if(_coinDisplay != null && _coinAmount != null)
            #endif
                _coinDisplay.text = (_coinAmount.Value > 999999999) ? _coinAmount.Value.ToString("E4") : _coinAmount.Value.ToString("F0");

            #if UNITY_EDITOR
            if(_soulAmount != null && _soulDisplay != null && _soulPercent != null)
            {
            #endif
                _soulDisplay.text = _soulAmount.Value.ToString();
                _soulPercent.text = (((double)_soulAmount.Value)/_maxSouls).CutToDecimalAmount(_decimalAmount).ToString()+"%";
            #if UNITY_EDITOR
            }
            #endif
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if(_coinAmount != null)
            #endif
                _coinAmount.ValueChangedEvent += UpdateCoinText;

            #if UNITY_EDITOR
            if(_soulAmount != null)
            #endif
                _soulAmount.ValueChangedEvent += UpdateSoulText;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if(_coinAmount != null)
            #endif
                _coinAmount.ValueChangedEvent -= UpdateCoinText;

            #if UNITY_EDITOR
            if(_soulAmount != null)
            #endif
                _soulAmount.ValueChangedEvent -= UpdateSoulText;
        }

        private void UpdateCoinText(BigDouble newVal) => _coinDisplay.text = (newVal > 999999999) ? newVal.ToString("E4") : newVal.ToString("F0");

        private void UpdateSoulText(long newVal)
        {
            _soulDisplay.text = newVal.ToString();
            _soulPercent.text = (((double)newVal)/_maxSouls).CutToDecimalAmount(_decimalAmount).ToString()+"%";
        }
        #endregion
    }
}
