using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Education.FeelPhysics.PhotonTutorial
{
    /// <summary>
    /// プレイヤー管理
    /// fire Input とビームを扱う
    /// プレイヤーの体力を管理する
    /// </summary>
    public class PlayerManager : Photon.MonoBehaviour
    {

        #region Public Properties

        [Tooltip("制御するビーム GameObject")]
        public GameObject Beams;

        [Tooltip("プレイヤーの現在の体力")]
        public float health = 1f;

        #endregion

        #region Private Properties

        // ユーザーがfireしているときは true
        bool isFiring;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// 初期化の早い段階で Unity によって GameObject 上に呼ばれる MonoBegaviour のメソッド
        /// </summary>
        private void Awake()
        {
            if (Beams == null)
            {
                // [Unity] Debug.Log で出力する文字の色を変えよう - Qiita https://qiita.com/phi/items/d98a177f4e12426d9f4f
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("Beams プロパティへの参照が<Color=Red>ありません</Color> "));
            }
            else
            {
                Beams.SetActive(false);
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ProcessInputs();

            // Beams オブジェクトの active 状態を変化させる
            if (Beams!=null && isFiring != Beams.GetActive())
            {
                Beams.SetActive(isFiring);
            }

            if(health <= 0)
            {
                GameManager.Instance.LeaveRoom();
            }
        }

        /// <summary>
        /// 何か（other）衝突物が衝突してきたときに呼ばれる MonoBehaviour のメソッド
        /// 衝突物がビームのとき、プレイヤーの体力に影響する
        /// 注：ジャンプとビーム発射を同時にしたときは、プレイヤー自身とプレイヤーのビームは交差する
        /// 衝突物を遠くへ移動させるか、ビームがそのプレイヤーのものなのかチェックすることで、
        /// この現象を避けることができる
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // 衝突物の所有者が自分でなければ何もしない
            if (!photonView.isMine)
            {
                return;
            }

            // 「Baem」タグが付いていないものをすべて排除する。Beam だけ扱う。
            if (!other.CompareTag("Beam"))
            {
                return;
            }

            // 自分の体力が減る
            health -= 0.1f;
        }

        /// <summary>
        /// 何か（other）衝突物が衝突してきたときに呼ばれる MonoBehaviour のメソッド
        /// ビームがプレイヤーに触れているあいだ、体力に影響する
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
        {
            // 衝突物の所有者が自分でなければ何もしない
            if (!photonView.isMine)
            {
                return;
            }

            // 「Baem」タグが付いていないものをすべて排除する。Beam だけ扱う。
            if (!other.CompareTag("Beam"))
            {
                return;
            }

            // ビームがずっと自分に当たっているあいだ、ゆっくりと体力を減らす
            // 死んでしまわないためにはプレイヤーは移動しなければならない
            // deltaTime はフレーム間時間
            health -= 0.1f * Time.deltaTime;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 入力を処理する。ユーザが fire を押しているかをあらわすフラグを管理する
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isFiring)
                {
                    isFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (isFiring)
                {
                    isFiring = false;
                }
            }
        }

        #endregion
    }
}