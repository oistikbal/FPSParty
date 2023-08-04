using Chutpot.FPSParty.Persistent;
using Cinemachine;
using Doozy.Runtime.Signals;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.TextCore.Text;

namespace Chutpot.FPSParty.Game
{
    public class Player : NetworkBehaviour
    {
        NetworkVariable<Quaternion> _cameraRotation;

        private InputModel _inputModel;
        private SignalStream _escapeStream;
        private PlayerController _playerController;
        private KinematicCharacterMotor _kinematicCharacterMotor;
        private PlayerActionMap _playerActionMap;
        private CinemachineVirtualCamera _virtualCamera;
        private Rigidbody _rigidbody;
        private Vector2 _moveInput;
        private Vector2 _cameraInput;
        private bool _jumpInput;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _cameraRotation = new NetworkVariable<Quaternion>(default,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            _inputModel = ((PersistentContext)FindObjectOfType<PersistentContextView>().context).injectionBinder.GetInstance<InputModel>();
            _playerActionMap = _inputModel.PlayerActionMap;
            _playerController = GetComponent<PlayerController>();
            _kinematicCharacterMotor = GetComponent<KinematicCharacterMotor>();
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            _escapeStream = SignalStream.Get("MainMenuUI", "Escape");
        }


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
                OnGainedOwnership();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

        }

        public override void OnGainedOwnership()
        {
            base.OnGainedOwnership();
            _playerActionMap.Player.Movement.performed += OnMovementPerformed;
            _playerActionMap.Player.Movement.canceled += OnMovementCanceled;
            _playerActionMap.Player.Jump.performed += OnJumpPerformed;
            _playerActionMap.Player.Jump.canceled += OnJumpCancelled;
            _playerActionMap.Player.Escape.performed += OnEscapePerformed;

            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = true;
            _playerController.enabled = true;
            _kinematicCharacterMotor.enabled = true;
            _virtualCamera.m_Priority = 1;
        }

        public override void OnLostOwnership()
        {
            base.OnLostOwnership();
            
            _playerActionMap.Player.Movement.performed -= OnMovementPerformed;
            _playerActionMap.Player.Movement.canceled -= OnMovementCanceled;
            _playerActionMap.Player.Jump.performed -= OnJumpPerformed;
            _playerActionMap.Player.Jump.canceled -= OnJumpCancelled;
            _playerActionMap.Player.Escape.performed -= OnEscapePerformed;

            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = false;
            _playerController.enabled = false;
            _kinematicCharacterMotor.enabled = false;
            _virtualCamera.m_Priority = 0;
            
        }

        private void Update()
        {
            if (IsOwner)
            {
                _cameraRotation.Value = _virtualCamera.State.FinalOrientation;
                HandleCharacterInput();
            }

        }
        

        private void OnJumpCancelled(InputAction.CallbackContext obj)
        {
            _jumpInput = false;
        }

        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
            _jumpInput = true;
        }

        private void OnMovementCanceled(InputAction.CallbackContext obj)
        {
            _moveInput = Vector2.zero;
        }


        private void OnMovementPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _moveInput = obj.ReadValue<Vector2>();
        }

        private void OnEscapePerformed(InputAction.CallbackContext obj)
        {
            _escapeStream.SendSignal();
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = _moveInput.y;
            characterInputs.MoveAxisRight = _moveInput.x;
            characterInputs.CameraRotation = _virtualCamera.State.FinalOrientation;
            characterInputs.JumpDown = _jumpInput;
            characterInputs.CrouchDown = false;
            characterInputs.CrouchUp = true;

            // Apply inputs to character
            _playerController.SetInputs(ref characterInputs);
        }

    }
}
