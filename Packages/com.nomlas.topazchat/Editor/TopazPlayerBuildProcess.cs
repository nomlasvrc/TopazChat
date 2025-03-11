using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nomlas.TopazChat
{
    public class TopazPlayerBuildProcess : IProcessSceneWithReport
    {
        public int callbackOrder => default;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            var controllers = Object
                .FindObjectsByType<PlayerEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => !IsEditorOnly(x.transform));

            var player = Object
                .FindObjectsByType<TopazChatPlayerBase>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .FirstOrDefault(x => !IsEditorOnly(x.transform));

            foreach (var controller in controllers)
            {
                if (controller.player == null)
                {
                    controller.player = player;
                }
            }
        }

        private static bool IsEditorOnly(Transform t)
        {
            for (; t != null; t = t.parent)
                if (t.gameObject.CompareTag("EditorOnly"))
                    return true;

            return false;
        }
    }
}