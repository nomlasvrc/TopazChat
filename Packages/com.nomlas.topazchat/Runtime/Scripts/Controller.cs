
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class Controller : PlayerEventListener
    {
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TextMeshProUGUI address;
        [SerializeField] private VRCUrlInputField urlInputField;

        private void Start()
        {
            player.AddEventListener(this);
            if (Utilities.IsValid(urlInputField)) urlInputField.SetUrl(player.defaultStreamURL);
        }

        public void ReSync()
        {
            player.Resync();
        }

        public void ChangeVolume()
        {
            player.volume = volumeSlider.value;
        }

        public void OnEndStreamKeyEdit() //StreamKeyのInputFieldの変更が終わったときに発火
        {
            var _url = urlInputField.GetUrl();
            if (string.IsNullOrWhiteSpace(_url.ToString()))
            {
                urlInputField.SetUrl(player.defaultStreamURL); //streamURLをセット
                Log("Set default URL");
            }
            else
            {
                player.SetUrl(_url); //Globalで変更
            }
        }

        public void GlobalSync()
        {
            player.GlobalSync();
        }

        public override void UpdateURL(VRCUrl url)
        {
            if (!Utilities.IsValid(urlInputField)) return;
            urlInputField.SetUrl(url);
            address.text = url.ToString().Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
            Log("UI Updated");
        }
    }
}