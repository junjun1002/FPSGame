namespace FPS
{
    /// <summary>
    /// �ߋ����̓G�N���X
    /// </summary>
    public class ShortEnemy : EnemyBase
    {
        public void Update()
        {
            m_anim.SetFloat("Speed", m_agent.velocity.magnitude);
        }

        /// <summary>
        /// �U��
        /// </summary>
        public override void Attack()
        {
            m_anim.SetTrigger("Punch");
        }

        /// <summary>
        /// �v���C���[��ǐ�
        /// </summary>
        public override void Chasing()
        {
            m_agent.destination = m_player.transform.position;
            m_anim.SetFloat("Speed", m_agent.velocity.magnitude);
        }
    }
}
