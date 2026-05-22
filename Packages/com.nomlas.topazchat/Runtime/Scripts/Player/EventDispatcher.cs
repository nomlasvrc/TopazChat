using JetBrains.Annotations;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class EventDispatcher : TopazChatBase
    {
        /// <summary>
        /// イベントリスナーの配列。
        /// AddEventListenerされるまではnullになっていることに注意してください。
        /// </summary>
        private PlayerEventListener[] listeners;

        [PublicAPI]
        public void AddEventListener(PlayerEventListener listener)
        {
            if (listener == null) //nullのlistenerを渡すな
            {
                LogError("A null EventListener was provided.");
                return;
            }
            if (listeners == null) //初めてAddEventListenerされたときに配列を初期化
            {
                listeners = new PlayerEventListener[0];
            }

            //配列を拡張して新しいlistenerを追加
            var array = new PlayerEventListener[listeners.Length + 1];
            listeners.CopyTo(array, 0);
            array[listeners.Length] = listener;
            listeners = array;

            //ログと通知
            Log($"Added EventListener: {listener.ListenerName}");
            listener.OnListenerReady();
        }

        /// <summary>
        /// 情報メッセージをUIに表示します。
        /// </summary>
        /// <param name="msg"></param>
        protected void ShowMessage(string msg)
        {
            _ShowMessage(msg, MessageLevel.Info);
        }

        /// <summary>
        /// メッセージをUIに表示します。
        /// </summary>
        protected void ShowMessage(string msg, MessageLevel level)
        {
            _ShowMessage(msg, level);
        }

        private void _ShowMessage(string msg, MessageLevel level)
        {
            if (!Utilities.IsValid(listeners)) return;
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].UpdateMessage($"<color={TopazUtils.MessageLevelColor(level)}>{msg}</color>");
            }
        }

        /// <summary>
        /// 再生状態の変更をイベントリスナーに通知します。
        /// </summary>
        /// <param name="playerStatus"></param>
        protected void UpdatePlayerStatus(PlayerStatus playerStatus)
        {
            if (!Utilities.IsValid(listeners)) return;
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].UpdateStatus(playerStatus);
            }
        }

        /// <summary>
        /// URLの変更をイベントリスナーに通知します。
        /// </summary>
        protected void UpdateURL(VRCUrl url, VRCUrl url_Android)
        {
            if (!Utilities.IsValid(listeners)) return;
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].UpdateURL(url, url_Android);
            }
        }

        /// <summary>
        /// 音量の変更をイベントリスナーに通知します。
        /// </summary>
        protected void ChangeVolume(float volume)
        {
            if (!Utilities.IsValid(listeners)) return;
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].OnChangeVolume(volume);
            }
        }
    }
}
