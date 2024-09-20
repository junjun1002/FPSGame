using System;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �G�̃X�e�[�^�X�f�[�^
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "EnemyStatusData", menuName = "Data/EnemyStatus")]
    public class EnemyStatusData : ScriptableObject
    {
        // �G�̃X�e�[�^�X
        [SerializeField, Header("�ő�HP")] int maxHP = 100;
        [SerializeField, Header("�U����")] int power = 10;
        [SerializeField, Header("���񑬓x")] float patrolSpeed = 2.0f;
        [SerializeField, Header("�ǐՑ��x")] float chaseSpeed = 3.5f;


        /// <summary>�ő�HP���擾</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>�U���͂��擾</summary>
        public int GetPower() { return power; }
        /// <summary>���񑬓x���擾</summary>
        public float GetPatrolSpeed() { return patrolSpeed; }
        /// <summary>�ǐՑ��x���擾</summary>
        public float GetChaseSpeed() { return chaseSpeed; }
    }
}
