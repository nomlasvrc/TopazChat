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
            var oldStreamKey = streamKey;
            streamKey = EditorGUILayout.TextField("ストリームキー", streamKey);
            EditorGUILayout.HelpBox("このテキストフィールドに入力すると、StreamURLが自動で更新されます。", MessageType.None);

            if (oldStreamKey != streamKey)
            {
                TopazChatPlayer player = (TopazChatPlayer)target;
                player.defaultStreamURL = new VRCUrl("rtspt://topaz.chat/live/" + streamKey);
                player.defaultStreamURL_Android = new VRCUrl("rtsp://topaz.chat/live/" + streamKey);
                if (player.addressText != null) player.addressText.text = streamKey;

                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}