
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nomlas.TopazChat
{
    public class ControllerBase : PlayerEventListener
    {
        [SerializeField] private GameObject playIcon;
        [SerializeField] private GameObject pauseIcon;
        [SerializeField] private GameObject stopIcon;
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
