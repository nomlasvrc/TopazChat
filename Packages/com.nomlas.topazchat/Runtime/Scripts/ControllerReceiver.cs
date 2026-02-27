using JetBrains.Annotations;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class ControllerReceiver : VideoEventListener
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
            }
        }

        private void OnVolumeChange()
        {
            foreach (AudioSource speaker in speakers)
            {
                if (Utilities.IsValid(speaker)) speaker.volume = Volume;
            }
        }

        /// <summary>
        /// ReSyncします。
        /// </summary>
        public void Resync()
        {
            _Resync();
        }

        /// <summary>
        /// GlobalでReSyncします。
        /// </summary>
        public void GlobalSync() //GlobalSyncボタンが押されたときに発火
        {
            Log("Global Sync");
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Resync");
            _Resync();
        }
    }
}
