using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 巡回のステート
    /// </summary>
    public class Patrol : IState<EnemyBase>
    {
        private Coroutine m_tagetPointMonitoring;

        /// <summary>
        /// 巡回中の処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnEnter(EnemyBase owner)
        {
            Debug.Log("Patrol");

            if (owner.GetNavMeshAgent() == null)
            {
                Debug.LogError("NavMeshAgent component not found.");
            }

            owner.GetNavMeshAgent().speed = owner.GetEnemyData().GetPatrolSpeed();
            owner.GetNavMeshAgent().stoppingDistance = 0f;
            owner.Patroling();

            if (m_tagetPointMonitoring == null)
            {
                m_tagetPointMonitoring = owner.StartCoroutine(TagetPointMonitoring(owner));
            }
        }

        /// <summary>
        /// 巡回ステートの終了処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            Debug.Log("Patrol Exit");
            owner.StopCoroutine(m_tagetPointMonitoring);
            m_tagetPointMonitoring = null;
        }

        /// <summary>
        /// ターゲット地点の監視
        /// </summary>
        /// <returns></returns>
        private IEnumerator TagetPointMonitoring(EnemyBase owner)
        {
            while (true)
            {
                // エージェントが現目標地点に近づいてきたら、
                // 次の目標地点を選択します
                if (!owner.GetNavMeshAgent().pathPending && owner.GetNavMeshAgent().remainingDistance < 0.5f)
                {
                    owner.Patroling();
                }

                yield return null;
            }
        }
    }
}