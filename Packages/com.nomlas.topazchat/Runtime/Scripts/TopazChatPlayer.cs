
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;

namespace Nomlas.TopazChat{
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TopazChatPlayer : UdonSharpBehaviour
{
    [SerializeField] VRCAVProVideoPlayer videoPlayer;
    [SerializeField] VRCUrlInputField urlInputField;
    [SerializeField] TextMeshProUGUI address;
    [SerializeField] VRCUrl streamURL;

    //同期関係
    [UdonSynced, FieldChangeCallback(nameof(SyncStreamURL))] VRCUrl _SyncStreamURL; //これの直接操作は禁止
    public VRCUrl SyncStreamURL //これを操作する（これはLocalなのでGlobalで変更する場合はSteUrl()を使う）
    {
        get => _SyncStreamURL;
        set
        {
            _SyncStreamURL = value;
            ApplySyncStreamURL();
        }
    }
    private void ApplySyncStreamURL() //SyncStreamURLが変更されたときに発火（これの直接操作は非推奨）
    {
        Debug.Log("Play URL: " + SyncStreamURL.ToString());
        Stop();
        urlInputField.SetUrl(SyncStreamURL);
        address.text = SyncStreamURL.ToString().Replace("rtspt://topaz.chat/live/", "").Replace("rtsp://topaz.chat/live/", "");
        videoPlayer.PlayURL(SyncStreamURL);
    }


    private void SetUrl(VRCUrl tmpStreamURL) //Globalで変更する場合はこちら
    {
        if (IsTopazLink(tmpStreamURL.ToString()))
        {
            TakeOwner();
            SyncStreamURL = tmpStreamURL;
            RequestSerialization();
        }
    }

    public void GlobalSync() //GlobalSyncボタンが押されたときに発火
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Resync");
        Resync();
    }

    public void Resync() //GlobalSync又はResyncボタンが押されたときに発火
    {
        videoPlayer.PlayURL(SyncStreamURL);
    }

    public void Stop()
    {
        videoPlayer.Stop();
    }

    public void OnEndStreamKeyEdit() //StreamKeyのInputFieldの変更が終わったときに発火
    {
        if (urlInputField.GetUrl().ToString() == "") //空欄なら
        {
            urlInputField.SetUrl(streamURL); //streamURLをセット
        }
        else
        {
            SetUrl(urlInputField.GetUrl()); //Globalで変更
        }
    }

    private bool IsTopazLink(string url) //URLはTopazChatのリンクか？
    {
        return url.StartsWith("rtspt://topaz.chat/live") || url.StartsWith("rtsp://topaz.chat/live");
    }

    private void TakeOwner()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (VRCPlayerApi.GetPlayerCount() == 1) //インスタンス人数がひとりなら
        {
            SetUrl(streamURL); //streamURLで再生
        }
        else if (player == Networking.LocalPlayer)
        {
            RequestSerialization(); //SyncStreamURLをオーナーからもらう
        }
    }
}
}