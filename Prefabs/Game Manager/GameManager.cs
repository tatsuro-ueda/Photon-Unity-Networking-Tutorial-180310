using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Education.FeelPhysics.PhotonTutorial
{
    public class GameManager : Photon.PunBehaviour
    {
        #region Public Variables

        static public GameManager Instance;

        [Tooltip("プレイヤーをあらわすためのプレハブ")]
        public GameObject playerPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            Instance = this;

            if (playerPrefab == null)
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
                        SceneManager.GetActiveScene().name + "シーンから、ローカルプレイヤーをインスタンス化します"));

                    // ルームに入ったので、ローカルプレイヤーのためにキャラクターを出現させる。
                    // PhotonNetwork.Instantiate を使うことで同期することができる。
                    // Quaternion.identity: このクォータニオンは「回転していない」のと同じで、
                    // オブジェクトは完全にワールドか、親の軸にそろっています。
                    PhotonNetwork.Instantiate(
                        playerPrefab.name, new Vector3(0f, 1.0f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.Log(MyHelper.FileAndMethodNameWithMessage(
                        SceneManager.GetActiveScene().name + "シーンのロードを無視します"));
                }
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            // アリーナの外側にいるかチェックする。外側にいる場合は、安全なアリーナの中心付近に出現させる
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
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// プレイヤーが参加してきたとき
        /// 自分がマスタークライアントならば、対応する人数用のアリーナをロードする
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            base.OnPhotonPlayerConnected(newPlayer);

            // 自分が接続した場合はこのメッセージは見えない
            Debug.Log(MyHelper.FileAndMethodNameWithMessage(newPlayer.NickName));

            if (PhotonNetwork.isMasterClient)
            {
                // OnPhotonPlayerDisconnected の前に呼ばれる
                Debug.Log(MyHelper.FileAndMethodNameWithMessage("PhotonNetwork.isMasterClient " + PhotonNetwork.isMasterClient));
                LoadArena();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// アリーナをロードする
        /// マスターの場合のみ呼び出すことができる
        /// </summary>
        void LoadArena()
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

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
    }
}