using VRC.SDK3.Components.Video;

namespace Nomlas.TopazChat
{
    public class VideoStatusWatcher : PlayerEventListener
    {
        public override void OnVideoEnd() { player.PlayerVideoEnd(); }
        public override void OnVideoError(VideoError videoError) { player.PlayerVideoError(videoError); }
        public override void OnVideoLoop() { player.PlayerVideoLoop(); }
        public override void OnVideoPause() { player.PlayerVideoPause(); }
        public override void OnVideoPlay() { player.PlayerVideoPlay(); }
        public override void OnVideoReady() { player.PlayerVideoReady(); }
        public override void OnVideoStart() { player.PlayerVideoStart(); }
    }
}