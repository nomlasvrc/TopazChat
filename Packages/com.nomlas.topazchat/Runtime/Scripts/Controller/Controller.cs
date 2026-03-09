using TMPro;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Controller : ControllerBase
    {
        [Space]
        [SerializeField] internal TextMeshProUGUI address;
        [SerializeField] private VRCUrlInputField urlInputField;
        [SerializeField] private VRCUrlInputField urlInputField_Android;
        [Space]
        private Save saver;
        [SerializeField] private TextMeshProUGUI savedStreamKeyText;
        [Space]
        [SerializeField] internal bool androidMode;
        private VRCUrl tempURL;

        protected override void Start()
        {
            base.Start();
            if (Utilities.IsValid(urlInputField))
            {
                urlInputField.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Windows));
            }
            else
            {
                LogWarning("URL入力欄が見つかりません");
            }
            if (Utilities.IsValid(urlInputField_Android))
            {
                urlInputField_Android.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Android));
            }
            else
            {
                LogWarning("URL入力欄(Android)が見つかりません");
            }
            if (!Utilities.IsValid(address))
            {
                LogWarning("StreamKey表示欄が見つかりません");
            }
        }

        [UnityEvent]
        public void OnEndStreamKeyEdit() //StreamKeyのInputFieldの変更が終わったときに発火
        {
            var _url = urlInputField.GetUrl();
            if (TopazUtils.IsValidTopazLink(_url))
            {
                if (androidMode)
                {
                    tempURL = _url;
                    urlInputField_Android.ActivateInputField();
                }
                else
                {
                    player.SetUrl(_url, _url); //Globalで変更
                    _SaveURLAndUpdateKey(_url, _url);
                }
            }
            else
            {
                //streamURLをセット
                urlInputField.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Windows));
                urlInputField_Android.SetUrl(player.GetPlatformDefaultStreamURL(Platform.Android));
                Log("Set default URL");
            }
        }

        [UnityEvent]
        public void OnEndStreamKeyEditAndroid()
        {
            var _url = urlInputField_Android.GetUrl();
            if (TopazUtils.IsValidTopazLink(_url))
            {
                player.SetUrl(tempURL, _url);
                _SaveURLAndUpdateKey(tempURL, _url);
            }
        }

        [UnityEvent]
        public void LoadPersistenceURL()
        {
            var savedUrl = saver.SavedURL;
            var savedUrl_Android = saver.SavedURL_Android;
            if (TopazUtils.IsValidTopazLink(savedUrl) && TopazUtils.IsValidTopazLink(savedUrl_Android))
            {
                player.SetUrl(savedUrl, savedUrl_Android);
            }
        }

        [UnityEvent]
        public void UIStop()
        {
            Log("Stop by user");
            player.UserStop();
        }

        public void _OnRestoredUrl(Save sender)
        {
            saver = sender;
            savedStreamKeyText.text = TopazUtils.StreamKey(saver.SavedURL);
            Log("Restored URL");
        }

        private void _SaveURLAndUpdateKey(VRCUrl url, VRCUrl url_Android)
        {
            saver._SaveKey(url, url_Android);
            savedStreamKeyText.text = TopazUtils.StreamKey(url);
        }

        public override void UpdateURL(VRCUrl url, VRCUrl url_Android)
        {
            urlInputField.SetUrl(url);
            urlInputField_Android.SetUrl(url_Android);
            address.text = TopazUtils.StreamKey(url);
            Log("UI Updated");
        }

        public override string GetListenerName()
        {
            return "Controller";
        }
    }
}