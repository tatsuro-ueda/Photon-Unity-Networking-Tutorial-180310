using UnityEngine;
using UnityEngine.UI;

namespace Education.FeelPhysics.PhotonTutorial
{
    /// <summary>
    /// プレイヤー名の入力欄。
    /// ユーザに名前を入力させ、ゲーム内ではプレイヤーの上に表示される。
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameImputField : MonoBehaviour
    {
        #region Private Variables

        /// <summary>
        /// タイプミスを避けるため、PlayerPrefキーを格納する 
        /// </summary>
        private static string playerNamePrefKey = "PlayerName";

        #endregion

        #region Public Methods
        /// <summary>
        /// プレイヤー名を設定し、以後のセッションのためにPlayerPrefsに保存する
        /// </summary>
        /// <param name="value">入力されたプレイヤー名</param>
        public void SetPlayerName(string value)
        {
            var nameInputted = value;

            // 【重要】
            // 変数が空だったときのために末尾にスペースを付ける
            PhotonNetwork.playerName = nameInputted + " ";

            PlayerPrefs.SetString(playerNamePrefKey, nameInputted);
            Debug.Log(MyHelper.FileAndMethodNameWithMessage("プレイヤー名が " + nameInputted + " に設定されました。"));
        }

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// 過去に使用したプレイヤー名を入力欄に表示する
        /// </summary>
        private void Start()
        {
            string defaultName = "";
            InputField inputField = this.GetComponent<InputField>();
            if (inputField == null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        #endregion
    }
}