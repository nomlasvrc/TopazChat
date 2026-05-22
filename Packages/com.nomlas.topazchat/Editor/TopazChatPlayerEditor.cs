using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    [CustomEditor(typeof(TopazChatPlayer))]
    public class TopazChatPlayerEditor : Editor
    {
        private string streamKey = "";
        public static VRCUrl StopURL => new VRCUrl("stop");

        public override void OnInspectorGUI()
        {
            DrawTopazChatPlayerInspector();
            EditorGUILayout.Space();
            DrawSaverInspector();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("インスペクターの値", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            DrawDefaultInspector();
            EditorGUI.indentLevel--;
        }

        private void DrawTopazChatPlayerInspector()
        {
            TopazChatPlayer player = (TopazChatPlayer)target;

            if (player.controller == null)
            {
                EditorGUILayout.HelpBox("コントローラーが見つかりません。一部機能が無効化されます。", MessageType.Warning);
                return;
            }

            var oldStreamKey = streamKey;
            streamKey = EditorGUILayout.TextField("ストリームキー", streamKey);
            EditorGUILayout.HelpBox("このテキストフィールドに入力すると、StreamURLが自動で更新されます。\n空欄にすると、インスタンス作成時は停止します。", MessageType.None);
            if (oldStreamKey != streamKey)
            {
                Undo.RecordObject(player, "Update Topaz Stream Key");
                if (player.controller.address != null)
                {
                    Undo.RecordObject(player.controller.address, "Update Topaz Stream Key");
                }

                if (string.IsNullOrEmpty(streamKey))
                {
                    player.defaultStreamURL = StopURL;
                    player.defaultStreamURL_Android = StopURL;
                    if (player.controller.address != null) player.controller.address.text = "-";
                }
                else
                {
                    player.defaultStreamURL = new VRCUrl($"{TopazUtils.TopazURL}/{streamKey}");
                    player.defaultStreamURL_Android = new VRCUrl($"{TopazUtils.TopazURL_Android}/{streamKey}");
                    if (player.controller.address != null) player.controller.address.text = streamKey;
                }

                EditorUtility.SetDirty(target);
                if (player.controller.address != null)
                {
                    EditorUtility.SetDirty(player.controller.address);
                }
            }

            /*
            bool oldAndroidMode = player.controller.androidMode;
            player.controller.androidMode = EditorGUILayout.Toggle("Quest/Android向けURL変更機能", player.controller.androidMode);
            EditorGUILayout.HelpBox("VRChatの制限により、Quest/Android向けのURL（rtsp://topaz.chat/live/XXX）を手動で入力する必要があります。この機能がオフになっていてもデフォルトのストリームURLはプラットフォーム別に使用されます。", MessageType.Info);
            if (oldAndroidMode != player.controller.androidMode)
            {
                EditorUtility.SetDirty(player.controller);
            }
            */
        }

        private const string StreamKeySaverPrefabPath = "Packages/com.nomlas.topazchat/Runtime/Prefabs/StreamKey Saver.prefab";
        private void DrawSaverInspector()
        {
            var saverCount = SaveEditor.GetSaveComponentCount();
            if (saverCount == 0)
            {
                EditorGUILayout.HelpBox("ストリームキーを保存するコンポーネントが見つかりません。ストリームキーは保存されません。", MessageType.Warning);
                if (GUILayout.Button("追加する"))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(StreamKeySaverPrefabPath);
                    if (prefab != null)
                    {
                        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        Undo.RegisterCreatedObjectUndo(instance, "Create Stream Key Saver");
                    }
                    else
                    {
                        Debug.LogError($"Failed to load prefab at path: {StreamKeySaverPrefabPath}");
                    }
                }
                return;
            }
            else if (saverCount > 1)
            {
                EditorGUILayout.HelpBox("ストリームキーを保存するコンポーネントが複数見つかりました。誤動作の原因になる可能性があるため、削除してください。", MessageType.Error);
                if (GUILayout.Button("確認する"))
                {
                    var savers = FindObjectsOfType<Save>();
                    if (savers.Length > 0)
                    {
                        Selection.objects = savers.Select(s => s.gameObject).ToArray();
                    }
                }
                return;
            }
        }
    }
}