using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FPS
{
    public class EnemyBase : MonoBehaviour
    {
        /// <summary>敵のステータスデータ</summary>
        [SerializeField] EnemyStatusData m_enemyData;

        /// <summary>敵のステータスデータを取得</summary>
        public EnemyStatusData GetEnemyData() { return m_enemyData; }

        /// <summary>敵のアニメーター</summary>
        [SerializeField] protected Animator m_anim;
        /// <summary>攻撃判定</summary>
        [SerializeField] protected GameObject m_attackDecision;
        /// <summary>ナビメッシュエージェント</summary>
        [SerializeField] protected NavMeshAgent m_agent;

        [SerializeField] protected GameObject m_player;

        /// <summary>現在のHP</summary>
        private int m_currentHP;

        private void Awake()
        {
            m_currentHP = m_enemyData.GetMaxHP();
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="playerPower"></param>
        public void TakeDamage(int playerPower, string hitPart)
        {
            int damage = playerPower;
            // ヒットした部位によってダメージを変更
            if (hitPart == "Head")
            {
                damage *= 5;
                m_anim.SetTrigger("HitHead");
            }
            else if (hitPart == "Body")
            {
                m_anim.SetTrigger("HitBody");
            }

            Debug.Log("敵に" + damage + "のダメージを与えた");
            m_currentHP -= damage;

            if (m_currentHP <= 0)
            {
                Destroy(gameObject);
                Debug.Log("敵を倒した");
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
