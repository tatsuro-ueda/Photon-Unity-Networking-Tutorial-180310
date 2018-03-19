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

        /// <summary>
        /// ローカルプレイヤーのインスタンス。ローカルプレイヤーがシーンに現れたか知るためにこれを使って下さい。
        /// </summary>
        public static GameObject LocalPlayerInstance;

        [Tooltip("制御するビーム GameObject")]
        public GameObject Beams;

        [Tooltip("プレイヤーの現在の体力")]
        public float Health = 1f;

        [Tooltip("プレイヤーUI")]
        public GameObject PlayerUIPrefab;

        #endregion

        #region Private Variables

        // ユーザーがfireしているときは true
        private bool isFiring;

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
            Object.DontDestroyOnLoad(this.gameObject);

            if (this.Beams == null)
            {
                // [Unity] Debug.Log で出力する文字の色を変えよう - Qiita https://qiita.com/phi/items/d98a177f4e12426d9f4f
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage("Beams プロパティへの参照が<Color=Red>ありません</Color> "));
            }
            else
            {
                this.Beams.SetActive(false);
            }
        }

        /// <summary>
        /// シーンの最初に追尾カメラをセットする
        ///   ＋
        /// UIをインスタンス化します。
        /// 作成したインスタンスにメッセージを送信しています。レシーバが必要です。
        /// つまり、SetTargetが反応するコンポーネントを見つけらなかった場合に通知されます。
        /// </summary>
        private void Start()
        {
            // まず、CameraWorkコンポーネントを取得します。 見つからない場合、エラーが記録されます。
            MyCameraWork cameraWork = this.gameObject.GetComponent<MyCameraWork>();

            if (cameraWork != null)
            {
                // 次に、photonView.isMineがtrueの場合は、このインスタンスを追走する必要があることを意味するので、
                // _cameraWork.OnStartFollowing()を呼び出し、シーン内のそのインスタンスをカメラが効果的に追従させます。
                // 他のすべてのプレイヤーインスタンスは、photonView.isMineをfalseに設定されているため、
                // それぞれの_cameraWorkは何も行いません。
                if (photonView.isMine)
                {
                    cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError(MyHelper.FileAndMethodNameWithMessage(
                    "CameraWorkコンポーネントがプレイヤープレハブに<Color=Red>ありません</Color>。"));
            }

            if (PlayerUIPrefab != null)
            {
                GameObject uiGo = Instantiate(PlayerUIPrefab);

                // ※わからん
                uiGo.SendMessage("SetTargetPlayer", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning(MyHelper.FileAndMethodNameWithMessage(
                    "プレイヤープレハブ上の PlayerUIPrefab への参照が<Color=Red>ありません</Color>"));
            }
        }

        /// <summary>
        /// プレイヤーが自分であれば入力を処理し、ビームの表示・非表示を切り替え、死亡したらルームから抜ける
        /// </summary>
        private void Update()
        {
            if (photonView.isMine)
            {
                this.ProcessInputs();
            }

            // Beams オブジェクトの active 状態を変化させる
            if (this.Beams != null && this.isFiring != this.Beams.GetActive())
            {
                this.Beams.SetActive(this.isFiring);
            }

            if (this.Health <= 0)
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
        /// <param name="other">プレイヤーに衝突してきた何か</param>
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
            this.Health -= 0.1f;
        }

        /// <summary>
        /// 何か（other）衝突物が衝突してきたときに呼ばれる MonoBehaviour のメソッド
        /// ビームがプレイヤーに触れているあいだ、体力に影響する
        /// </summary>
        /// <param name="other">プレイヤーに衝突してきた何か</param>
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
            this.Health -= 0.1f * Time.deltaTime;
        }

        #endregion

        #region Photon CallBacks

        /// <summary>
        /// 射撃をテストする際、ローカルプレイヤーによる射撃しか確認することができません。
        /// 他のインスタンスがいつ射撃するかを確認する必要があります。
        /// ネットワーク上で射撃を同期させるメカニズムが必要です。
        /// これを行うには、IsFiringブール値を手動で同期させます。
        /// これまでは、PhotonTransformView と PhotonAnimatorView を使って変数を内部的に同期させることができました。
        /// Unity Inspectorで公開されていたもののみの調整で済みましたが、
        /// 今回必要なのはこのゲーム特有のものなので、手動で行う必要があります。
        /// このIPunObservable.OnPhotonSerializeViewメソッドでは、stream変数が渡されます。
        /// localPlayer（PhotonView.isMine == true）の場合のみ書きこむことができます。それ以外の場合は読み込みます。
        /// </summary>
        /// <param name="stream">手動で同期させる値を格納する変数</param>
        /// <param name="info">よくわからん</param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // これをネットワーク上で送信し、データを読み書きする場合に呼び出します。
                // stream.SendNext()を使用して、データのストリームにIsFiring値を追加します。
                stream.SendNext(this.isFiring);
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
        private void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!this.isFiring)
                {
                    this.isFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (this.isFiring)
                {
                    this.isFiring = false;
                }
            }
        }

        #endregion
    }
}