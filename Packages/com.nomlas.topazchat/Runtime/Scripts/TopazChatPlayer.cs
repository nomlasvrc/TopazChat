
using UdonSharp;
using UnityEngine;

namespace Nomlas.TopazChat
{
    public class TopazChatPlayer : URLSync
    {
        private void Start()
        {
            player = this;
        }
    }
}