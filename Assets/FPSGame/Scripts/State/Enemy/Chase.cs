using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// ����̃X�e�[�g
    /// </summary>
    public class Chase : IState<EnemyBase>
    {
        private Coroutine m_chaseTarget;

        /// <summary>
        /// ���񒆂̏���
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
        /// ����X�e�[�g�̏I������
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            Debug.Log("Chase Exit");
            owner.StopCoroutine(m_chaseTarget);
            m_chaseTarget = null;
        }

        /// <summary>
        /// �^�[�Q�b�g�̒ǐ�
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