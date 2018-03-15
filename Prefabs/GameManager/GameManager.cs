using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Education.FeelPhysics.PhotonTutorial
{
    public class GameManager : MonoBehaviour
    {

        #region Photon Messages

        /// <summary>
        /// ローカルプレイヤーが退室するときに呼ばれる。Launcherシーンをロードしなければならない。
        /// </summary>
        public void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}