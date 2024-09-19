using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// カメラの垂直方向の回転角度の制限
        /// </summary>
        [SerializeField] float m_minPitch = -45.0f;
        [SerializeField] float m_maxPitch = 45.0f;

        private float m_currentPitch = 0.0f;

        /// <summary>プレイヤーのカメラ</summary>
        [SerializeField] GameObject m_camera;

        [SerializeField] GameObject m_gun;
        [SerializeField] GameObject m_rightHandPos;
        [SerializeField] GameObject m_leftHandPos;
        /// <summary>プレイヤーの銃口</summary>
        [SerializeField] GameObject m_muzzle;
        /// <summary>射撃時のエフェクト/summary>
        [SerializeField] ParticleSystem m_muzzleFlash;
        /// <summary>射撃の着弾地点のエフェクト</summary>
        [SerializeField] ParticleSystem m_hitSpark;

        /// <summary>キャラクターのアニメーション</summary>
        [SerializeField] Animator m_anim;

        /// <summary>マウスの感度を調整するための係数</summary>
        [SerializeField] float m_sensitivity = 0.1f;

        private PlayerInput m_playerInput;
        private Rigidbody m_rb;
        private PlayerStatus m_status;
        /// <summary>射撃のラインレンダラー</summary>
        private LineRenderer m_lineRenderer;

        private Vector2 m_move;
        private float m_horizontal;
        private float m_vertical;

        private Coroutine m_clearLine;

        /// <summary>プレイヤーが歩いているかどうか</summary>
        private bool m_isWalking = false;
        /// <summary>プレイヤーが地面にいるかどうか</summary>
        private bool m_isGround = true;

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
        }

        private void Update()
        {
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
            if (collision.gameObject.tag == "Ground")
            {
                Debug.Log("Grounded");
                m_isGround = true;
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
                m_rb.velocity = dir * m_status.GetPlayerStatusData().GetMoveSpeed();
            }
            else
            {
                m_rb.velocity = Vector3.zero;
            }

            if (m_isWalking)
            {
                m_rb.velocity = m_rb.velocity.normalized * m_status.GetPlayerStatusData().GetWalkSpeed();
            }
            else if (m_rb.velocity.magnitude > m_status.GetPlayerStatusData().GetMaxSpeed())
            {
                m_rb.velocity = m_rb.velocity.normalized * m_status.GetPlayerStatusData().GetMaxSpeed();
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

                // ヒットしたオブジェクトに対して何か処理を行う（例：ダメージを与える）
                // hit.collider.gameObject.GetComponent<Health>()?.TakeDamage(damage);
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
        /// プレイヤーの移動を制御する
        /// </summary>
        /// <param name="context"></param>
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 move = context.ReadValue<Vector2>();
            m_move = move;
            m_horizontal = move.x;
            m_vertical = move.y;
            Debug.Log("Move: " + move);
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
                Debug.Log("Walk started");
            }
            else if (context.canceled)
            {
                m_isWalking = false;
                Debug.Log("Walk canceled");
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

            Debug.Log("Look: " + look);
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
                Debug.Log("Jump");
                m_rb.AddForce(Vector3.up * m_status.GetPlayerStatusData().GetJumpForce(), ForceMode.Impulse);
                m_isGround = false;
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
    }
}
