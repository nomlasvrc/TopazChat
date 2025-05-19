
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Player : EventDispatcher
    {
        #region Inspector
        [SerializeField] internal VRCUrl defaultStreamURL;
        [SerializeField] internal VRCUrl defaultStreamURL_Android;
        [SerializeField] private VRCAVProVideoPlayer videoPlayer;
        [SerializeField] private AudioSource[] speakers;
        [SerializeField] private MeshRenderer screen;
        #endregion
        private PlayerStatus _PlayerStatus;
        public PlayerStatus PlayerStatus
        {
            get
            {
                return _PlayerStatus;
            }
            private set
            {
                _PlayerStatus = value;
                UpdatePlayerStatus(value);
            }
        }
        public VRCUrl GetPlatformDefaultStreamURL(Platform platform)
        {
            return platform == Platform.Android ? defaultStreamURL_Android : defaultStreamURL;
        }

        protected virtual VRCUrl GetPlatformSyncStreamURL() { return null; }

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
                OnVolumeChange();
            }
        }

        public Material ScreenMaterial { get => screen.sharedMaterial; }

        /// <summary>
        /// 指定したURLで再生します
        /// </summary>
        /// <param name="platformURL">プラットフォームに応じたURLにしてください。</param>
        private void PlayURL(VRCUrl platformURL, PlayType playType)
        {
            if (TopazUtils.IsValidTopazLink(platformURL))
            {
                Log("URL Changed: " + platformURL.ToString());
                ShowMessage("Streaming: " + platformURL.ToString());
                videoPlayer.PlayURL(platformURL);
                PlayerStatus = PlayerStatus.Play;
            }
            else
            {
                LogError("URLが無効です。再生できません。");
                ShowMessage("Invalid URL: " + TopazUtils.CheckInvalidTopazLink(platformURL), MessageLevel.Error);
                SafeStop();
                return;
            }
        }

        internal protected void StartStream(VRCUrl url, VRCUrl url_Android)
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("ポーズ中に再生開始イベントを受信しました。無視します。");
                UpdateURL(url, url_Android);
                return;
            }
            Stop(StopType.Stop);
            UpdateURL(url, url_Android);
            if (GetRunningPlatform() == Platform.Android)
            {
                PlayURL(url_Android, PlayType.Play);
            }
            else
            {
                PlayURL(url, PlayType.Play);
            }
        }

        /// <summary>
        /// GlobalでReSyncします。
        /// </summary>
        internal void GlobalSync() //GlobalSyncボタンが押されたときに発火
        {
            Log("Global Sync");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Resync");
            Resync();
        }

        /// <summary>
        /// ReSyncします。
        /// </summary>
        internal void Resync()
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("ポーズ中にResyncイベントを受信しました。無視します。");
                return;
            }
            else
            {
                Log("Resync");
                PlayURL(GetPlatformSyncStreamURL(), PlayType.ReSync);
            }
        }

        /// <summary>
        /// 再生を一時停止します。
        /// </summary>
        internal void Pause()
        {
            Log("Paused");
            ShowMessage("Paused");
            Stop(StopType.Pause);
            PlayerStatus = PlayerStatus.Pause;
        }

        /// <summary>
        /// 再生を再開します。
        /// </summary>
        internal void Resume()
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Resume");
                PlayURL(GetPlatformSyncStreamURL(), PlayType.Resume);
            }
        }

        private void OnVolumeChange()
        {
            foreach (AudioSource speaker in speakers)
            {
                if (Utilities.IsValid(speaker)) speaker.volume = Volume;
            }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        internal void Stop(StopType stopType)
        {
            videoPlayer.Stop();
            if (stopType == StopType.Stop)
            {
                ShowMessage("");
            }
            PlayerStatus = PlayerStatus.Stop;
        }

        /// <summary>
        /// 何か再生できない事情が発生した場合に明示的に再生を停止します。
        /// </summary>
        internal void SafeStop()
        {
            Stop(StopType.ErrorStop);
        }

        protected void PVideoError(VideoError videoError)
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
    }
}
