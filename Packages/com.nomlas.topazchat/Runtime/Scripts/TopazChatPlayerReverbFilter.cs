
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class TopazChatPlayerReverbFilter : TopazChatPlayerBase
    {
        [SerializeField] private AudioOutputTunnel audioOutputTunnel;
        private void Start()
        {
            player = this;
        }

        protected override void VolumeChange()
        {
            audioOutputTunnel.volume = volume;
        }
    }
}