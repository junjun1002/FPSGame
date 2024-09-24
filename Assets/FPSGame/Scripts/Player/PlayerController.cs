using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    /// <summary>
    /// プレイヤーの操作を制御するクラス
    /// </summary>
    public class PlayerController : EventReceiver<PlayerController>
    {
        /// <summary>
        /// カメラの垂直方向の回転角度の下限
        /// </summary>
        [SerializeField] float m_minPitch = -45.0f;
        /// <summary>
        /// カメラの垂直方向の回転角度の上限
        /// </summary>
        [SerializeField] float m_maxPitch = 45.0f;

        /// <summary>現在のピッチ角度</summary>
        private float m_currentPitch = 0.0f;

        /// <summary>プレイヤーのカメラ</summary>
        [SerializeField] GameObject m_camera;

        /// <summary>プレイヤーの銃</summary>
        [SerializeField] GameObject m_gun;
        /// <summary>プレイヤーの右手の位置</summary>
        [SerializeField] GameObject m_rightHandPos;
        /// <summary>プレイヤーの左手の位置</summary>
        [SerializeField] GameObject m_leftHandPos;

        /// <summary>プレイヤーの銃口</summary>
        [SerializeField] GameObject m_muzzle;
        /// <summary>射撃時のエフェクト/summary>
        [SerializeField] ParticleSystem m_muzzleFlash;
        /// <summary>射撃の着弾地点のエフェクト</summary>
        [SerializeField] ParticleSystem m_hitSpark;

        /// <summary>プレイヤーのダメージエフェクト</summary>
        [SerializeField] GameObject m_damageEffect;

        /// <summary>キャラクターのアニメーション</summary>
        [SerializeField] Animator m_anim;

        /// <summary>マウスの感度を調整するための係数</summary>
        [SerializeField] float m_sensitivity = 0.1f;

        /// <summary>プレイヤーの入力</summary>
        private PlayerInput m_playerInput;
        /// <summary>プレイヤーのRigidbody</summary>
        private Rigidbody m_rb;
        /// <summary>プレイヤーのステータス</summary>
        private PlayerStatus m_status;
        /// <summary>射撃のラインレンダラー</summary>
        private LineRenderer m_lineRenderer;

        /// <summary>水平方向の入力</summary>
        private float m_horizontal;
        /// <summary>垂直方向の入力</summary>
        private float m_vertical;

        /// <summary>ラインを消すコルーチン</summary>
        private Coroutine m_clearLine;
        /// <summary>ダメージエフェクトを再生するコルーチン</summary>
        private Coroutine m_playDamageEffect;

        /// <summary>プレイヤーが歩いているかどうか</summary>
        private bool m_isWalking = false;
        /// <summary>プレイヤーが地面にいるかどうか</summary>
        private bool m_isGround = true;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            m_playerInput = this.GetComponentInParent<PlayerInput>();

            if (m_playerInput == null)
            {
                Debug.LogError("PlayerInput component not found.");
            }

            m_rb = GetComponent<Rigidbody>();

            if (m_rb == null)
            {
                Debug.LogError("Rigidbody component not found.");
            }

            m_lineRenderer = this.GetComponentInParent<LineRenderer>();

            if (m_lineRenderer == null)
            {
                Debug.LogError("LineRenderer component not found.");
            }

            m_status = this.GetComponentInParent<PlayerStatus>();

            if (m_status == null)
            {
                Debug.LogError("PlayerStatus component not found.");
            }

            m_status.OnPlayerDeadAction += OnPlayerDie;
        }

        private void Update()
        {
            // カーソルの表示を切り替える
            if (Cursor.visible)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            if (m_isGround)
            {
                Move();
            }
        }

        /// <summary>
        /// プレイヤーの当たり判定を制御する
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (!m_isGround)
            {
                if (collision.gameObject.tag == "Ground")
                {
                    m_isGround = true;
                }
            }
        }

        /// <summary>
        /// プレイヤーが敵の攻撃を受けたときの処理
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "EnemyAttack")
            {
                if (other.transform.root.gameObject.TryGetComponent<EnemyBase>(out var enemy))
                {
                    m_status.TakeDamege(enemy.GetEnemyData().GetPower());
                    m_playDamageEffect = StartCoroutine(PlayDamageEffect());
                }
            }
        }

        /// <summary>
        /// プレイヤーの移動を制御する
        /// </summary>
        private void Move()
        {
            Vector3 dir = (this.transform.forward * m_vertical + this.transform.right * m_horizontal).normalized;

            // 入力があれば、プレイヤーの向いている向きを基準に入力方向に動かす
            if (dir != Vector3.zero)
            {
                if (m_isWalking)
                {
                    m_rb.velocity = dir * m_status.GetPlayerStatusData().GetWalkSpeed();
                }
                else if (m_rb.velocity.magnitude > m_status.GetPlayerStatusData().GetMaxSpeed())
                {
                    m_rb.velocity = m_rb.velocity.normalized * m_status.GetPlayerStatusData().GetMaxSpeed();
                }
                else
                {
                    m_rb.velocity = dir * m_status.GetPlayerStatusData().GetMoveSpeed();
                }
            }
            else
            {
                m_rb.velocity = Vector3.zero;
            }

            m_anim.SetFloat("Speed", m_rb.velocity.magnitude);
        }

        /// <summary>
        /// 弾を発射する
        /// </summary>
        public void Fire()
        {
            if (m_status.CurrentBulletCount <= 0)
            {
                m_anim.SetBool("isReload", true);
                return;
            }

            m_status.CurrentBulletCount--;

            // カメラの中央からレイを飛ばす
            Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
            RaycastHit hit;

            // 射撃時のエフェクトを再生
            m_muzzleFlash.transform.position = m_muzzle.transform.position;
            m_muzzleFlash.Play();

            // レイが当たった場合
            if (Physics.Raycast(ray, out hit))
            {
                // 銃の先端からレイが当たった場所までのラインを描画
                m_lineRenderer.SetPosition(0, m_muzzle.transform.position);
                m_lineRenderer.SetPosition(1, hit.point);

                // 着弾地点のエフェクトを再生
                m_hitSpark.transform.position = hit.point;
                m_hitSpark.Play();

                // ラインを消すコルーチンを開始
                m_clearLine = StartCoroutine(ClearLineAfterSeconds(0.05f)); // 0.05秒後にラインを消す

                // ヒットしたオブジェクトがIEventCollisionを実装していればイベントを発生させる
                if (hit.transform.root.gameObject.TryGetComponent<IEventCollision>(out var eventCollision))
                {
                    eventCollision.CollisionEvent(m_eventSystemInGame);
                    // ヒットしたオブジェクトが敵であればダメージを与える
                    if (hit.transform.root.gameObject.TryGetComponent<EnemyBase>(out var enemy))
                    {
                        enemy.TakeDamage(m_status.GetPlayerStatusData().GetPower(), hit.collider.gameObject.name);
                    }
                }
            }
            else
            {
                // レイが当たらなかった場合、100ユニット先までのラインを描画
                Vector3 endPosition = ray.origin + ray.direction * 100;
                m_lineRenderer.SetPosition(0, m_muzzle.transform.position);
                m_lineRenderer.SetPosition(1, endPosition);

                // ラインを消すコルーチンを開始
                m_clearLine = StartCoroutine(ClearLineAfterSeconds(0.05f)); // 0.05秒後にラインを消す
            }
        }

        /// <summary>
        /// 弾をリロードする
        /// </summary>
        public void Reload()
        {
            m_status.CurrentBulletCount = m_status.GetPlayerStatusData().GetMaxBulletCount();
            m_anim.SetBool("isReload", false);
        }

        /// <summary>
        /// 一定時間後にラインを消す
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private IEnumerator ClearLineAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            // ラインを無効にする
            m_lineRenderer.SetPosition(0, Vector3.zero);
            m_lineRenderer.SetPosition(1, Vector3.zero);

            StopCoroutine(m_clearLine);
        }

        /// <summary>
        /// ダメージエフェクトを再生する
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayDamageEffect()
        {
            m_damageEffect.SetActive(true);

            yield return new WaitForSeconds(0.1f);

            m_damageEffect.SetActive(false);
        }

        /// <summary>
        /// プレイヤーが死亡したときの処理
        /// </summary>
        private void OnPlayerDie()
        {
            m_anim.SetBool("isDie", true);
            m_playerInput.enabled = false;
        }

        /// <summary>
        /// プレイヤーの移動を制御する
        /// </summary>
        /// <param name="context"></param>
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 move = context.ReadValue<Vector2>();
            m_horizontal = move.x;
            m_vertical = move.y;
            m_anim.SetFloat("VelocityX", m_horizontal);
            m_anim.SetFloat("VelocityY", m_vertical);
        }

        /// <summary>
        /// プレイヤーの歩行を制御する
        /// </summary>
        /// <param name="context"></param>
        public void OnWalk(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                m_isWalking = true;
            }
            else if (context.canceled)
            {
                m_isWalking = false;
            }
        }

        /// <summary>
        /// プレイヤーの視線を変更する
        /// </summary>
        /// <param name="context"></param>
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 look = context.ReadValue<Vector2>();

            // プレイヤーの回転を計算
            float yaw = look.x * m_sensitivity;
            float pitch = -look.y * m_sensitivity; // 上下反転

            // プレイヤーの回転を適用
            this.transform.Rotate(Vector3.up, yaw, Space.World);

            // 現在のピッチ角度を更新
            m_currentPitch = Mathf.Clamp(m_currentPitch + pitch, m_minPitch, m_maxPitch);

            // カメラのピッチ角度を適用
            m_camera.transform.localEulerAngles = new Vector3(m_currentPitch, 0.0f, 0.0f);
        }

        /// <summary>
        /// プレイヤーのジャンプを制御する
        /// </summary>
        /// <param name="context"></param>
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!m_isGround) return;
            if (context.started)
            {
                m_rb.AddForce(Vector3.up * m_status.GetPlayerStatusData().GetJumpForce(), ForceMode.Impulse);
                m_isGround = false;
                m_anim.SetTrigger("Jump");
            }
        }

        /// <summary>
        /// プレイヤーの射撃を制御する
        /// </summary>
        /// <param name="context"></param>
        public void OnFire(InputAction.CallbackContext context)
        {
            if (m_status.CurrentBulletCount <= 0)
            {
                m_anim.SetBool("isReload", true);
                m_anim.SetBool("isFire", false);
                return;
            }

            if (context.started)
            {
                m_anim.SetBool("isFire", true);
            }
            else if (context.canceled)
            {
                m_anim.SetBool("isFire", false);
            }
        }

        /// <summary>
        /// プレイヤーのリロードを制御する
        /// </summary>
        /// <param name="context"></param>
        public void OnReload(InputAction.CallbackContext context)
        {
            if (m_status.CurrentBulletCount == m_status.GetPlayerStatusData().GetMaxBulletCount()) return;

            if (context.started)
            {
                m_anim.SetBool("isReload", true);
            }
        }

        /// <summary>
        /// アニメーションのIKを制御する
        /// </summary>
        void OnAnimatorIK()
        {
            m_anim.SetLookAtWeight(1, 1, 1, 1, 1);
            m_anim.SetLookAtPosition(Camera.main.transform.position + Camera.main.transform.forward * 100);

            Vector3 ikTarget = Camera.main.transform.position + Camera.main.transform.forward * 100;

            AnimatorStateInfo stateInfo = m_anim.GetCurrentAnimatorStateInfo(0);

            // 死亡中はIKを無効にする
            if (stateInfo.IsName("Die"))
            {
                m_gun.transform.position = m_anim.GetBoneTransform(HumanBodyBones.RightHand).position;
                m_gun.transform.rotation = Quaternion.LookRotation(-m_anim.GetBoneTransform(HumanBodyBones.RightHand).right);
                m_camera.transform.position = m_anim.GetBoneTransform(HumanBodyBones.Head).position;
                m_camera.transform.rotation = Quaternion.LookRotation(-m_anim.GetBoneTransform(HumanBodyBones.Head).up);
                return;
            }
            else if(stateInfo.IsName("Jump"))
            {
                m_gun.transform.position = m_anim.GetBoneTransform(HumanBodyBones.RightHand).position;
                // 銃の回転をカメラの中央に合わせる
                m_gun.transform.rotation = Quaternion.LookRotation(ikTarget - m_muzzle.transform.position);
            }

            // 右手のIKを設定
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKPosition(AvatarIKGoal.RightHand, m_rightHandPos.transform.position);
            m_anim.SetIKRotation(AvatarIKGoal.RightHand, m_rightHandPos.transform.rotation);

            // 銃の回転をカメラの中央に合わせる
            m_gun.transform.rotation = Quaternion.LookRotation(ikTarget - m_muzzle.transform.position);

            // リロード中は左手のIKを無効にする
            if (stateInfo.IsName("Reload")) return;

            // 左手のIKを設定
            m_anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            m_anim.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHandPos.transform.position);
            m_anim.SetIKRotation(AvatarIKGoal.LeftHand, m_leftHandPos.transform.rotation);
        }

        protected override void OnEnable()
        {

        }

        protected override void OnDisable()
        {

        }
    }
}
