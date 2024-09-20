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

        /// <summary>視界角度</summary>
        [SerializeField] float m_sightAngle = 45.0f;
        /// <summary>プレイヤー</summary>
        [SerializeField] protected GameObject m_player;
        /// <summary>巡回地点</summary>
        [SerializeField] Transform[] m_patrolPoints;
        /// <summary>現在の巡回地点のインデックス</summary>
        private int m_currentPatrolPointIndex = 0;

        /// <summary>現在のHP</summary>
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
        /// ダメージを受ける
        /// </summary>
        /// <param name="playerPower"></param>
        public void TakeDamage(int playerPower, string hitPart)
        {
            m_agent.isStopped = true;
            Debug.Log("敵にヒットした部位：" + hitPart);

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

        /// <summary>
        /// 巡回
        /// </summary>
        public void Patroling()
        {
            // 地点がなにも設定されていないときに返します
            if (m_patrolPoints.Length == 0)
                return;

            // エージェントが設定された目標地点に行くように設定
            m_agent.destination = m_patrolPoints[m_currentPatrolPointIndex].position;
            // 配列内の次の位置を目標地点に設定し、次がなければ最初に戻す
            m_currentPatrolPointIndex = (m_currentPatrolPointIndex + 1) % m_patrolPoints.Length;
        }

        /// <summary>
        /// 追跡開始
        /// </summary>
        public void OnChase()
        {
            m_agent.isStopped = false;
        }

        /// <summary>
        /// 滑らかにターゲットの方向を向くように
        /// </summary>
        public void LookAtTarget(Transform target)
        {
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = target.position - this.transform.position;
            // 方向を、回転情報に変換
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            // 現在の回転情報と、ターゲット方向の回転情報を補完する
            transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, 0.1f);
        }

        /// <summary>
        /// ターゲットまでの距離を取得
        /// </summary>
        /// <returns></returns>
        public float GetTargetDistance()
        {
            return Vector3.Distance(transform.position, m_player.transform.position);
        }

        /// <summary>エージェントを取得</summary>
        public NavMeshAgent GetNavMeshAgent() { return m_agent; }

        /// <summary>敵のステータスデータを取得</summary>
        public EnemyStatusData GetEnemyData() { return m_enemyData; }

        public EnemyState GetEnemyState() { return m_enemyState; }

        /// <summary>
        /// 攻撃
        /// </summary>
        public virtual void Attack() { }
        /// <summary>
        /// プレイヤーに向かって移動
        /// </summary>
        public virtual void Chasing() { }
    }
}
