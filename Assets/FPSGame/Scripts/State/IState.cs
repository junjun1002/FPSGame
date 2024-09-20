using UnityEngine;

namespace FPS
{
    /// <summary>
    /// GameManagerやEnemyのステートを管理する汎用的なステートマシン
    /// ジェネリック型（T）の部分にステートのオーナーを定義する
    /// 継承はステートの状態側で継承する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IState<T> where T : MonoBehaviour
    {
        /// <summary>
        /// ステートが切り替わる瞬間に呼ばれる関数
        /// </summary>
        /// <param name="owner"></param>
        void OnExecute(T owner);

        /// <summary>
        /// ステートが終了する瞬間に呼ばれる関数
        /// </summary>
        /// <param name="owner"></param>
        void OnExit(T owner);
    }
}