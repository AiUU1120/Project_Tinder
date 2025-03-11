/*
 * @Author: AiUU
 * @Description: IK 控制 PlayableBehaviour
 * @AkanyaTech.PlayableKami
 */

using UnityEngine;
using UnityEngine.Playables;

namespace AkanyaTools.PlayableKami.CustomPlayableBehaviour
{
    public sealed class IKPlayableBehaviour : PlayableBehaviour
    {
        private Animator m_Animator;

        private Transform m_TargetLHand;

        private Transform m_TargetRHand;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="targetLHand"></param>
        /// <param name="targetRHand"></param>
        public void Init(Animator animator, Transform targetLHand = null, Transform targetRHand = null)
        {
            m_Animator = animator;
            m_TargetLHand = targetLHand;
            m_TargetRHand = targetRHand;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Debug.Log("IKPlayableBehaviour ProcessFrame");
            // if (m_Animator == null)
            // {
            //     Debug.LogWarning("IKPlayableBehaviour animator is null");
            //     return;
            // }
            // if (m_TargetLHand != null)
            // {
            //     m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            //     m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            // }
            // if (m_TargetRHand != null)
            // {
            //     m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            //     m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            // }
            // m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            // m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            // m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            // m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        }

        /// <summary>
        /// 设置 IK 目标
        /// </summary>
        /// <param name="targetL"></param>
        /// <param name="targetR"></param>
        public void SetHandIKTarget(Transform targetL = null, Transform targetR = null)
        {
            m_TargetLHand = targetL;
            m_TargetRHand = targetR;
        }

        /// <summary>
        /// 重置 IK 目标， 将IK Target 置空
        /// </summary>
        public void ResetHandIKTarget()
        {
            m_TargetLHand = null;
            m_TargetRHand = null;
        }
    }
}