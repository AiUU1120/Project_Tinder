/*
* @Author: AiUU
* @Description: SkillMaster 动画帧事件
* @AkanyaTech.SkillMaster
*/

using UnityEngine;

namespace AkanyaTools.SkillMaster.Scripts.Event
{
    public sealed class SkillAnimationFrameEvent : SkillFrameEventBase
    {
        public AnimationClip animationClip;

        public bool applyRootMotion;

        public float transitionTime;

#if UNITY_EDITOR
        public int durationFrame;
#endif
    }
}