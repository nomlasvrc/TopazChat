
using VRC.SDK3.Components.Video;

namespace Nomlas.TopazChat
{
    public class VideoEventListener : Player
    {
        internal void PlayerVideoEnd() { }
        internal void PlayerVideoError(VideoError videoError) { PVideoError(videoError); }
        internal void PlayerVideoLoop() { }
        internal void PlayerVideoPause() { }
        internal void PlayerVideoPlay() { }
        internal void PlayerVideoReady() { }
        internal void PlayerVideoStart() { }
    }
}
