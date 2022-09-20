/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.Cards;
using _Game.Scripts.Extensions;

namespace _Game.Scripts.UI
{
    public class CardViewManager : MonoBehaviour
    {
        public static CardViewManager Instance {get; private set;}

        #region Properties
        [SerializeField] private SpriteList _rarityIcons = null;
        [Space(3f)]
        [SerializeField] private Transform _cardViewWindowTransform = null;
        [SerializeField] private Image _cardArtImageComponent = null;
        [SerializeField] private TMP_Text _cardNameTextComponent = null;
        [SerializeField] private Image _cardRarityImageComponent = null;
        [SerializeField] private TMP_Text _cardFlavourTextComponent = null;
        [SerializeField] private TMP_Text _cardAbilityTextComponent = null;
        [SerializeField] private Button _closeMenuButton = null;
        [Space(3f)]
        [SerializeField] private bool _pauseGameWhileViewing = false;

        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            #if UNITY_EDITOR
            if(_rarityIcons is null || _cardViewWindowTransform is null || _cardArtImageComponent is null || _cardNameTextComponent is null || _cardRarityImageComponent is null || _cardFlavourTextComponent is null || _cardAbilityTextComponent is null || _closeMenuButton is null)
                Debug.LogWarning("CardViewManager.cs is missing Object References.");
            #endif
        }

        private void Start()
        {
            #if UNITY_EDITOR
            if(_closeMenuButton != null)
            #endif
                _closeMenuButton.onClick.AddListener(delegate {CloseMenu();});
        }
        #endregion
        
        #region Methods
        public void ViewCard(CardInstance card)
        {
            if(_pauseGameWhileViewing) Time.timeScale = 0f;

            _cardArtImageComponent.sprite = card.CardArt;
            _cardNameTextComponent.text = card.Name;
            switch(card.CardRef.Rarity)
            {
                case Card.CardRarity.Common:
                    _cardRarityImageComponent.sprite = _rarityIcons[0];
                    break;
                
                case Card.CardRarity.Rare:
                    _cardRarityImageComponent.sprite = _rarityIcons[1];
                    break;

                case Card.CardRarity.VeryRare:
                    _cardRarityImageComponent.sprite = _rarityIcons[2];
                    break;

                case Card.CardRarity.Special:
                    _cardRarityImageComponent.sprite = _rarityIcons[3];
                    break;
            }

            _cardFlavourTextComponent.text = card.CardRef.FlavourText;
            _cardAbilityTextComponent.text = card.AbilityText;

            _cardViewWindowTransform.gameObject.SetActive(true);
        }

        private void CloseMenu()
        {
            _cardViewWindowTransform.gameObject.SetActive(false);
            if(_pauseGameWhileViewing) Time.timeScale = 1f;
        }
        #endregion
    }
}
