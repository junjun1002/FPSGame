using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �C�x���g���w�ǂ��钊�ۃN���X
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EventReceiver<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected EventSystemInGame m_eventSystemInGame;

        /// <summary>
        /// ����������
        /// </summary>
        private void Awake()
        {
            m_eventSystemInGame = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystemInGame>();
        }

        /// <summary>
        /// �C�x���g��o�^
        /// </summary>
        protected abstract void OnEnable();

        /// <summary>
        /// �C�x���g������
        /// </summary>
        protected abstract void OnDisable();
    }
}