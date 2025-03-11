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
        protected float cdTime => skillConfig.cdTime;

        protected PlayerControllerBase playerController;

        protected SkillConfig skillConfig;

        protected SkillBrainBase skillBrain;

        protected SkillPlayer skillPlayer;

        protected bool canRotate;

        protected bool isPlaying;

        protected float cdTimer;

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
            UpdateCDTimer();
            RotateOnUpdate();
        }

        public virtual void UpdateCDTimer()
        {
            if (cdTime <= 0 || cdTimer <= 0)
            {
                return;
            }
            cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
        }

        /// <summary>
        /// 基类实现包含消耗代价
        /// </summary>
        public virtual void Release()
        {
            canRotate = false;
            isPlaying = true;
            skillBrain.SetCanReleaseFlag(false);
            ApplyCost();
        }

        public virtual void ApplyCost()
        {
            foreach (var cost in skillConfig.releaseCostDic)
            {
                skillBrain.ApplyCost(cost.Key, cost.Value);
            }
        }

        public virtual bool CheckRelease() => CheckReleaseCost();

        /// <summary>
        /// 检测技能消耗是否满足 默认实现遍历消耗字典
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckReleaseCost()
        {
            foreach (var cost in skillConfig.releaseCostDic)
            {
                if (!skillBrain.CheckCost(cost.Key, cost.Value))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void RotateOnUpdate()
        {
        }

        /// <summary>
        /// 技能行为切换时调用
        /// </summary>
        public virtual void OnSkillBehaviourSwitch()
        {
            OnSkillBehaviourSwitchOrClipEnd();
        }

        /// <summary>
        /// 技能片段播放完毕时调用
        /// 注意如果技能中途切换 该技能不会调用此方法
        /// </summary>
        public virtual void OnSkillClipEnd()
        {
            skillBrain.SetCanReleaseFlag(true);
            OnSkillBehaviourSwitchOrClipEnd();
        }

        /// <summary>
        /// OnSkillBehaviourSwitch 与 OnSkillClipEnd 的公共逻辑
        /// </summary>
        public virtual void OnSkillBehaviourSwitchOrClipEnd()
        {
            isPlaying = false;
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

        public virtual void OnAttackDetection(Collider collider)
        {
        }

        public virtual void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
        }

        public virtual void AfterSkillCustomEventFrameEvent(SkillCustomEventFrameEvent customEventFrameEvent)
        {
            switch (customEventFrameEvent.eventType)
            {
                case SkillEventType.UnFreezeRelease:
                    skillBrain.SetCanReleaseFlag(true);
                    break;
                case SkillEventType.LockRotation:
                    canRotate = false;
                    break;
                case SkillEventType.UnlockRotation:
                    canRotate = true;
                    break;
                case SkillEventType.Custom:
                    break;
            }
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