using System;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �V���O���g���p�^�[�����������钊�ۃN���X
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    Type t = typeof(T);
                    m_instance = (T)FindObjectOfType(t);
                    if (m_instance == null)
                    {
                        Debug.LogWarning(t + "���A�^�b�`���Ă���GameObject�͂���܂���");
                        return null;
                    }
                }
                return m_instance;
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        virtual protected void Awake()
        {
            if (this != Instance)
            {
                Debug.LogWarning(typeof(T) + "�͊��ɑ���GameObject�ɃA�^�b�`����Ă��邽�߁A�R���|�[�l���g��j�����܂���"
                    + "�A�^�b�`����Ă���GameObject��" + Instance.gameObject.name + "�ł�");
                Destroy(gameObject);
                return;
            }
            //DontDestroyOnLoad(gameObject);�͌p����ŏ���
        }
    }
}