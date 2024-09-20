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

        [SerializeField] protected GameObject m_player;

        [SerializeField] Transform[] m_patrolPoints;
        private int m_currentPatrolPointIndex = 0;

        /// <summary>���݂�HP</summary>
        private int m_currentHP;

        private void Awake()
        {
            m_currentHP = m_enemyData.GetMaxHP();

            m_agent = GetComponent<NavMeshAgent>();

            if (m_agent == null)
            {
                Debug.LogError("NavMeshAgent component not found.");
            }
        }

        /// <summary>
        /// �_���[�W���󂯂�
        /// </summary>
        /// <param name="playerPower"></param>
        public void TakeDamage(int playerPower, string hitPart)
        {
            m_agent.isStopped = true;

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

        public void Patroling()
        {
            // �n�_���Ȃɂ��ݒ肳��Ă��Ȃ��Ƃ��ɕԂ��܂�
            if (m_patrolPoints.Length == 0)
                return;

            // �G�[�W�F���g�����ݐݒ肳�ꂽ�ڕW�n�_�ɍs���悤�ɐݒ肵�܂�
            m_agent.destination = m_patrolPoints[m_currentPatrolPointIndex].position;
            // �z����̎��̈ʒu��ڕW�n�_�ɐݒ肵�A
            // �K�v�Ȃ�Ώo���n�_�ɂ��ǂ�܂�
            m_currentPatrolPointIndex = (m_currentPatrolPointIndex + 1) % m_patrolPoints.Length;
        }

        /// <summary>
        /// �ǐՊJ�n
        /// </summary>
        public void OnChase()
        {
            m_agent.isStopped = false;
        }

        /// <summary>�G�[�W�F���g���擾</summary>
        public NavMeshAgent GetNavMeshAgent() { return m_agent; }

        /// <summary>�G�̃X�e�[�^�X�f�[�^���擾</summary>
        public EnemyStatusData GetEnemyData() { return m_enemyData; }

        /// <summary>
        /// �U��
        /// </summary>
        protected virtual void Attack() { }
        /// <summary>
        /// �v���C���[�Ɍ������Ĉړ�
        /// </summary>
        protected virtual void MoveToPlayer() { }
    }
}
