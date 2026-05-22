
using UnityEditor;

namespace Nomlas.TopazChat
{
    [CustomEditor(typeof(Save))]
    public class SaveEditor : Editor
    {
        private int saveComponentCount;
        private void OnEnable()
        {
            saveComponentCount = GetSaveComponentCount();
        }

        public static int GetSaveComponentCount() => FindObjectsOfType<Save>().Length;

        public override void OnInspectorGUI()
        {
            if (saveComponentCount > 1)
            {
                EditorGUILayout.HelpBox("ストリームキーを保存するコンポーネントがシーン上に複数存在します！\n誤動作の原因になる可能性があるため、削除してください！", MessageType.Error);
            }
            else if (saveComponentCount == 1)
            {
                EditorGUILayout.HelpBox("正しく設置されています。", MessageType.None);
            }
            else
            {
                EditorGUILayout.HelpBox("ストリームキーを保存するコンポーネントがシーン上に存在しません！\nストリームキーは保存されません。", MessageType.Warning);
            }

            //DrawDefaultInspector();
        }
    }
}