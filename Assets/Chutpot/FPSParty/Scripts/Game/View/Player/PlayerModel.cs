using Doozy.Runtime.Reactor.Animators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chutpot.FPSParty.Game
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField]
        private GameObject Head;

        private Animator _animator;
        private PlayerMovement _playerMovement;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _playerMovement = GetComponentInParent<PlayerMovement>();
            _playerMovement.CameraRotation.OnValueChanged += OnCameraRotationUpdated;
            if (_playerMovement.IsOwner)
            {
                foreach (Transform child in transform)
                {
                    //ignore this while testing
                    //child.layer = LayerMask.NameToLayer("Self");
                }
            }        
        }
        private void OnCameraRotationUpdated(Quaternion previous, Quaternion current)
        {
            transform.rotation = Quaternion.Euler(Vector3.up * current.eulerAngles.y);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            Ray lookAtRay = new Ray(transform.position + Vector3.up, _playerMovement.CameraRotation.Value * Vector3.forward);

            _animator.SetLookAtWeight(0.8f);
            _animator.SetLookAtPosition(lookAtRay.GetPoint(25));
        }
    }
}
