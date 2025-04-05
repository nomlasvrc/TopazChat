using UnityEngine;
using UnityEditor;

namespace Nomlas.TopazChat
{
    [CustomEditor(typeof(AutoPause))]
    public class AutoPauseEditor : Editor
    {
        SerializedProperty pauseDistance;
        SerializedProperty resumeDistance;

        // OnEnableでプロパティを取得
        private void OnEnable()
        {
            pauseDistance = serializedObject.FindProperty("pauseDistance");
            resumeDistance = serializedObject.FindProperty("resumeDistance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            float minValue = 0.1f;
            float maxValue = 100f;

            pauseDistance.floatValue = EditorGUILayout.Slider("Pause Distence", Mathf.Max(pauseDistance.floatValue, minValue), minValue, maxValue);

            float resumeMax = pauseDistance.floatValue - 0.1f;
            resumeDistance.floatValue = EditorGUILayout.Slider("Resume Distance", Mathf.Max(resumeDistance.floatValue, minValue), minValue, resumeMax);

            if (resumeDistance.floatValue >= pauseDistance.floatValue)
            {
                resumeDistance.floatValue = pauseDistance.floatValue - 0.1f;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}