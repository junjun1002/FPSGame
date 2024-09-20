using System;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �v���C���[�̃X�e�[�^�X�f�[�^
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "PlayerStatusData", menuName = "Data/PlayerStatus")]
    public class PlayerStatusData : ScriptableObject
    {
        // �v���C���[�̃X�e�[�^�X
        [SerializeField, Header("�ő�HP")] int maxHP = 100;
        [SerializeField, Header("�U����")] int power = 10;
        [SerializeField, Header("�v���C���[�̍ő�e��")] int maxBulletCount = 25;
        [SerializeField, Header("�v���C���[�̈ړ����x")] float moveSpeed = 5.0f;
        [SerializeField, Header("�v���C���[�̕��s���x")] float walkSpeed = 2.0f;
        [SerializeField, Header("�v���C���[�̍ő呬�x")] float maxSpeed = 10.0f;
        [SerializeField, Header("�v���C���[�̃W�����v��")] float jumpForce = 5.0f;

        /// <summary>�v���C���[�̍ő�HP���擾</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>�v���C���[�̍U���͂��擾</summary>
        public int GetPower() { return power; }
        /// <summary>�v���C���[�̍ő�e�����擾</summary>
        public int GetMaxBulletCount() { return maxBulletCount; }
        /// <summary>�v���C���[�̈ړ����x���擾</summary>
        public float GetMoveSpeed() { return moveSpeed; }
        /// <summary>�v���C���[�̕��s���x���擾</summary>
        public float GetWalkSpeed() { return walkSpeed; }
        /// <summary>�v���C���[�̍ő呬�x���擾</summary>
        public float GetMaxSpeed() { return maxSpeed; }
        /// <summary>�v���C���[�̃W�����v�͂��擾</summary>ga
        public float GetJumpForce() { return jumpForce; }
    }
}
