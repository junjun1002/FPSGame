using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class ShortEnemy : EnemyBase
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Attack();
            }
        }

        protected override void Attack()
        {
            m_anim.SetTrigger("Punch");
        }
    }
}
