using UnityEngine;
using UnityEngine.AI;

namespace FPS
{
    /// <summary>
    /// 敵の基底クラス
    /// </summary>
    public class EnemyBase : MonoBehaviour, IEventCollision
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

        /// <summary>ラグドール</summary>
        [SerializeField] private GameObject m_ragdoll;
        /// <summary>ルートボーン</summary>
        [SerializeField] private Transform m_rootBone;

        /// <summary>巡回地点</summary>
        [SerializeField] Transform[] m_patrolPoints;
        /// <summary>現在の巡回地点のインデックス</summary>
        private int m_currentPatrolPointIndex = 0;

        /// <summary>現在のHP</summary>
        private int m_currentHP;

        private EnemyState m_enemyState;

        /// <summary>初期化処理</summary>
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

        /// <summary>
        /// ゲーム中の衝突イベント
        /// </summary>
        /// <param name="eventSystemInGame"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CollisionEvent(EventSystemInGame eventSystemInGame)
        {
            if (m_enemyState.stateMachine.CurrentState == m_enemyState.PatrolState)
            {
                m_enemyState.stateMachine.ChangeMachine(m_enemyState.ChaseState);
            }
        }

        /// <summary>
        /// 視界にプレイヤーがいるかの確認
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
        {
            // 攻撃中は追跡しない
            if (m_enemyState.stateMachine.CurrentState == m_enemyState.AttackState) return;

            // プレイヤーが視界に入ったら追跡ステートに変更
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                Vector3 posDelta = other.transform.position - transform.position;
                float targetAngle = Vector3.Angle(transform.forward, posDelta);
                if (targetAngle < m_sightAngle)
                {
                    if (Physics.Raycast(transform.position, posDelta, out RaycastHit hit))
                    {
                        if (hit.collider == other)
                        {
                            m_enemyState.stateMachine.ChangeMachine(m_enemyState.ChaseState);
                        }
                        else if (hit.collider == null)
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

        private void OnDead()
        {
            m_ragdoll.transform.position = transform.position;
            m_ragdoll.SetActive(true);
            this.gameObject.SetActive(false);
            m_ragdoll.GetComponent<EnemyRagdoll>().RagdollSetup(m_rootBone);
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
                OnDead();
                //Destroy(gameObject);
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
        /// </summary>animatorm_
        public void LookAtPlayer()
        {
            AnimatorStateInfo stateInfo = m_anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Punch")) return;

            // ターゲット方向のベクトルを取得
            Vector3 relativePos = m_player.transform.position - this.transform.position;
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

        /// <summary>アニメーターを取得</summary>
        public Animator GetAnimator() { return m_anim; }

        /// <summary>敵のステータスデータを取得</summary>
        public EnemyStatusData GetEnemyData() { return m_enemyData; }

        /// <summary>敵のステートを取得</summary>
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
