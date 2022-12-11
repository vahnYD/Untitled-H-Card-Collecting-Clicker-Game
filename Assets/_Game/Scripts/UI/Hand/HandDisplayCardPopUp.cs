/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.Cards;
using _Game.Scripts.Extensions;

namespace _Game.Scripts.UI
{
    public class HandDisplayCardPopUp : MonoBehaviour
    {
        #region Properties
        [SerializeField] private SpriteListCard _rarityIconList = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _cardArtImgObj = null;
        [SerializeField] private Image _cardRarityImgObj = null;
        [SerializeField] private TMP_Text _typeText = null;
        [SerializeField] private TMP_Text _abilityText = null;
        [SerializeField] private Button _closeWindowBtn = null;
        private CardInstance _displayedCard = null;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_rarityIconList is null || _nameText is null || _cardArtImgObj is null || _cardRarityImgObj is null || _typeText is null || _abilityText is null || _closeWindowBtn is null)
                Debug.LogWarning("HandDisplayCardPopUp.cs is missing Object References.");
            #endif
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _closeWindowBtn.onClick.AddListener(delegate {CloseWindow();});
        }

        private void OnDisable()
        {
            _closeWindowBtn.onClick.RemoveListener(delegate {CloseWindow();});
        }
        #endregion
        
        #region Methods
        public void DisplayCard(CardInstance card)
        {
            if(_displayedCard != null && _displayedCard.CardArt == card.CardArt) return;
            _displayedCard = card;
            _nameText.text = card.Name;
            _typeText.text = Enum.GetName(typeof(Card.CardType),card.CardRef.Type);
            _abilityText.text = card.AbilityText;
            _cardArtImgObj.sprite = card.CardArt;
            switch(card.CardRef.Rarity)
            {
                case Card.CardRarity.Common:
                    _cardRarityImgObj.sprite = _rarityIconList.RaritySprites.Common;
                    break;
                case Card.CardRarity.Rare:
                    _cardRarityImgObj.sprite = _rarityIconList.RaritySprites.Rare;
                    break;
                case Card.CardRarity.VeryRare:
                    _cardRarityImgObj.sprite = _rarityIconList.RaritySprites.VeryRare;
                    break;
                case Card.CardRarity.Special:
                    _cardRarityImgObj.sprite = _rarityIconList.RaritySprites.Special;
                    break;
            }

            if(gameObject.activeSelf is false) gameObject.SetActive(true);
        }

        private void CloseWindow()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}
