
using UdonSharp;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class URLSync : Player
    {
        #region UdonSync
        [UdonSynced] private VRCUrl _SyncStreamURL;
        [UdonSynced] private VRCUrl _SyncStreamURL_Android;

        public override void OnDeserialization()
        {
            if (!Utilities.IsValid(_SyncStreamURL) || !Utilities.IsValid(_SyncStreamURL_Android))
            {
                LogError("UdonSyncに失敗しました。再生できません。");
                ShowMessage("UdonSync failed. Unable to play.", MessageLevel.Error);
                SafeStop();
                return;
            }
            StartStream(_SyncStreamURL, _SyncStreamURL_Android);
        }
        #endregion

        #region PlatformURL
        /// <summary>
        /// プラットフォームに応じたStreamURLを返します。
        /// </summary>
        public VRCUrl GetSyncStreamURL(Platform platform)
        {
            return platform == Platform.Android ? _SyncStreamURL_Android : _SyncStreamURL;
        }

        /// <summary>
        /// 現在実行中のプラットフォームに応じたStreamURLを返します。
        /// </summary>
        public VRCUrl PlatformSyncStreamURL { get => GetSyncStreamURL(GetRunningPlatform()); }

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
        #endregion

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
                var _url = GetSyncStreamURL(Platform.Windows);
                var _url_Android = GetSyncStreamURL(Platform.Android);
                Log("Send URL to new player");
                if (TopazUtils.IsValidTopazLink(_url) && TopazUtils.IsValidTopazLink(_url_Android))
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