
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("com.nomlas.topazchat.Editor")]
namespace Nomlas.TopazChat
{
    [HelpURL("https://github.com/nomlasvrc/TopazChat")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TopazChatBase : UdonSharpBehaviour
    {
        private const string prefix = "[<color=orange>TopazPlayer-Nmls</color>] ";

        private protected void Log(string message)
        {
            Debug.Log(prefix + message);
        }

        private protected void LogWarning(string message)
        {
            Debug.LogWarning(prefix + message);
        }

        private protected void LogError(string message)
        {
            Debug.LogError(prefix + message);
        }

        /// <summary>
        /// 現在実行中のプラットフォームを返します。
        /// </summary>
        [PublicAPI] public const Platform RunningPlatform = IsAndroid ? Platform.Android : Platform.Windows;

        /// <summary>
        /// 現在実行中のプラットフォームがAndroidかどうか返します。
        /// </summary>
        private const bool IsAndroid =
#if UNITY_ANDROID
        true;
#else
        false;
#endif

    }
}
