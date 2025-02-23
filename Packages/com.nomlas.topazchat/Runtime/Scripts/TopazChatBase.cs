
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("com.nomlas.topazchat.Editor")]
namespace Nomlas.TopazChat
{
    public class TopazChatBase : UdonSharpBehaviour
    {
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
    }
}
