using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public abstract class PlayerEventListener : TopazChatBase
    {
        public TopazChatPlayer player;
        protected virtual void Start()
        {
            player.AddEventListener(this);
        }

        public virtual void UpdateURL(VRCUrl url, VRCUrl url_Android) { }
        public virtual void OnListenerReady() { }
        public virtual void UpdateMessage(string msg) { }
        public virtual void UpdateStatus(PlayerStatus playerStatus) { }
        public virtual void OnChangeVolume(float volume) { }
        public virtual string GetListenerName()
        {
            return "Generic Event Listener";
        }
    }
}
