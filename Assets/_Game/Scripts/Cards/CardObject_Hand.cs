/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.Extensions;
using _Game.Scripts.UI;

namespace _Game.Scripts.Cards
{
    public class CardObject_Hand : MonoBehaviour
    {
        #region Properties
        [SerializeField] private CardInstance _cardInstance = null;
        public CardInstance CardInstanceRef => _cardInstance;
        [SerializeField] private SpriteList _rarityIconList = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _rarityIconImageObj = null;
        [SerializeField] private TMP_Text _typeText = null;
        [SerializeField] private TMP_Text _flavourText = null;
        [SerializeField] private Transform _abilityContainerObj = null;
        [SerializeField] private TMP_Text _abilityText = null;
        private bool _isInitialised = false;
        private Action _clickExecute = null;
        private float _originalPosX = 0;
        private HandDisplay _handler = null;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_rarityIconList is null || _cardArtImageObj is null || _nameText is null || _rarityIconImageObj is null || _typeText is null || _flavourText is null || _abilityContainerObj is null || _abilityText is null)
                Debug.LogWarning("CardObject_Hand.cs " + this.name + " is missing Object References.");
            #endif
        }
        #endregion
        
        #region Methods
        public void Initialise(CardInstance card, HandDisplay handler)
        {
            if(_isInitialised) return;

            this.name = card.Name + "_Hand";
            _cardInstance = card;
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
            _typeText.text = Enum.GetName(typeof(Card.CardType), card.CardRef.Type);
            _handler = handler;
            _clickExecute = () => handler.CardClicked(this._cardInstance);
            _isInitialised = true;
        }

        public async Task Spawn()
        {
            //TODO
        }

        public async Task Despawn()
        {
            //TODO            
        }

        public void SetPositionOffSetX(float x) => this._originalPosX = x;

        public void OnClick()
        {
            //TODO
        }

        public void OnHoverEnter()
        {
            //TODO
        }

        public void OnHoverExit()
        {
            //TODO
        }
        #endregion
    }
}
