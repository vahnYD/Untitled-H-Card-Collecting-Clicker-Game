/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace _Game.Scripts.UI
{
    public class DeckSizeDisplay : MonoBehaviour
    {
        #region Properties
        [SerializeField] private DeckScreenHandler _deckScreenHandler = null;
        [SerializeField] private TMP_Text _deckSizeText = null;
        [SerializeField] private TMP_Text _specialCardsAmountText = null;
        [SerializeField] private TMP_Text _veryRareCardsAmountText = null;
        [SerializeField] private TMP_Text _rareCardsAmountText = null;
        private GameSettingsScriptableObject _gameSettings = null;
        #endregion

        #region Unity Event Functions
        #if UNITY_EDITOR
        private void Awake()
        {
            if(_deckScreenHandler is null || _deckSizeText is null || _specialCardsAmountText is null || _veryRareCardsAmountText is null || _rareCardsAmountText is null)
                Debug.LogWarning("DeckSizeDisplay.cs is missing Object References");
        }
        #endif

        private void Start()
        {
            _gameSettings = GameManager.Instance.GameSettingsRef;
            UpdateText(0,0,0,0);
        }

        private void OnEnable()
        {
            _deckScreenHandler.SelectedCardsChangeEvent += UpdateText;
        }

        private void OnDisable()
        {
            _deckScreenHandler.SelectedCardsChangeEvent -= UpdateText;
        }
        #endregion
        
        #region Methods
        public void UpdateText(int deckCount, int rareCardsAmount, int veryRareCardsAmount, int specialCardsAmount)
        {
            int deckSize = _gameSettings.DefaultDeckSize + _gameSettings.DeckSizeIncreasePerVeryRare * veryRareCardsAmount + _gameSettings.DeckSizeIncreasePerSpecial * specialCardsAmount;
            //int veryCardCardsMaxAmount = _gameSettings.MaximumVRareCardsAllowedInDeckAtBase;
            //int specialCardsMaxAmount = _gameSettings.MaximumSpecialCardsAllowedInDeckAtBase;

            _deckSizeText.text = deckCount.ToString() + "/" + deckSize.ToString();
            _rareCardsAmountText.text = rareCardsAmount.ToString();
            _veryRareCardsAmountText.text = veryRareCardsAmount.ToString();
            _specialCardsAmountText.text = specialCardsAmount.ToString();
        }
        #endregion
    }
}