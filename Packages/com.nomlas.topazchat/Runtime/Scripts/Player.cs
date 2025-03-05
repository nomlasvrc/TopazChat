
using TMPro;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Player : TopazChatBase
    {
        #region Inspector
        [SerializeField] internal VRCUrl defaultStreamURL;
        [SerializeField] internal TextMeshProUGUI addressText;
        [SerializeField] private VRCAVProVideoPlayer videoPlayer;
        [SerializeField] private MeshRenderer screen;
        [SerializeField] private AudioSource[] speakers;
        #endregion

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

        internal Material screenMaterial {get => screen.sharedMaterial;}

        #region Listener
        private PlayerEventListener[] listeners;

        internal void AddEventListener(PlayerEventListener listener)
        {
            if (listeners == null)
                listeners = new PlayerEventListener[0];
            var array = new PlayerEventListener[listeners.Length + 1];
            listeners.CopyTo(array, 0);
            array[listeners.Length] = listener;
            listeners = array;
            Log("Added EventListener");
        }
        #endregion

        private void PlayURL(VRCUrl url)
        {
            Log("URL Changed: " + url.ToString());
            videoPlayer.PlayURL(url);
        }

        internal protected void StartStream(VRCUrl url)
        {
            Stop();
            UpdateURL(url);
            PlayURL(url);
        }

        private void UpdateURL(VRCUrl url)
        {
            foreach (PlayerEventListener listener in listeners)
            {
                listener.UpdateURL(url);
            }
        }

        public void GlobalSync() //GlobalSyncボタンが押されたときに発火
        {
            Log("Global Sync");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Resync");
            Resync();
        }

        internal void Resync()
        {
            Log("Resync");
            PlayURL(streamURL);
        }

        internal void Stop()
        {
            videoPlayer.Stop();
        }
    }
}
