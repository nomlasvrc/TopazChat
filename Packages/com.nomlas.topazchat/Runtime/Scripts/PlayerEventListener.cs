using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public abstract class PlayerEventListener : TopazChatBase
    {
        public TopazChatPlayer player;
        public virtual void UpdateURL(VRCUrl url) { }
    }
}
