using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        [Space]
        [SerializeField][Header("保存されたストリームキーを読み込むボタン")] private Button loadButton;
        [SerializeField][Header("保存されたストリームキーの表示欄")] private TextMeshProUGUI savedStreamKeyText;
        /// <summary>
        /// ストリームキーのPersistenceを行うコンポーネント。
        /// </summary>
        private Save saver;
        private protected override void Start()
        {
            base.Start();
            if (Utilities.IsValid(urlInputField))
            {
                urlInputField.SetUrl(player.DefaultStreamURL);
            }
            else
            {
                LogWarning("URL入力欄が見つかりません");
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
                player.SetUrl(_url); //Globalで変更
                _SaveURLAndUpdateKey(_url);
            }
            else
            {
                //streamURLをセット
                urlInputField.SetUrl(player.DefaultStreamURL);
                Log("Set default URL");
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
            if (TopazUtils.IsValidTopazLink(savedUrl))
            {
                player.SetUrl(savedUrl);
            }
        }

        [UnityEvent]
        public void UIStop()
        {
            Log("Stop by user");
            player.GlobalUserStop();
        }

        public void OnRestoredUrlInternal(Save sender)
        {
            if (!Utilities.IsValid(sender))
            {
                LogError("URL Saverが正しくありません");
                return;
            }
            saver = sender;

            var streamKey = TopazUtils.StreamKey(saver.SavedURL);
            savedStreamKeyText.text = streamKey;
            loadButton.interactable = true;
            Log("URL Restored. StreamKey: " + streamKey);
        }

        private void _SaveURLAndUpdateKey(VRCUrl url)
        {
            if (!Utilities.IsValid(saver))
            {
                LogWarning("URL Saverが見つからないため、URLを保存できません");
                return;
            }
            saver.SaveKeyInternal(url);
            savedStreamKeyText.text = TopazUtils.StreamKey(url);
        }

        public override void UpdateURL(VRCUrl url)
        {
            urlInputField.SetUrl(url);
            address.text = TopazUtils.StreamKey(url);
            Log("UI Updated");
        }
    }
}