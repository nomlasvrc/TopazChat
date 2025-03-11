
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
        [SerializeField] internal VRCUrl defaultStreamURL_Android;
        [SerializeField] private VRCAVProVideoPlayer videoPlayer;
        [SerializeField] private MeshRenderer screen;
        [SerializeField] private AudioSource[] speakers;
        #endregion
        public VRCUrl GetPlatformDefaultStreamURL(Platform platform)
        {
            return platform == Platform.Android ? defaultStreamURL_Android : defaultStreamURL;
        }

        protected TopazChatPlayer player;

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
                VolumeChange();
            }
        }

        internal Material screenMaterial { get => screen.sharedMaterial; }

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
            listener.OnListenerReady();
        }
        #endregion

        /// <summary>
        /// 指定したURLで再生します
        /// </summary>
        /// <param name="platformURL">プラットフォームに応じたURLにしてください。</param>
        private void PlayURL(VRCUrl platformURL)
        {
            Log("URL Changed: " + platformURL.ToString());
            videoPlayer.PlayURL(platformURL);
        }

        internal protected void StartStream(VRCUrl url, VRCUrl url_Android)
        {
            Stop();
            UpdateURL(url, url_Android);
            if (RunningPlatformIsAndroid())
            {
                PlayURL(url_Android);
            }
            else
            {
                PlayURL(url);
            }
        }

        private void UpdateURL(VRCUrl url, VRCUrl url_Android)
        {
            foreach (PlayerEventListener listener in listeners)
            {
                listener.UpdateURL(url, url_Android);
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
            PlayURL(player.PlatformSyncStreamURL);
        }

        private void VolumeChange()
        {
            foreach (AudioSource speaker in speakers)
            {
                if (Utilities.IsValid(speaker)) speaker.volume = volume;
            }
        }

        internal void Stop()
        {
            videoPlayer.Stop();
        }
    }
}
