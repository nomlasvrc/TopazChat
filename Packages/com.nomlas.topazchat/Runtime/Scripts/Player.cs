
using TMPro;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Player : Control
    {
        [SerializeField] private VRCUrlInputField urlInputField;
        [SerializeField] private TextMeshProUGUI address;
        [SerializeField] private VRCAVProVideoPlayer videoPlayer;
        [SerializeField] internal VRCUrl defaultStreamURL;

        private VRCUrl streamURL { get => player.SyncStreamURL; }

        internal protected void StartStream(VRCUrl url)
        {
            Debug.Log("Play URL: " + url.ToString());
            Stop();
            urlInputField.SetUrl(url);
            address.text = url.ToString().Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
            videoPlayer.PlayURL(url);
        }

        public void GlobalSync() //GlobalSyncボタンが押されたときに発火
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Resync");
            Resync();
        }

        public void Resync() //GlobalSync又はResyncボタンが押されたときに発火
        {
            videoPlayer.PlayURL(streamURL);
        }

        public void Stop()
        {
            videoPlayer.Stop();
        }

        public void OnEndStreamKeyEdit() //StreamKeyのInputFieldの変更が終わったときに発火
        {
            var _url = urlInputField.GetUrl();
            if (string.IsNullOrWhiteSpace(_url.ToString()))
            {
                urlInputField.SetUrl(defaultStreamURL); //streamURLをセット
            }
            else
            {
                player.SetUrl(_url); //Globalで変更
            }
        }
    }
}
