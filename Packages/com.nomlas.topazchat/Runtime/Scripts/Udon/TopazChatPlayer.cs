using UnityEngine;

namespace Nomlas.TopazChat
{
    public class TopazChatPlayer : URLSync
    {
        [SerializeField] internal Controller controller;
        
        private void Start()
        {
            player = this;
        }
    }
}