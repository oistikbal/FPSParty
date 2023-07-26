using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using DG.Tweening;

namespace Chutpot.Project2D.Persistent
{
    public class LoadingScreenView : View
    {
        [SerializeField]
        private CanvasGroup _panelGroup;
        [SerializeField]
        private Canvas _canvas;

        private SceneInstance _currentInstance;

        public Signal<LoadingScreenStatus> LoadingScreenSignal { get; private set; }

        protected Tween FadeInTween { get; set; }
        protected Tween FadeOutTween { get; set; }


        protected override void Awake()
        {
            LoadingScreenSignal = new Signal<LoadingScreenStatus>();
            FadeOutTween = DOTween.To(ApplyAlpha, 1f, 0f, 0.75f).SetEase(Ease.InQuad);
            FadeInTween = DOTween.To(ApplyAlpha, 0f, 1f, 0.75f).SetEase(Ease.InQuad);

            FadeInTween.SetUpdate(true);
            FadeOutTween.SetUpdate(true);
        }

        public void LoadScene(string sceneAddress) 
        {
            StartCoroutine(LoadSceneCoroutine(sceneAddress));
        }

        private IEnumerator LoadSceneCoroutine(string sceneReferance)
        {
            yield return FadeIn();

            if (_currentInstance.Scene.isLoaded)
            {
                yield return Addressables.UnloadSceneAsync(_currentInstance); // Wait until old scene unloads
            }

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneReferance, UnityEngine.SceneManagement.LoadSceneMode.Additive, false);
            yield return handle; // Wait until new scene loads;
            yield return FadeOut();

            _currentInstance = handle.Result;
            _currentInstance.ActivateAsync();
        }

        private void ApplyAlpha(float alpha)
        {
            _panelGroup.alpha = alpha;
        }

        private IEnumerator FadeIn() 
        {
            _canvas.enabled = true;
            LoadingScreenSignal.Dispatch(LoadingScreenStatus.Awake);
            _panelGroup.alpha = 0f;
            FadeInTween.Rewind(false);
            FadeInTween.Play();
            yield return FadeInTween.WaitForCompletion();
            LoadingScreenSignal.Dispatch(LoadingScreenStatus.In);
        }

        private IEnumerator FadeOut()
        {
            _panelGroup.alpha = 0f;
            FadeOutTween.Rewind(false);
            FadeOutTween.Play();
            yield return FadeOutTween.WaitForCompletion();
            _canvas.enabled = false;
            LoadingScreenSignal.Dispatch(LoadingScreenStatus.Out);
        }
    }
}
