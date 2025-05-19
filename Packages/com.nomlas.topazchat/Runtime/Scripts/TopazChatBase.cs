
using UdonSharp;
using UnityEngine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("com.nomlas.topazchat.Editor")]
namespace Nomlas.TopazChat
{
    [HelpURL("https://github.com/nomlasvrc/TopazChat")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TopazChatBase : UdonSharpBehaviour
    {
        const string prefix = "[<color=orange>TopazPlayer-Nmls</color>] ";

        protected void Log(string message)
        {
            Debug.Log(prefix + message);
        }

        protected void LogWarning(string message)
        {
            Debug.LogWarning(prefix + message);
        }

        protected void LogError(string message)
        {
            Debug.LogError(prefix + message);
        }

        /// <summary>
        /// 現在実行中のプラットフォームを返します。
        /// </summary>
        public static Platform GetRunningPlatform()
        {
            return RunningPlatformIsAndroid() ? Platform.Android : Platform.Windows;
        }

        /// <summary>
        /// 現在実行中のプラットフォームがAndroidかどうか返します。
        /// </summary>
        /// <returns>現在実行中のプラットフォームがAndroidかどうか。</returns>
        public static bool RunningPlatformIsAndroid()
        {
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }
    }
}
