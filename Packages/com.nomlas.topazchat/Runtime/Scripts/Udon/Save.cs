
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
        
        [PublicAPI]
        public VRCUrl SavedURL => savedUrl;

        public void SaveKey(VRCUrl url)
        {
            savedUrl = url;
            RequestSerialization();
        }

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
            if (Utilities.IsValid(savedUrl))
            {
                foreach (var c in controller)
                {
                    c.OnRestoredUrl(savedUrl);
                }
            }
        }

    }
}