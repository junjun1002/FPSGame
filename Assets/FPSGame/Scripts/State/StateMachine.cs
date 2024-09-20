using FPS;
using UnityEngine;

namespace FPS
{
    public class StateMachine<T> where T : MonoBehaviour
    {
        /// <summary>ステートの種類によってオーナーを設定する</summary>
        T owner;

        /// <summary>現在のステート</summary>
        private IState<T> m_currentState;
        public IState<T> CurrentState
        {
            get => m_currentState;
            set
            {
                // ステートが切り替わったらステートの処理を実行する
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
        /// オーナーと初期ステートを設定するコンストラクター
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initState"></param>
        public StateMachine(T owner, IState<T> initState)
        {
            this.owner = owner;
            CurrentState = initState;
        }

        /// <summary>
        /// 次のステートに移行させる関数
        /// </summary>
        /// <param name="nextState"></param>
        public void ChangeMachine(IState<T> nextState)
        {
            CurrentState = nextState;
        }
    }
}