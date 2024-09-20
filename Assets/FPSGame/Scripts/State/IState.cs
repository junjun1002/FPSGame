using UnityEngine;

namespace FPS
{
    /// <summary>
    /// ジェネリック型（T）の部分にステートのオーナーを定義する
    /// 継承はステートの状態側で継承する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IState<T> where T : MonoBehaviour
    {
        /// <summary>
        /// ステートが開始する瞬間に呼ばれる関数
        /// </summary>
        /// <param name="owner"></param>
        void OnEnter(T owner);

        /// <summary>
        /// ステートが終了する瞬間に呼ばれる関数
        /// </summary>
        /// <param name="owner"></param>
        void OnExit(T owner);
    }
}