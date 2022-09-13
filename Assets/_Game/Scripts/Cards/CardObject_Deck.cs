/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.Extensions;
using _Game.Scripts.UI;

namespace _Game.Scripts.Cards
{
    public class CardObject_Deck : MonoBehaviour
    {
        #region Properties
        [SerializeField] private CardInstance _cardInstance = null;
        public CardInstance CardInstanceRef => _cardInstance;
        [SerializeField] private SpriteList _rarityIcons = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _rarityIconImageObj = null;
        [SerializeField] private TMP_Text _typeText = null;
        [SerializeField] private TMP_Text _strengthText = null;
        private DeckScreenHandler _deckHandler = null;
        private bool _isInitialised = false;
        private bool _isClickable = false;
        private Action _clickExecute = null;
        #endregion

        #region Unity Event Functions
        #if UNITY_EDITOR
        private void Awake()
        {
            if(_rarityIcons is null || _cardArtImageObj is null || _nameText is null || _rarityIconImageObj is null || _typeText is null || _strengthText is null)
                Debug.LogWarning("CardObject_Deck.cs of " + this.name + " is missing Object References.");   
        }
        #endif
        #endregion
        
        #region Methods
        public void Initialise(CardInstance card, DeckScreenHandler handler, bool isClickable = false, Action clickExecute = null)
        {
            this.name = card.Name + "_" + transform.parent.gameObject.name;
            _cardInstance = card;
            _cardArtImageObj.sprite = card.CardArt;
            _nameText.text = card.Name;
            switch(card.CardRef.Rarity)
            {
                case Card.CardRarity.Common:
                    _rarityIconImageObj.sprite = _rarityIcons[0];
                    break;
                case Card.CardRarity.Rare:
                    _rarityIconImageObj.sprite = _rarityIcons[1];
                    break;
                case Card.CardRarity.VeryRare:
                    _rarityIconImageObj.sprite = _rarityIcons[2];
                    break;
                case Card.CardRarity.Special:
                    _rarityIconImageObj.sprite = _rarityIcons[3];
                    break;
            }
            _typeText.text = Enum.GetName(typeof(Card.CardType), card.CardRef.Type);
            _strengthText.text = card.CardRef.Strength.ToString();
            _deckHandler = handler;
            _isInitialised = true;
        }

        public void Click()
        {
            if(!_isClickable) return;
            _clickExecute?.Invoke();
        }
        #endregion
    }
}
