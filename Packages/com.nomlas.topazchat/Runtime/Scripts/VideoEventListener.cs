
using VRC.SDK3.Components.Video;

namespace Nomlas.TopazChat
{
    public class VideoEventListener : Player
    {
        public void PlayerVideoEnd() { }
        public void PlayerVideoError(VideoError videoError) { PVideoError(videoError); }
        public void PlayerVideoLoop() { }
        public void PlayerVideoPause() { }
        public void PlayerVideoPlay() { }
        public void PlayerVideoReady() { }
        public void PlayerVideoStart() { }
    }
}
