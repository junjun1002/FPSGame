using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーの移動速度
        /// </summary>
        [SerializeField] float m_moveSpeed = 5.0f;
        /// <summary>
        /// プレイヤーのカメラ
        /// </summary>
        [SerializeField] GameObject m_camera;

        /// <summary>
        /// マウスの感度を調整するための係数
        /// </summary>
        [SerializeField] float m_sensitivity = 0.1f;

        private PlayerInput m_playerInput;
        private Rigidbody m_rb;

        private float m_horizontal;
        private float m_vertical;

        private void Awake()
        {
            m_playerInput = GetComponent<PlayerInput>();

            if (m_playerInput == null)
            {
                Debug.LogError("PlayerInput component not found.");
            }

            m_rb = GetComponent<Rigidbody>();

            if (m_rb == null)
            {
                Debug.LogError("Rigidbody component not found.");
            }
        }

        private void Update()
        {
            Vector3 dir = (this.transform.forward * m_vertical + this.transform.right * m_horizontal).normalized;

            // 入力があれば、そちらの方向に動かす
            if (dir != Vector3.zero)
            {
                //this.transform.forward = dir;
                m_rb.AddForce(dir * m_moveSpeed);
                //m_rb.velocity = this.transform.forward * m_moveSpeed;
            }
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
            Debug.Log("Move: " + move);
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
            // 縦軸方向の回転はカメラだけに適用
            m_camera.transform.Rotate(Vector3.right, pitch, Space.Self);

            Debug.Log("Look: " + look);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            Debug.Log("Jump");
        }
    }
}
