/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.UI;

namespace _Game.Scripts.Cards
{
    public class CardObject : MonoBehaviour
    {
        #region Properties
        [SerializeField] private CardInstance _card;
        public CardInstance CardInstanceRef => _card;
        [SerializeField] private SpriteListCard _cardSpriteObject = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _rarityIconImageObject = null;
        [SerializeField] private TMP_Text _soulValueText = null;
        [SerializeField] private Button _inspectButton = null;
        [SerializeField] private TMP_Text _typeText = null;
        [SerializeField] private TMP_Text _flavourText = null;
        [SerializeField] private Transform _abilityContainerObj = null;
        [SerializeField] private TMP_Text _abilityText = null;
        private bool _isInitialised = false;
        private bool _isClickable = false;
        private bool _clickIsInspection = false;
        private Action _clickExecute = null;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_cardSpriteObject is null || _cardArtImageObj is null || _nameText is null || _rarityIconImageObject is null || _soulValueText is null || _inspectButton is null || _typeText is null || _flavourText is null || _abilityContainerObj is null || _abilityText is null)
                Debug.LogWarning("CardObject.cs of " + this.name + " is missing Object References.");
            
            if(_inspectButton != null)
            #endif
                _inspectButton.onClick.AddListener(delegate {InspectButtonClick();});

            _inspectButton.gameObject.SetActive(false);
        }

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
        ///<summary>
        ///Initialises the card object with the given parameters.
        ///</summary>
        ///<param name="card">CardInstance to use as Base for the Object.</param>
        ///<param name="isClickable">Optional. Bool flag to set if this Object is clickable. Defaults to false.</param>
        ///<param name="clickIsInspection">Optional. Bool flag to set of the click is used to inspect the Object. Overwrides the clickExecute Action. Defaults to false.</param>
        ///<param name="clickExecute">Optional. System.Action that'll get invoked when clicked. Defaults to null.</param>
        public void Initialise(CardInstance card, bool isClickable = false, bool clickIsInspection = false, Action clickExecute = null)
        {
            if(_isInitialised) return;
            this.name = card.Name + "_" + transform.parent.gameObject.name;
            _card = card;
            _cardArtImageObj.sprite = card.CardArt;
            _nameText.text = card.Name;
            switch(card.CardRef.Rarity)
            {
                case Card.CardRarity.Common:
                    _rarityIconImageObject.sprite = _cardSpriteObject.RaritySprites.Common;
                    break;
                case Card.CardRarity.Rare:
                    _rarityIconImageObject.sprite = _cardSpriteObject.RaritySprites.Rare;
                    break;
                case Card.CardRarity.VeryRare:
                    _rarityIconImageObject.sprite = _cardSpriteObject.RaritySprites.VeryRare;
                    break;
                case Card.CardRarity.Special:
                    _rarityIconImageObject.sprite = _cardSpriteObject.RaritySprites.Special;
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
            _isClickable = isClickable;
            _clickIsInspection = clickIsInspection;
            _clickExecute = clickExecute;
        }

        private void CatchAbilityUpgrade()
        {
            _abilityText.text = _card.AbilityText;
        }

        ///<summary>
        ///Calls ViewCard() on the CardViewManager if the clickIsInspection flag was set to true during Initialisation, otherwise invokes the clickExecute Action.
        ///Fails if the isClickable flag was not set to true during Initialisation.
        ///</summary>
        public void Click()
        {
            if(!_isClickable) return;
            if(_clickIsInspection)
            {
                InspectButtonClick();
                return;
            }
            _clickExecute?.Invoke();
        }

        ///<summary>
        ///Enables the Inspection Button on the Object.
        ///</summary>
        public void OnHoverEnter()
        {
            if(_clickIsInspection) _inspectButton.gameObject.SetActive(true);
        }

        ///<summary>
        ///Disables the Inspection Button on the Object.
        ///</summary>
        public void OnHoverExit()
        {
            if(_clickIsInspection) _inspectButton.gameObject.SetActive(false);
        }

        private void InspectButtonClick()
        {
            CardViewManager.Instance.ViewCard(_card);
        }
        #endregion
    }
}
