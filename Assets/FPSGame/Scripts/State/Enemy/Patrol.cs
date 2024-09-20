using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// ����̃X�e�[�g
    /// </summary>
    public class Patrol : IState<EnemyBase>
    {
        private Coroutine m_tagetPointMonitoring;

        /// <summary>
        /// ���񒆂̏���
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
        /// ����X�e�[�g�̏I������
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            Debug.Log("Patrol Exit");
            owner.StopCoroutine(m_tagetPointMonitoring);
            m_tagetPointMonitoring = null;
        }

        /// <summary>
        /// �^�[�Q�b�g�n�_�̊Ď�
        /// </summary>
        /// <returns></returns>
        private IEnumerator TagetPointMonitoring(EnemyBase owner)
        {
            while (true)
            {
                // �G�[�W�F���g�����ڕW�n�_�ɋ߂Â��Ă�����A
                // ���̖ڕW�n�_��I�����܂�
                if (!owner.GetNavMeshAgent().pathPending && owner.GetNavMeshAgent().remainingDistance < 0.5f)
                {
                    owner.Patroling();
                }

                yield return null;
            }
        }
    }
}