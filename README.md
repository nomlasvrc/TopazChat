# のむらす式 TopazChat Player
[**VPMに登録**](https://nomlasvrc.github.io/nomlas-package-listing/)<br>
[booth](https://nomlas.booth.pm/items/6054043)

### 配信方法
※詳しくは[本家boothページ](https://booth.pm/ja/items/1752066)にてご確認ください。
#### 音声のみ配信する場合
TopazChat Streamerを使用すると、ストリームキーを入れてワンクリックで配信できます。
ダウンロード方法や使い方は以下のURLで確認してください。
https://tyounanmoti.booth.pm/items/1756789

#### 映像も配信する場合
映像の配信は映像2Mbps、音声320kbpsの上限ビットレートで試験運用しています。予告なく停止したり、不安定な視聴になったりする可能性があります。

OBS等の動画配信ソフトを使用して、下記の設定で配信開始してください。
- サーバー: rtmp://topaz.chat/live
- ストリームキー: プレイヤーの中央に表示されている文字列
- ビットレート: 映像2000kbps以下、音声320kbps以下

OBSであれば、下記の設定をすると遅延時間が最短になります。
- 映像
- フレームレート: 60fps
- 出力
- エンコーダ: NVENC
- プリセット: Max Performance
- Profile: High
- Look-ahead: OFF
- 心理視覚チューニング: OFF
- 最大 B フレーム: 0

x264のzerolatencyチューンや、NVENCのLow Latencyプリセットを使用するとVRChat内で映像が描画されないことがあるようです。

### リンク集
TopazChat Playerは[よしたか](https://x.com/TyounanMOTI) 氏によって開発され、TopazChatは同氏によって運営されています。また、このプロジェクトは@nomlasvrcによって非公式で開発されています。

本家TopazChat Player 3.0: https://booth.pm/ja/items/1752066<br>
よしたか氏 PixivFANBOX: https://tyounanmoti.fanbox.cc/<br>
