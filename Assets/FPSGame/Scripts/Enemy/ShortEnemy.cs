namespace FPS
{
    /// <summary>
    /// 近距離の敵クラス
    /// </summary>
    public class ShortEnemy : EnemyBase
    {
        public void Update()
        {
            m_anim.SetFloat("Speed", m_agent.velocity.magnitude);
        }

        /// <summary>
        /// 攻撃
        /// </summary>
        public override void Attack()
        {
            m_anim.SetTrigger("Punch");
        }

        /// <summary>
        /// プレイヤーを追跡
        /// </summary>
        public override void Chasing()
        {
            m_agent.destination = m_player.transform.position;
            m_anim.SetFloat("Speed", m_agent.velocity.magnitude);
        }
    }
}
