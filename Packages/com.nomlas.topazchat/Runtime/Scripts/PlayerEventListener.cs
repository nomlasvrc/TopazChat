using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class PlayerEventListener : TopazChatBase
    {
        [HideInInspector] public TopazChatPlayer player {get; internal set;}
        internal virtual void UpdateURL(VRCUrl url) { }

        private void Start()
        {
            player.AddEventListener(this);
        }
    }
}
