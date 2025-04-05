
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
        [HideInInspector] public PlayerStatus playerStatus { get; private set; }
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
        private void PlayURL(VRCUrl platformURL, PlayType playType)
        {
            if (IsValidTopazLink(platformURL))
            {
                Log("URL Changed: " + platformURL.ToString());
                ShowMessage("Streaming: " + platformURL.ToString());
                videoPlayer.PlayURL(platformURL);
                playerStatus = PlayerStatus.Play;
            }
            else
            {
                LogError("URLが無効です。再生できません。");
                ShowMessage("Invalid URL. Unable to play.", MessageLevel.Error);
                SafeStop();
                return;
            }
        }

        internal protected void StartStream(VRCUrl url, VRCUrl url_Android)
        {
            Stop(true);
            UpdateURL(url, url_Android);
            if (RunningPlatformIsAndroid())
            {
                PlayURL(url_Android, PlayType.Play);
            }
            else
            {
                PlayURL(url, PlayType.Play);
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
            PlayURL(player.PlatformSyncStreamURL, PlayType.ReSync);
        }

        private VRCUrl resumeURL;
        internal void Pause()
        {
            Log("Paused");
            ShowMessage("Paused");
            resumeURL = player.PlatformSyncStreamURL;
            videoPlayer.Stop();
            playerStatus = PlayerStatus.Pause;
        }

        internal void Resume()
        {
            if ((playerStatus == PlayerStatus.Pause) && IsValidTopazLink(resumeURL))
            {
                Log("Resume");
                PlayURL(resumeURL, PlayType.Resume);
            }
        }

        protected virtual void VolumeChange() { }

        internal void Stop(bool hideMessage)
        {
            videoPlayer.Stop();
            if (hideMessage) ShowMessage("");
            playerStatus = PlayerStatus.Stop;
        }

        internal void SafeStop()
        {
            Stop(false);
        }

        private void PVideoError(VideoError videoError)
        {
            LogError("Video Error: " + videoError.ToString());
            switch (videoError)
            {
                case VideoError.RateLimited:
                    ShowMessage("VideoError: Rate Limited", MessageLevel.Error);
                    break;
                case VideoError.AccessDenied:
                    ShowMessage("VideoError: Access Denied", MessageLevel.Error);
                    break;
                case VideoError.InvalidURL:
                    ShowMessage("VideoError: Invalid URL", MessageLevel.Error);
                    break;
                case VideoError.PlayerError:
                    ShowMessage("VideoError: Player Error", MessageLevel.Error);
                    break;
                case VideoError.Unknown:
                    ShowMessage("VideoError: Unknown Error", MessageLevel.Error);
                    break;
            }
            SafeStop();
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
