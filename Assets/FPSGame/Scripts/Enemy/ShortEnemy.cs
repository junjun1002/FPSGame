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

            if (!m_agent.isStopped)
            {
                MoveToPlayer();
            }
        }

        protected override void Attack()
        {
            m_anim.SetTrigger("Punch");
        }

        protected override void MoveToPlayer()
        {
            Debug.Log(m_player.transform.position);
            m_agent.destination = m_player.transform.position;
            m_anim.SetFloat("Speed", m_agent.velocity.magnitude);
        }
    }
}
