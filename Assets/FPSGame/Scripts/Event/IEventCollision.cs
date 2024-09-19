namespace FPS
{
    /// <summary>
    /// ゲーム中の衝突イベント
    /// </summary>
    public interface IEventCollision
    {
        void CollisionEvent(EventSystemInGame eventSystemInGame);
    }
}