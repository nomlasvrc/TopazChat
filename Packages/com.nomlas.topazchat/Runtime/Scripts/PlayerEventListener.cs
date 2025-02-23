using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class PlayerEventListener : TopazChatBase
    {
        public TopazChatPlayer player;
        internal virtual void UpdateURL(VRCUrl url) { }

        private void Start()
        {
            player.AddEventListener(this);
        }
    }
}
