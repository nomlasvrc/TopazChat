# TopazChat Player 3.3.1
TopazChatは音声や映像を1秒程度の低遅延でVRChatワールド内に配信する個人運営のサービスです。
このパッケージは、TopazChatで配信される音声や映像を視聴するためのVRChatワールドギミックです。

- PC/Quest両対応です。Questでは３～５秒程度の遅延が発生します。
- VRChat SDK3でのみ動作します。VRChat SDK2では動作しません。

個人利用では無償でご利用いただけます。法人利用はご相談ください。

# 2.0.0との違い
- 視聴者がVRChatの設定で Allow Untrusted URLs にチェックを入れる必要があります。
- VRChat SDK2では動作しません。
- AVPro Videoアセットの購入は必要なくなりました。

# 必要なもの
- Unity 2019.4.31f1
- VRChat SDK3 - Worlds
- UdonSharp v0.20.3 以降

# 使い方
## 設置方法
1. UdonSharp (https://github.com/vrchat-community/UdonSharp/releases) をUnityプロジェクトにインポートする
2. TopazPlayer_3.3.1.unitypackage をUnityプロジェクトにインポートする
3. Projectウィンドウから `Assets/TopazChat Player VRCSDK3/Prefabs/TopazChat Player.prefab` プレハブをシーンに配置
4. ヒエラルキーウィンドウで `TopazChat Player/VideoPlayer` ゲームオブジェクトを選択し、インスペクタウィンドウで開く
5. インスペクタウィンドウのPubic Variables一覧で「StreamURL」という変数を配信URLに変更する  
  デフォルトでは `rtspt://topaz.chat/live/StreamKey` となっています。  
  「Stream Key」の部分を任意の英数字へ自由に変更してください。この文字列が配信時に指定するストリームキーになります。
  Quest向けのワールドでは、rtspt:// ではなく rtsp:// としてください。

さらに以下のようにストリームキーを表示しておくと、制作者以外がTopazChatで配信するときに便利です。

6. ヒエラルキーウィンドウで `TopazChat Player/UI/Address` ゲームオブジェクトを選択し、インスペクタウィンドウで開く
7. Input Fieldコンポーネントの Text を手順４で設定したストリームキーに変更（デフォルトではStream Key）

## VRChat上での使い方
1. VRChatクライアントのSettingsメニューで、Allow Untrusted URLsにチェックを入れてください。視聴者全員が設定する必要があるので、ワールド参加者へのアナウンスが必要です。
2. World音量を上げてください。TopazChatの音声はWorld音声として再生されます。
3. 音声・映像の配信を開始したら、Global Syncボタンを押してください。全員が視聴開始できるようになります。

VRChat上ではResyncボタンとGlobal Syncボタンが表示されます。
- Resyncボタンを押すと、押した人だけ音声・映像を再読み込みします。
- Global Syncボタンを押すと、インスタンス参加者全員が映像・音声を再読み込みします。

インスタンスに参加すると、自動で再生開始されます。インスタンス参加時にAllow Untrusted URLsにチェックが入っていなかった場合は、Resyncボタンで再読み込みしてください。

高負荷などで一時的に音途切れが発生すると、遅延が蓄積することがあります。遅延が気になった場合は、Resyncボタンで自分だけ音声・動画を再読み込みすると直ることがあります。

インスタンス参加時やGlobalSyncやResyncボタンを押した後の30秒間は、再生開始できるまで数秒おきに再接続を繰り返します。

## リバーブ等の音声フィルタを利用する場合
`TopazChat Player.prefab`はリバーブ等の音声フィルタに対応していません。
音声フィルタを使用したい場合は、代わりに`TopazChat Player + Reverb Filter.prefab`をシーンに配置してください。

- 10fps以下などフレームレートが非常に低い場合に、音声が途切れる可能性があります。気になる場合は`Dummy AudioSources`の`Buffer Length`を増やしてみてください。
- 映像も配信する場合は、音声が遅れて聞こえる場合があります。気になる場合は`Dummy AudioSources`の`Buffer Length`を減らしてみてください。数値を減らした場合は、低フレームレートで途切れる可能性があります。
- 音声フィルタは`VideoPlayer`ゲームオブジェクト以下の`Left 3D`, `Right 3D`, `Stereo 2D`に追加してください。例としてリバーブフィルタを追加してあります。ローパスフィルタを追加するなど、ご自由に編集してください。
- デフォルトでは3D立体音響で再生するようになっています。2Dステレオ再生（ヘッドホンに直接ステレオ再生）で再生する場合は、VideoPlayerゲームオブジェクト以下の`Left 3D`, `Right 3D`ゲームオブジェクトを無効にし、`Stereo 2D`を有効にしてください。
- `Dummy AudioSources`ゲームオブジェクト以下のAudioSourceは無効にしないでください。このAudioSourceから出力される音をVideoPlayer以下のAudioSourceにコピーすることで、音声フィルタを動作させる仕組みになっています。

## 配信方法
### 音声のみ配信する場合
TopazChat Streamerを使用すると、ストリームキーを入れてワンクリックで配信できます。
ダウンロード方法や使い方は以下のURLで確認してください。
https://tyounanmoti.booth.pm/items/1756789

### 映像も配信する場合
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

# 既知の不具合
- 高ビットレートの映像では、再読み込みのたびに再生が安定したり不安定になったりすることがあります
- VRChat SDK Control PanelのBuilderタブで以下の警告が表示されます。無視してください。
  "Video Players do not have automatic resync enabled; audio may become desynchronized from video during low performance."

# 今後の更新予定
- Stream URLをVRChatクライアント上で変更できるようにする
- 立体音響処理をしないステレオ再生時の距離減衰サンプル
- UIをコンパクトに、カッコよくする

# よくある質問
- Q: 今までのTopazChat Streamerは使えますか？  
  A: はい、これまでと変わらず配信できます。
- Q: 立体音響ではなくステレオで聴かせることはできますか？
  A: はい、Audio Sourceを一つにして、VRCAV Pro Video SpeakerコンポーネントのModeをStereo Mix、VRC Spatial Audio SourceのEnable Spatializationを切れば直接ステレオで聴かせることができます。ただし、参加した瞬間に爆音で再生される可能性があるので、Pan Levelの減衰カーブを書いたり入室時のみ再生開始するようなんらかのギミックを作成してください。今後サンプルを作る予定です。
- Q: GlobalSyncやResyncが効きにくいことがあります。どうすればよいですか？
  A: YouTubeやTwitchの動画を見たり、カレンダーやポスターをWebから取得したりするような、youtube-dl.exeを使用するギミックがあると、VRChat側のレートリミットで5秒間だけGlobal SyncとResyncが効かなくなります。30秒間の接続リトライで自動的に接続されますので、しばらく待ってみてください。
  レートリミットに関するVRChatのドキュメント：https://docs.vrchat.com/docs/video-players#rate-limiting

# VRCSDK2向け TopazChat Player について
TopazChat Player 2.0.0は、2021年1月12日のAVPro Video メジャーバージョンアップ(1.x -> 2.0)により動作しなくなりました。
VRCSDK2でTopazChatを使う場合、以下の更新作業が必要になります。
1. AVPro Video 2.x系列をアセットストアで購入、またはRenderHeadsの公式サイトで試用版を入手
2. TopazChat Player 2.1.0を配置しなおす

旧バージョンは下記Google Driveフォルダからダウンロードしてください。
https://drive.google.com/drive/folders/1ffXUaiejE7xoE_IqGeIILFaDZYotWBuU?usp=sharing

# 謝辞
サーバーに使用しているソフトウェアのライセンス費用は VoxelKei (@VoxelKei) さんが肩代わりしてくださっています。ありがとうございます。

# サーバー料金について
TopazChatの音声・映像転送には費用がかかっており、作者の よしたか がインスタンス維持やデータ転送にかかる費用を支払っています。
下記URLのPixivFANBOXにて月額のカンパを募集していますので、ご協力いただけると助かります。
https://tyounanmoti.fanbox.cc/

「TopazChatスタンダードスポンサー」以上で支援してくださっている皆様のお名前をSPONSORS.txtに記載しております。

# 連絡先
- Twitter: よしたか (@tyounanmoti)
- mailto: tyounan.moti@gmail.com
- Discordサーバー: https://discord.com/invite/fCMcJ8A

# 変更履歴
## 3.3.1 (2022/10/24)
- `TopazChat Player + Reverb Filter.prefab`を使用時にフレームドロップが連続で発生すると、再生が不安定になり続ける不具合を修正
- README更新

## 3.3.0 (2022/05/08)
- UdonSharpを必要なものに追加
- リバーブ等の音声フィルタを掛けることのできるプレハブ`TopazChat Player + Reverb Filter.prefab`を追加

## 3.2.0 (2021/06/26)
- 30秒間の接続リトライ機能を追加
- 複数ストリームの再生を既存の不具合から削除。TopazPlayerのようにyoutube-dl.exeを使用しない場合はレートリミットが働かないため、意図した動作なのかcannyにて問い合わせ中。

## 3.1.0 (2021/05/29)
- ストリームURLをインスペクタから設定するように変更
- Udon BehaviourのSync MethodをManualに設定

## 3.0.0 (2021/01/22)
- VRChat SDK3向けにターゲット変更

## 2.0.0
- 2019/12/30 最初のリリース
- 2020/07/08 README更新
2Mbpsまで配信可能にしました。既存Playerの置き換えは不要です。
- 2020/07/20 ヨーロッパ版パッケージになっていた不具合を修正
rtspt://eu.topaz.chat/ を参照していたのを、rtspt://topaz.chat/ へ正しく修正。2020/07/08よりエンバグしていた。
- 2021/01/22 README同梱
