using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public class EnemyRagdoll : MonoBehaviour
    {
        /// <summary>ルートボーン</summary>
        [SerializeField] private Transform m_rootBone;

        public void RagdollSetup(Transform rootBone)
        {
            CloneTransforms(rootBone, m_rootBone);
        }

        private void CloneTransforms(Transform root, Transform clone)
        {
            foreach (Transform child in root)
            {
                Transform cloneChild = clone.Find(child.name);
                if (cloneChild != null)
                {
                    cloneChild.position = child.position;
                    cloneChild.rotation = child.rotation;

                    CloneTransforms(child, cloneChild);
                }
            }
        }
    }
}
