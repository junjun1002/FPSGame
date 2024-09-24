using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS
{
    /// <summary>
    /// �v���C���[�̑���𐧌䂷��N���X
    /// </summary>
    public class PlayerController : EventReceiver<PlayerController>
    {
        /// <summary>
        /// �J�����̐��������̉�]�p�x�̉���
        /// </summary>
        [SerializeField] float m_minPitch = -45.0f;
        /// <summary>
        /// �J�����̐��������̉�]�p�x�̏��
        /// </summary>
        [SerializeField] float m_maxPitch = 45.0f;

        /// <summary>���݂̃s�b�`�p�x</summary>
        private float m_currentPitch = 0.0f;

        /// <summary>�v���C���[�̃J����</summary>
        [SerializeField] GameObject m_camera;

        /// <summary>�v���C���[�̏e</summary>
        [SerializeField] GameObject m_gun;
        /// <summary>�v���C���[�̉E��̈ʒu</summary>
        [SerializeField] GameObject m_rightHandPos;
        /// <summary>�v���C���[�̍���̈ʒu</summary>
        [SerializeField] GameObject m_leftHandPos;

        /// <summary>�v���C���[�̏e��</summary>
        [SerializeField] GameObject m_muzzle;
        /// <summary>�ˌ����̃G�t�F�N�g/summary>
        [SerializeField] ParticleSystem m_muzzleFlash;
        /// <summary>�ˌ��̒��e�n�_�̃G�t�F�N�g</summary>
        [SerializeField] ParticleSystem m_hitSpark;

        /// <summary>�v���C���[�̃_���[�W�G�t�F�N�g</summary>
        [SerializeField] GameObject m_damageEffect;

        /// <summary>�L�����N�^�[�̃A�j���[�V����</summary>
        [SerializeField] Animator m_anim;

        /// <summary>�}�E�X�̊��x�𒲐����邽�߂̌W��</summary>
        [SerializeField] float m_sensitivity = 0.1f;

        /// <summary>�v���C���[�̓���</summary>
        private PlayerInput m_playerInput;
        /// <summary>�v���C���[��Rigidbody</summary>
        private Rigidbody m_rb;
        /// <summary>�v���C���[�̃X�e�[�^�X</summary>
        private PlayerStatus m_status;
        /// <summary>�ˌ��̃��C�������_���[</summary>
        private LineRenderer m_lineRenderer;

        /// <summary>���������̓���</summary>
        private float m_horizontal;
        /// <summary>���������̓���</summary>
        private float m_vertical;

        /// <summary>���C���������R���[�`��</summary>
        private Coroutine m_clearLine;
        /// <summary>�_���[�W�G�t�F�N�g���Đ�����R���[�`��</summary>
        private Coroutine m_playDamageEffect;

        /// <summary>�v���C���[�������Ă��邩�ǂ���</summary>
        private bool m_isWalking = false;
        /// <summary>�v���C���[���n�ʂɂ��邩�ǂ���</summary>
        private bool m_isGround = true;

        /// <summary>
        /// ����������
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
            // �J�[�\���̕\����؂�ւ���
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
        /// �v���C���[�̓����蔻��𐧌䂷��
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
        /// �v���C���[���G�̍U�����󂯂��Ƃ��̏���
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
        /// �v���C���[�̈ړ��𐧌䂷��
        /// </summary>
        private void Move()
        {
            Vector3 dir = (this.transform.forward * m_vertical + this.transform.right * m_horizontal).normalized;

            // ���͂�����΁A�v���C���[�̌����Ă����������ɓ��͕����ɓ�����
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
        /// �e�𔭎˂���
        /// </summary>
        public void Fire()
        {
            if (m_status.CurrentBulletCount <= 0)
            {
                m_anim.SetBool("isReload", true);
                return;
            }

            m_status.CurrentBulletCount--;

            // �J�����̒������烌�C���΂�
            Ray ray = new Ray(m_camera.transform.position, m_camera.transform.forward);
            RaycastHit hit;

            // �ˌ����̃G�t�F�N�g���Đ�
            m_muzzleFlash.transform.position = m_muzzle.transform.position;
            m_muzzleFlash.Play();

            // ���C�����������ꍇ
            if (Physics.Raycast(ray, out hit))
            {
                // �e�̐�[���烌�C�����������ꏊ�܂ł̃��C����`��
                m_lineRenderer.SetPosition(0, m_muzzle.transform.position);
                m_lineRenderer.SetPosition(1, hit.point);

                // ���e�n�_�̃G�t�F�N�g���Đ�
                m_hitSpark.transform.position = hit.point;
                m_hitSpark.Play();

                // ���C���������R���[�`�����J�n
                m_clearLine = StartCoroutine(ClearLineAfterSeconds(0.05f)); // 0.05�b��Ƀ��C��������

                // �q�b�g�����I�u�W�F�N�g��IEventCollision���������Ă���΃C�x���g�𔭐�������
                if (hit.transform.root.gameObject.TryGetComponent<IEventCollision>(out var eventCollision))
                {
                    eventCollision.CollisionEvent(m_eventSystemInGame);
                    // �q�b�g�����I�u�W�F�N�g���G�ł���΃_���[�W��^����
                    if (hit.transform.root.gameObject.TryGetComponent<EnemyBase>(out var enemy))
                    {
                        enemy.TakeDamage(m_status.GetPlayerStatusData().GetPower(), hit.collider.gameObject.name);
                    }
                }
            }
            else
            {
                // ���C��������Ȃ������ꍇ�A100���j�b�g��܂ł̃��C����`��
                Vector3 endPosition = ray.origin + ray.direction * 100;
                m_lineRenderer.SetPosition(0, m_muzzle.transform.position);
                m_lineRenderer.SetPosition(1, endPosition);

                // ���C���������R���[�`�����J�n
                m_clearLine = StartCoroutine(ClearLineAfterSeconds(0.05f)); // 0.05�b��Ƀ��C��������
            }
        }

        /// <summary>
        /// �e�������[�h����
        /// </summary>
        public void Reload()
        {
            m_status.CurrentBulletCount = m_status.GetPlayerStatusData().GetMaxBulletCount();
            m_anim.SetBool("isReload", false);
        }

        /// <summary>
        /// ��莞�Ԍ�Ƀ��C��������
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private IEnumerator ClearLineAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            // ���C���𖳌��ɂ���
            m_lineRenderer.SetPosition(0, Vector3.zero);
            m_lineRenderer.SetPosition(1, Vector3.zero);

            StopCoroutine(m_clearLine);
        }

        /// <summary>
        /// �_���[�W�G�t�F�N�g���Đ�����
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayDamageEffect()
        {
            m_damageEffect.SetActive(true);

            yield return new WaitForSeconds(0.1f);

            m_damageEffect.SetActive(false);
        }

        /// <summary>
        /// �v���C���[�����S�����Ƃ��̏���
        /// </summary>
        private void OnPlayerDie()
        {
            m_anim.SetBool("isDie", true);
            m_playerInput.enabled = false;
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
            m_anim.SetFloat("VelocityX", m_horizontal);
            m_anim.SetFloat("VelocityY", m_vertical);
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
            }
            else if (context.canceled)
            {
                m_isWalking = false;
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
        }

        /// <summary>
        /// �v���C���[�̃W�����v�𐧌䂷��
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
        /// �v���C���[�̎ˌ��𐧌䂷��
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
        /// �v���C���[�̃����[�h�𐧌䂷��
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
        /// �A�j���[�V������IK�𐧌䂷��
        /// </summary>
        void OnAnimatorIK()
        {
            m_anim.SetLookAtWeight(1, 1, 1, 1, 1);
            m_anim.SetLookAtPosition(Camera.main.transform.position + Camera.main.transform.forward * 100);

            Vector3 ikTarget = Camera.main.transform.position + Camera.main.transform.forward * 100;

            AnimatorStateInfo stateInfo = m_anim.GetCurrentAnimatorStateInfo(0);

            // ���S����IK�𖳌��ɂ���
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
                // �e�̉�]���J�����̒����ɍ��킹��
                m_gun.transform.rotation = Quaternion.LookRotation(ikTarget - m_muzzle.transform.position);
            }

            // �E���IK��ݒ�
            m_anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            m_anim.SetIKPosition(AvatarIKGoal.RightHand, m_rightHandPos.transform.position);
            m_anim.SetIKRotation(AvatarIKGoal.RightHand, m_rightHandPos.transform.rotation);

            // �e�̉�]���J�����̒����ɍ��킹��
            m_gun.transform.rotation = Quaternion.LookRotation(ikTarget - m_muzzle.transform.position);

            // �����[�h���͍����IK�𖳌��ɂ���
            if (stateInfo.IsName("Reload")) return;

            // �����IK��ݒ�
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
