using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �ǐՂ̃X�e�[�g
    /// </summary>
    public class Chase : IState<EnemyBase>
    {
        /// <summary>
        /// �ǐՒ��̏���
        /// </summary>
        /// <param name="owner"></param>
        public void OnEnter(EnemyBase owner)
        {
            Debug.Log("Chase");
            owner.GetNavMeshAgent().speed = owner.GetEnemyData().GetChaseSpeed();
            owner.GetNavMeshAgent().stoppingDistance = 2.5f;
        }

        /// <summary>
        /// �ǐՃX�e�[�g�̏I������
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
        /// �^�[�Q�b�g�̒ǐ�
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