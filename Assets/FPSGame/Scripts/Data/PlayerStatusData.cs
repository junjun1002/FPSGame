using System;
using UnityEngine;

namespace FPS
{
    /// <summary>
    /// プレイヤーのステータスデータ
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "PlayerStatusData", menuName = "Data/PlayerStatus")]
    public class PlayerStatusData : ScriptableObject
    {
        // プレイヤーのステータス
        [SerializeField, Header("最大HP")] int maxHP = 100;
        [SerializeField, Header("攻撃力")] int power = 10;
        [SerializeField, Header("プレイヤーの最大弾数")] int maxBulletCount = 25;
        [SerializeField, Header("プレイヤーの移動速度")] float moveSpeed = 5.0f;
        [SerializeField, Header("プレイヤーの歩行速度")] float walkSpeed = 2.0f;
        [SerializeField, Header("プレイヤーの最大速度")] float maxSpeed = 10.0f;
        [SerializeField, Header("プレイヤーのジャンプ力")] float jumpForce = 5.0f;

        /// <summary>プレイヤーの最大HPを取得</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>プレイヤーの攻撃力を取得</summary>
        public int GetPower() { return power; }
        /// <summary>プレイヤーの最大弾数を取得</summary>
        public int GetMaxBulletCount() { return maxBulletCount; }
        /// <summary>プレイヤーの移動速度を取得</summary>
        public float GetMoveSpeed() { return moveSpeed; }
        /// <summary>プレイヤーの歩行速度を取得</summary>
        public float GetWalkSpeed() { return walkSpeed; }
        /// <summary>プレイヤーの最大速度を取得</summary>
        public float GetMaxSpeed() { return maxSpeed; }
        /// <summary>プレイヤーのジャンプ力を取得</summary>ga
        public float GetJumpForce() { return jumpForce; }
    }
}
