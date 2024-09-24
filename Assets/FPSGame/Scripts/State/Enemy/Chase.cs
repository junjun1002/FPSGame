using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 追跡のステート
    /// </summary>
    public class Chase : IState<EnemyBase>
    {
        /// <summary>
        /// 追跡中の処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnEnter(EnemyBase owner)
        {
            Debug.Log("Chase");
            owner.GetNavMeshAgent().speed = owner.GetEnemyData().GetChaseSpeed();
            owner.GetNavMeshAgent().stoppingDistance = 2.5f;
        }

        /// <summary>
        /// 追跡ステートの終了処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            Debug.Log("Chase Exit");
        }

        public void Update(EnemyBase owner)
        {
            ChaseTarget(owner);
        }

        /// <summary>
        /// ターゲットの追跡
        /// </summary>
        /// <param name="owner"></param>
        private void ChaseTarget(EnemyBase owner)
        {
            owner.Chasing();
            if (owner.GetNavMeshAgent().stoppingDistance >= owner.GetTargetDistance())
            {
                owner.GetEnemyState().stateMachine.ChangeMachine(owner.GetEnemyState().AttackState);
            }
        }
    }
}