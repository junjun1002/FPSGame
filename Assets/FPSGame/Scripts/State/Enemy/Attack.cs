using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �U���̃X�e�[�g
    /// </summary>
    public class Attack : IState<EnemyBase>
    {
        private Coroutine m_attacking;

        /// <summary>
        /// �U���̏���
        /// </summary>
        /// <param name="owner"></param>
        public void OnEnter(EnemyBase owner)
        {
            owner.GetNavMeshAgent().isStopped = true;
            Debug.Log("Attack");
            m_attacking = owner.StartCoroutine(Attacking(owner));
        }

        /// <summary>
        /// �U���X�e�[�g�̏I������
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
        }

        /// <summary>
        /// �U��
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private IEnumerator Attacking(EnemyBase owner)
        {
            while (true)
            {
                if (owner.GetNavMeshAgent().stoppingDistance < owner.GetTargetDistance())
                {
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