using UnityEngine;

namespace Education.FeelPhysics.PhotonTutorial
{
    /// <summary>
    /// ルームに接続する
    /// </summary>
    public class Launcher : Photon.PunBehaviour, IPunCallbacks
    {
        #region Public Variables

        [Tooltip("PUN ログレベル")]
        public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

        [Tooltip("ルームの最大プレイヤー人数。ルームが満員になると、その部屋に新しいプレイヤーが入ることはできないため、新しいルームが作成されます。")]
        public byte MaxPlayersPerRoom = 4;

        [Tooltip("ユーザに名前を入力させ、接続してプレイさせるためのUIパネル")]
        public GameObject ControlPanel;

        [Tooltip("ユーザに接続が進行中であることを知らせるUIラベル")]
        public GameObject ProgressLabel;

        #endregion

        #region Private Variables

        /// <summary>
        /// バージョン番号。
        /// ユーザーはバージョン別の世界に入る。
        /// このため、破壊的変更をすることができる。
        /// </summary>
        private string gameVersion = "1";

        /// <summary>
        /// 現在のプロセスを追跡し続ける。
        /// 接続は非同期で Photon のいくつかのコールバックに依存しているため、
        /// Photon からのコールバックを受け取ったときの挙動を適切に調節するために、現在のプロセスを追跡し続けなければならない。
        /// これは、特に OnConnectedToMaster() コールバックのために使われる。
        /// </summary>
        private bool isConnecting;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// 初期化の早い段階で Unity によって GameObject 上に呼ばれる MonoBegaviour のメソッド
        /// </summary>
        private void Awake()
        {
            // 【些細】
            // ログレベルを指定する
            PhotonNetwork.logLevel = this.Loglevel;

            // 【必要】
            // ロビーに入って他のプレイヤーのリストを見ることはしない
            PhotonNetwork.autoJoinLobby = false;

            // 【必要】
            // PhotonNetwork.LoadLevel()をマスタークライアントで実行すれば
            // 他のクライアントはすべて同じルームに入る
            // 読み込まれたシーンが接続された全てのプレイヤーに対して同じになるようにする、
            // Photonが提供する非常に便利な機能
            PhotonNetwork.automaticallySyncScene = true;
        }

        /// <summary>
        /// 初期化の段階でUnityによってGameObject上に呼ばれるMonoBegaviourのメソッド
        /// </summary>
        private void Start()
        {
            // PUNのバージョンをログに表示
            Debug.Log("PUN バージョン: " + PhotonNetwork.versionPUN);

            // コントロールパネルは表示し、進行中ラベルは非表示
            this.ControlPanel.SetActive(true);
            this.ProgressLabel.SetActive(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Play Button を押下すると呼ばれる
        /// </summary>
        public void Connect()
        {
            // ルームに参加する意思を追跡し続ける。
            // プレイヤーがゲームを出て帰ってきたときに「接続した」ときのコールバックを受け取ってしまうため、
            // プレイヤーが何をしようとしているのかを知る必要がある。
            this.isConnecting = true;

            // コントロールパネルは非表示、進行中ラベルは表示
            this.ControlPanel.SetActive(false);
            this.ProgressLabel.SetActive(true);

            // 接続していればルームに入り、
            // そうでなければサーバー接続を初期化する
            if (PhotonNetwork.connected)
            {
                // 【必要】ランダムなルームに入る必要がある
                // 失敗した場合はOnPhotonRandomJoinFailed()によって通知され、
                // ルームをつくる
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 【必要】まず最初にPhotonオンラインサーバに接続しなければならない
                PhotonNetwork.ConnectUsingSettings(this.gameVersion);
            }
        }

        #endregion

        #region Photon.PunBefaviour Callbacks

        /// <summary>
        /// サーバに接続したらルームに参加する
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log(MyHelper.FileAndMethodNameWithMessage(""));

            // プレイヤーがルームに参加しようとしていないときは、何もしたくない。
            // isConnecting が false の場合は、プレイヤーがゲームで負けたかゲームを終了するときであり、
            // このレベルがロードされたとき OnConnectedToMaster が呼ばれるが、この場合は、何もしない。
            if (this.isConnecting)
            {
                // 【必要】すでに存在するかもしれないルームに入る最初の試み
                // 既に存在すればよいし、さもなくばOnPhotonRandomJoinFailed()が呼ばれる
                PhotonNetwork.JoinRandomRoom();
            }
        }

        /// <summary>
        /// サーバから切断されたらコントロールパネルを表示する
        /// </summary>
        public override void OnDisconnectedFromPhoton()
        {
            // コントロールパネルは表示し、進行中ラベルは非表示
            this.ControlPanel.SetActive(true);
            this.ProgressLabel.SetActive(false);

            Debug.Log(MyHelper.FileAndMethodNameWithMessage(""));
        }

        /// <summary>
        /// サーバに接続したときにルームがなかったときは、ルームをつくる
        /// </summary>
        /// <param name="codeAndMsg">コードとメッセージ</param>
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log(MyHelper.FileAndMethodNameWithMessage(codeAndMsg.ToString()));
            Debug.Log(MyHelper.FileAndMethodNameWithMessage("利用可能なランダムルームがないので、作成します\n" +
                "呼び出し中: PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 4}, null);"));

            // 【必要】ランダムルームに参加するのに失敗しました。
            // 1つも存在しないか、すべて満員です。心配ありません、新しく1つルームを作成します。
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = this.MaxPlayersPerRoom }, null);
        }

        /// <summary>
        /// ルームに参加したラシーンをロードする
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log(MyHelper.FileAndMethodNameWithMessage("今、このクライアントはルームの中にいます。"));

            // 【必要】自分が一人目のプレイヤーのときのみルームをロードし、
            // それ以外のときは、インスタンスシーンに同期するのは 
            // PhotonNetwork.automaticallySyncScene にまかせる
            if (PhotonNetwork.room.PlayerCount == 1)
            {
                Debug.Log(MyHelper.FileAndMethodNameWithMessage("「Room for 1」をロードします"));

                // 【必要】ルームをロードする
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }
        #endregion
    }
}