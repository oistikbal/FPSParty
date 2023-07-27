using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using Cinemachine;
using UnityEngine.Events;

namespace Chutpot.FPSParty.Persistent
{
    public class CameraView : View
    {
        [HideInInspector]
        public UnityEvent<ICinemachineCamera, ICinemachineCamera> OnCameraActivatedEvent;
        [HideInInspector]
        public UnityEvent<CinemachineBrain> OnCameraCutEvent;
        

        private CinemachineBrain _cinemachineBrain;
        protected override void Awake()
        {
            base.Awake();
            _cinemachineBrain = GetComponent<CinemachineBrain>();
            OnCameraActivatedEvent = new UnityEvent<ICinemachineCamera, ICinemachineCamera>();
            OnCameraCutEvent = new UnityEvent<CinemachineBrain>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _cinemachineBrain.m_CameraCutEvent.AddListener(OnCameraCut);
            _cinemachineBrain.m_CameraActivatedEvent.AddListener(OnCameraActivated);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _cinemachineBrain.m_CameraCutEvent.RemoveListener(OnCameraCut);
            _cinemachineBrain.m_CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }

        private void OnCameraActivated(ICinemachineCamera incomingCamera, ICinemachineCamera outcomingCamera)
        {
            OnCameraActivatedEvent?.Invoke(incomingCamera, outcomingCamera);
        }

        private void OnCameraCut(CinemachineBrain brain)
        {
            OnCameraCutEvent?.Invoke(brain);
        }
    }
}
