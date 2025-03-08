
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
        [SerializeField] private VRCUrlInputField urlInputField_Android;

        private void Start()
        {
            player.AddEventListener(this);
            if (Utilities.IsValid(urlInputField)) urlInputField.SetUrl(player.defaultStreamURL);
            if (Utilities.IsValid(urlInputField_Android)) urlInputField_Android.SetUrl(player.defaultStreamURL_Android);
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
                //streamURLをセット
                urlInputField.SetUrl(player.defaultStreamURL);
                Log("Set default URL");
            }
            else
            {
                player.SetUrl(_url, null); //Globalで変更
            }
        }

        public void OnEndStreamKeyEditAndroid()
        {
        }

        public void GlobalSync()
        {
            player.GlobalSync();
        }

        public override void UpdateURL(VRCUrl url, VRCUrl url_Android)
        {
            if (!Utilities.IsValid(urlInputField)) return;
            urlInputField.SetUrl(url);
            if (Utilities.IsValid(urlInputField_Android)) urlInputField_Android.SetUrl(url_Android);
            address.text = url.ToString().Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
            Log("UI Updated");
        }
    }
}