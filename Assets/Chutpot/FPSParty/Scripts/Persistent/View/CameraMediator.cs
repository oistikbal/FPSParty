using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using UnityEngine.Events;
using Cinemachine;

namespace Chutpot.Project2D.Persistent
{
    public class CameraMediator : Mediator
    {
        [Inject]
        public CameraView View { get; set; }

        [Inject]
        public CameraSignal CameraSignal { get; set; }
        [Inject]
        public CameraModel CameraModel { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.OnCameraCutEvent.AddListener(OnCameraCut);
            View.OnCameraActivatedEvent.AddListener(OnCameraActivated);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.OnCameraCutEvent.RemoveListener(OnCameraCut);
            View.OnCameraActivatedEvent.RemoveListener(OnCameraActivated);
        }

        private void OnCameraActivated(ICinemachineCamera incomingCamera, ICinemachineCamera outcomingCamera)
        {
            //Dont wait to blend in/out for timeline cameras
            if(CameraModel.VirtualCameras.Contains(incomingCamera) && CameraModel.VirtualCameras.Contains(outcomingCamera))
            {
                StartCoroutine(WaitBlend());
            }
            else
            {
                CameraSignal.Dispatch(false);
            }

        }

        private void OnCameraCut(CinemachineBrain brain)
        {
        }

        private IEnumerator WaitBlend() 
        {
            yield return new WaitForEndOfFrame();
            CameraSignal.Dispatch(true);
            yield return new WaitForSecondsRealtime(1);
            CameraSignal.Dispatch(false);
        }
    }
}
