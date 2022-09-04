/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace _Game.Scripts.UI
{
    public class UIPanelMovement : MonoBehaviour
    {
        #region Properties
        [SerializeField] private RectTransform _panelToggleBtn = null;
        private RectTransform _panelToMove = null;
        private Vector2 _openPos;
        [SerializeField] private Vector2 _closePos;
        private bool _isOpen;
        private bool _isMoving = false;
        private bool isMoving
        {
            get => _isMoving;
            set
            {
                if(value) _panelToggleBtn.gameObject.SetActive(false);
                else _panelToggleBtn.gameObject.SetActive(true);
                _isMoving = value;
            }
        }
        [SerializeField] private bool _defaultIsOpen = true;
        [SerializeField, Range(0, 2)] private float _duration = 1f;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            _panelToMove = gameObject.GetComponent<RectTransform>();
            _openPos = _panelToMove.anchoredPosition;
            if(!_defaultIsOpen) ClosePanel();
            else _isOpen = true;

        }
        #endregion
        
        #region Methods
        public void TogglePanel()
        {
            if(isMoving) return;
            if(_isOpen) ClosePanel();
            else OpenPanel();
        }

        private void OpenPanel()
        {
            isMoving = true;
            _panelToMove.DOAnchorPos(_openPos, _duration).OnComplete(()=>{_isOpen = true; isMoving = false;}).timeScale=1;
        }

        private void ClosePanel()
        {
            isMoving = true;
            _panelToMove.DOAnchorPos(_closePos, _duration).OnComplete(()=>{_isOpen = false; isMoving = false;}).timeScale=1;
        }
        #endregion
    }
}
