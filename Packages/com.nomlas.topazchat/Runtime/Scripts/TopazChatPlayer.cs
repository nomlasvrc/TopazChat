
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class TopazChatPlayer : TopazChatPlayerBase
    {
        [SerializeField] private AudioSource[] speakers;
        private void Start()
        {
            player = this;
            foreach (AudioSource speaker in speakers)
            {
                if (!Utilities.IsValid(speaker)) LogWarning("nullのSpeakerがあります");
            }
        }

        protected override void VolumeChange()
        {
            foreach (AudioSource speaker in speakers)
            {
                if (Utilities.IsValid(speaker)) speaker.volume = Volume;
            }
        }
    }
}