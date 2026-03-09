
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Player : EventDispatcher
    {
        // ----- default URLs -----

        [SerializeField] internal VRCUrl defaultStreamURL;
        [SerializeField] internal VRCUrl defaultStreamURL_Android;

        [PublicAPI]
        public VRCUrl GetPlatformDefaultStreamURL(Platform platform)
        {
            return platform == Platform.Android ? defaultStreamURL_Android : defaultStreamURL;
        }

        // ----- VRC AVPro Video Player -----

        [SerializeField] private VRCAVProVideoPlayer videoPlayer;

        // ----- screen -----

        [SerializeField] private MeshRenderer screen;
        [PublicAPI]
        public Material ScreenMaterial { get => screen.sharedMaterial; }

        // ----- Player status -----

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

        // ----- VRCURL -----
        private readonly VRCUrl StopURL = new VRCUrl("stop");

        // ---------------------------------------------

        /// <summary>
        /// 指定したURLで再生します。
        /// </summary>
        /// <param name="platformURL">プラットフォームに応じたURLにしてください。</param>
        private void _PlayURL(VRCUrl platformURL, PlayType playType)
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
                LogError("Invalid URL. Unable to play.");
                ShowMessage("Invalid URL: " + TopazUtils.InvalidTopazLinkReason(platformURL), MessageLevel.Error);
                _SafeStop();
                return;
            }
        }

        /// <summary>
        /// 指定したURLで再生処理をします。
        /// </summary>
        private void _StartStream(VRCUrl url, VRCUrl url_Android)
        {
            UpdateURL(url, url_Android);
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Received a start playback event while paused. Ignoring.");
                return;
            }
            ShowMessage("Starting stream...");
            _Stop(StopType.PlayNext);
            if (GetRunningPlatform() == Platform.Android)
            {
                _PlayURL(url_Android, PlayType.Play);
            }
            else
            {
                _PlayURL(url, PlayType.Play);
            }
        }

        /// <summary>
        /// ReSyncします。
        /// </summary>
        protected void _Resync()
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Received a resync event while paused. Ignoring.");
                return;
            }
            else
            {
                Log("Resync");
                _PlayURL(GetPlatformSyncStreamURL(), PlayType.ReSync);
            }
        }

        /// <summary>
        /// 再生を一時停止します。
        /// </summary>
        protected void _Pause()
        {
            Log("Paused");
            ShowMessage("Paused");
            _Stop(StopType.Pause);
            PlayerStatus = PlayerStatus.Pause;
        }

        /// <summary>
        /// 再生を再開します。
        /// </summary>
        protected void _Resume()
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Resume");
                _PlayURL(GetPlatformSyncStreamURL(), PlayType.Resume);
            }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        protected void _Stop(StopType stopType)
        {
            videoPlayer.Stop();
            if (stopType == StopType.UserStop)
            {
                ShowMessage("");
            }
            PlayerStatus = PlayerStatus.Stop;
        }

        /// <summary>
        /// 何か再生できない事情が発生した場合に明示的に再生を停止します。
        /// </summary>
        private void _SafeStop()
        {
            _Stop(StopType.ErrorStop);
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
            _SafeStop();
        }





        // ----------------------------------------

        // --------- UdonSync ----------
        [UdonSynced] private VRCUrl _SyncStreamURL;
        [UdonSynced] private VRCUrl _SyncStreamURL_Android;

        // ----------- Get用 ------------
        public VRCUrl SyncStreamURL => _SyncStreamURL;
        public VRCUrl SyncStreamURL_Android => _SyncStreamURL_Android;

        // ----------- Set用 ------------

        public void SetUrl(VRCUrl tmpStreamURL, VRCUrl tmpStreamURL_Android)
        {
            if (tmpStreamURL != null && tmpStreamURL.ToString() == StopURL.ToString())
            {
                Log("Stopping stream...");
                TakeOwner();
                _SyncStreamURL = StopURL;
                _SyncStreamURL_Android = StopURL;
                RequestSerialization();
                _Stop(StopType.UserStop);
                return;
            }
            if (TopazUtils.IsTopazLink(tmpStreamURL) && TopazUtils.IsTopazLink(tmpStreamURL_Android))
            {
                TakeOwner();
                _SyncStreamURL = tmpStreamURL;
                _SyncStreamURL_Android = tmpStreamURL_Android;
                RequestSerialization();
                _StartStream(tmpStreamURL, tmpStreamURL_Android);
            }
            else
            {
                LogWarning("Only TopazChat URLs can be played.");
                return;
            }
        }

        // ----------------------------------------

        public VRCUrl GetPlatformSyncStreamURL()
        {
            if (GetRunningPlatform() == Platform.Android)
            {
                return SyncStreamURL_Android;
            }
            else
            {
                return SyncStreamURL;
            }
        }

        private void _CheckReceivedURL()
        {
            if (Utilities.IsValid(SyncStreamURL) && Utilities.IsValid(SyncStreamURL_Android))
            {
                if (SyncStreamURL.ToString() == StopURL.ToString())
                {
                    Log("Received URL to stop stream.");
                    _Stop(StopType.UserStop);
                    return;
                }
                else
                {
                    Log("Received valid stream URL. Starting stream...");
                    _StartStream(SyncStreamURL, SyncStreamURL_Android);
                }
            }
            else
            {
                LogError("UdonSync failed. Unable to play.");
                ShowMessage("UdonSync failed. Unable to play.", MessageLevel.Error);
                _SafeStop();
            }
        }

        private void _PlayerJoinSync(VRCPlayerApi joinedPlayer)
        {
            if (VRCPlayerApi.GetPlayerCount() <= 1) //インスタンス人数がひとりなら
            {
                Log("Welcome! Play with defalut URL...");
                _SetDefaultURL();
            }
            else if (joinedPlayer.isLocal) //インスタンス人数が二人以上で、あなたがJoinした人なら
            {
                Log("Welcome! checking if received URLs can be played...");
                _CheckReceivedURL();
            }
        }

        protected void _UserStop()
        {
            SetUrl(StopURL, StopURL);
        }

        private void _SetDefaultURL()
        {
            SetUrl(GetPlatformDefaultStreamURL(Platform.Windows), GetPlatformDefaultStreamURL(Platform.Android));
        }

        private void TakeOwner()
        {
            var local = Networking.LocalPlayer;
            if (!Networking.IsOwner(local, this.gameObject))
            {
                Networking.SetOwner(local, this.gameObject);
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            _PlayerJoinSync(player);
        }

        public override void OnDeserialization()
        {
            _CheckReceivedURL();
        }
    }
}
