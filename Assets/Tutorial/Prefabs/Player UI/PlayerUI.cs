using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Education.FeelPhysics.PhotonTutorial
{
    public class PlayerUI : MonoBehaviour
    {

        #region Public Valuables

        [Tooltip("プレイヤー名を表示する表示するUIテキスト")]
        public Text PlayerNameText;

        [Tooltip("プレイヤーの体力を表示するUIスライダ")]
        public Slider PlayerHealthSlider;

        #endregion

        #region Private Valuables

        // 定期的に体力をルックアップすることになるので、計画的に考える必要があります。
        // 効率を良くするためにPlayerManagerの参照をキャッシュすると有効です。
        PlayerManager _target;

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // プレイヤーの体力を反映させる
            if (PlayerHealthSlider != null)
            {
                PlayerHealthSlider.value = _target.Health;
            }
        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerManager target)
        {
            if (target == null)
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("PlayerManager targetが<Color-Red>ありません</Color>"));
            }
            // 効率化のために参照をキャッシュする
            _target = target;
            if (PlayerNameText != null)
            {
                PlayerNameText.text = _target.photonView.owner.NickName;
            }
        }

        #endregion
    }
}