using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;
using DG.Tweening;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public abstract class MenuView : View, IPointerEnterHandler
    {
        public MenuShowSignal MenuShowSignal { get; protected set; }
        public MenuHideSignal MenuHideSignal { get; protected set; }
        public MenuPointerSignal MenuPointerSignal { get; protected set; }
        public MenuPreHideSignal MenuPreHideSignal { get; protected set; }
        public GameObject SelectedGO { get; protected set; }
        public PlayerActionMap PlayerActionMap { get; set; }

        protected CanvasGroup CanvasGroup { get; set; }
        protected Tween HideTween { get; set; }
        protected Tween ShowTween { get; set; }

        public bool IsShowing { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            CanvasGroup = GetComponent<CanvasGroup>();
            MenuShowSignal = new MenuShowSignal();
            MenuHideSignal = new MenuHideSignal();
            MenuPointerSignal = new MenuPointerSignal();
            MenuPreHideSignal = new MenuPreHideSignal();

            HideTween = DOTween.To(ApplyAlpha, 1f, 0f, 0.75f).SetEase(Ease.InQuad).OnComplete(OnCompleteHide);
            ShowTween = DOTween.To(ApplyAlpha, 0f, 1f, 0.75f).SetEase(Ease.InQuad).OnComplete(OnCompleteShow);
            HideTween.SetUpdate(true);
            ShowTween.SetUpdate(true);
        }

        protected override void Start()
        {
            base.Start();
            PlayerActionMap.UI.Cancel.performed += OnCancelPressed;
        }

        private void ApplyAlpha(float alpha)
        {
            CanvasGroup.alpha = alpha;
        }

        public virtual IEnumerator Hide() 
        {
            MenuPreHideSignal.Dispatch();
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            HideTween.Rewind(false);
            HideTween.Play();
            yield return HideTween.WaitForCompletion();
            IsShowing = false;
        }

        public virtual IEnumerator Show() 
        {
            ShowTween.Rewind(false);
            ShowTween.Play();
            yield return ShowTween.WaitForCompletion();
            IsShowing = true;
        }

        protected virtual void OnCompleteHide()
        {
        }
        protected virtual void OnCompleteShow()
        {
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
        }

        protected virtual void OnCancelPressed(InputAction.CallbackContext obj)
        {
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            MenuPointerSignal.Dispatch(eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject);
        }
    }
}
