using UnityEngine;

namespace Nomlas.TopazChat
{
    public class SubScreen : PlayerEventListener
    {
        [SerializeField] private MeshRenderer target;
        public override void OnListenerReady()
        {
            Material screenMat = player.ScreenMaterial;
            if (screenMat != null)
            {
                target.sharedMaterial = screenMat;
                Log("サブスクリーンを設定しました");
            }
            else
            {
                LogError("サブスクリーンの設定に失敗しました");
            }
        }

        public override string GetListenerName()
        {
            return "SubScreen";
        }
    }
}