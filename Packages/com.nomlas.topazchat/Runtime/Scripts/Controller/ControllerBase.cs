
using JetBrains.Annotations;
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
        [Space]
        [SerializeField] private TextMeshProUGUI message;
        [Space]
        [SerializeField] private Slider volumeSlider;
        [Space]
        private float previousVolume;

        [UnityEvent]
        public void ReSync()
        {
            player.Resync();
        }

        [UnityEvent]
        public void GlobalSync()
        {
            player.GlobalSync();
        }

        [UnityEvent]
        public void ChangeVolume()
        {
            player.Volume = volumeSlider.value;
        }

        [UnityEvent]
        public void ToggleMute()
        {
            if (player.Volume > 0)
            { // 音声出力時
                previousVolume = player.Volume;
                player.Volume = 0;
                volumeSlider.value = 0;
            }
            else
            { // ミュート時
                player.Volume = previousVolume;
                volumeSlider.value = previousVolume;
            }
        }

        [PublicAPI]
        public override void UpdateMessage(string msg)
        {
            message.text = msg;
        }

        [PublicAPI]
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

        public override void OnChangeVolume(float volume)
        {
            volumeSlider.SetValueWithoutNotify(volume);
        }
    }
}
