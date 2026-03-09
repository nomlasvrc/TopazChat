
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    [RequireComponent(typeof(VRCEnablePersistence), typeof(VRCPlayerObject))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Save : UdonSharpBehaviour
    {
        [SerializeField][Header("コントローラー")] internal Controller[] controller;
        [UdonSynced] private VRCUrl savedUrl;
        [UdonSynced] private VRCUrl savedUrl_Android;

        [PublicAPI] public VRCUrl SavedURL => savedUrl;
        [PublicAPI] public VRCUrl SavedURL_Android => savedUrl_Android;

        public void _SaveKey(VRCUrl url, VRCUrl url_Android)
        {
            savedUrl = url;
            savedUrl_Android = url_Android;
            RequestSerialization();
        }

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;

            foreach (var c in controller)
            {
                c._OnRestoredUrl(this);
            }
        }

    }
}