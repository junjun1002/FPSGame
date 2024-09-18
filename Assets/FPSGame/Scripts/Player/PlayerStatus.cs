using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] PlayerStatusData playerStatusData = default;

        public PlayerStatusData GetPlayerStatusData() { return playerStatusData; }
    }
}
