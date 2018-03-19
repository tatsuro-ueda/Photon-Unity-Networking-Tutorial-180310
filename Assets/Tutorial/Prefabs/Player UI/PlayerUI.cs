using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Education.FeelPhysics.PhotonTutorial
{
    /// <summary>
    /// キャラクターの上にプレイヤー名と体力ゲージを表示させる
    /// </summary>
    public class PlayerUI : MonoBehaviour
    {
        #region Public Valuables

        [Tooltip("プレイヤー名を表示する表示するUIテキスト")]
        public Text PlayerNameText;

        [Tooltip("プレイヤーの体力を表示するUIスライダ")]
        public Slider PlayerHealthSlider;

        #endregion

        #region Private Valuables

        /// <summary>
        /// 定期的に体力をルックアップすることになるので、計画的に考える必要があります。
        /// 効率を良くするためにPlayerManagerの参照をキャッシュすると有効です。
        /// </summary>
        private PlayerManager target;

        #endregion

        #region Public Methods

        /// <summary>
        /// target変数にプレイヤーを入れる
        /// </summary>
        /// <param name="target">プレイヤー</param>
        public void SetTarget(PlayerManager target)
        {
            if (this.target == null)
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("PlayerManager targetが<Color-Red>ありません</Color>"));
            }

            // 効率化のために参照をキャッシュする
            this.target = target;
            if (this.PlayerNameText != null)
            {
                this.PlayerNameText.text = this.target.photonView.owner.NickName;
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        private void Start()
        {
            
        }

        /// <summary>
        /// プレイヤーの体力を体力ゲージに反映させる
        /// </summary>
        private void Update()
        {
            if (this.PlayerHealthSlider != null)
            {
                this.PlayerHealthSlider.value = this.target.Health;
            }
        }

        #endregion
    }
}