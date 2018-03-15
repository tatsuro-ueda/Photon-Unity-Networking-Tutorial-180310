// Launcher.cs

using UnityEngine;

namespace Education.FeelPhysics.PhotonTutorial
{
    public class Launcher : Photon.PunBehaviour, IPunCallbacks
    {
        #region Public Variables

        /// <summary>
        /// PUN ログレベル
        /// </summary>
        public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

        /// <summary>
        /// ルームの最大プレイヤー人数。ルームが満員になると、その部屋に新しいプレイヤーが入ることはできないため、新しいルームが作成されます。
        /// </summary>   
        [Tooltip("ルームの最大プレイヤー人数。ルームが満員になると、その部屋に新しいプレイヤーが入ることはできないため、新しいルームが作成されます。")]
        public byte MaxPlayersPerRoom = 4;

        #endregion

        #region Private Variables

        /// <summary>
        /// バージョン番号。
        /// ユーザーはバージョン別の世界に入る。
        /// このため、破壊的変更をすることができる。
        /// </summary>
        string _gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// 初期化の早い段階でUnityによってGameObject上に呼ばれる
        /// MonoBegaviourのメソッド
        /// </summary>
        private void Awake()
        {
            // 【些細】
            // ログレベルを指定する
            PhotonNetwork.logLevel = Loglevel;

            // 【致命的】
            // ロビーに入って他のプレイヤーのリストを見ることはしない
            PhotonNetwork.autoJoinLobby = false;

            // 【致命的】
            // PhotonNetwork.LoadLevel()をマスタークライアントで実行すれば
            // 他のクライアントはすべて同じルームに入る
            PhotonNetwork.automaticallySyncScene = true;
        }
        // Use this for initialization

        /// <summary>
        /// 初期化の段階でUnityによってGameObject上に呼ばれるMonoBegaviourのメソッド
        /// </summary>
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region Public Methods

        public void Connect()
        {
            // 接続していればルームに入り、
            // そうでなければサーバー接続を初期化する
            if (PhotonNetwork.connected)
            {
                // 【致命的】ランダムなルームに入る必要がある
                // 失敗した場合はOnPhotonRandomJoinFailed()によって通知され、
                // ルームをつくる
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 【致命的】まず最初にPhotonオンラインサーバに接続しなければならない
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #endregion

        #region Photon.PunBefaviour Callbacks

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            MyHelper.DebugLog("");

            // 【致命的】すでに存在するかもしれないルームに入る最初の試み
            // 既に存在すれば良いし、さもなくばOnPhotonRandomJoinFailed()が呼ばれる
            PhotonNetwork.JoinRandomRoom();
        }
        public override void OnDisconnectedFromPhoton()
        {
            base.OnDisconnectedFromPhoton();
            MyHelper.DebugLog("");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            base.OnPhotonRandomJoinFailed(codeAndMsg);
            MyHelper.DebugLog("利用可能なランダムルームがないので、作成します\n" +
                "呼び出し中: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

            // 【致命的】ランダムルームに参加するのに失敗しました。
            // 1つも存在しないか、すべて満員です。心配ありません、新しく1つルームを作成します。
            PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = MaxPlayersPerRoom }, null);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            MyHelper.DebugLog("今、このクライアントはルームの中にいます。");
        }
        #endregion
    }
}