
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class ControllerBase : PlayerEventListener
    {
        public void ReSync()
        {
            player.Resync();
        }

        public void GlobalSync()
        {
            player.GlobalSync();
        }
    }
}
