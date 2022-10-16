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
using TMPro;
using _Game.Scripts.Extensions;
using _Game.Scripts.UI;

namespace _Game.Scripts.Cards
{
    public class CardObject_Selection : MonoBehaviour
    {
        #region Properties
        [SerializeField] private CardInstance _card;
        public CardInstance CardInstanceRef => _card;
        [SerializeField] private SpriteList _rarityIconList = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _rarityIconImageObj = null;
        [SerializeField] private TMP_Text _soulValueText = null;
        [SerializeField] private TMP_Text _typeText = null;
        [SerializeField] private TMP_Text _flavourText = null;
        [SerializeField] private Transform _abilityContainerObj = null;
        [SerializeField] private TMP_Text _abilityText = null;
        [SerializeField] private Transform _selectionIndicatorTransform = null;
        [SerializeField] private TMP_Text _cooldownText = null;
        private bool _isInitialised = false;
        private bool _isCooldownRelated = false;
        private bool _isSelected = false;
        private float _remainingCD = 0f;
        private float _cdReductionAmount = 0f;
        private bool _cdReductionIsFlat = true;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_rarityIconList is null || _cardArtImageObj is null || _nameText is null || _rarityIconImageObj is null || _soulValueText is null || _typeText is null || _flavourText is null || _abilityContainerObj is null || _abilityText is null || _selectionIndicatorTransform is null || _cooldownText is null)
                Debug.LogWarning("CardObject_Selection.cs of " + this.name + " is missing Object References.");
            #endif
        }
        #endregion
        
        #region Methods
        public void Initialise(CardInstance card, bool isCooldownRelated = false, bool cdReductionIsFlat = true, float cdReductionAmount = 0f)
        {
            if(_isInitialised) return;
            this.name = card.Name + "_" + transform.parent.gameObject.name;
            _card = card;
            _cardArtImageObj.sprite = card.CardArt;
            _nameText.text = card.Name;
            switch(card.CardRef.Rarity)
            {
                case Card.CardRarity.Common:
                    _rarityIconImageObj.sprite = _rarityIconList[0];
                    break;
                case Card.CardRarity.Rare:
                    _rarityIconImageObj.sprite = _rarityIconList[1];
                    break;
                case Card.CardRarity.VeryRare:
                    _rarityIconImageObj.sprite = _rarityIconList[2];
                    break;
                case Card.CardRarity.Special:
                    _rarityIconImageObj.sprite = _rarityIconList[3];
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
            _isInitialised = true;
            _isCooldownRelated = isCooldownRelated;
            float? remainingCD = CardCooldownManager.Instance.GetRemainingCooldownForCard(card);
            if(!_isCooldownRelated || remainingCD is null) _cooldownText.gameObject.SetActive(false);
            else
            {
                _remainingCD = (float)remainingCD;
                _cdReductionIsFlat = cdReductionIsFlat;
                StartCoroutine(CooldownTextUpdatingCoroutine());
            }
        }

        public void Click()
        {
            _isSelected = SelectionWindowManager.Instance.CardClicked(_card);
            if(_isSelected)
            {
                _selectionIndicatorTransform.gameObject.SetActive(true);
                if(_isCooldownRelated)
                {
                    _remainingCD = (_cdReductionIsFlat) ? _remainingCD - _cdReductionAmount : _remainingCD * (1 - _cdReductionAmount);
                    UpdateCooldownText();
                }
            }
            else
            {
                _selectionIndicatorTransform.gameObject.SetActive(false);
                if(_isCooldownRelated)
                {
                    float? remainingCD = CardCooldownManager.Instance.GetRemainingCooldownForCard(_card);
                    if(!(remainingCD is null)) _remainingCD = (float) remainingCD;
                }
            }
        }

        public void OnHoverEnter()
        {
            if(_isSelected) return;
            _selectionIndicatorTransform.gameObject.SetActive(true);
            if(_isCooldownRelated)
            {
                _remainingCD = (_cdReductionIsFlat) ? _remainingCD - _cdReductionAmount : _remainingCD * (1 - _cdReductionAmount);
                UpdateCooldownText();
            }
        }

        public void OnHoverExit()
        {
            if(_isSelected) return;
            _selectionIndicatorTransform.gameObject.SetActive(false);
            if(_isCooldownRelated)
            {
                float? remainingCD = CardCooldownManager.Instance.GetRemainingCooldownForCard(_card);
                if(!(remainingCD is null)) _remainingCD = (float)remainingCD;
            }
        }

        private void UpdateCooldownText()
        {
            if(_remainingCD < 0f) _remainingCD = 0f;
            TimeSpan cd = TimeSpan.FromSeconds(_remainingCD);
            _cooldownText.text = cd.Hours.ToString() + ":" + cd.Minutes.ToString() + ":" + cd.Seconds.ToString();
        }
        #endregion

        private IEnumerator CooldownTextUpdatingCoroutine()
        {
            for(;;)
            {
                UpdateCooldownText();
                yield return new WaitForSeconds(1f);
                _remainingCD--;
            }
        }
    }
}
