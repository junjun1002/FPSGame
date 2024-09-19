using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class EnemyBase : MonoBehaviour
    {
        /// <summary>敵のステータスデータ</summary>
        [SerializeField] EnemyStatusData m_enemyData;
        /// <summary>敵のアニメーター</summary>
        [SerializeField] Animator m_anim;

        /// <summary>現在のHP</summary>
        private int m_currentHP;

        private void Awake()
        {
            m_currentHP = m_enemyData.GetMaxHP();
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="playerPower"></param>
        public void TakeDamage(int playerPower, string hitPart)
        {
            int damage = playerPower;
            // ヒットした部位によってダメージを変更
            if (hitPart == "Head")
            {
                damage *= 5;
                m_anim.SetTrigger("HitHead");
            }
            else if (hitPart == "Body")
            {
                m_anim.SetTrigger("HitBody");
            }

            Debug.Log("敵に" + damage + "のダメージを与えた");
            m_currentHP -= damage;

            if (m_currentHP <= 0)
            {
                Destroy(gameObject);
                Debug.Log("敵を倒した");
            }
        }
    }
}
