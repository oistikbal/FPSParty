using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;

namespace Chutpot.FPSParty.Persistent
{
    public class HostCreateButton : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _hostName;

        [SerializeField]
        private UIToggle _toggle;


        public void SendSignal()
        {
            Doozy.Runtime.Signals.SignalsService.SendSignal<HostCreate>("MainMenuUI", "HostCreate", new HostCreate(_hostName.text, _toggle.isOn));
        }
    }
}
