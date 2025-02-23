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
                string prefix = "rtspt://topaz.chat/live/";
                string fullURL = prefix + streamKey;

                TopazChatPlayer player = (TopazChatPlayer)target;
                player.streamURL = new VRCUrl(fullURL);

                EditorUtility.SetDirty(target);
            }
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}