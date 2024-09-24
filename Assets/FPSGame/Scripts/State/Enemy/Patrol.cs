using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 巡回のステート
    /// </summary>
    public class Patrol : IState<EnemyBase>
    {
        /// <summary>
        /// 巡回中の処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnEnter(EnemyBase owner)
        {
            Debug.Log("Patrol");
            owner.GetNavMeshAgent().isStopped = false;

            owner.GetNavMeshAgent().speed = owner.GetEnemyData().GetPatrolSpeed();
            owner.GetNavMeshAgent().stoppingDistance = 0f;
        }

        /// <summary>
        /// 巡回ステートの終了処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            Debug.Log("Patrol Exit");
        }

        public void Update(EnemyBase owner)
        {
            TargetPointMonitoring(owner);
        }

        /// <summary>
        /// ターゲット地点の監視
        /// </summary>
        private void TargetPointMonitoring(EnemyBase owner)
        {
            // エージェントが現目標地点に近づいてきたら、
            // 次の目標地点を選択します
            if (!owner.GetNavMeshAgent().pathPending && owner.GetNavMeshAgent().remainingDistance < 0.5f)
            {
                owner.Patroling();
            }
        }
    }
}