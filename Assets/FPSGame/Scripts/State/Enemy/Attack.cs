using System.Collections;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �U���̃X�e�[�g
    /// </summary>
    public class Attack : IState<EnemyBase>
    {
        bool m_isAttacking;

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
            AnimatorStateInfo stateInfo = owner.GetAnimator().GetCurrentAnimatorStateInfo(0);
            m_isAttacking = stateInfo.IsName("Punch"); 
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
                    // �U���A�j���[�V�������Đ����Ȃ�A�A�j���[�V�������I������܂ő҂�
                    if (m_isAttacking)
                    {
                        Debug.Log("�A�j���V�����̍Đ����I����Ă���p�g���[���X�e�[�g�ɑJ��");
                        yield return new WaitUntil(() => !m_isAttacking);
                        Debug.Log("�A�j���V�����̍Đ��I��");
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