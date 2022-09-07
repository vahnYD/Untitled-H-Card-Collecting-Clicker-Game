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
using TMPro;

namespace _Game.Scripts.Cards
{
    public class CardObject : MonoBehaviour
    {
        #region Properties
        [SerializeField] private CardInstance _card;
        public CardInstance CardInstanceRef => _card;
        [SerializeField] private SpriteList _rarityIconList = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _rarityIconImageObject = null;
        [SerializeField] private TMP_Text _soulValueText = null;
        [SerializeField] private TMP_Text _typeText = null;
        [SerializeField] private TMP_Text _flavourText = null;
        [SerializeField] private Transform _abilityContainerObj = null;
        [SerializeField] private TMP_Text _abilityText = null;
        private bool _isInitialised = false;
        #endregion

        #region Unity Event Functions
        #if UNITY_EDITOR
        private void Awake()
        {
            if(_rarityIconList is null || _cardArtImageObj is null || _nameText is null || _rarityIconImageObject is null || _soulValueText is null || _typeText is null || _flavourText is null || _abilityContainerObj is null || _abilityText is null)
                Debug.LogWarning("CardObject.cs of " + this.name + " is missing Object References.");
        }
        #endif

        private void OnEnable()
        {
            if(_isInitialised) _card.AbilityUpgradeEvent += CatchAbilityUpgrade;
        }

        private void OnDisable()
        {
            if(_isInitialised) _card.AbilityUpgradeEvent -= CatchAbilityUpgrade;
        }
        #endregion

        #region Methods
        public void Initialise(CardInstance card)
        {
            this.name = transform.parent.gameObject.name + "_" + card.Name;
            _card = card;
            _cardArtImageObj.sprite = card.CardArt;
            _nameText.text = card.Name;
            switch(card.CardRef.Rarity)
            {
                case Card.CardRarity.Common:
                    _rarityIconImageObject.sprite = _rarityIconList[0];
                    break;
                case Card.CardRarity.Rare:
                    _rarityIconImageObject.sprite = _rarityIconList[1];
                    break;
                case Card.CardRarity.VeryRare:
                    _rarityIconImageObject.sprite = _rarityIconList[2];
                    break;
                case Card.CardRarity.Special:
                    _rarityIconImageObject.sprite = _rarityIconList[3];
                    break;
            }
            _soulValueText.text = card.CardRef.SoulValue.ToString();
            _typeText.text = "[ " + card.CardRef.Type + " - STR: " + card.CardRef.Strength + " ]";
            _flavourText.text = card.CardRef.FlavourText;
            if(!card.HasAbility) _abilityContainerObj.gameObject.SetActive(false);
            else
            {
                _abilityText.text = card.AbilityText;

            }
            card.AbilityUpgradeEvent += CatchAbilityUpgrade;
            _isInitialised = true;
        }

        private void CatchAbilityUpgrade()
        {
            _abilityText.text = _card.AbilityText;
        }
        #endregion
    }
}
