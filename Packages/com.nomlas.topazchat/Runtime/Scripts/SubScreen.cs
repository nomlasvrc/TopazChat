
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class SubScreen : PlayerEventListener
    {
        [SerializeField] MeshRenderer target;
        public override void OnListenerReady()
        {
            if (player.ScreenMaterial != null)
            {
                target.sharedMaterial = player.ScreenMaterial;
                Log("サブスクリーンを設定しました");
            }
            else
            {
                LogError("サブスクリーンの設定に失敗しました");
            }
        }
    }
}