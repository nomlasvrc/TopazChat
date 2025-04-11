
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class AutoPause : PlayerEventListener
    {
        [SerializeField] internal float pauseDistance = 40;
        [SerializeField] internal float resumeDistance = 38;
        private float sqrPauseDistance;
        private float sqrResumeDistance;
        private bool isPausing;
        private VRCPlayerApi local;
        protected override void Start()
        {
            base.Start();
            sqrPauseDistance = pauseDistance * pauseDistance;
            sqrResumeDistance = resumeDistance * resumeDistance;
            local = Networking.LocalPlayer;
            isPausing = false;
        }

        private void Update()
        {
            var playerPos = local.GetPosition();
            var checkPos = this.transform.position;
            float sqrDistance = (checkPos - playerPos).sqrMagnitude;
            if (sqrDistance > sqrPauseDistance)
            {
                if (!isPausing) player.Pause();
                isPausing = true;
            }
            else if (sqrDistance < sqrResumeDistance)
            {
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