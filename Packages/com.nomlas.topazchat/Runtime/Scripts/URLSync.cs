
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class URLSync : Player
    {
        [UdonSynced, FieldChangeCallback(nameof(SyncStreamURL))]
        private VRCUrl _SyncStreamURL;

        internal VRCUrl SyncStreamURL // Local
        {
            get => _SyncStreamURL;
            set
            {
                _SyncStreamURL = value;
                StartStream(value);
            }
        }

        internal void SetUrl(VRCUrl tmpStreamURL) // Global
        {
            if (!IsTopazLink(tmpStreamURL)) return;
            if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            SyncStreamURL = tmpStreamURL;
            RequestSerialization();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (VRCPlayerApi.GetPlayerCount() <= 1) //インスタンス人数がひとりなら
            {
                SetUrl(defaultStreamURL); //streamURLで再生
            }
            else if (player.isLocal)
            {
                RequestSerialization(); //StreamURLをオーナーからもらう
            }
        }
    }
}