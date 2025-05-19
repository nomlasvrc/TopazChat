
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class ControllerBase : PlayerEventListener
    {
        [SerializeField] GameObject playIcon;
        [SerializeField] GameObject pauseIcon;
        [SerializeField] GameObject stopIcon;
        [SerializeField] private TextMeshProUGUI message;
        [SerializeField] private Slider volumeSlider;
        public void ReSync()
        {
            player.Resync();
        }

        public void GlobalSync()
        {
            player.GlobalSync();
        }

        public void ChangeVolume()
        {
            player.Volume = volumeSlider.value;
        }

        public override void UpdateMessage(string msg)
        {
            message.text = msg;
        }

        public override void UpdateStatus(PlayerStatus playerStatus)
        {
            switch (playerStatus)
            {
                case PlayerStatus.Play:
                    playIcon.SetActive(true);
                    pauseIcon.SetActive(false);
                    stopIcon.SetActive(false);
                    break;
                case PlayerStatus.Pause:
                    playIcon.SetActive(false);
                    pauseIcon.SetActive(true);
                    stopIcon.SetActive(false);
                    break;
                case PlayerStatus.Stop:
                    playIcon.SetActive(false);
                    pauseIcon.SetActive(false);
                    stopIcon.SetActive(true);
                    break;
            }
        }
    }
}
