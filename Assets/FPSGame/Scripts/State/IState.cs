using UnityEngine;

namespace FPS
{
    /// <summary>
    /// GameManager��Enemy�̃X�e�[�g���Ǘ�����ėp�I�ȃX�e�[�g�}�V��
    /// �W�F�l���b�N�^�iT�j�̕����ɃX�e�[�g�̃I�[�i�[���`����
    /// �p���̓X�e�[�g�̏�ԑ��Ōp������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IState<T> where T : MonoBehaviour
    {
        /// <summary>
        /// �X�e�[�g���؂�ւ��u�ԂɌĂ΂��֐�
        /// </summary>
        /// <param name="owner"></param>
        void OnExecute(T owner);

        /// <summary>
        /// �X�e�[�g���I������u�ԂɌĂ΂��֐�
        /// </summary>
        /// <param name="owner"></param>
        void OnExit(T owner);
    }
}