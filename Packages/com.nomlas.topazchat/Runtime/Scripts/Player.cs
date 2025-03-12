
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Player : TopazChatBase
    {
        #region Inspector
        [SerializeField] internal VRCUrl defaultStreamURL;
        [SerializeField] internal VRCUrl defaultStreamURL_Android;
        [SerializeField] private VRCAVProVideoPlayer videoPlayer;
        [SerializeField] private MeshRenderer screen;
        #endregion
        public VRCUrl GetPlatformDefaultStreamURL(Platform platform)
        {
            return platform == Platform.Android ? defaultStreamURL_Android : defaultStreamURL;
        }

        protected TopazChatPlayerBase player;

        private float _volume;
        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = Mathf.Clamp01(value);
                if (_volume != value)
                {
                    Log($"Volume: {_volume} => {value}");
                }
                VolumeChange();
            }
        }

        public Material ScreenMaterial { get => screen.sharedMaterial; }

        #region Listener
        private PlayerEventListener[] listeners;

        internal void AddEventListener(PlayerEventListener listener)
        {
            if (listeners == null)
                listeners = new PlayerEventListener[0];
            var array = new PlayerEventListener[listeners.Length + 1];
            listeners.CopyTo(array, 0);
            array[listeners.Length] = listener;
            listeners = array;
            Log("Added EventListener");
            listener.OnListenerReady();
        }

        internal void ShowMessage(string msg)
        {
            _ShowMessage(msg, MessageLevel.Info);
        }

        internal void ShowMessage(string msg, MessageLevel level)
        {
            _ShowMessage(msg, level);
        }

        private void _ShowMessage(string msg, MessageLevel level)
        {
            foreach (PlayerEventListener listener in listeners)
            {
                listener.UpdateMessage($"<color={MessageLevelColor(level)}>{msg}</color>");
            }
        }
        #endregion

        /// <summary>
        /// 指定したURLで再生します
        /// </summary>
        /// <param name="platformURL">プラットフォームに応じたURLにしてください。</param>
        private void PlayURL(VRCUrl platformURL)
        {
            if (!Utilities.IsValid(platformURL))
            {
                LogError("URLが無効です。再生できません。");
                ShowMessage("Invalid URL", MessageLevel.Error);
                return;
            }
            Log("URL Changed: " + platformURL.ToString());
            ShowMessage("Streaming: " + platformURL.ToString());
            videoPlayer.PlayURL(platformURL);
        }

        internal protected void StartStream(VRCUrl url, VRCUrl url_Android)
        {
            Stop();
            UpdateURL(url, url_Android);
            if (RunningPlatformIsAndroid())
            {
                PlayURL(url_Android);
            }
            else
            {
                PlayURL(url);
            }
        }

        private void UpdateURL(VRCUrl url, VRCUrl url_Android)
        {
            foreach (PlayerEventListener listener in listeners)
            {
                listener.UpdateURL(url, url_Android);
            }
        }

        public void GlobalSync() //GlobalSyncボタンが押されたときに発火
        {
            Log("Global Sync");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Resync");
            Resync();
        }

        internal void Resync()
        {
            Log("Resync");
            PlayURL(player.PlatformSyncStreamURL);
        }

        protected virtual void VolumeChange() { }

        internal void Stop()
        {
            videoPlayer.Stop();
            ShowMessage("");
        }

        private void PVideoError(VideoError videoError)
        {
            LogError("Video Error: " + videoError.ToString());
            switch (videoError)
            {
                case VideoError.RateLimited:
                    ShowMessage("Error: Rate Limited", MessageLevel.Error);
                    break;
                case VideoError.AccessDenied:
                    ShowMessage("Error: Access Denied", MessageLevel.Error);
                    break;
                case VideoError.InvalidURL:
                    ShowMessage("Error: Invalid URL", MessageLevel.Error);
                    break;
                case VideoError.PlayerError:
                    ShowMessage("Error: Player Error", MessageLevel.Error);
                    break;
                case VideoError.Unknown:
                    ShowMessage("Error: Unknown Error", MessageLevel.Error);
                    break;
            }
        }

        #region Video Events
        internal void PlayerVideoEnd() { }
        internal void PlayerVideoError(VideoError videoError) { PVideoError(videoError); }
        internal void PlayerVideoLoop() { }
        internal void PlayerVideoPause() { }
        internal void PlayerVideoPlay() { }
        internal void PlayerVideoReady() { }
        internal void PlayerVideoStart() { }
        #endregion
    }
}
