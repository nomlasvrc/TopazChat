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

            // スライダーの最小値(0より大きい値)と最大値の設定
            float minValue = 0.1f;
            float maxValue = 100f;

            // pauseDistenceのスライダー
            // pauseDistenceはminValueからmaxValueまで調整可能
            pauseDistance.floatValue = EditorGUILayout.Slider("Pause Distence", Mathf.Max(pauseDistance.floatValue, minValue), minValue, maxValue);

            // resumeDistanceのスライダー
            // resumeDistanceはminValueから(pauseDistence-小さな余裕値)まで調整可能とする
            float resumeMax = pauseDistance.floatValue - 0.1f;
            resumeDistance.floatValue = EditorGUILayout.Slider("Resume Distance", Mathf.Max(resumeDistance.floatValue, minValue), minValue, resumeMax);

            // 万が一、resumeDistanceがpauseDistence以上になってしまった場合は補正
            if (resumeDistance.floatValue >= pauseDistance.floatValue)
            {
                resumeDistance.floatValue = pauseDistance.floatValue - 0.1f;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}