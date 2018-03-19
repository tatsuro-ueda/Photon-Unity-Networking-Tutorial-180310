using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Education.FeelPhysics.PhotonTutorial
{
    /// <summary>
    /// キー入力によってAnimatorの状態と変数が変わるのを管理する
    /// </summary>
    public class PlayerAnimatorManager : Photon.MonoBehaviour
    {
        #region Public Variables

        /// <summary>
        /// Damp Time は「待ち時間」
        /// 希望する値（最大の回転能力）に到達するまでの時間
        /// ※変えても動きはあまり変化しないような？
        /// </summary>
        public float DirectionDampTime = .25f;

        #endregion

        #region MonoBehavior CallBacks

        /// <summary>
        /// Animatorコンポーネントを格納して使い回す
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Animatorコンポーネントを animator 変数に格納する
        /// </summary>
        private void Start()
        {
            this.animator = this.GetComponent<Animator>();
            if (!this.animator)
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("Animatorコンポーネントがありません。"));
            }
        }

        /// <summary>
        /// 速さ、方向、ジャンプなどをキー入力に応じて変化させる
        /// </summary>
        private void Update()
        {
            // インスタンスが クライアントアプリケーションによって制御されている場合、
            // このインスタンスはこのコンピュータのこのアプリケーション内でプレイしているユーザを表し、
            // PhotonView.isMine は true になります。
            // PhotonNetwork.connected == true を強制するのは、
            // 開発中に接続されていない状態でこのプレハブをテストできるようにするためです。
            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }

            if (!this.animator)
            {
                return;
            }

            float h = Input.GetAxis("Horizontal"); 　// 左右 方向の入力
            float v = Input.GetAxis("Vertical");   　// 前後 方向の入力

            if (v < 0)
            {
                v = 0;  // 後退しない
            }

            // 【前進】
            // 右か左のいずれかのみを入力した際、ターンをしながら加速できるようにする
            this.animator.SetFloat("Speed", (h * h) + (v * v));

            // 【ターン】
            // 左右方向の入力があるとターンする
            // deltaTime はフレーム間の時間。Update() はフレームレートに依存するため、この変数を使う
            this.animator.SetFloat("Direction", h, this.DirectionDampTime, Time.deltaTime);

            // 【ジャンプ】
            AnimatorStateInfo stateInfo = this.animator.GetCurrentAnimatorStateInfo(0);

            // 走っているときだけジャンプできる
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // Fire2 Input（Altキー、右クリック）があれば Jump トリガーを発生させる
                if (Input.GetButtonDown("Fire2"))
                {
                    this.animator.SetTrigger("Jump");
                }
            }
        }

        #endregion
    }
}