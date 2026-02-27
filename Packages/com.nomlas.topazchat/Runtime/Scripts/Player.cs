
using JetBrains.Annotations;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Player : EventDispatcher
    {
        // ---------------------------------------------

        [SerializeField] internal VRCUrl defaultStreamURL;
        [SerializeField] internal VRCUrl defaultStreamURL_Android;

        [PublicAPI]
        public VRCUrl GetPlatformDefaultStreamURL(Platform platform)
        {
            return platform == Platform.Android ? defaultStreamURL_Android : defaultStreamURL;
        }

        // ---------------------------------------------

        [SerializeField] private VRCAVProVideoPlayer videoPlayer;

        // ---------------------------------------------

        [SerializeField] private MeshRenderer screen;
        [PublicAPI]
        public Material ScreenMaterial { get => screen.sharedMaterial; }

        // ---------------------------------------------

        private PlayerStatus _PlayerStatus;
        [PublicAPI]
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

        // ---------------------------------------------
        // virtualだが必須
        protected virtual VRCUrl GetPlatformSyncStreamURL() { return null; }

        // ---------------------------------------------

        /// <summary>
        /// 指定したURLで再生します
        /// </summary>
        /// <param name="platformURL">プラットフォームに応じたURLにしてください。</param>
        private void PlayURL(VRCUrl platformURL, PlayType playType)
        {
            if (TopazUtils.IsValidTopazLink(platformURL))
            {
                var platformURLString = platformURL.ToString();
                switch (playType)
                {
                    case PlayType.Play:
                        Log("Play: " + platformURLString);
                        break;
                    case PlayType.Resume:
                        Log("Resume: " + platformURLString);
                        break;
                    case PlayType.ReSync:
                        Log("ReSync: " + platformURLString);
                        break;
                }
                ShowMessage("Streaming: " + platformURLString);
                videoPlayer.PlayURL(platformURL);
                PlayerStatus = PlayerStatus.Play;
            }
            else
            {
                LogError("URLが無効です。再生できません。");
                ShowMessage("Invalid URL: " + TopazUtils.InvalidTopazLinkReason(platformURL), MessageLevel.Error);
                SafeStop();
                return;
            }
        }

        protected void StartStream(VRCUrl url, VRCUrl url_Android)
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("ポーズ中に再生開始イベントを受信しました。無視します。");
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
        /// ReSyncします。
        /// </summary>
        protected void _Resync()
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
        public void Pause()
        {
            Log("Paused");
            ShowMessage("Paused");
            Stop(StopType.Pause);
            PlayerStatus = PlayerStatus.Pause;
        }

        /// <summary>
        /// 再生を再開します。
        /// </summary>
        public void Resume()
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Resume");
                PlayURL(GetPlatformSyncStreamURL(), PlayType.Resume);
            }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        public void Stop(StopType stopType)
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
        public void SafeStop()
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
