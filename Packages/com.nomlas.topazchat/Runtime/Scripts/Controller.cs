
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.TopazChat
{
    public class Controller : VolumeController
    {
        [SerializeField] internal TextMeshProUGUI address;
        [SerializeField] private VRCUrlInputField urlInputField;
        [SerializeField] private VRCUrlInputField urlInputField_Android;
        [SerializeField] internal bool androidMode;
        private VRCUrl tempURL;

        private void Start()
        {
            player.AddEventListener(this);
            if (Utilities.IsValid(urlInputField)) urlInputField.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Windows));
            if (Utilities.IsValid(urlInputField_Android)) urlInputField_Android.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Android));
        }

        public void OnEndStreamKeyEdit() //StreamKeyのInputFieldの変更が終わったときに発火
        {
            var _url = urlInputField.GetUrl();
            if (string.IsNullOrWhiteSpace(_url.ToString()))
            {
                //streamURLをセット
                urlInputField.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Windows));
                urlInputField_Android.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Android));
                Log("Set default URL");
            }
            else
            {
                if (androidMode)
                {
                    tempURL = _url;
                    urlInputField_Android.Select();
                }
                else
                {
                    player.SetUrl(_url, VRCUrl.Empty); //Globalで変更
                }
            }
        }

        public void OnEndStreamKeyEditAndroid()
        {
            var _url = urlInputField_Android.GetUrl();
            if (!string.IsNullOrWhiteSpace(_url.ToString()))
            {
                player.SetUrl(tempURL, _url);
            }
        }

        public override void UpdateURL(VRCUrl url, VRCUrl url_Android)
        {
            if (Utilities.IsValid(urlInputField)) urlInputField.SetUrl(url);
            if (Utilities.IsValid(urlInputField_Android)) urlInputField_Android.SetUrl(url_Android);
            if (Utilities.IsValid(address)) address.text = url.ToString().Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
            Log("UI Updated");
        }
    }
}