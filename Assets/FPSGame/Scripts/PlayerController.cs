using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>�v���C���[�̈ړ����x</summary>
        [SerializeField] float m_moveSpeed = 5.0f;
        /// <summary>�v���C���[�̕��s���x</summary>
        [SerializeField] float m_walkSpeed = 2.0f;
        /// <summary>�v���C���[�̍ő呬�x</summary>
        [SerializeField] float m_maxSpeed = 10.0f;

        /// <summary>�v���C���[�̃W�����v��</summary>
        [SerializeField] float m_jumpForce = 5.0f;

        /// <summary>�v���C���[�̃J����</summary>
        [SerializeField] GameObject m_camera;

        /// <summary>�}�E�X�̊��x�𒲐����邽�߂̌W��</summary>
        [SerializeField] float m_sensitivity = 0.1f;

        private PlayerInput m_playerInput;
        private Rigidbody m_rb;

        private float m_horizontal;
        private float m_vertical;

        /// <summary>�v���C���[�������Ă��邩�ǂ���</summary>
        private bool m_isWalking = false;
        /// <summary>�v���C���[���n�ʂɂ��邩�ǂ���</summary>
        private bool m_isGround = true;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

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

            Vector3 dir = (this.transform.forward * m_vertical + this.transform.right * m_horizontal).normalized;

            // ���͂�����΁A�v���C���[�̌����Ă����������ɓ��͕����ɓ�����
            if (dir != Vector3.zero)
            {
                m_rb.AddForce(dir * m_moveSpeed);
            }

            if (m_isWalking)
            {
                m_rb.velocity = m_rb.velocity.normalized * m_walkSpeed;
            }
            else if (m_rb.velocity.magnitude > m_maxSpeed)
            {
                m_rb.velocity = m_rb.velocity.normalized * m_maxSpeed;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Ground")
            {
                Debug.Log("Grounded");
                m_isGround = true;
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
        /// �v���C���[�̕��s�𐧌䂷��
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
            if (!m_isGround) return;
            if (context.started)
            {
                m_rb.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
                m_isGround = false;
            }
            Debug.Log("Jump");
        }
    }
}
