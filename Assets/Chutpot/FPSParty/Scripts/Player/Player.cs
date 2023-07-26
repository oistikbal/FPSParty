using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Chutpot.FPSParty
{
    public class Player : NetworkBehaviour
    {
        private PlayerController _playerController;
        private PlayerActionMap _playerActionMap;

        private Vector2 _input;

        private void Awake()
        {
            _playerActionMap = new PlayerActionMap();
            _playerActionMap.Enable();
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            FindObjectOfType<InputSystemUIInputModule>().actionsAsset = _playerActionMap.asset;
        }

        private void Update()
        {
            _playerController.SetInputs(_input);
        }

        private void OnEnable()
        {
            _playerActionMap.Player.Movement.performed += OnMovementPerformed;
            _playerActionMap.Player.Movement.canceled += OnMovementCanceled;
        }

        private void OnDisable()
        {
            _playerActionMap.Player.Movement.performed -= OnMovementPerformed;
            _playerActionMap.Player.Movement.canceled -= OnMovementCanceled;
        }

        private void OnMovementCanceled(InputAction.CallbackContext obj)
        {
            _input = Vector2.zero;
        }


        private void OnMovementPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _input = obj.ReadValue<Vector2>();
        }

    }
}
