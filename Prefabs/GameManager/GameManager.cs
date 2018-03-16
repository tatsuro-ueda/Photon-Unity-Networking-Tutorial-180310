using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Education.FeelPhysics.PhotonTutorial
{
    public class GameManager : Photon.PunBehaviour
    {
        #region Public Properties

        static public GameManager Instance;

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            Instance = this;
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