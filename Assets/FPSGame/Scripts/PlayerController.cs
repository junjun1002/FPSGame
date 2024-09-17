using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float m_moveSpeed = 5.0f;

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
            Vector3 dir = (Vector3.forward * m_vertical + Vector3.right * m_horizontal).normalized;

            // 入力があれば、そちらの方向に動かす
            if (dir != Vector3.zero)
            {
                //this.transform.forward = dir;
                m_rb.AddForce(dir * m_moveSpeed);
                //m_rb.velocity = this.transform.forward * m_moveSpeed;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 move = context.ReadValue<Vector2>();
            m_horizontal = move.x;
            m_vertical = move.y;
            Debug.Log("Move: " + move);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 look = context.ReadValue<Vector2>();

            // マウスの感度を調整するための係数
            float sensitivity = 0.1f;

            // プレイヤーの回転を計算
            float yaw = look.x * sensitivity;
            float pitch = -look.y * sensitivity; // 上下反転

            // プレイヤーの回転を適用
            this.transform.Rotate(Vector3.up, yaw, Space.World);
            this.transform.Rotate(Vector3.right, pitch, Space.Self);

            Debug.Log("Look: " + look);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            Debug.Log("Jump");
        }
    }
}
