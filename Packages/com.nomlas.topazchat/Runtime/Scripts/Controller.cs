
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
        
        public void Resync()
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
                urlInputField.SetUrl(player.SyncStreamURL); //streamURLをセット
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

        internal override void UpdateURL(VRCUrl url)
        {
            urlInputField.SetUrl(url);
            address.text = url.ToString().Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
        }
    }
}