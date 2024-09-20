using System.Collections;
using UnityEngine;

namespace FPS
{ 
    /// <summary>
    /// 巡回のステート
    /// </summary>
    public class Patrol : IState<EnemyBase>
    {
        private EnemyBase m_enemyBase;

        private Coroutine m_tagetPointMonitoring;

        /// <summary>
        /// 巡回中の処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnExecute(EnemyBase owner)
        {
            Debug.Log("Patrol");
            m_enemyBase = owner;
            owner.GetNavMeshAgent().speed = owner.GetEnemyData().GetPatrolSpeed();
            owner.Patroling();

            if(m_tagetPointMonitoring == null)
            {
                m_tagetPointMonitoring = owner.StartCoroutine(TagetPointMonitoring());
            }
        }

        /// <summary>
        /// 巡回ステートの終了処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            owner.StopCoroutine(m_tagetPointMonitoring);
            m_tagetPointMonitoring = null;
        }

        /// <summary>
        /// ターゲット地点の監視
        /// </summary>
        /// <returns></returns>
        private IEnumerator TagetPointMonitoring()
        {
            while(true)
            {
                // エージェントが現目標地点に近づいてきたら、
                // 次の目標地点を選択します
                if (!m_enemyBase.GetNavMeshAgent().pathPending && m_enemyBase.GetNavMeshAgent().remainingDistance < 0.5f)
                {
                    m_enemyBase.Patroling();
                }

                yield return null;
            }
        }
    }
}