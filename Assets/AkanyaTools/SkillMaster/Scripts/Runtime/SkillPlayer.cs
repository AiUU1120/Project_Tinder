/*
* @Author: AiUU
* @Description: 技能播放器组件
* @AkanyaTech.SkillMaster
*/

using System;
using AkanyaTools.PlayableKami;
using AkanyaTools.SkillMaster.Scripts.Config;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Scripts.Runtime
{
    public sealed class SkillPlayer : MonoBehaviour
    {
        private AnimationController m_AnimationController;

        public bool isPlaying { get; private set; }

        private SkillConfig m_SkillConfig;

        private int m_CurFrameIndex;

        private float m_FrameRate;

        private float m_PlayTotalTime;

        private Action<Vector3, Quaternion> m_OnRootMotion;

        private Action m_OnSkillEnd;

        private void Update()
        {
            if (!isPlaying)
            {
                return;
            }

            m_PlayTotalTime += Time.deltaTime;
            var targetFrameIndex = (int) m_PlayTotalTime * m_FrameRate;
            // 追帧
            while (m_CurFrameIndex < targetFrameIndex)
            {
                TickSkill();
            }

            if (targetFrameIndex >= m_SkillConfig.frameCount)
            {
                isPlaying = false;
                m_SkillConfig = null;
                if (m_OnRootMotion != null)
                {
                    m_AnimationController.ClearOnRootMotion();
                }
                m_OnRootMotion = null;
                m_OnSkillEnd?.Invoke();
            }
        }

        public void Init(AnimationController animationController)
        {
            m_AnimationController = animationController;
        }

        /// <summary>
        /// 播放技能
        /// </summary>
        /// <param name="skillConfig">技能配置</param>
        /// <param name="skillEndAction">技能结束回调</param>
        /// <param name="rootMotionAction">根运动回调</param>
        public void PlaySkill(SkillConfig skillConfig, Action skillEndAction, Action<Vector3, Quaternion> rootMotionAction = null)
        {
            m_SkillConfig = skillConfig;
            m_OnSkillEnd = skillEndAction;
            m_OnRootMotion = rootMotionAction;
            m_CurFrameIndex = -1;
            m_FrameRate = skillConfig.frameRate;
            m_PlayTotalTime = 0;
            isPlaying = true;
            TickSkill();
        }

        /// <summary>
        /// 驱动技能
        /// </summary>
        private void TickSkill()
        {
            if (m_AnimationController == null)
            {
                Debug.LogError("SkillMaster: AnimationController is null!");
                return;
            }
            m_CurFrameIndex++;
            if (m_SkillConfig.skillAnimationData.frameData.TryGetValue(m_CurFrameIndex, out var frameData))
            {
                m_AnimationController.PlaySingleAnimation(frameData.animationClip, speed: 1f, blockSameAnim: false, mixingTime: frameData.transitionTime);
                if (frameData.applyRootMotion)
                {
                    m_AnimationController.SetOnRootMotion(m_OnRootMotion);
                }
                else
                {
                    m_AnimationController.ClearOnRootMotion();
                }
            }
        }
    }
}