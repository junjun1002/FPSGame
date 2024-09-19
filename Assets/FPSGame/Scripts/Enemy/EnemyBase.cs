using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class EnemyBase : MonoBehaviour
    {
        /// <summary>�G�̃X�e�[�^�X�f�[�^</summary>
        [SerializeField] EnemyStatusData m_enemyData;
        /// <summary>�G�̃A�j���[�^�[</summary>
        [SerializeField] Animator m_anim;

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
    }
}
