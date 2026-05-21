using TMPro;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class Controller : ControllerBase
    {
        public override string ListenerName => "Controller";
        [Space]
        [SerializeField][Header("ストリームキーの表示欄")] internal TextMeshProUGUI address;
        [SerializeField][Header("URL入力欄")] private VRCUrlInputField urlInputField;
        [SerializeField][Header("URL入力欄(Android)")] private VRCUrlInputField urlInputField_Android;
        [Space]
        [SerializeField][Header("保存されたストリームキーの表示欄")] private TextMeshProUGUI savedStreamKeyText;
        [Space]
        [SerializeField] internal bool androidMode;
        /// <summary>
        /// ストリームキーのPersistenceを行うコンポーネント。
        /// </summary>
        private Save saver;
        /// <summary>
        /// AndroidMode時にURLを一時的に保存するための変数。
        /// </summary>
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
                LogWarning("ストリームキー表示欄が見つかりません");
            }
            if (!Utilities.IsValid(savedStreamKeyText))
            {
                LogWarning("保存されたストリームキーの表示欄が見つかりません");
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
            if (!Utilities.IsValid(saver))
            {
                LogWarning("URL Saverが見つかりません");
                return;
            }
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
            player.GlobalUserStop();
        }

        public void _OnRestoredUrl(Save sender)
        {
            if (!Utilities.IsValid(sender))
            {
                LogError("URL Saverが正しくありません");
                return;
            }
            saver = sender;

            var streamKey = TopazUtils.StreamKey(saver.SavedURL);
            savedStreamKeyText.text = streamKey;
            Log("URL Restored. StreamKey: " + streamKey);
        }

        private void _SaveURLAndUpdateKey(VRCUrl url, VRCUrl url_Android)
        {
            if (!Utilities.IsValid(saver))
            {
                LogWarning("URL Saverが見つからないため、URLを保存できません");
                return;
            }
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
    }
}