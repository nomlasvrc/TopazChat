
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class VideoStatusWatcher : PlayerEventListener
    {
        public override void OnVideoEnd() { player.PlayerVideoEnd(); }
        public override void OnVideoError(VRC.SDK3.Components.Video.VideoError videoError) { player.PlayerVideoError(videoError); }
        public override void OnVideoLoop() { player.PlayerVideoLoop(); }
        public override void OnVideoPause() { player.PlayerVideoPause(); }
        public override void OnVideoPlay() { player.PlayerVideoPlay(); }
        public override void OnVideoReady() { player.PlayerVideoReady(); }
        public override void OnVideoStart() { player.PlayerVideoStart(); }
    }
}