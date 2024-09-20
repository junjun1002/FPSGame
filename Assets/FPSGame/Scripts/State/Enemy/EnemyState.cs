using UnityEngine;

namespace FPS
{
    /// <summary>
    /// Enemy�̃X�e�[�g��񂾂����������Ă���N���X
    /// </summary>
    public class EnemyState : MonoBehaviour
    {
        #region EnemyState
        public StateMachine<EnemyBase> stateMachine;

        private EnemyBase m_enemyBase;

        /// <summary>
        /// ����X�e�[�g
        /// </summary>
        private IState<EnemyBase> patrolState = new Patrol();
        public IState<EnemyBase> PatrolState { get => patrolState; }

        /// <summary>
        /// �ǐՃX�e�[�g
        /// </summary>
        private IState<EnemyBase> chaseState = new Chase();
        public IState<EnemyBase> ChaseState { get => chaseState; }

        #endregion

        /// <summary>
        /// ����������
        /// EnemyBase�̏�����������҂������̂�Start�ŏ�����
        /// Awake����EnemyBase�̏������������I����Ă��Ȃ�����
        /// </summary>
        private void Start()
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