/*
 * contact information:
 * Email: simon.gemmel@gmail.com
 * Discord: TheSimlier#6781
 */
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using _Game.Scripts.UI;

namespace _Game.Scripts.Cards
{
    public class CardObject_Hand : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Properties
        [SerializeField] private CardInstance _cardInstance = null;
        public CardInstance CardInstanceRef => _cardInstance;
        [SerializeField] private SpriteListCard _cardSpriteObject = null;
        [SerializeField] private Image _cardArtImageObj = null;
        [SerializeField] private TMP_Text _nameText = null;
        [SerializeField] private Image _rarityIconImageObj = null;
        [SerializeField, Range(0.5f, 1.5f)] private float _tweenDuration = 0.3f;
        private bool _isInitialised = false;
        private Action _clickExecute = null;
        private float _originalPosX = 0;
        public float OriginalXAxisPosition => _originalPosX;
        private float _cardPosDelta = 0;
        private HandDisplay _handler = null;
        private RectTransform _localRectTransform = null;
        private CanvasGroup _localCanvasGroup = null;
        private Canvas _rootCanvas;
        private bool _skillActivationSuccesfull = false;
        private bool _isBeingDragged = false;
        public bool isBeingDragged => _isBeingDragged;
        private Action _hoverStartAction = null;
        private Action _hoverEndAction = null;
        #endregion

        #region Unity Event Functions
        private void Awake()
        {
            #if UNITY_EDITOR
            if(_cardSpriteObject is null || _cardArtImageObj is null || _nameText is null || _rarityIconImageObj is null)
                Debug.LogWarning("CardObject_Hand.cs " + this.name + " is missing Object References.");
            #endif

            _localRectTransform = gameObject.GetComponent<RectTransform>();
            _localCanvasGroup = gameObject.GetComponent<CanvasGroup>();
        }
        #endregion
        
        #region Methods
        public void Initialise(CardInstance card, HandDisplay handler, Canvas canvasForScaleFactor, Action hoverStartAction, Action hoverEndAction)
        {
            if(_isInitialised) return;

            this.name = card.Name + "_Hand";
            _cardInstance = card;
            _cardArtImageObj.sprite = card.CardArt;
            _nameText.text = card.Name;
            switch(card.CardRef.Rarity)
            {
                case Card.CardRarity.Common:
                    _rarityIconImageObj.sprite = _cardSpriteObject.RaritySprites.Common;
                    break;
                case Card.CardRarity.Rare:
                    _rarityIconImageObj.sprite = _cardSpriteObject.RaritySprites.Rare;
                    break;
                case Card.CardRarity.VeryRare:
                    _rarityIconImageObj.sprite = _cardSpriteObject.RaritySprites.VeryRare;
                    break;
                case Card.CardRarity.Special:
                    _rarityIconImageObj.sprite = _cardSpriteObject.RaritySprites.Special;
                    break;
            }
            _handler = handler;
            this._rootCanvas = canvasForScaleFactor;
            _clickExecute = () => handler.CardClicked(this._cardInstance);
            _hoverStartAction = hoverStartAction;
            _hoverEndAction = hoverEndAction;
            _isInitialised = true;
        }

        public async Task Spawn()
        {
            //TODO
        }

        public async Task Despawn(bool isActivation = false, int cooldown = 0)
        {
            //TODO 

            if(isActivation)
            {
                CardCooldownManager.Instance.StartCooldownForCard(_cardInstance, cooldown);
                GameManager.Instance.MoveSpecificCard(_cardInstance, GameManager.CardGameStates.Hand, GameManager.CardGameStates.Grave); 
            }
        }

        public void SetPositionOffSetX(float x, float delta)
        {
            this._originalPosX = x;
            this._cardPosDelta = delta;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Invoke(nameof(DelayedPointerDown), 0.15f);
        }

        private void DelayedPointerDown()
        {
            if(_isBeingDragged) return;
            _clickExecute?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hoverStartAction?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hoverEndAction?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _localRectTransform.anchoredPosition += eventData.delta / transform.lossyScale;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _localCanvasGroup.blocksRaycasts = false;
            _isBeingDragged = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isBeingDragged = false;
            if(_localRectTransform.localPosition != new Vector3(_originalPosX, 0, 0)) Invoke(nameof(DelayedReturn), 2);
        }

        private void DelayedReturn()
        {
            if(_skillActivationSuccesfull || _isBeingDragged) return;

            _localRectTransform.DOLocalMove(new Vector2(_originalPosX, 0), _tweenDuration)
                .OnComplete(()=> _localCanvasGroup.blocksRaycasts = true);
        }

        public void AttemptSkillActivation()
        {
            int cooldown = 0;
            _skillActivationSuccesfull = _cardInstance.ActivateAbility(ref cooldown);
            if(_skillActivationSuccesfull) Despawn(true, cooldown).RunSynchronously(); //should check if this does what I think it does
            else PlayShakeAnimation();
        }

        private void PlayShakeAnimation()
        {
            //TODO
        }
        #endregion
    }
}
