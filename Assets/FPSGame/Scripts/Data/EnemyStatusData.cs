using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [Serializable, CreateAssetMenu(fileName = "EnemyStatusData", menuName = "Data/EnemyStatus")]
    public class EnemyStatusData : ScriptableObject
    {
        [SerializeField, Header("Å‘åHP")] int maxHP = 100;
        [SerializeField, Header("UŒ‚—Í")] int power = 10;
        [SerializeField, Header("ˆÚ“®‘¬“x")] float moveSpeed = 5.0f;

        /// <summary>Å‘åHP‚ğæ“¾</summary>
        public int GetMaxHP() { return maxHP; }
        /// <summary>UŒ‚—Í‚ğæ“¾</summary>
        public int GetPower() { return power; }
        /// <summary>ˆÚ“®‘¬“x‚ğæ“¾</summary>
        public float GetMoveSpeed() { return moveSpeed; }
    }
}
