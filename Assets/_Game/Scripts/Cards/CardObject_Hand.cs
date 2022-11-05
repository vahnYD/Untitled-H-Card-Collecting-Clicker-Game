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
        public async Task Spawn()
        {
            
        }

        public async Task Despawn()
        {
            
        }
        #endregion
    }
}
