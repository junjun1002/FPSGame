using FPS;
using UnityEngine;

namespace FPS
{
    public class StateMachine<T> where T : MonoBehaviour
    {
        /// <summary>�X�e�[�g�̎�ނɂ���ăI�[�i�[��ݒ肷��</summary>
        T owner;

        /// <summary>���݂̃X�e�[�g</summary>
        private IState<T> m_currentState;
        public IState<T> CurrentState
        {
            get => m_currentState;
            set
            {
                // �X�e�[�g���؂�ւ������X�e�[�g�̏��������s����
                if (m_currentState != value)
                {
                    if (m_currentState != null)
                    {
                        PreviousState = m_currentState;
                    }
                    m_currentState = value;
                    m_currentState.OnEnter(owner);
                }
            }
        }

        private IState<T> m_previousState;
        public IState<T> PreviousState
        {
            get => m_previousState;
            set
            {
                if (m_previousState != value)
                {
                    m_previousState = value;
                    m_previousState.OnExit(owner);
                }
            }
        }

        /// <summary>
        /// �I�[�i�[�Ə����X�e�[�g��ݒ肷��R���X�g���N�^�[
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initState"></param>
        public StateMachine(T owner, IState<T> initState)
        {
            this.owner = owner;
            CurrentState = initState;
        }

        /// <summary>
        /// ���̃X�e�[�g�Ɉڍs������֐�
        /// </summary>
        /// <param name="nextState"></param>
        public void ChangeMachine(IState<T> nextState)
        {
            CurrentState = nextState;
        }
    }
}