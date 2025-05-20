
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class TopazUtils
    {
        public static bool IsValidTopazLink(VRCUrl url)
        {
            if (!Utilities.IsValid(url)) return false;
            var _url = url.ToString();
            if (string.IsNullOrWhiteSpace(_url) || !IsTopazLink(_url)) return false;
            var _streamKey = _url.Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
            return !string.IsNullOrWhiteSpace(_streamKey);
        }

        public static string InvalidTopazLinkReason(VRCUrl url)
        {
            if (!Utilities.IsValid(url)) return "Invalid VRCUrl";
            var _url = url.ToString();
            if (_url == null) return "Null URL";
            if (string.IsNullOrWhiteSpace(_url)) return "Empty URL";
            if (!IsTopazLink(_url)) return "Not TopazChat URL";
            var _streamKey = _url.Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
            if (string.IsNullOrWhiteSpace(_streamKey))
            {
                return "Empty StreamKey";
            }
            else
            {
                return "Unknown Error";
            }
        }

        /// <summary>
        /// TopazChatのリンクならばTrueを返します。
        /// </summary>
        public static bool IsTopazLink(VRCUrl url)
        {
            var _url = url.ToString();
            return _url.StartsWith("rtspt://topaz.chat/live") || _url.StartsWith("rtsp://topaz.chat/live");
        }

        /// <summary>
        /// TopazChatのリンクならばTrueを返します。
        /// </summary>
        public static bool IsTopazLink(string url)
        {
            return url.StartsWith("rtspt://topaz.chat/live") || url.StartsWith("rtsp://topaz.chat/live");
        }

        public static string MessageLevelColor(MessageLevel level)
        {
            switch (level)
            {
                case MessageLevel.Info:
                    return "#CCCCCC";
                case MessageLevel.Error:
                    return "#CC0000";
                default:
                    return "#CCCCCC";
            }
        }
    }

    public enum Platform
    {
        Windows,
        Android
    }

    public enum MessageLevel
    {
        Info,
        Error
    }

    public enum PlayType
    {
        Play,
        Resume,
        ReSync
    }
    public enum StopType
    {
        Stop,
        ErrorStop,
        Pause
    }

    public enum PlayerStatus
    {
        Play,
        Pause,
        Stop
    }
}
