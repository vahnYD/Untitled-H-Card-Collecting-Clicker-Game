/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using UnityEngine;
using TMPro;
using _Game.Scripts.Extensions;

namespace _Game.Scripts.UI
{
    public class LewdPointDisplay : MonoBehaviour
    {
        #region Properties
        [SerializeField] private IntValue _lewdPointAmount = null;
        [SerializeField] private TMP_Text _calcText = null;
        [SerializeField] private TMP_Text _resultText = null;
        private GameManager _gameManager;
        #endregion

        #region Unity Event Functions
        #if UNITY_EDITOR
        private void Awake()
        {
            if(_lewdPointAmount is null || _calcText is null || _resultText is null)
                Debug.LogWarning("LewdPointDisplay.cs is missing Object References");
        }
        #endif

        private void Start()
        {
            _gameManager = GameManager.Instance;
            #if UNITY_EDITOR
            if(_lewdPointAmount != null)
            #endif
                CatchLewdPointValueChange(_lewdPointAmount.Value);
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if(_lewdPointAmount != null)
            {
            #endif
                _lewdPointAmount.ValueChangedEvent += CatchLewdPointValueChange;
                CatchLewdPointValueChange(_lewdPointAmount.Value);
            #if UNITY_EDITOR
            }
            #endif
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if(_lewdPointAmount != null)
            #endif
                _lewdPointAmount.ValueChangedEvent -= CatchLewdPointValueChange;
        }
        #endregion
        
        #region Methods
        private void CatchLewdPointValueChange(int newVal)
        {
            if(_gameManager is null) return;
            _calcText.text = _gameManager.LewdPointStrength.ToString() + " * (x" + _gameManager.LewdPointTypeMultiplier.ToString() + ") * (x" + _gameManager.LewdPointSoulMultiplier.ToString() + ") =";
            _resultText.text = newVal.ToString();
        }
        #endregion
    }
}
