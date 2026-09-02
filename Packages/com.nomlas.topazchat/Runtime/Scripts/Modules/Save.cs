
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

        /// <summary>
        /// 保存されたURLを返します。
        /// </summary>
        [PublicAPI] public VRCUrl SavedURL => savedUrl;

        /// <summary>
        /// URLを保存します。
        /// </summary>
        public void SaveKeyInternal(VRCUrl url)
        {
            savedUrl = url;
            RequestSerialization();
        }

        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            foreach (var c in controller)
            {
                c.OnRestoredUrlInternal(this);
            }
        }

    }
}