/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.UI
{
    public class FoldMenuSelection : MonoBehaviour
    {
        #region Properties
        [SerializeField] private RectTransform _cardListPage = null;
        [SerializeField] private RectTransform _deckPage = null;
        [SerializeField] private RectTransform _burningMedalPage = null;
        private bool _cardListIsOpen =  true;
        private bool _deckIsOpen =  false;
        private bool _burningMedalsAreOpen = false;
        private bool _optionsAreOpen = false;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_cardListPage is null || _deckPage is null || _burningMedalPage is null)
                Debug.LogWarning("FoldMenuSelection.cs is missing Object References.");
            #endif
        }
        #endregion
        
        #region Methods
        public void OpenCardList()
        {
            if(_cardListIsOpen) return;
            _cardListPage.gameObject.SetActive(true);
            _deckPage.gameObject.SetActive(true);
            _burningMedalPage.gameObject.SetActive(true);
            _cardListIsOpen = true;
            _deckIsOpen = false;
            _burningMedalsAreOpen = false;
            _optionsAreOpen = false;
        }

        public void OpenDeckPage()
        {
            if(_deckIsOpen) return;
            _deckPage.gameObject.SetActive(true);
            _cardListPage.gameObject.SetActive(false);
            _burningMedalPage.gameObject.SetActive(true);
            _deckIsOpen = true;
            _cardListIsOpen = false;
            _burningMedalsAreOpen = false;
            _optionsAreOpen = false;
        }

        public void OpenBurningMedals()
        {
            if(_burningMedalsAreOpen) return;
            _burningMedalPage.gameObject.SetActive(true);
            _cardListPage.gameObject.SetActive(false);
            _deckPage.gameObject.SetActive(false);
            _burningMedalsAreOpen = true;
            _cardListIsOpen = false;
            _deckIsOpen = false;
            _optionsAreOpen = false;
        }

        public void OpenOptions()
        {
            if(_optionsAreOpen) return;
            _cardListPage.gameObject.SetActive(false);
            _deckPage.gameObject.SetActive(false);
            _burningMedalPage.gameObject.SetActive(false);
            _optionsAreOpen = true;
            _cardListIsOpen = false;
            _deckIsOpen = false;
            _burningMedalsAreOpen = false;
        }
        #endregion
    }
}
