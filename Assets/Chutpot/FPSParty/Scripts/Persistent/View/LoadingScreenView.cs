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
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.Signals;
using Doozy.Runtime.Reactor;
using System.Linq;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Chutpot.FPSParty.Persistent
{
    public class LoadingScreenView : View
    {
        private AsyncOperationHandle<SceneInstance> _sceneHandle;
        private SceneInstance _currentInstance;
        private SignalStream _loadingScreenStream;
        private SignalStream _gameLoadedStream;
        private Progressor _loadingScreenProgressor;
        private Scene _persistentSceneInstance;
        public Signal<LoadingScreenStatus> LoadingScreenSignal { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            LoadingScreenSignal = new Signal<LoadingScreenStatus>();
            _loadingScreenStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "LoadingScreen");
            _gameLoadedStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "GameLoaded");
            _persistentSceneInstance = SceneManager.GetActiveScene();
        }

        protected override void Start()
        {
            base.Start();
            _loadingScreenProgressor = Progressor.GetProgressors("MainMenuUI", "LoadingScreen").First();
            NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        }

        private void OnClientStopped(bool obj)
        {
            if (_currentInstance.Scene.isLoaded)
            {
                _sceneHandle = Addressables.UnloadSceneAsync(_currentInstance);
                SceneManager.SetActiveScene(_persistentSceneInstance);
            }
        }

        public void LoadScene(string sceneAddress) 
        {
            StartCoroutine(LoadSceneCoroutine(sceneAddress));
        }

        private IEnumerator LoadSceneCoroutine(string sceneReferance)
        {
            UIPopup.ClearQueue();
            var popup = UIPopup.Get("PopupBlock");
            popup.SetTexts("Game is Starting...");
            popup.AutoHideAfterShow = true;
            popup.AutoHideAfterShowDelay = 1f;
            popup.Show();
            yield return new WaitForSeconds(1);
            _loadingScreenStream.SendSignal();
            yield return new WaitForSeconds(0.5f);
            if (!_sceneHandle.IsDone)
            {
                yield return _sceneHandle;
            }
            else if (_currentInstance.Scene.isLoaded)
            {
                _sceneHandle = Addressables.UnloadSceneAsync(_currentInstance);
                yield return _sceneHandle; // Wait until old scene unloads
            }

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneReferance, UnityEngine.SceneManagement.LoadSceneMode.Additive, false);
            while (!handle.IsDone)
            {
                _loadingScreenProgressor.PlayToValue(handle.PercentComplete);
                yield return null;
            }

            _loadingScreenProgressor.PlayToValue(1);
            _currentInstance = handle.Result;
            _currentInstance.ActivateAsync().completed += asyncObj => 
            {
                SceneManager.SetActiveScene(_currentInstance.Scene);
                _gameLoadedStream.SendSignal();
            };
        }

    }
}
