
using TMPro;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Player : TopazChatBase
    {
        [SerializeField] internal VRCUrl defaultStreamURL;
        [SerializeField] private VRCAVProVideoPlayer videoPlayer;
        [SerializeField] private AudioSource[] speakers;

        protected TopazChatPlayer player;

        private VRCUrl streamURL { get => player.SyncStreamURL; }

        private float _volume;
        internal float volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = Mathf.Clamp01(value);
                foreach (AudioSource audioSource in speakers)
                {
                    audioSource.volume = _volume;
                }
            }
        }

        internal protected void StartStream(VRCUrl url)
        {
            Debug.Log("Play URL: " + url.ToString());
            Stop();
            UpdateURL(url);
            videoPlayer.PlayURL(url);
        }

        public Controller[] controllers;
        private void UpdateURL(VRCUrl url)
        {
            foreach (Controller controller in controllers)
            {
                controller.UpdateURL(url);
            }
        }

        public void GlobalSync() //GlobalSyncボタンが押されたときに発火
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Resync");
            Resync();
        }

        internal void Resync()
        {
            videoPlayer.PlayURL(streamURL);
        }

        internal void Stop()
        {
            videoPlayer.Stop();
        }
    }
}
