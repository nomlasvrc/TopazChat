
using UdonSharp;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class URLSync : VideoEventListener
    {
        // ----------------------------------------
        //
        //                UdonSync

        [UdonSynced] private VRCUrl _SyncStreamURL;
        [UdonSynced] private VRCUrl _SyncStreamURL_Android;

        // -----------------------------------------

        private VRCUrl SyncStreamURL => _SyncStreamURL;
        private VRCUrl SyncStreamURL_Android => _SyncStreamURL_Android;

        public override void OnDeserialization()
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

        /// <summary>
        /// StreamURLを設定します。
        /// </summary>
        internal void SetSyncStreamURL(VRCUrl url, Platform platform)
        {
            if (platform == Platform.Android)
            {
                _SyncStreamURL_Android = url;
            }
            else
            {
                _SyncStreamURL = url;
            }
        }

        internal void SetUrl(VRCUrl tmpStreamURL, VRCUrl tmpStreamURL_Android) // Global
        {
            if (!TopazUtils.IsTopazLink(tmpStreamURL) || !TopazUtils.IsTopazLink(tmpStreamURL_Android))
            {
                LogWarning("TopazChat以外のURLは再生できません。");
                return;
            }
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            SetSyncStreamURL(tmpStreamURL, Platform.Windows);
            SetSyncStreamURL(tmpStreamURL_Android, Platform.Android);
            RequestSerialization();
            StartStream(tmpStreamURL, tmpStreamURL_Android);
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
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
            else if (player.isLocal)
            {
                Log("Hello! Please wait while get URL from owner...");
            }
        }
    }
}