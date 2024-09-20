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

        /// <summary>敵のアニメーター</summary>
        [SerializeField] protected Animator m_anim;
        /// <summary>攻撃判定</summary>
        [SerializeField] protected GameObject m_attackDecision;
        /// <summary>ナビメッシュエージェント</summary>
        protected NavMeshAgent m_agent;

        [SerializeField] protected GameObject m_player;

        [SerializeField] Transform[] m_patrolPoints;
        private int m_currentPatrolPointIndex = 0;

        /// <summary>現在のHP</summary>
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
        /// ダメージを受ける
        /// </summary>
        /// <param name="playerPower"></param>
        public void TakeDamage(int playerPower, string hitPart)
        {
            m_agent.isStopped = true;

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

        /// <summary>
        /// 攻撃判定をアクティブにする
        /// </summary>
        public void OnActiveAttackDecision()
        {
            m_attackDecision.SetActive(true);
        }

        /// <summary>
        /// 攻撃判定を非アクティブにする
        /// </summary>
        public void OnDeactiveAttackDecision()
        {
            m_attackDecision.SetActive(false);
        }

        public void Patroling()
        {
            // 地点がなにも設定されていないときに返します
            if (m_patrolPoints.Length == 0)
                return;

            // エージェントが現在設定された目標地点に行くように設定します
            m_agent.destination = m_patrolPoints[m_currentPatrolPointIndex].position;
            // 配列内の次の位置を目標地点に設定し、
            // 必要ならば出発地点にもどります
            m_currentPatrolPointIndex = (m_currentPatrolPointIndex + 1) % m_patrolPoints.Length;
        }

        /// <summary>
        /// 追跡開始
        /// </summary>
        public void OnChase()
        {
            m_agent.isStopped = false;
        }

        /// <summary>エージェントを取得</summary>
        public NavMeshAgent GetNavMeshAgent() { return m_agent; }

        /// <summary>敵のステータスデータを取得</summary>
        public EnemyStatusData GetEnemyData() { return m_enemyData; }

        /// <summary>
        /// 攻撃
        /// </summary>
        protected virtual void Attack() { }
        /// <summary>
        /// プレイヤーに向かって移動
        /// </summary>
        protected virtual void MoveToPlayer() { }
    }
}
