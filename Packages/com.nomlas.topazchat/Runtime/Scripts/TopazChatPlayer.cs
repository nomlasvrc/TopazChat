
using UdonSharp;
using UnityEngine;

namespace Nomlas.TopazChat
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class TopazChatPlayer : URLSync
    {
        private void Start()
        {
            player = this;
        }
    }
}