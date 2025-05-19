
using UdonSharp;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class URLSync : VideoEventListener
    {
        // ----------------------------------------

        // --------- UdonSync ----------
        [UdonSynced] private VRCUrl _SyncStreamURL;
        [UdonSynced] private VRCUrl _SyncStreamURL_Android;

        // ----------- Get用 ------------
        private VRCUrl SyncStreamURL => _SyncStreamURL;
        private VRCUrl SyncStreamURL_Android => _SyncStreamURL_Android;

        // ----------- Set用 ------------

        internal void SetUrl(VRCUrl tmpStreamURL, VRCUrl tmpStreamURL_Android)
        {
            if (TopazUtils.IsTopazLink(tmpStreamURL) && TopazUtils.IsTopazLink(tmpStreamURL_Android))
            {
                TakeOwner();
                _SyncStreamURL = tmpStreamURL;
                _SyncStreamURL_Android = tmpStreamURL_Android;
                RequestSerialization();
                StartStream(tmpStreamURL, tmpStreamURL_Android);
            }
            else
            {
                LogWarning("TopazChat以外のURLは再生できません。");
                return;
            }
        }

        // ----------------------------------------

        /// <summary>
        /// 現在実行中のプラットフォームに応じたStreamURLを返します。
        /// </summary>
        public VRCUrl PlatformSyncStreamURL
        {
            get
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
        }

        private void CheckAndStartStream()
        {
            if (Utilities.IsValid(SyncStreamURL) && Utilities.IsValid(SyncStreamURL_Android))
            {
                StartStream(SyncStreamURL, SyncStreamURL_Android);
            }
            else
            {
                LogError("UdonSyncに失敗しました。再生できません。");
                ShowMessage("UdonSync failed. Unable to play.", MessageLevel.Error);
                SafeStop();
            }
        }

        private void PlayerJoinSync(VRCPlayerApi joinedPlayer)
        {
            if (VRCPlayerApi.GetPlayerCount() <= 1) //インスタンス人数がひとりなら
            {
                Log("Play with defalut URL");
                SetUrl(GetPlatformDefaultStreamURL(Platform.Windows), GetPlatformDefaultStreamURL(Platform.Android)); //DefaultStreamURLで再生
            }
            else if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                Log("Send URL to new player");
                if (TopazUtils.IsValidTopazLink(SyncStreamURL) && TopazUtils.IsValidTopazLink(SyncStreamURL_Android))
                {
                    RequestSerialization();
                }
                else
                {
                    LogWarning("Streaming URL is invalid. Play with defalut URL.");
                    SetUrl(GetPlatformDefaultStreamURL(Platform.Windows), GetPlatformDefaultStreamURL(Platform.Android));
                }
            }
            else if (joinedPlayer.isLocal)
            {
                Log("Hello! Please wait while get URL from owner...");
            }
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
            PlayerJoinSync(player);
        }

        public override void OnDeserialization()
        {
            CheckAndStartStream();
        }
    }
}