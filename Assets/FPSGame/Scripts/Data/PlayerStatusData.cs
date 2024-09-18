using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [Serializable, CreateAssetMenu(fileName = "PlayerStatusData", menuName = "Data/PlayerStatus")]
    public class PlayerStatusData : ScriptableObject
    {
        [SerializeField, Header("最大HP")] int maxHP = 100;
        [SerializeField, Header("プレイヤーの最大弾数")] int maxAmount = 25;
        [SerializeField, Header("プレイヤーの移動速度")] float moveSpeed = 5.0f;
        [SerializeField, Header("プレイヤーの歩行速度")] float walkSpeed = 2.0f;
        [SerializeField, Header("プレイヤーの最大速度")] float maxSpeed = 10.0f;
        [SerializeField, Header("プレイヤーのジャンプ力")] float jumpForce = 5.0f;

        /// <summary>プレイヤーの最大HPを取得</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>プレイヤーの最大弾数を取得</summary>
        public int GetMaxAmount() { return maxAmount; }
        /// <summary>プレイヤーの移動速度を取得</summary>
        public float GetMoveSpeed() { return moveSpeed; }
        /// <summary>プレイヤーの歩行速度を取得</summary>
        public float GetWalkSpeed() { return walkSpeed; }
        /// <summary>プレイヤーの最大速度を取得</summary>
        public float GetMaxSpeed() { return maxSpeed; }
        /// <summary>プレイヤーのジャンプ力を取得</summary>
        public float GetJumpForce() { return jumpForce; }
    }
}
