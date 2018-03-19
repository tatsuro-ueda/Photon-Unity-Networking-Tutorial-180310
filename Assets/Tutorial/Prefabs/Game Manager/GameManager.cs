using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Education.FeelPhysics.PhotonTutorial
{
    /// <summary>
    /// ゲームを管理する
    /// プレイヤーの入室（生成）と退室、シーンのロード
    /// </summary>
    public class GameManager : Photon.PunBehaviour
    {
        #region Public Variables        

        /// <summary>
        /// シングルトンである本クラスのインスタンスを取得する
        /// </summary>
        public static GameManager Instance;

        [Tooltip("プレイヤーをあらわすためのプレハブ")]
        public GameObject PlayerPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// シーンがロードされたらプレイヤーがアリーナの外に出てしまっていないか確認する
        /// </summary>
        private void Awake()
        {
            SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
        }

        /// <summary>
        /// シーンが変わったときに必要ならプレイヤーをインスタンス化する
        /// </summary>
        private void Start()
        {
            Instance = this;

            if (this.PlayerPrefab == null)
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage(
                    "プレハブへの参照が<Color=Red>ありません</Color>。" +
                    "GameObject 'Game Manager'でセットアップして下さい"));
            }
            else
            {
                // プレイヤーのDontDestroyOnLoadフラグを立てたため、
                // GameManagerスクリプト内で必要な場合にのみインスタンス化するためのチェックを実装することができます。
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.Log(MyHelper.FileAndMethodNameWithMessage(
                        SceneManager.GetActiveScene().name + " シーンから、ローカルプレイヤーをインスタンス化します"));

                    // ルームに入ったので、ローカルプレイヤーのためにキャラクターを出現させる。
                    // PhotonNetwork.Instantiate を使うことで同期することができる。
                    // Quaternion.identity: このクォータニオンは「回転していない」のと同じで、
                    // オブジェクトは完全にワールドか、親の軸にそろっています。
                    PhotonNetwork.Instantiate(
                        this.PlayerPrefab.name, new Vector3(0f, 1.0f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.Log(MyHelper.FileAndMethodNameWithMessage(
                        SceneManager.GetActiveScene().name + " シーンのロードを無視します"));
                }
            }
        }

        /// <summary>
        /// シーンが終了したらイベントを削除する
        /// </summary>
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= this.SceneManager_sceneLoaded;
        }

        /// <summary>
        /// シーンがロードされたときに呼ばれる
        /// プレイヤーがアリーナの外側にいるかチェックする。外側にいる場合は、安全なアリーナの中心付近に出現させる
        /// </summary>
        /// <param name="arg0">シーン</param>
        /// <param name="arg1">シーンをロードするモード</param>
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            // 現在のプレイヤーの位置を下方向にレイキャストして、何かに衝突するかを確認します。 
            // 何にも当たらない場合はアリーナの地面より上にはいないことを意味するので、
            // 初めてルームに入った時のように中心に再配置する必要があります。
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 1.0f, 0f);
            }
        }

        #endregion

        #region Photon CallBacks

        /// <summary>
        /// ローカルプレイヤーが退室するときに呼ばれる。Launcherシーンをロードしなければならない。
        /// チュートリアルでは「public void void OnLeftRoom()」とタイポしてます。気をつけて下さい。
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Launcher");
        }

        /// <summary>
        /// プレイヤーが参加してきたとき
        /// 自分がマスタークライアントならば、対応する人数用のアリーナをロードする
        /// </summary>
        /// <param name="newPlayer">参加してきたプレイヤー</param>
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            // 自分が接続した場合はこのメッセージは見えない
            Debug.Log(MyHelper.FileAndMethodNameWithMessage(newPlayer.NickName));

            if (PhotonNetwork.isMasterClient)
            {
                // OnPhotonPlayerDisconnected の前に呼ばれる
                Debug.Log(MyHelper.FileAndMethodNameWithMessage("PhotonNetwork.isMasterClient " + PhotonNetwork.isMasterClient));
                this.LoadArena();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// プレイヤーの体力が0になり死亡するとPlayerManagerから呼ばれる
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// アリーナをロードする
        /// マスターの場合のみ呼び出すことができる
        /// </summary>
        private void LoadArena()
        {
            // マスターかチェックする
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("アリーナをロードしようとしていますが、我々はマスタークライアントではありません"));
            }

            Debug.Log(MyHelper.FileAndMethodNameWithMessage("ロードしているレベル：" + PhotonNetwork.room.PlayerCount));

            // 対応するシーンをロードする
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
        }

        #endregion
    }
}