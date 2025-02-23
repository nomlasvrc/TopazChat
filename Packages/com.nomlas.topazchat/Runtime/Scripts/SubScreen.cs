
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class SubScreen : PlayerEventListener
    {
        [SerializeField] MeshRenderer target;
        void Start()
        {
            target.sharedMaterial = player.screenMaterial;
        }
    }
}