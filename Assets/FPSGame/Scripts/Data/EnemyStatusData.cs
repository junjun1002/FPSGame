using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [Serializable, CreateAssetMenu(fileName = "EnemyStatusData", menuName = "Data/EnemyStatus")]
    public class EnemyStatusData : ScriptableObject
    {
        [SerializeField, Header("�ő�HP")] int maxHP = 100;
        [SerializeField, Header("�U����")] int power = 10;
        [SerializeField, Header("�ړ����x")] float moveSpeed = 5.0f;

        /// <summary>�ő�HP���擾</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>�U���͂��擾</summary>
        public int GetPower() { return power; }
        /// <summary>�ړ����x���擾</summary>
        public float GetMoveSpeed() { return moveSpeed; }
    }
}
