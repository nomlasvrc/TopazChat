using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    [CustomEditor(typeof(TopazChatPlayer))]
    public class TopazChatPlayerEditor : TopazChatPlayerEditorBase
    {
        public override void OnInspectorGUI()
        {
            DrawTopazChatPlayerInspector();
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }

    [CustomEditor(typeof(TopazChatPlayerReverbFilter))]
    public class TopazChatPlayerReverbFilterEditor : TopazChatPlayerEditorBase
    {
        public override void OnInspectorGUI()
        {
            DrawTopazChatPlayerInspector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("ReverbFilterバリアントでは音量調整ができません。", MessageType.Info);
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }

    public class TopazChatPlayerEditorBase : Editor
    {
        private string streamKey = "";

        protected void DrawTopazChatPlayerInspector()
        {
            TopazChatPlayerBase player = (TopazChatPlayerBase)target;

            var oldStreamKey = streamKey;
            streamKey = EditorGUILayout.TextField("ストリームキー", streamKey);
            EditorGUILayout.HelpBox("このテキストフィールドに入力すると、StreamURLが自動で更新されます。", MessageType.None);

            EditorGUILayout.Space();

            if (player.controller != null)
            {
                bool oldAndroidMode = player.controller.androidMode;
                player.controller.androidMode = EditorGUILayout.Toggle("Quest/Android向けURL変更機能", player.controller.androidMode);
                EditorGUILayout.HelpBox("VRChatの制限により、Quest/Android向けのURL（rtsp://topaz.chat/live/XXX）を手動で入力する必要があります。この機能がオフになっていてもデフォルトのストリームURLはプラットフォーム別に使用されます。", MessageType.Info);
                if (oldAndroidMode != player.controller.androidMode)
                {
                    EditorUtility.SetDirty(player.controller);
                }

                if (oldStreamKey != streamKey)
                {
                    player.defaultStreamURL = new VRCUrl("rtspt://topaz.chat/live/" + streamKey);
                    player.defaultStreamURL_Android = new VRCUrl("rtsp://topaz.chat/live/" + streamKey);
                    if (player.controller.address != null) player.controller.address.text = streamKey;

                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(player.controller.address);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("コントローラーが見つかりません。AndroidModeをオンにできません。", MessageType.Warning);
            }
        }
    }
}