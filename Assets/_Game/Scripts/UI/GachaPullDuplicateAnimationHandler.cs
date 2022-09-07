/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using _Game.Scripts.Cards;

namespace _Game.Scripts.UI
{
    public class GachaPullDuplicateAnimationHandler : MonoBehaviour
    {
        #region Properties
        [SerializeField] private Transform _duplicateAnimObj = null;
        [SerializeField] private TMP_Text _obtainedCrystalsText = null;
        private GameSettingsScriptableObject _gameSettings = null;
        #endregion

        private void Awake()
        {
            _gameSettings = GameManager.Instance.GameSettingsRef;

            #if UNITY_EDITOR
            if(_duplicateAnimObj is null || _obtainedCrystalsText is null || _gameSettings is null)
                Debug.LogWarning("GachaPullDuplicateAnimationHandle.cs at " + this.name + " is missing Object references.");
            #endif
        }
        
        #region Methods
        public void Duplicate(Card.CardRarity rarity)
        {
            int crystalAmount = 1;
            switch(rarity)
            {
                case Card.CardRarity.Common:
                    crystalAmount = _gameSettings.CardDismantleCrystalValues[0];
                    break;
                case Card.CardRarity.Rare:
                    crystalAmount = _gameSettings.CardDismantleCrystalValues[1];
                    break;
                case Card.CardRarity.VeryRare:
                    crystalAmount = _gameSettings.CardDismantleCrystalValues[2];
                    break;
                case Card.CardRarity.Special:
                    crystalAmount = _gameSettings.CardDismantleCrystalValues[3];
                    break;
            }
            _obtainedCrystalsText.text = crystalAmount.ToString();
            _duplicateAnimObj.GetComponent<RectTransform>().DOSizeDelta(new Vector2(110, 152), 0.05f);
        }

        public void Reset()
        {
            _duplicateAnimObj.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 0);
        }
        #endregion
    }
}
