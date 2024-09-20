using System.Collections;
using UnityEngine;

namespace FPS
{ 
    /// <summary>
    /// ����̃X�e�[�g
    /// </summary>
    public class Patrol : IState<EnemyBase>
    {
        private EnemyBase m_enemyBase;

        private Coroutine m_tagetPointMonitoring;

        /// <summary>
        /// ���񒆂̏���
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
        /// ����X�e�[�g�̏I������
        /// </summary>
        /// <param name="owner"></param>
        public void OnExit(EnemyBase owner)
        {
            owner.StopCoroutine(m_tagetPointMonitoring);
            m_tagetPointMonitoring = null;
        }

        /// <summary>
        /// �^�[�Q�b�g�n�_�̊Ď�
        /// </summary>
        /// <returns></returns>
        private IEnumerator TagetPointMonitoring()
        {
            while(true)
            {
                // �G�[�W�F���g�����ڕW�n�_�ɋ߂Â��Ă�����A
                // ���̖ڕW�n�_��I�����܂�
                if (!m_enemyBase.GetNavMeshAgent().pathPending && m_enemyBase.GetNavMeshAgent().remainingDistance < 0.5f)
                {
                    m_enemyBase.Patroling();
                }

                yield return null;
            }
        }
    }
}