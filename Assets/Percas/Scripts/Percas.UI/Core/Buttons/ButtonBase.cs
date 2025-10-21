using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Percas.UI
{
    public class ButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        public enum ButtonAnimType
        {
            Scale,
            Move,
            None,
        }

        [SerializeField] ButtonAnimType buttonAnimType;
        [SerializeField] RectTransform _currenRect;
        [SerializeField] float _duration = 0.1f;
        [SerializeField] Vector3 _targetMove;
        [SerializeField] Vector3 _scaleTarget = new(0.85f, 0.85f, 0.85f);
        [HideInInspector] [SerializeField] UnityEvent _onPointerDown;
        [HideInInspector] [SerializeField] UnityEvent _onPointerUp;
        [SerializeField] UnityEvent _onPointerClick;
        [HideInInspector] [SerializeField] UnityEvent _onPointerExit;
        public string placement;
        public string shortcut;


        private Vector2 _originalPos;
        private bool _canClick = true;
        private bool _isPointDown = false;

        public Action OnClickHandler;
        public Action OnPointerDownHandler;
        public Action OnPointerUpHandler;

        private Tween tween;

        protected virtual void Awake()
        {
            if (buttonAnimType.Equals(ButtonAnimType.None)) return;
            _originalPos = _currenRect.anchoredPosition;
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }

        public void SetPointerClickEvent(Action onAction)
        {
            OnClickHandler = onAction;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!_canClick) return;
            _canClick = false;
            ShowButton();
            _onPointerDown?.Invoke();
            _isPointDown = true;
            OnPointerDownHandler?.Invoke();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!_isPointDown) return;
            HideButton();
            _onPointerExit?.Invoke();
            _isPointDown = false;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            HideButton();
            _onPointerUp?.Invoke();
            _isPointDown = false;
            OnPointerUpHandler?.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            _onPointerClick?.Invoke();
            OnClickHandler?.Invoke();
            _isPointDown = false;
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayButtonClick();
                AudioController.Instance.PlayVibration(HapticType.Medium, 10);
            }
        }

        private void ShowButton()
        {
            switch (buttonAnimType)
            {
                case ButtonAnimType.Scale:
                    tween = _currenRect.DOScale(_scaleTarget, _duration).SetEase(Ease.OutQuad);
                    break;

                case ButtonAnimType.Move:
                    tween = _currenRect.DOAnchorPos(_targetMove, _duration).SetEase(Ease.OutQuad);
                    break;
            }
        }

        private void HideButton()
        {
            switch (buttonAnimType)
            {
                case ButtonAnimType.Scale:
                    tween = _currenRect.DOScale(1, _duration).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        _canClick = true;
                    });
                    break;

                case ButtonAnimType.Move:
                    tween = _currenRect.DOAnchorPos(_originalPos, _duration).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        _canClick = true;
                    });
                    break;

                default:
                    _canClick = true;
                    break;
            }
        }
    }
}
