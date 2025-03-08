
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class URLSync : Player
    {
        [UdonSynced, FieldChangeCallback(nameof(FieldChangeCallbackedSyncStreamURL))]
        private VRCUrl _SyncStreamURL;
        [UdonSynced]
        private VRCUrl _SyncStreamURL_Android;

        private VRCUrl FieldChangeCallbackedSyncStreamURL
        {
            set
            {
                StartStream(_SyncStreamURL, _SyncStreamURL_Android);
            }
        }

        internal VRCUrl SyncStreamURL
        {
            get
            {
#if UNITY_ANDROID
                return _SyncStreamURL_Android;
#else
                return _SyncStreamURL;
#endif
            }
        }

        internal void SetUrl(VRCUrl tmpStreamURL, VRCUrl tmpStreamURL_Android) // Global
        {
            if (!IsTopazLink(tmpStreamURL) || !IsTopazLink(tmpStreamURL_Android)) return;
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            _SyncStreamURL = tmpStreamURL;
            _SyncStreamURL_Android = tmpStreamURL_Android;
            RequestSerialization();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (VRCPlayerApi.GetPlayerCount() <= 1) //インスタンス人数がひとりなら
            {
                Log("Play with defalut URL");
                SetUrl(defaultStreamURL, defaultStreamURL_Android); //streamURLで再生
            }
            else if (player.isLocal)
            {
                Log("Hello! Please wait while get URL from owner...");
                RequestSerialization(); //StreamURLをオーナーからもらう
            }
        }
    }
}