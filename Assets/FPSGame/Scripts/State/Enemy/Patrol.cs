using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// ����̃X�e�[�g
    /// </summary>
    public class Patrol : IState<EnemyBase>
    {
        /// <summary>
        /// ���񒆂̏���
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
        /// ����X�e�[�g�̏I������
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
        /// �^�[�Q�b�g�n�_�̊Ď�
        /// </summary>
        private void TargetPointMonitoring(EnemyBase owner)
        {
            // �G�[�W�F���g�����ڕW�n�_�ɋ߂Â��Ă�����A
            // ���̖ڕW�n�_��I�����܂�
            if (!owner.GetNavMeshAgent().pathPending && owner.GetNavMeshAgent().remainingDistance < 0.5f)
            {
                owner.Patroling();
            }
        }
    }
}