using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    [CustomEditor(typeof(TopazChatPlayer))]
    public class TopazChatPlayerEditor : Editor
    {
        private string streamKey = "";

        public override void OnInspectorGUI()
        {
            DrawTopazChatPlayerInspector();
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }

        private void DrawTopazChatPlayerInspector()
        {
            TopazChatPlayer player = (TopazChatPlayer)target;

            var oldStreamKey = streamKey;
            streamKey = EditorGUILayout.TextField("ストリームキー", streamKey);
            EditorGUILayout.HelpBox("このテキストフィールドに入力すると、StreamURLが自動で更新されます。", MessageType.None);

            EditorGUILayout.Space();

            if (player.controller != null)
            {
                /*
                bool oldAndroidMode = player.controller.androidMode;
                player.controller.androidMode = EditorGUILayout.Toggle("Quest/Android向けURL変更機能", player.controller.androidMode);
                EditorGUILayout.HelpBox("VRChatの制限により、Quest/Android向けのURL（rtsp://topaz.chat/live/XXX）を手動で入力する必要があります。この機能がオフになっていてもデフォルトのストリームURLはプラットフォーム別に使用されます。", MessageType.Info);
                if (oldAndroidMode != player.controller.androidMode)
                {
                    EditorUtility.SetDirty(player.controller);
                }
                */

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
                EditorGUILayout.HelpBox("コントローラーが見つかりません。一部機能が無効化されます。", MessageType.Warning);
            }
        }
    }
}