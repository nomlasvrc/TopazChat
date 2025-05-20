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

        public void AddEventListener(PlayerEventListener listener)
        {
            if (listener == null)
            {
                LogError("空のEventListenerが渡されました");
                return;
            }
            if (listeners == null)
                listeners = new PlayerEventListener[0];
            var array = new PlayerEventListener[listeners.Length + 1];
            listeners.CopyTo(array, 0);
            array[listeners.Length] = listener;
            listeners = array;
            Log("Added EventListener: " + listener.GetListenerName());
            listener.OnListenerReady();
        }

        protected void ShowMessage(string msg)
        {
            _ShowMessage(msg, MessageLevel.Info);
        }

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

        protected void UpdatePlayerStatus(PlayerStatus playerStatus)
        {
            if (!Utilities.IsValid(listeners)) return;
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].UpdateStatus(playerStatus);
            }
        }

        protected void UpdateURL(VRCUrl url, VRCUrl url_Android)
        {
            if (!Utilities.IsValid(listeners)) return;
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].UpdateURL(url, url_Android);
            }
        }
    }
}
