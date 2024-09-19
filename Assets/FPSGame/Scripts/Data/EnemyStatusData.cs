using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [Serializable, CreateAssetMenu(fileName = "EnemyStatusData", menuName = "Data/EnemyStatus")]
    public class EnemyStatusData : ScriptableObject
    {
        [SerializeField, Header("最大HP")] int maxHP = 100;
        [SerializeField, Header("攻撃力")] int power = 10;
        [SerializeField, Header("移動速度")] float moveSpeed = 5.0f;

        /// <summary>最大HPを取得</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>攻撃力を取得</summary>
        public int GetPower() { return power; }
        /// <summary>移動速度を取得</summary>
        public float GetMoveSpeed() { return moveSpeed; }
    }
}
