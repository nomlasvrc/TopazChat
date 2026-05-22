using UnityEngine;
using UnityEditor;

namespace Nomlas.TopazChat
{
    [CustomEditor(typeof(AutoPause))]
    public class AutoPauseEditor : Editor
    {
        SerializedProperty pauseDistance;
        SerializedProperty resumeDistance;
        public const float PauseResumeGap = 2f;
        public const float MinDistance = 1f;
        public const float MaxDistance = 100f;

        // OnEnableでプロパティを取得
        private void OnEnable()
        {
            pauseDistance = serializedObject.FindProperty("pauseDistance");
            resumeDistance = serializedObject.FindProperty("resumeDistance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            pauseDistance.floatValue = EditorGUILayout.Slider("Pause Distance", pauseDistance.floatValue, MinDistance + PauseResumeGap, MaxDistance);

            float resumeMax = pauseDistance.floatValue - PauseResumeGap;
            resumeDistance.floatValue = EditorGUILayout.Slider("Resume Distance", resumeDistance.floatValue, MinDistance, resumeMax);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.Space(10);
            EditorGUI.indentLevel++;
            DrawDefaultInspector();
            EditorGUI.indentLevel--;
        }
    }
}