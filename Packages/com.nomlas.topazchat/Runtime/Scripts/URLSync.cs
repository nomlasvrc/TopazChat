
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class URLSync : Player
    {
        #region UdonSync
        [UdonSynced, FieldChangeCallback(nameof(FieldChangeCallbackedSyncStreamURL))]
        private VRCUrl _SyncStreamURL;
        [UdonSynced]
        private VRCUrl _SyncStreamURL_Android;
        #endregion

        private VRCUrl FieldChangeCallbackedSyncStreamURL
        {
            set
            {
                StartStream(GetSyncStreamURL(Platform.Windows), GetSyncStreamURL(Platform.Android));
            }
        }

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
        internal VRCUrl PlatformSyncStreamURL { get => GetSyncStreamURL(GetRunningPlatform()); }

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
            if (!IsTopazLink(tmpStreamURL) || !IsTopazLink(tmpStreamURL_Android)) return;
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
                SetUrl(GetPlatformDefaultStreamURL(Platform.Windows), GetPlatformDefaultStreamURL(Platform.Android)); //streamURLで再生
            }
            else if (player.isLocal)
            {
                Log("Hello! Please wait while get URL from owner...");
                RequestSerialization(); //StreamURLをオーナーからもらう
            }
        }
    }
}