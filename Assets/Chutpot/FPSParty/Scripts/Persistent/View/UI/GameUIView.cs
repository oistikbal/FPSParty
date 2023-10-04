using Doozy.Runtime.Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chutpot.FPSParty.Persistent
{
    public class GameUIView : MonoBehaviour
    {
        private InputModel _inputModel;
        private SignalStream _cancelStream;
        private bool _isGameStarted;

        private void Start()
        {
            _inputModel = ((PersistentContext)FindObjectOfType<PersistentContextView>().context).injectionBinder.GetInstance<InputModel>();
            _cancelStream = SignalStream.Get("MainMenuUI", "Cancel");
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "StartGame").OnSignal += signal => { _isGameStarted = true; };
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "ExitLobby").OnSignal += signal => { _isGameStarted = false; };

            _inputModel.PlayerActionMap.Player.Cancel.performed += OnPlayerCancelPerformed;
            _inputModel.PlayerActionMap.UI.Cancel.performed += OnUICancelPerformed;
        }

        private void OnPlayerCancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            _cancelStream.SendSignal();
            Cursor.visible = true;
        }

        private void OnUICancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (_isGameStarted)
            {
                _cancelStream.SendSignal();
                Cursor.visible = false;
            }
        }
    }
}
