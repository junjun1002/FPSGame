using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// �v���C���[�̈ړ����x
        /// </summary>
        [SerializeField] float m_moveSpeed = 5.0f;
        /// <summary>
        /// �v���C���[�̃J����
        /// </summary>
        [SerializeField] GameObject m_camera;

        /// <summary>
        /// �}�E�X�̊��x�𒲐����邽�߂̌W��
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

            // ���͂�����΁A������̕����ɓ�����
            if (dir != Vector3.zero)
            {
                //this.transform.forward = dir;
                m_rb.AddForce(dir * m_moveSpeed);
                //m_rb.velocity = this.transform.forward * m_moveSpeed;
            }
        }

        /// <summary>
        /// �v���C���[�̈ړ��𐧌䂷��
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
        /// �v���C���[�̎�����ύX����
        /// </summary>
        /// <param name="context"></param>
        public void OnLook(InputAction.CallbackContext context)
        {
            Vector2 look = context.ReadValue<Vector2>();

            // �v���C���[�̉�]���v�Z
            float yaw = look.x * m_sensitivity;
            float pitch = -look.y * m_sensitivity; // �㉺���]

            // �v���C���[�̉�]��K�p
            this.transform.Rotate(Vector3.up, yaw, Space.World);
            // �c�������̉�]�̓J���������ɓK�p
            m_camera.transform.Rotate(Vector3.right, pitch, Space.Self);

            Debug.Log("Look: " + look);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            Debug.Log("Jump");
        }
    }
}
