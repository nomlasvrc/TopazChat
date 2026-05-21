
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

        /// <summary>
        /// 保存されたURLを返します。
        /// </summary>
        [PublicAPI] public VRCUrl SavedURL => savedUrl;
        /// <summary>
        /// 保存されたURL(Android)を返します。
        /// </summary>
        [PublicAPI] public VRCUrl SavedURL_Android => savedUrl_Android;

        /// <summary>
        /// URLを保存します。
        /// 内部用
        /// </summary>
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