
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class ControllerBase : PlayerEventListener
    {
        [SerializeField] private TextMeshProUGUI message;
        public void ReSync()
        {
            player.Resync();
        }

        public void GlobalSync()
        {
            player.GlobalSync();
        }

        public override void UpdateMessage(string msg)
        {
            message.text = msg;
        }
    }
}
