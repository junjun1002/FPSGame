using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 攻撃のステート
    /// </summary>
    public class Attack : IState<EnemyBase>
    {
        bool m_isAttacking;

        private Coroutine m_attacking;

        /// <summary>
        /// 攻撃の処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnEnter(EnemyBase owner)
        {
            owner.GetNavMeshAgent().isStopped = true;
            Debug.Log("Attack");
            m_attacking = owner.StartCoroutine(Attacking(owner));
        }

        /// <summary>
        /// 攻撃ステートの終了処理
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            Debug.Log("Attack Exit");

            if (m_attacking != null)
            {
                Debug.Log("m_attacking is not null");
                owner.StopCoroutine(m_attacking);
            }
            else
            {
                Debug.LogError("m_attacking is null");
            }
        }

        public void Update(EnemyBase owner)
        {
            owner.LookAtPlayer();
            AnimatorStateInfo stateInfo = owner.GetAnimator().GetCurrentAnimatorStateInfo(0);
            m_isAttacking = stateInfo.IsName("Punch"); 
        }

        /// <summary>
        /// 攻撃
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private IEnumerator Attacking(EnemyBase owner)
        {
            while (true)
            {
                if (owner.GetNavMeshAgent().stoppingDistance < owner.GetTargetDistance())
                {
                    // 攻撃アニメーションが再生中なら、アニメーションが終了するまで待つ
                    if (m_isAttacking)
                    {
                        Debug.Log("アニメションの再生が終わってからパトロールステートに遷移");
                        yield return new WaitUntil(() => !m_isAttacking);
                        Debug.Log("アニメションの再生終了");
                    }
                    owner.GetEnemyState().stateMachine.ChangeMachine(owner.GetEnemyState().PatrolState);
                }
                else
                {
                    owner.Attack();
                }

                yield return new WaitForSeconds(2);
            }
        }
    }
}