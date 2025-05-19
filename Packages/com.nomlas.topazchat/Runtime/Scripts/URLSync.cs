
using UdonSharp;
using VRC.SDKBase;

namespace Nomlas.TopazChat
{
    public class URLSync : ControllerReceiver
    {
        // ----------------------------------------

        // --------- UdonSync ----------
        [UdonSynced] private VRCUrl _SyncStreamURL;
        [UdonSynced] private VRCUrl _SyncStreamURL_Android;

        // ----------- Get用 ------------
        private VRCUrl SyncStreamURL => _SyncStreamURL;
        private VRCUrl SyncStreamURL_Android => _SyncStreamURL_Android;

        // ----------- Set用 ------------

        internal void SetUrl(VRCUrl tmpStreamURL, VRCUrl tmpStreamURL_Android)
        {
            if (TopazUtils.IsTopazLink(tmpStreamURL) && TopazUtils.IsTopazLink(tmpStreamURL_Android))
            {
                TakeOwner();
                _SyncStreamURL = tmpStreamURL;
                _SyncStreamURL_Android = tmpStreamURL_Android;
                RequestSerialization();
                StartStream(tmpStreamURL, tmpStreamURL_Android);
            }
            else
            {
                LogWarning("TopazChat以外のURLは再生できません。");
                return;
            }
        }

        // ----------------------------------------

        /// <summary>
        /// 現在実行中のプラットフォームに応じたStreamURLを返します。
        /// </summary>
        protected override VRCUrl GetPlatformSyncStreamURL()
        {
            if (GetRunningPlatform() == Platform.Android)
            {
                return SyncStreamURL_Android;
            }
            else
            {
                return SyncStreamURL;
            }
        }

        private void CheckReceivedURLAndStartStream()
        {
            if (Utilities.IsValid(SyncStreamURL) && Utilities.IsValid(SyncStreamURL_Android))
            {
                StartStream(SyncStreamURL, SyncStreamURL_Android);
            }
            else
            {
                LogError("UdonSyncに失敗しました。再生できません。");
                ShowMessage("UdonSync failed. Unable to play.", MessageLevel.Error);
                SafeStop();
            }
        }

        private void PlayerJoinSync(VRCPlayerApi joinedPlayer)
        {
            if (VRCPlayerApi.GetPlayerCount() <= 1) //インスタンス人数がひとりなら
            {
                Log("Welcome! Play with defalut URL...");
                SetDefaultURL();
            }
            else if (joinedPlayer.isLocal) //インスタンス人数が二人以上で、あなたがJoinした人なら
            {
                Log("Welcome! checking if received URLs can be played...");
                CheckReceivedURLAndStartStream();
            }
        }

        private void SetDefaultURL()
        {
            SetUrl(GetPlatformDefaultStreamURL(Platform.Windows), GetPlatformDefaultStreamURL(Platform.Android));
        }

        private void TakeOwner()
        {
            var local = Networking.LocalPlayer;
            if (!Networking.IsOwner(local, this.gameObject))
            {
                Networking.SetOwner(local, this.gameObject);
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            PlayerJoinSync(player);
        }

        public override void OnDeserialization()
        {
            CheckReceivedURLAndStartStream();
        }
    }
}