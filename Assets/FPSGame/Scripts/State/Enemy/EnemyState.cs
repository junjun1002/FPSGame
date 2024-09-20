using UnityEngine;

namespace FPS
{
    /// <summary>
    /// Enemyのステート情報だけを所持しているクラス
    /// </summary>
    public class EnemyState : MonoBehaviour
    {
        #region EnemyState
        public StateMachine<EnemyBase> stateMachine;

        private EnemyBase m_enemyBase;

        /// <summary>
        /// 巡回ステート
        /// </summary>
        private IState<EnemyBase> patrolState = new Patrol();
        public IState<EnemyBase> PatrolState { get => patrolState; }

        /// <summary>
        /// 追跡ステート
        /// </summary>
        private IState<EnemyBase> chaseState = new Chase();
        public IState<EnemyBase> ChaseState { get => chaseState; }

        #endregion

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            m_enemyBase = GetComponent<EnemyBase>();

            if (m_enemyBase == null)
            {
                Debug.LogError("EnemyBase component not found.");
            }

            stateMachine = new StateMachine<EnemyBase>(m_enemyBase, patrolState);
        }
    }
}