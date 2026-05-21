using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public abstract class PlayerEventListener : TopazChatBase
    {
        /// <summary>
        /// 全てのListenerに渡される、TopazChatPlayerのコンポーネント。
        /// </summary>
        public TopazChatPlayer player;
        protected virtual void Start()
        {
            player.AddEventListener(this);
        }

        /// <summary>
        /// URLが変更されたときに発火します。
        /// </summary>
        public virtual void UpdateURL(VRCUrl url, VRCUrl url_Android) { }
        /// <summary>
        /// Listenerが登録され、準備ができたときに発火します。
        /// </summary>
        public virtual void OnListenerReady() { }
        /// <summary>
        /// UI表示用メッセージが更新されたときに発火します。
        /// </summary>
        public virtual void UpdateMessage(string msg) { }
        /// <summary>
        /// TopazChatPlayerの再生状態が変化したときに発火します。
        /// </summary>
        public virtual void UpdateStatus(PlayerStatus playerStatus) { }
        /// <summary>
        /// 音量が変更されたときに発火します。
        /// </summary>
        public virtual void OnChangeVolume(float volume) { }
        /// <summary>
        /// Listenerの名前を取得/設定します。
        /// </summary>
        public virtual string ListenerName => "Generic Event Listener";
    }
}
