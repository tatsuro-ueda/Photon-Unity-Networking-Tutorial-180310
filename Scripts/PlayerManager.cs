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
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {

        #region Public Variables

        [Tooltip("制御するビーム GameObject")]
        public GameObject Beams;

        [Tooltip("プレイヤーの現在の体力")]
        public float Health = 1f;

        [Tooltip("ローカルプレイヤーのインスタンス。ローカルプレイヤーがシーンに現れたか知るためにこれを使って下さい")]
        static public GameObject LocalPlayerInstance; 

        #endregion

        #region Private Variables

        // ユーザーがfireしているときは true
        bool isFiring;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// 初期化の早い段階で Unity によって GameObject 上に呼ばれる MonoBegaviour のメソッド
        /// </summary>
        private void Awake()
        {
            // 【重要】
            // レベルが同期されるときにインスタンス化されることを避けるために、ローカルプレイヤーのインスタンスを追跡し続ける
            if (photonView.isMine)
            {
                LocalPlayerInstance = this.gameObject;
            }
            // 【必要】
            // ロードする際に破壊しないというフラグを立てて、インスタンスがレベルが同期しても維持されるようにする
            // これにより、レベルがロードされたときにシームレスに移行する
            DontDestroyOnLoad(this.gameObject);

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
            // まず、CameraWorkコンポーネントを取得します。 見つからない場合、エラーが記録されます。
            MyCameraWork _cameraWork = this.gameObject.GetComponent<MyCameraWork>();

            if (_cameraWork != null)
            {
                // 次に、photonView.isMineがtrueの場合は、このインスタンスを追走する必要があることを意味するので、
                // _cameraWork.OnStartFollowing()を呼び出し、シーン内のそのインスタンスをカメラが効果的に追従させます。
                // 他のすべてのプレイヤーインスタンスは、photonView.isMineをfalseに設定されているため、
                // それぞれの_cameraWorkは何も行いません。
                if (photonView.isMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage(
                    "CameraWorkコンポーネントがプレイヤープレハブに<Color=Red>ありません</Color>。"));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.isMine)
            {
                ProcessInputs();
            }

            // Beams オブジェクトの active 状態を変化させる
            if (Beams!=null && isFiring != Beams.GetActive())
            {
                Beams.SetActive(isFiring);
            }

            if(Health <= 0)
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
            Health -= 0.1f;
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
            Health -= 0.1f * Time.deltaTime;
        }

        #endregion

        #region Photon CallBacks

        /// <summary>
        /// このIPunObservable.OnPhotonSerializeViewメソッドでは、stream変数が渡されます。
        /// localPlayer（PhotonView.isMine == true）の場合のみ書きこむことができます。それ以外の場合は読み込みます。
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // これをネットワーク上で送信し、データを読み書きする場合に呼び出します。
                // stream.SendNext()を使用して、データのストリームにIsFiring値を追加します。
                stream.SendNext(isFiring);
            }
            else
            {
                // ネットワーク上のプレイヤーはデータを受け取る
                // 読み込みが必要な場合は、stream.ReceiveNext()を使用します。
                this.isFiring = (bool)stream.ReceiveNext();
            }
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