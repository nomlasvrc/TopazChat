
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
        // ----- default URL -----

        [SerializeField] internal VRCUrl defaultStreamURL;

        /// <summary>
        /// デフォルトのストリームURLを返します。
        /// </summary>
        [PublicAPI] public VRCUrl DefaultStreamURL => defaultStreamURL;

        // ----- VRC AVPro Video Player -----

        [SerializeField] private VRCAVProVideoPlayer videoPlayer;

        // ----- screen -----

        [SerializeField] private MeshRenderer screen;
        /// <summary>
        /// 映像が表示されるマテリアルを返します。
        /// </summary>
        [PublicAPI] public Material ScreenMaterial { get => screen.sharedMaterial; }

        // ----- Player status -----

        private PlayerStatus _PlayerStatus;

        /// <summary>
        /// 再生状態を返します。変更した場合はイベントリスナーに通知されます。
        /// </summary>
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
        private void _PlayURL(VRCUrl url, PlayType playType)
        {
            if (TopazUtils.IsValidTopazLink(url))
            {
                var urlString = url.ToString();
                switch (playType)
                {
                    case PlayType.Play:
                        Log("Play: " + urlString);
                        break;
                    case PlayType.Resume:
                        Log("Resume: " + urlString);
                        break;
                    case PlayType.ReSync:
                        Log("ReSync: " + urlString);
                        break;
                }
                ShowMessage("Streaming: " + urlString);
                videoPlayer.PlayURL(url);
                PlayerStatus = PlayerStatus.Play;
            }
            else
            {
                LogError("Invalid URL. Unable to play.");
                ShowMessage("Invalid URL: " + TopazUtils.InvalidTopazLinkReason(url), MessageLevel.Error);
                _SafeStop();
                return;
            }
        }

        /// <summary>
        /// 指定したURLで再生処理をします。
        /// </summary>
        private void _StartStream(VRCUrl url)
        {
            UpdateURL(url);
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Received a start playback event while paused. Ignoring.");
                return;
            }
            ShowMessage("Starting stream...");
            _Stop(StopType.PlayNext);
            _PlayURL(url, PlayType.Play);
        }

        /// <summary>
        /// ReSyncします。
        /// </summary>
        private protected void _Resync()
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Received a resync event while paused. Ignoring.");
                return;
            }
            else
            {
                Log("Resync");
                _PlayURL(SyncStreamURL, PlayType.ReSync);
            }
        }

        /// <summary>
        /// 再生を一時停止します。
        /// </summary>
        private protected void _Pause()
        {
            Log("Paused");
            ShowMessage("Paused");
            _Stop(StopType.Pause);
            PlayerStatus = PlayerStatus.Pause;
        }

        /// <summary>
        /// 再生を再開します。
        /// </summary>
        private protected void _Resume()
        {
            if (PlayerStatus == PlayerStatus.Pause)
            {
                Log("Resume");
                _PlayURL(SyncStreamURL, PlayType.Resume);
            }
        }

        /// <summary>
        /// 再生を停止します。
        /// </summary>
        private void _Stop(StopType stopType)
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

        private protected void PVideoError(VideoError videoError)
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

        // ----------- Get用 ------------
        /// <summary>
        /// 現在のストリームURLを返します。
        /// </summary>
        [PublicAPI] public VRCUrl SyncStreamURL => _SyncStreamURL;

        // ----------- Set用 ------------

        /// <summary>
        /// ストリームURLをセットします。stopURLやGlobal同期にも対応しています。
        /// </summary>
        [PublicAPI]
        public void SetUrl(VRCUrl tmpStreamURL)
        {
            if (tmpStreamURL != null && tmpStreamURL.ToString() == StopURL.ToString())
            {
                Log("Stopping stream...");
                TakeOwner();
                _SyncStreamURL = StopURL;
                RequestSerialization();
                _Stop(StopType.UserStop);
                return;
            }
            if (TopazUtils.IsTopazLink(tmpStreamURL))
            {
                TakeOwner();
                _SyncStreamURL = tmpStreamURL;
                RequestSerialization();
                _StartStream(tmpStreamURL);
            }
            else
            {
                LogWarning("Only TopazChat URLs can be played.");
                return;
            }
        }

        // ----------------------------------------

        /// <summary>
        /// 受け取ったURLが再生可能かつTopazChatのURLか確認し、再生又は停止処理を行います。
        /// </summary>
        private void _CheckReceivedURL()
        {
            if (Utilities.IsValid(SyncStreamURL))
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
                    _StartStream(SyncStreamURL);
                }
            }
            else
            {
                LogError("UdonSync failed. Unable to play.");
                ShowMessage("UdonSync failed. Unable to play.", MessageLevel.Error);
                _SafeStop();
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (VRCPlayerApi.GetPlayerCount() <= 1) //インスタンス人数がひとりなら
            {
                Log("Welcome! Play with defalut URL...");
                _SetDefaultURL();
            }
            else if (player.isLocal) //インスタンス人数が二人以上で、あなたがJoinした人なら
            {
                Log("Welcome! checking if received URL can be played...");
                _CheckReceivedURL();
            }
        }

        /// <summary>
        /// ユーザー入力により、Globalで再生を停止します。
        /// </summary>
        private protected void _UserStop()
        {
            SetUrl(StopURL);
        }

        /// <summary>
        /// GlobalでデフォルトのURLをセットします。
        /// </summary>
        private void _SetDefaultURL()
        {
            SetUrl(DefaultStreamURL);
        }

        private void TakeOwner()
        {
            var local = Networking.LocalPlayer;
            if (!Networking.IsOwner(local, this.gameObject))
            {
                Networking.SetOwner(local, this.gameObject);
            }
        }

        public override void OnDeserialization()
        {
            _CheckReceivedURL();
        }
    }
}
