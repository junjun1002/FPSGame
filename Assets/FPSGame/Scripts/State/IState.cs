using UnityEngine;

namespace FPS
{
    /// <summary>
    /// �W�F�l���b�N�^�iT�j�̕����ɃX�e�[�g�̃I�[�i�[���`����
    /// �p���̓X�e�[�g�̏�ԑ��Ōp������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IState<T> where T : MonoBehaviour
    {
        /// <summary>
        /// �X�e�[�g���J�n����u�ԂɌĂ΂��֐�
        /// </summary>
        /// <param name="owner"></param>
        void OnEnter(T owner);

        /// <summary>
        /// �X�e�[�g���I������u�ԂɌĂ΂��֐�
        /// </summary>
        /// <param name="owner"></param>
        void OnExit(T owner);
    }
}