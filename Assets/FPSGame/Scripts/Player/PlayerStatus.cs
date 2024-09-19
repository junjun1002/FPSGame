using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS
{
    public class PlayerStatus : MonoBehaviour
    {

        /// <summary>Playerが死亡したときのイベント</summary>
        public Action OnPlayerDeadAction { get; set; }

        [SerializeField] PlayerStatusData playerStatusData = default;

        public PlayerStatusData GetPlayerStatusData() { return playerStatusData; }

        [SerializeField] Slider m_hpBar;
        [SerializeField] Slider m_bulletCount;
        [SerializeField] TextMeshProUGUI m_bulletCountText;

        private int m_currentHP;

        private int m_currentBulletCount;

        public int CurrentBulletCount
        {
            get => m_currentBulletCount;
            set
            {
                // Blockの数が変わったらイベントを実行
                if (m_currentBulletCount != value)
                {
                    m_currentBulletCount = value;
                    m_bulletCount.value = m_currentBulletCount;
                    m_bulletCountText.text = m_currentBulletCount.ToString() + " / " + playerStatusData.GetMaxBulletCount().ToString();
                }
            }
        }

        private void Awake()
        {
            m_currentHP = playerStatusData.GetMaxHP();
            m_currentBulletCount = playerStatusData.GetMaxBulletCount();
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamege(int damage)
        {
            m_currentHP -= damage;

            m_hpBar.value = m_currentHP;

            if (m_currentHP <= 0)
            {
                Debug.Log("Playerが死亡しました");
                OnPlayerDead();
            }
        }

        private void OnPlayerDead()
        {
            OnPlayerDeadAction?.Invoke();
        }
    }
}
