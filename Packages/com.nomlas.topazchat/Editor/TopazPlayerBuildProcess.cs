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
            var listeners = Object
                .FindObjectsByType<PlayerEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => !IsEditorOnly(x.transform));

            var player = Object
                .FindObjectsByType<TopazChatPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .FirstOrDefault(x => !IsEditorOnly(x.transform));

            foreach (var listener in listeners)
            {
                if (listener.player == null)
                {
                    listener.player = player;
                }
            }

            var controllers = Object
                .FindObjectsByType<Controller>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(x => !IsEditorOnly(x.transform));

            var saver = Object
                .FindObjectsByType<Save>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
                .FirstOrDefault(x => !IsEditorOnly(x.transform));
            
            if (saver != null)
            {
                saver.controller = controllers.ToArray();
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