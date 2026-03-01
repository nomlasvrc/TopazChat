
using JetBrains.Annotations;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class TopazUtils
    {
        public const string TopazURL = "rtspt://topaz.chat/live";
        public const string TopazURL_Android = "rtsp://topaz.chat/live";
        public const string PebbleURL = "https://pebble.xrift.net";

        /// <summary>
        /// URLが有効であり、かつ、ストリームキーが有効ならばTrueを返します。
        /// </summary>
        [PublicAPI]
        public static bool IsValidTopazLink(VRCUrl url)
        {
            if (!Utilities.IsValid(url)) return false;
            var _url = url.ToString();
            if (string.IsNullOrWhiteSpace(_url) || !IsTopazLink(url)) return false;
            var _streamKey = StreamKey(url);
            return !string.IsNullOrWhiteSpace(_streamKey);
        }

        [PublicAPI]
        public static string InvalidTopazLinkReason(VRCUrl url)
        {
            if (!Utilities.IsValid(url)) return "Invalid VRCUrl";
            var _url = url.ToString();
            if (_url == null) return "Null URL";
            if (string.IsNullOrWhiteSpace(_url)) return "Empty URL";
            if (!IsTopazLink(url)) return "Not TopazChat URL";
            var _streamKey = StreamKey(url);
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
        /// VRCURLからStreamKeyを返します。
        /// </summary>
        [PublicAPI]
        public static string StreamKey(VRCUrl url)
        {
            if (!Utilities.IsValid(url)) return null;
            var _url = url.ToString();
            return _url.Replace($"{TopazURL}/", "").Replace($"{TopazURL_Android}/", "").Replace($"{PebbleURL}/", "");
        }

        /// <summary>
        /// TopazChatのリンクならばTrueを返します。
        /// </summary>
        [PublicAPI]
        public static bool IsTopazLink(VRCUrl url)
        {
            if (!Utilities.IsValid(url)) return false;
            var _url = url.ToString();
            return _url.StartsWith(TopazURL) || _url.StartsWith(TopazURL_Android) || _url.StartsWith(PebbleURL);
        }

        [PublicAPI]
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
