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
        /// <summary>プレイヤーの移動速度</summary>
        [SerializeField] float m_moveSpeed = 5.0f;
        /// <summary>プレイヤーの歩行速度</summary>
        [SerializeField] float m_walkSpeed = 2.0f;
        /// <summary>プレイヤーの最大速度</summary>
        [SerializeField] float m_maxSpeed = 10.0f;

        /// <summary>
        /// カメラの垂直方向の回転角度の制限
        /// </summary>
        [SerializeField] float m_minPitch = -45.0f;
        [SerializeField] float m_maxPitch = 45.0f;

        private float m_currentPitch = 0.0f;

        /// <summary>プレイヤーのジャンプ力</summary>
        [SerializeField] float m_jumpForce = 5.0f;

        /// <summary>プレイヤーのカメラ</summary>
        [SerializeField] GameObject m_camera;

        [SerializeField] GameObject m_gun;
        [SerializeField] GameObject m_rightHandPos;
        [SerializeField] GameObject m_leftHandPos;
        /// <summary>プレイヤーの銃口</summary>
        [SerializeField] GameObject m_muzzle;
        /// <summary>射撃の着弾地点のエフェクト</summary>
        [SerializeField] ParticleSystem m_hitSpark;

        /// <summary>キャラクターのアニメーション</summary>
        [SerializeField] Animator m_anim;

        /// <summary>マウスの感度を調整するための係数</summary>
        [SerializeField] float m_sensitivity = 0.1f;

        private PlayerInput m_playerInput;
        private Rigidbody m_rb;
        /// <summary>射撃のラインレンダラー</summary>
        private LineRenderer m_lineRenderer;

        private Vector2 m_move;
        private float m_horizontal;
        private float m_vertical;

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

            m_rb = this.GetComponentInParent<Rigidbody>();

            if (m_rb == null)
            {
                Debug.LogError("Rigidbody component not found.");
            }

            m_lineRenderer = this.GetComponentInParent<LineRenderer>();

            if (m_lineRenderer == null)
            {
                Debug.LogError("LineRenderer component not found.");
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
                Vector3 dir = (this.transform.forward * m_vertical + this.transform.right * m_horizontal).normalized;

                // 入力があれば、プレイヤーの向いている向きを基準に入力方向に動かす
                if (dir != Vector3.zero)
                {
                    m_rb.velocity = dir * m_moveSpeed;
                }
                else
                {
                    m_rb.velocity = Vector3.zero;
                    m_anim.SetFloat("Speed", m_rb.velocity.magnitude);
                }

                if (m_isWalking)
                {
                    m_rb.velocity = m_rb.velocity.normalized * m_walkSpeed;
                }
                else if (m_rb.velocity.magnitude > m_maxSpeed)
                {
                    m_rb.velocity = m_rb.velocity.normalized * m_maxSpeed;
                }

                m_anim.SetFloat("Speed", m_rb.velocity.magnitude);
            }
        }

        //private void Move()
        //{
        //    // set target speed based on move speed, sprint speed and if sprint is pressed
        //    float targetSpeed = m_isWalking ? m_walkSpeed : m_moveSpeed;

        //    // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        //    // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        //    // if there is no input, set the target speed to 0
        //    if (m_move == Vector2.zero) targetSpeed = 0.0f;

        //    // a reference to the players current horizontal velocity
        //    float currentHorizontalSpeed = new Vector3(m_characterController.velocity.x, 0.0f, m_characterController.velocity.z).magnitude;

        //    float speedOffset = 0.1f;
        //    float inputMagnitude = m_move.magnitude;
        //    float speed = 0.0f;

        //    // accelerate or decelerate to target speed
        //    if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        //    {
        //        // creates curved result rather than a linear one giving a more organic speed change
        //        // note T in Lerp is clamped, so we don't need to clamp our speed
        //        speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * 10);

        //        // round speed to 3 decimal places
        //        speed = Mathf.Round(speed * 1000f) / 1000f;
        //    }
        //    else
        //    {
        //        speed = targetSpeed;
        //    }

        //    // normalise input direction
        //    Vector3 inputDirection = new Vector3(m_move.x, 0.0f, m_move.y).normalized;

        //    // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        //    // if there is a move input rotate player when the player is moving
        //    if (m_move != Vector2.zero)
        //    {
        //        // move
        //        inputDirection = transform.right * m_move.x + transform.forward * m_move.y;
        //    }

        //    // move the player
        //    m_characterController.Move(inputDirection.normalized * (speed * Time.deltaTime));
        //}

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Ground")
            {
                Debug.Log("Grounded");
                m_isGround = true;
            }
        }

        public void Fire()
        {
            // カメラの中央からレイを飛ばす
            Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
            RaycastHit hit;

            // レイが当たった場合
            if (Physics.Raycast(ray, out hit))
            {
                // 銃の先端からレイが当たった場所までのラインを描画
                m_lineRenderer.SetPosition(0, m_muzzle.transform.position);
                m_lineRenderer.SetPosition(1, hit.point);

                m_hitSpark.transform.position = hit.point;
                m_hitSpark.Play();

                // 射撃のエフェクトを再生
                //m_shootEffect.Play();

                // ヒットしたオブジェクトに対して何か処理を行う（例：ダメージを与える）
                // hit.collider.gameObject.GetComponent<Health>()?.TakeDamage(damage);
            }
            else
            {
                // レイが当たらなかった場合、ラインを無効にする
                m_lineRenderer.SetPosition(0, Vector3.zero);
                m_lineRenderer.SetPosition(1, Vector3.zero);
            }
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

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!m_isGround) return;
            if (context.started)
            {
                Debug.Log("Jump");
                m_rb.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
                m_isGround = false;
            }
        }

        public void OnCrouching(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                m_anim.SetBool("isCrouching", true);
            }
            else if (context.canceled)
            {
                m_anim.SetBool("isCrouching", false);
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                m_anim.SetBool("isFire", true);
            }
            else if (context.canceled)
            {
                m_anim.SetBool("isFire", false);
            }
        }

        public void OnAiming(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                m_anim.SetBool("isAiming", true);
            }
            else if (context.canceled)
            {
                m_anim.SetBool("isAiming", false);
            }
        }

        void OnAnimatorIK()
        {
            m_anim.SetLookAtWeight(1, 1, 1, 1, 1);
            Debug.Log(Camera.main.transform.forward);
            Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward * 100, Color.red);

            m_anim.SetLookAtPosition(Camera.main.transform.position + Camera.main.transform.forward * 100);

            Vector3 ikTarget = Camera.main.transform.position + Camera.main.transform.forward * 100;

            // 右手のIKを設定
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKPosition(AvatarIKGoal.RightHand, m_rightHandPos.transform.position);
            m_anim.SetIKRotation(AvatarIKGoal.RightHand, m_rightHandPos.transform.rotation);

            // 左手のIKを設定
            m_anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            m_anim.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHandPos.transform.position);
            m_anim.SetIKRotation(AvatarIKGoal.LeftHand, m_leftHandPos.transform.rotation);

            // 銃の位置と回転を右手のIKターゲットに追従させる
            m_gun.transform.rotation = Quaternion.LookRotation(ikTarget - m_muzzle.transform.position);
        }
    }
}
