using System;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// 敵のステータスデータ
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "EnemyStatusData", menuName = "Data/EnemyStatus")]
    public class EnemyStatusData : ScriptableObject
    {
        // 敵のステータス
        [SerializeField, Header("最大HP")] int maxHP = 100;
        [SerializeField, Header("攻撃力")] int power = 10;
        [SerializeField, Header("巡回速度")] float patrolSpeed = 2.0f;
        [SerializeField, Header("追跡速度")] float chaseSpeed = 3.5f;


        /// <summary>最大HPを取得</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>攻撃力を取得</summary>
        public int GetPower() { return power; }
        /// <summary>巡回速度を取得</summary>
        public float GetPatrolSpeed() { return patrolSpeed; }
        /// <summary>追跡速度を取得</summary>
        public float GetChaseSpeed() { return chaseSpeed; }
    }
}
