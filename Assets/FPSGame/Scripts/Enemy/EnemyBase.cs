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

        /// <summary>�G�̃X�e�[�^�X�f�[�^���擾</summary>
        public EnemyStatusData GetEnemyData() { return m_enemyData; }

        /// <summary>�G�̃A�j���[�^�[</summary>
        [SerializeField] protected Animator m_anim;
        /// <summary>�U������</summary>
        [SerializeField] protected GameObject m_attackDecision;
        /// <summary>�i�r���b�V���G�[�W�F���g</summary>
        [SerializeField] protected NavMeshAgent m_agent;

        [SerializeField] protected GameObject m_player;

        /// <summary>���݂�HP</summary>
        private int m_currentHP;

        private void Awake()
        {
            m_currentHP = m_enemyData.GetMaxHP();
        }

        /// <summary>
        /// �_���[�W���󂯂�
        /// </summary>
        /// <param name="playerPower"></param>
        public void TakeDamage(int playerPower, string hitPart)
        {
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

        protected virtual void Attack() { }

        protected virtual void MoveToPlayer() { }

        public void OnActiveAttackDecision()
        {
            m_attackDecision.SetActive(true);
        }

        public void OnDeactiveAttackDecision()
        {
            m_attackDecision.SetActive(false);
        }
    }
}
