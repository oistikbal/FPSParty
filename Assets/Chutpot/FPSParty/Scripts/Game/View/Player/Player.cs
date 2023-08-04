using Cinemachine;
using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.TextCore.Text;

namespace Chutpot.FPSParty
{
    public class Player : NetworkBehaviour
    {
        private PlayerController _playerController;
        private PlayerActionMap _playerActionMap;
        private CinemachineVirtualCamera _virtualCamera;
        private Camera _camera;

        private Vector2 _moveInput;
        private Vector2 _cameraInput;
        private bool _jumpInput;

        private void Awake()
        {
            _playerActionMap = new PlayerActionMap();
            _playerActionMap.Enable();
            _playerController = GetComponent<PlayerController>();
            _camera = FindObjectOfType<Camera>();
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            _virtualCamera.Follow = transform;
        }

        private void Start()
        {
            FindObjectOfType<InputSystemUIInputModule>().actionsAsset = _playerActionMap.asset;
        }

        private void Update()
        {
            HandleCharacterInput();
        }
        

        private void OnEnable()
        {
            _playerActionMap.Player.Movement.performed += OnMovementPerformed;
            _playerActionMap.Player.Movement.canceled += OnMovementCanceled;
            _playerActionMap.Player.Jump.performed += OnJumpPerformed;
            _playerActionMap.Player.Jump.canceled += OnJumpCancelled;
        }


        private void OnDisable()
        {
            _playerActionMap.Player.Movement.performed -= OnMovementPerformed;
            _playerActionMap.Player.Movement.canceled -= OnMovementCanceled;
            _playerActionMap.Player.Jump.performed -= OnJumpPerformed;
            _playerActionMap.Player.Jump.canceled -= OnJumpCancelled;
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

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = _moveInput.y;
            characterInputs.MoveAxisRight = _moveInput.x;
            characterInputs.CameraRotation = _camera.transform.rotation;
            characterInputs.JumpDown = _jumpInput;
            characterInputs.CrouchDown = false;
            characterInputs.CrouchUp = true;

            // Apply inputs to character
            _playerController.SetInputs(ref characterInputs);
        }

    }
}
