/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using UnityEngine;
using TMPro;

namespace _Game.Scripts.UI
{
    public class CurrencyUpdater : MonoBehaviour
    {
        #region Properties
        [SerializeField] private TMP_Text _coinDisplay;
        [SerializeField] private TextMeshProUGUI _soulDisplay;
        private GameManager _gameManager;
        #endregion

        #region Unity Event Functions
        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            _coinDisplay.text = _gameManager.CoinTotal.ToString();
            _soulDisplay.text = _gameManager.SoulTotal.ToString();
        }
        #endregion
    }
}
