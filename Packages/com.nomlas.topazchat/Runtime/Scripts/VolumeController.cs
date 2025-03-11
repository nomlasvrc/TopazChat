using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class VolumeController : ControllerBase
    {
        [SerializeField] private Slider volumeSlider;
        public void ChangeVolume()
        {
            player.Volume = volumeSlider.value;
        }
    }
}
