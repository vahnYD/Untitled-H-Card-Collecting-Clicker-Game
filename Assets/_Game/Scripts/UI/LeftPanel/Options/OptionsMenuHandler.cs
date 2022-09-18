/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Game.Scripts.UI
{
    public class OptionsMenuHandler : MonoBehaviour
    {
        #region Properties
        [SerializeField] private List<ResolutionOptions> _resolutionOptions = new List<ResolutionOptions>();
        [SerializeField] private TMP_Dropdown _resolutionDropdownMenu = null;
        [SerializeField] private Button _optionsApplybutton = null;
        [SerializeField] private Toggle _fullscreenToggle = null;
        private float _lastWidth;
        private float _lastHeight;
        private ResolutionOptions _selectedResolution;
        private bool _fullscreenSelected;
        #endregion

        #region Unity Event Functions
        private void Start()
        {
            // Check if Resolution is 16:9 or not

            // IF Resolution isnt 16:9 disable full screen
            // set resolution to the next smallest 16:9

            _resolutionDropdownMenu.onValueChanged.AddListener(delegate {CatchDropdownValueChange();});
            _fullscreenToggle.onValueChanged.AddListener(delegate {CatchFullscreenToggleValueChange();});
            _optionsApplybutton.onClick.AddListener(delegate {CatchApplyButtonClicked();});
        }

        private void Update()
        {
            // If window was resized, keep 16:9 aspect ratio
        }
        #endregion
        
        #region Methods
        private void CatchDropdownValueChange()
        {
            _selectedResolution = _resolutionOptions.Where((resolution)=>resolution.Name == _resolutionDropdownMenu.options[_resolutionDropdownMenu.value].text).FirstOrDefault();
        }

        private void CatchFullscreenToggleValueChange()
        {

        }

        private void CatchApplyButtonClicked()
        {

        }
        #endregion

        [Serializable]
        public class ResolutionOptions
        {
            [SerializeField] private string _name;
            public string Name => _name;
            [SerializeField] private int _width;
            public int Width => _width;
            [SerializeField] private int _height;
            public int Height => _height;
        }
    }
}
