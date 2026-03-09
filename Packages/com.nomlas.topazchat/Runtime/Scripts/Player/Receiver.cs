using JetBrains.Annotations;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Receiver : Player
    {
        [SerializeField] private AudioSource[] speakers;
        private float _Volume;
        [PublicAPI]
        public float Volume
        {
            get
            {
                return _Volume;
            }
            set
            {
                _Volume = Mathf.Clamp01(value);
                if (_Volume != value)
                {
                    Log($"Volume: {_Volume} => {value}");
                }
                OnVolumeChange();
                ChangeVolume(value);
            }
        }

        private void OnVolumeChange()
        {
            foreach (AudioSource speaker in speakers)
            {
                if (Utilities.IsValid(speaker)) speaker.volume = Volume;
            }
        }

        public void Pause() => _Pause();
        public void Resume() => _Resume();
        public void Resync() => _Resync();
        public void GlobalUserStop()
        {
            Log("Global Stop");
            _UserStop();
        }

        /// <summary>
        /// GlobalでReSyncします。
        /// </summary>
        public void GlobalSync() //GlobalSyncボタンが押されたときに発火
        {
            Log("Global Sync");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ReceiveSync));
        }

        [NetworkCallable]
        public void ReceiveSync()
        {
            var callingPlayer = NetworkCalling.CallingPlayer;
            if (Utilities.IsValid(callingPlayer))
            {
                Log($"Global Sync from {callingPlayer.displayName}");
            }
            else
            {
                Log("Global Sync from unknown player");
            }
            _Resync();
        }

        // ----- Video Events -----
        public void PlayerVideoEnd() { }
        public void PlayerVideoError(VideoError videoError) { PVideoError(videoError); }
        public void PlayerVideoLoop() { }
        public void PlayerVideoPause() { }
        public void PlayerVideoPlay() { }
        public void PlayerVideoReady() { }
        public void PlayerVideoStart() { }
        // -----
    }
}
