using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS
{
    /// <summary>
    /// プレイヤーのステータスクラス
    /// </summary>
    public class PlayerStatus : MonoBehaviour
    {
        /// <summary>Playerが死亡したときのイベント</summary>
        public Action OnPlayerDeadAction { get; set; }

        /// <summary>プレイヤーのステータスデータ</summary>
        [SerializeField] PlayerStatusData playerStatusData = default;

        /// <summary>HPバー</summary>
        [SerializeField] Slider m_hpBar;
        /// <summary>弾数バー</summary>
        [SerializeField] Slider m_bulletCount;
        /// <summary>弾数テキスト</summary>
        [SerializeField] TextMeshProUGUI m_bulletCountText;

        /// <summary>現在のHP</summary>
        private int m_currentHP;

        /// <summary>現在の弾数</summary>
        private int m_currentBulletCount;
        public int CurrentBulletCount
        {
            get => m_currentBulletCount;
            set
            {
                // 弾数が変わったらイベントを実行
                if (m_currentBulletCount != value)
                {
                    m_currentBulletCount = value;
                    m_bulletCount.value = m_currentBulletCount;
                    m_bulletCountText.text = m_currentBulletCount.ToString() + " / " + playerStatusData.GetMaxBulletCount().ToString();
                }
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
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

        /// <summary>
        /// プレイヤーが死亡したときの処理
        /// </summary>
        private void OnPlayerDead()
        {
            OnPlayerDeadAction?.Invoke();
        }

        /// <summary>プレイヤーのステータスデータを取得する</summary>
        public PlayerStatusData GetPlayerStatusData() { return playerStatusData; }
    }
}
