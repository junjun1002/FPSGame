using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS
{
    public class PlayerStatus : MonoBehaviour
    {

        /// <summary>Player�����S�����Ƃ��̃C�x���g</summary>
        public Action OnPlayerDeadAction { get; set; }

        [SerializeField] PlayerStatusData playerStatusData = default;

        public PlayerStatusData GetPlayerStatusData() { return playerStatusData; }

        [SerializeField] Slider m_hpBar;
        [SerializeField] Slider m_bulletCount;
        [SerializeField] TextMeshProUGUI m_bulletCountText;

        private int m_currentHP;
        public int CurrentHP
        {
            get => m_currentHP;
            set
            {
                // HP���ς������C�x���g�����s
                if (m_currentHP != value)
                {
                    m_currentHP = value;
                    m_hpBar.value = m_currentHP;

                    if (m_currentHP <= 0)
                    {
                        Debug.Log("Player�����S���܂���");
                        OnPlayerDead();
                    }
                }
            }
        }

        private int m_currentBulletCount;

        public int CurrentBulletCount
        {
            get => m_currentBulletCount;
            set
            {
                // Block�̐����ς������C�x���g�����s
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("HP��20���炷");
                CurrentHP -= 20;
            }
        }

        private void OnPlayerDead()
        {
            OnPlayerDeadAction?.Invoke();
        }
    }
}
