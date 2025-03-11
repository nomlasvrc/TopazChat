using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public abstract class PlayerEventListener : TopazChatBase
    {
        public TopazChatPlayerBase player;
        public virtual void UpdateURL(VRCUrl url, VRCUrl url_Android) { }
        public virtual void OnListenerReady() { }
        public virtual void UpdateMessage(string msg) { }
    }
}
