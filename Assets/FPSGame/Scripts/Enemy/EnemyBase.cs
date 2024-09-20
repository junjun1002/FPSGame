using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FPS
{
    public class EnemyBase : MonoBehaviour
    {
        /// <summary>�G�̃X�e�[�^�X�f�[�^</summary>
        [SerializeField] EnemyStatusData m_enemyData;

        /// <summary>�G�̃A�j���[�^�[</summary>
        [SerializeField] protected Animator m_anim;
        /// <summary>�U������</summary>
        [SerializeField] protected GameObject m_attackDecision;
        /// <summary>�i�r���b�V���G�[�W�F���g</summary>
        protected NavMeshAgent m_agent;

        /// <summary>���E�p�x</summary>
        [SerializeField] float m_sightAngle = 45.0f;
        /// <summary>�v���C���[</summary>
        [SerializeField] protected GameObject m_player;
        /// <summary>����n�_</summary>
        [SerializeField] Transform[] m_patrolPoints;
        /// <summary>���݂̏���n�_�̃C���f�b�N�X</summary>
        private int m_currentPatrolPointIndex = 0;

        /// <summary>���݂�HP</summary>
        private int m_currentHP;

        private EnemyState m_enemyState;

        private void Awake()
        {
            m_currentHP = m_enemyData.GetMaxHP();

            m_agent = GetComponent<NavMeshAgent>();

            if (m_agent == null)
            {
                Debug.LogError("NavMeshAgent component not found.");
            }

            m_enemyState = GetComponent<EnemyState>();

            if (m_enemyState == null)
            {
                Debug.LogError("EnemyState component not found.");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (m_enemyState.stateMachine.CurrentState == m_enemyState.AttackState) return;

            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                Vector3 posDelta = other.transform.position - transform.position;
                float targetAngle = Vector3.Angle(transform.forward, posDelta);
                if (targetAngle < m_sightAngle)
                {
                    if (Physics.Raycast(transform.position, new Vector3(posDelta.x, 0f, posDelta.z), out RaycastHit hit))
                    {
                        if (hit.collider == other)
                        {
                            m_enemyState.stateMachine.ChangeMachine(m_enemyState.ChaseState);
                        }
                        else if(hit.collider == null)
                        {
                            m_enemyState.stateMachine.ChangeMachine(m_enemyState.PatrolState);
                        }
                        else
                        {
                            m_enemyState.stateMachine.ChangeMachine(m_enemyState.PatrolState);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �_���[�W���󂯂�
        /// </summary>
        /// <param name="playerPower"></param>
        public void TakeDamage(int playerPower, string hitPart)
        {
            m_agent.isStopped = true;
            Debug.Log("�G�Ƀq�b�g�������ʁF" + hitPart);

            int damage = playerPower;
            // �q�b�g�������ʂɂ���ă_���[�W��ύX
            if (hitPart == "Head")
            {
                damage *= 5;
                m_anim.SetTrigger("HitHead");
            }
            else if (hitPart == "Body")
            {
                m_anim.SetTrigger("HitBody");
            }

            Debug.Log("�G��" + damage + "�̃_���[�W��^����");
            m_currentHP -= damage;

            if (m_currentHP <= 0)
            {
                Destroy(gameObject);
                Debug.Log("�G��|����");
            }
        }

        /// <summary>
        /// �U��������A�N�e�B�u�ɂ���
        /// </summary>
        public void OnActiveAttackDecision()
        {
            m_attackDecision.SetActive(true);
        }

        /// <summary>
        /// �U��������A�N�e�B�u�ɂ���
        /// </summary>
        public void OnDeactiveAttackDecision()
        {
            m_attackDecision.SetActive(false);
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Patroling()
        {
            // �n�_���Ȃɂ��ݒ肳��Ă��Ȃ��Ƃ��ɕԂ��܂�
            if (m_patrolPoints.Length == 0)
                return;

            // �G�[�W�F���g���ݒ肳�ꂽ�ڕW�n�_�ɍs���悤�ɐݒ�
            m_agent.destination = m_patrolPoints[m_currentPatrolPointIndex].position;
            // �z����̎��̈ʒu��ڕW�n�_�ɐݒ肵�A�����Ȃ���΍ŏ��ɖ߂�
            m_currentPatrolPointIndex = (m_currentPatrolPointIndex + 1) % m_patrolPoints.Length;
        }

        /// <summary>
        /// �ǐՊJ�n
        /// </summary>
        public void OnChase()
        {
            m_agent.isStopped = false;
        }

        /// <summary>
        /// ���炩�Ƀ^�[�Q�b�g�̕����������悤��
        /// </summary>
        public void LookAtTarget(Transform target)
        {
            // �^�[�Q�b�g�����̃x�N�g�����擾
            Vector3 relativePos = target.position - this.transform.position;
            // �������A��]���ɕϊ�
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // ���݂̉�]���ƁA�^�[�Q�b�g�����̉�]����⊮����
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, 0.1f);
        }

        /// <summary>
        /// �^�[�Q�b�g�܂ł̋������擾
        /// </summary>
        /// <returns></returns>
        public float GetTargetDistance()
        {
            return Vector3.Distance(transform.position, m_player.transform.position);
        }

        /// <summary>�G�[�W�F���g���擾</summary>
        public NavMeshAgent GetNavMeshAgent() { return m_agent; }

        /// <summary>�G�̃X�e�[�^�X�f�[�^���擾</summary>
        public EnemyStatusData GetEnemyData() { return m_enemyData; }

        public EnemyState GetEnemyState() { return m_enemyState; }

        /// <summary>
        /// �U��
        /// </summary>
        public virtual void Attack() { }
        /// <summary>
        /// �v���C���[�Ɍ������Ĉړ�
        /// </summary>
        public virtual void Chasing() { }
    }
}
