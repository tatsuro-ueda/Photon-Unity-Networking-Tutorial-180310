using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Education.FeelPhysics.PhotonTutorial
{
    public class PlayerAnimatorManager : MonoBehaviour
    {
        #region Public Properties

        // Damp Time は「待ち時間」
        // 希望する値（最大の回転能力）に到達するまでの時間
        // 変えても動きはあまり変化しないような？
        public float DirectionDampTime = .25f;

        #endregion

        #region MonoBehavior CallBacks

        private Animator animator;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("Animatorコンポーネントがありません。"));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!animator)
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
            animator.SetFloat("Speed", h * h + v * v);

            // 【ターン】
            // 左右方向の入力があるとターンする
            // deltaTime はフレーム間の時間。Update() はフレームレートに依存するため、この変数を使う
            animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);

            // 【ジャンプ】
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            // 走っているときだけジャンプできる
            if (stateInfo.IsName("Base Layer.Run"))
            {
                // Fire2 Input（Altキー、右クリック）があれば Jump トリガーを発生させる
                if (Input.GetButtonDown("Fire2")) animator.SetTrigger("Jump");
            }
        }

        #endregion
    }
}