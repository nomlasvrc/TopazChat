using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class AutoPause : PlayerEventListener
    {
        public override string ListenerName => "AutoPause";
        [SerializeField] internal float pauseDistance = 40;
        [SerializeField] internal float resumeDistance = 38;
        /// <summary>
        /// 一時停止する距離の二乗
        /// </summary>
        private float sqrPauseDistance;
        /// <summary>
        /// 再生を再開する距離の二乗
        /// </summary>
        private float sqrResumeDistance;
        private bool isPausing;
        private VRCPlayerApi local;
        protected override void Start()
        {
            base.Start();

            // 距離の二乗を計算して保存
            sqrPauseDistance = pauseDistance * pauseDistance;
            sqrResumeDistance = resumeDistance * resumeDistance;

            local = Networking.LocalPlayer;
            isPausing = false;
        }

        private void Update()
        {
            var playerPos = local.GetPosition();
            var checkPos = this.transform.position;//一応持ち運び可能なようにする
            float sqrDistance = (checkPos - playerPos).sqrMagnitude;
            if (sqrDistance > sqrPauseDistance)
            {
                // 一時停止中でないなら一時停止する
                if (!isPausing) player.Pause();
                isPausing = true;
            }
            else if (sqrDistance < sqrResumeDistance)
            {
                // 一時停止中なら再開する
                if (isPausing) player.Resume();
                isPausing = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.transform.position, pauseDistance);
            Gizmos.DrawWireSphere(this.transform.position, resumeDistance);
        }
    }
}