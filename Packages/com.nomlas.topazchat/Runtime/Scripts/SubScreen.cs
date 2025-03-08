
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class SubScreen : PlayerEventListener
    {
        [SerializeField] MeshRenderer target;
        public override void OnListenerReady()
        {
            target.sharedMaterial = player.screenMaterial;
        }
    }
}