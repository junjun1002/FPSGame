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
        /// <summary>�v���C���[�̈ړ����x</summary>
        [SerializeField] float m_moveSpeed = 5.0f;
        /// <summary>�v���C���[�̕��s���x</summary>
        [SerializeField] float m_walkSpeed = 2.0f;
        /// <summary>�v���C���[�̍ő呬�x</summary>
        [SerializeField] float m_maxSpeed = 10.0f;

        /// <summary>
        /// �J�����̐��������̉�]�p�x�̐���
        /// </summary>
        [SerializeField] float m_minPitch = -45.0f;
        [SerializeField] float m_maxPitch = 45.0f;

        private float m_currentPitch = 0.0f;

        /// <summary>�v���C���[�̃W�����v��</summary>
        [SerializeField] float m_jumpForce = 5.0f;

        /// <summary>�v���C���[�̃J����</summary>
        [SerializeField] GameObject m_camera;

        [SerializeField] GameObject m_gun;
        [SerializeField] GameObject m_rightHandPos;
        [SerializeField] GameObject m_leftHandPos;
        /// <summary>�v���C���[�̏e��</summary>
        [SerializeField] GameObject m_muzzle;
        /// <summary>�ˌ��̒��e�n�_�̃G�t�F�N�g</summary>
        [SerializeField] ParticleSystem m_hitSpark;

        /// <summary>�L�����N�^�[�̃A�j���[�V����</summary>
        [SerializeField] Animator m_anim;

        /// <summary>�}�E�X�̊��x�𒲐����邽�߂̌W��</summary>
        [SerializeField] float m_sensitivity = 0.1f;

        private PlayerInput m_playerInput;
        private Rigidbody m_rb;
        /// <summary>�ˌ��̃��C�������_���[</summary>
        private LineRenderer m_lineRenderer;

        private Vector2 m_move;
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

                // ���͂�����΁A�v���C���[�̌����Ă����������ɓ��͕����ɓ�����
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
            // �J�����̒������烌�C���΂�
            Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
            RaycastHit hit;

            // ���C�����������ꍇ
            if (Physics.Raycast(ray, out hit))
            {
                // �e�̐�[���烌�C�����������ꏊ�܂ł̃��C����`��
                m_lineRenderer.SetPosition(0, m_muzzle.transform.position);
                m_lineRenderer.SetPosition(1, hit.point);

                m_hitSpark.transform.position = hit.point;
                m_hitSpark.Play();

                // �ˌ��̃G�t�F�N�g���Đ�
                //m_shootEffect.Play();

                // �q�b�g�����I�u�W�F�N�g�ɑ΂��ĉ����������s���i��F�_���[�W��^����j
                // hit.collider.gameObject.GetComponent<Health>()?.TakeDamage(damage);
            }
            else
            {
                // ���C��������Ȃ������ꍇ�A���C���𖳌��ɂ���
                m_lineRenderer.SetPosition(0, Vector3.zero);
                m_lineRenderer.SetPosition(1, Vector3.zero);
            }
        }

        /// <summary>
        /// �v���C���[�̈ړ��𐧌䂷��
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

            // ���݂̃s�b�`�p�x���X�V
            m_currentPitch = Mathf.Clamp(m_currentPitch + pitch, m_minPitch, m_maxPitch);

            // �J�����̃s�b�`�p�x��K�p
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

            // �E���IK��ݒ�
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKPosition(AvatarIKGoal.RightHand, m_rightHandPos.transform.position);
            m_anim.SetIKRotation(AvatarIKGoal.RightHand, m_rightHandPos.transform.rotation);

            // �����IK��ݒ�
            m_anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            m_anim.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHandPos.transform.position);
            m_anim.SetIKRotation(AvatarIKGoal.LeftHand, m_leftHandPos.transform.rotation);

            // �e�̈ʒu�Ɖ�]���E���IK�^�[�Q�b�g�ɒǏ]������
            m_gun.transform.rotation = Quaternion.LookRotation(ikTarget - m_muzzle.transform.position);
        }
    }
}
