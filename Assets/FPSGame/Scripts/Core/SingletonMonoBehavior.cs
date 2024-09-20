using System;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// シングルトンパターンを実装する抽象クラス
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
                        Debug.LogWarning(t + "をアタッチしているGameObjectはありません");
                        return null;
                    }
                }
                return m_instance;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        virtual protected void Awake()
        {
            if (this != Instance)
            {
                Debug.LogWarning(typeof(T) + "は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました"
                    + "アタッチされているGameObjectは" + Instance.gameObject.name + "です");
                Destroy(gameObject);
                return;
            }
            //DontDestroyOnLoad(gameObject);は継承先で書く
        }
    }
}