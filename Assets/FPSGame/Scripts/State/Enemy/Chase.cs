using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 巡回のステート
    /// </summary>
    public class Chase : IState<EnemyBase>
    {
        private Coroutine m_chaseTarget;

        /// <summary>
        /// 巡回中の処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnEnter(EnemyBase owner)
        {
            Debug.Log("Chase");
            owner.GetNavMeshAgent().speed = owner.GetEnemyData().GetChaseSpeed();
            owner.GetNavMeshAgent().stoppingDistance = 2.5f;
            m_chaseTarget = owner.StartCoroutine(ChaseTarget(owner));
        }

        /// <summary>
        /// 巡回ステートの終了処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            Debug.Log("Chase Exit");
            owner.StopCoroutine(m_chaseTarget);
            m_chaseTarget = null;
        }

        /// <summary>
        /// ターゲットの追跡
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private IEnumerator ChaseTarget(EnemyBase owner)
        {
            while (true)
            {
                owner.Chaseing();
                yield return null;
            }
        }
    }
}