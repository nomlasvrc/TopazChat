
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class TopazChatPlayerReverbFilter : TopazChatPlayerBase
    {
        private void Start()
        {
            player = this;
        }
    }
}