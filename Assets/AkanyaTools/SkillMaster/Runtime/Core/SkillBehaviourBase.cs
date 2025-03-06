/*
 * @Author: AiUU
 * @Description: 技能行为基类
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Core
{
    public abstract class SkillBehaviourBase
    {
        protected PlayerControllerBase playerController;

        protected SkillConfig skillConfig;

        protected SkillBrainBase skillBrain;

        protected SkillPlayer skillPlayer;

        public abstract SkillBehaviourBase DeepCopy();

        public virtual void Init(PlayerControllerBase playerController, SkillConfig skillConfig, SkillBrainBase skillBrain, SkillPlayer skillPlayer)
        {
            this.playerController = playerController;
            this.skillConfig = skillConfig;
            this.skillBrain = skillBrain;
            this.skillPlayer = skillPlayer;
        }

        public virtual void Update()
        {
        }

        public virtual void Release()
        {
        }

        #region 技能驱动事件

        public virtual SkillCustomEventFrameEvent BeforeSkillCustomEventFrameEvent(SkillCustomEventFrameEvent customEventFrameEvent) => customEventFrameEvent;
        public virtual SkillAnimationFrameEvent BeforeSkillAnimationFrameEvent(SkillAnimationFrameEvent animationFrameEvent) => animationFrameEvent;
        public virtual SkillAudioFrameEvent BeforeSkillAudioFrameEvent(SkillAudioFrameEvent audioFrameEvent) => audioFrameEvent;
        public virtual SkillEffectFrameEvent BeforeSkillEffectFrameEvent(SkillEffectFrameEvent effectFrameEvent) => effectFrameEvent;
        public virtual SkillDetectionFrameEvent BeforeSkillDetectionFrameEvent(SkillDetectionFrameEvent detectionFrameEvent) => detectionFrameEvent;

        public virtual void OnTick(int frameIndex)
        {
        }

        public virtual void OnSkillClipEnd()
        {
        }

        public virtual void OnAttackDetection(Collider collider)
        {
        }

        public virtual void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
        }

        public virtual void AfterSkillCustomEventFrameEvent(SkillCustomEventFrameEvent customEventFrameEvent)
        {
        }

        public virtual void AfterSkillAnimationFrameEvent(SkillAnimationFrameEvent animationFrameEvent)
        {
        }

        public virtual void AfterSkillAudioFrameEvent(SkillAudioFrameEvent audioFrameEvent)
        {
        }

        public virtual void AfterSkillEffectFrameEvent(SkillEffectFrameEvent effectFrameEvent)
        {
        }

        public virtual void AfterSkillDetectionFrameEvent(SkillDetectionFrameEvent detectionFrameEvent)
        {
        }

        #endregion
    }
}