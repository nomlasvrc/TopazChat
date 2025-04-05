
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
        private Vector3 checkPosition;
        private void Start()
        {
            sqrPauseDistance = pauseDistance * pauseDistance;
            sqrResumeDistance = resumeDistance * resumeDistance;
            local = Networking.LocalPlayer;
            checkPosition = this.transform.position;
            isPausing = false;
        }

        private void Update()
        {
            var pos = local.GetPosition();
            float sqrDistance = (checkPosition - pos).sqrMagnitude;
            if (sqrDistance > sqrPauseDistance)
            {
                player.Pause();
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