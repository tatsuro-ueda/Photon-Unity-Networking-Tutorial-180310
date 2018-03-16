using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace Education.FeelPhysics.PhotonTutorial
{
    /// <summary>
    /// プレイヤー名の入力欄。
    /// ユーザに名前を入力させ、ゲーム内ではプレイヤーの上に表示される。
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameImputField : MonoBehaviour
    {
        #region Private Properties

        // タイプミスを避けるため、PlayerPrefキーを格納する
        static string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {
            string defaultName = "";
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField == null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region Public Methods
        /// <summary>
        /// プレイヤー名を設定し、以後のセッションのためにPlayerPrefsに保存する
        /// </summary>
        /// <param name="userField"></param>
        public void SetPlayerName(InputField userField)
        {
            string name = userField.text;
            // 【重要】
            // 変数が空だったときのために末尾にスペースを付ける
            PhotonNetwork.playerName = name + " ";

            PlayerPrefs.SetString(playerNamePrefKey, name);
            Debug.Log(MyHelper.FileAndMethodNameWithMessage("プレイヤー名が " + name + " に設定されました。"));
        }
        #endregion
    }
}