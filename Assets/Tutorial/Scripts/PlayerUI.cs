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

        [Tooltip("UIのターゲットのプレイヤーとのピクセル間隔")]
        public Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

        #endregion

        #region Private Valuables

        /// <summary>
        /// 定期的に体力をルックアップすることになるので、計画的に考える必要があります。
        /// 効率を良くするためにPlayerManagerの参照をキャッシュすると有効です。
        /// </summary>
        private PlayerManager targetPlayer;

        float characterControllerHeight = 0f;
        Transform targetTransform;

        #endregion

        #region Public Methods

        /// <summary>
        /// UIの target 変数にプレイヤーを入れる
        /// </summary>
        /// <param name="target">UIのターゲットプレイヤー</param>
        public void SetTargetPlayer(PlayerManager target)
        {
            if (target == null)
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("PlayerManager targetが<Color-Red>ありません</Color>"));
            }

            // 効率化のために参照をキャッシュする
            this.targetPlayer = target;

            // 名前を表示する
            if (this.PlayerNameText != null)
            {
                this.PlayerNameText.text = this.targetPlayer.photonView.owner.NickName;
            }

            CharacterController characterController =
                target.GetComponent<CharacterController>();

            // このコンポーネントが存在している限り変わらないデータをプレイヤーから取得する
            characterControllerHeight = characterController.height;

            targetTransform = target.gameObject.transform;
        }

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// 自身（UI）をCanvasオブジェクトの中に入れる
        /// Unity UIシステムの重要な制約の1つは、
        /// 如何なるUI要素もCanvas GameObject内に配置しなければならないことです。
        /// </summary>
        private void Awake()
        {
            this.GetComponent<Transform>().SetParent(
                GameObject.Find("Top Panel").GetComponent<Transform>());
        }

        /// <summary>
        /// プレイヤーの体力を体力ゲージに反映させる
        /// また、プレイヤーが破壊されたときにUIも破壊する
        /// </summary>
        private void Update()
        {
            if (this.PlayerHealthSlider != null)
            {
                this.PlayerHealthSlider.value = this.targetPlayer.Health;
            }

            // Destroy itself if the target is null, 
            // UIのターゲットがなくなったらUIそのものも破壊する
            // これは、ネットワーク越しにプレイヤーのインスタンスを破壊するときの安全策である
            // UIだけ残るとみっともない
            if (this.targetPlayer == null)
            {
                Object.Destroy(this.gameObject);
                return;
            }
        }

        private void LateUpdate()
        {
            // 【必要】
            // 画面上のUIターゲットのオブジェクトを追う
            if (targetTransform != null)
            {
                var targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position =
                    Camera.main.WorldToScreenPoint(targetPosition) + ScreenOffset;
            }
        }

        #endregion
    }
}