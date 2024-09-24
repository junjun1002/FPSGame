using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FPS
{
    /// <summary>
    /// �v���C���[�̃X�e�[�^�X�N���X
    /// </summary>
    public class PlayerStatus : MonoBehaviour
    {
        /// <summary>Player�����S�����Ƃ��̃C�x���g</summary>
        public Action OnPlayerDeadAction { get; set; }

        /// <summary>�v���C���[�̃X�e�[�^�X�f�[�^</summary>
        [SerializeField] PlayerStatusData playerStatusData = default;

        /// <summary>HP�o�[</summary>
        [SerializeField] Slider m_hpBar;
        /// <summary>�e���o�[</summary>
        [SerializeField] Slider m_bulletCount;
        /// <summary>�e���e�L�X�g</summary>
        [SerializeField] TextMeshProUGUI m_bulletCountText;

        /// <summary>���݂�HP</summary>
        private int m_currentHP;

        /// <summary>���݂̒e��</summary>
        private int m_currentBulletCount;
        public int CurrentBulletCount
        {
            get => m_currentBulletCount;
            set
            {
                // �e�����ς������C�x���g�����s
                if (m_currentBulletCount != value)
                {
                    m_currentBulletCount = value;
                    m_bulletCount.value = m_currentBulletCount;
                    m_bulletCountText.text = m_currentBulletCount.ToString() + " / " + playerStatusData.GetMaxBulletCount().ToString();
                }
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        private void Awake()
        {
            m_currentHP = playerStatusData.GetMaxHP();
            m_currentBulletCount = playerStatusData.GetMaxBulletCount();
        }

        /// <summary>
        /// �_���[�W���󂯂�
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamege(int damage)
        {
            m_currentHP -= damage;

            m_hpBar.value = m_currentHP;

            if (m_currentHP <= 0)
            {
                Debug.Log("Player�����S���܂���");
                OnPlayerDead();
            }
        }

        /// <summary>
        /// �v���C���[�����S�����Ƃ��̏���
        /// </summary>
        private void OnPlayerDead()
        {
            OnPlayerDeadAction?.Invoke();
        }

        /// <summary>�v���C���[�̃X�e�[�^�X�f�[�^���擾����</summary>
        public PlayerStatusData GetPlayerStatusData() { return playerStatusData; }
    }
}
