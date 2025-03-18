/*
 * @Author: AiUU
 * @Description: 技能行为基类
 * @AkanyaTech.SkillMaster
 */

using System;
using System.Collections.Generic;
using AkanyaTools.AudioSystem;
using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using JKFrame;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AkanyaTools.SkillMaster.Runtime.Core
{
    [Serializable]
    public abstract class SkillBehaviourBase
    {
        public int skillIndex { get; protected set; }

        public SkillConfig skillConfig => m_SkillConfig;

        protected ISkillCharacter m_SkillOwner;

        protected SkillConfig m_SkillConfig;

        protected SkillBrainBase m_SkillBrain;

        protected SkillPlayer m_SkillPlayer;

        protected bool m_CanRotate;

        protected bool m_IsPlaying;

        protected float m_CDTimer;

        private HashSet<IHitTarget> m_HitTargets = new();

        public abstract SkillBehaviourBase DeepCopy();

        public virtual void Init(ISkillCharacter skillOwner, SkillConfig skillConfig, SkillBrainBase skillBrain, SkillPlayer skillPlayer, int skillIndex = -1)
        {
            m_SkillOwner = skillOwner;
            m_SkillConfig = skillConfig;
            m_SkillBrain = skillBrain;
            m_SkillPlayer = skillPlayer;
            this.skillIndex = skillIndex;
        }

        public virtual void Update()
        {
            UpdateCDTimer();
            RotateOnUpdate();
        }

        public virtual void UpdateCDTimer()
        {
            if (GetCDTime() <= 0 || m_CDTimer <= 0)
            {
                return;
            }
            m_CDTimer = Mathf.Clamp(m_CDTimer - Time.deltaTime, 0, float.MaxValue);
        }

        /// <summary>
        /// 基类实现包含消耗代价
        /// </summary>
        public virtual void Release(bool calCDTimer = true)
        {
            if (calCDTimer)
            {
                m_CDTimer = GetCDTime();
            }
            m_HitTargets.Clear();
            m_CanRotate = false;
            m_IsPlaying = true;
            m_SkillBrain.SetCanReleaseFlag(false);
            ApplyCost();
        }

        public virtual void ApplyCost()
        {
            foreach (var cost in m_SkillConfig.releaseCostDic)
            {
                m_SkillBrain.ApplyCost(cost.Key, cost.Value);
            }
        }

        public virtual bool CheckRelease() => CheckReleaseCost() && CheckCD();

        public virtual bool CheckCD() => m_CDTimer <= 0;

        /// <summary>
        /// 检测技能消耗是否满足 默认实现遍历消耗字典
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckReleaseCost()
        {
            foreach (var cost in m_SkillConfig.releaseCostDic)
            {
                if (!m_SkillBrain.CheckCost(cost.Key, cost.Value))
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
            m_SkillBrain.SetCanReleaseFlag(true);
            OnSkillBehaviourSwitchOrClipEnd();
        }

        /// <summary>
        /// OnSkillBehaviourSwitch 与 OnSkillClipEnd 的公共逻辑
        /// </summary>
        public virtual void OnSkillBehaviourSwitchOrClipEnd()
        {
            m_IsPlaying = false;
            m_HitTargets.Clear();
        }

        public virtual float GetCDTime() => m_SkillConfig.baseCD;

        #region 技能驱动事件

        public virtual SkillCustomEventFrameEvent BeforeSkillCustomEventFrameEvent(SkillCustomEventFrameEvent customEventFrameEvent) => customEventFrameEvent;
        public virtual SkillAnimationFrameEvent BeforeSkillAnimationFrameEvent(SkillAnimationFrameEvent animationFrameEvent) => animationFrameEvent;
        public virtual SkillAudioFrameEvent BeforeSkillAudioFrameEvent(SkillAudioFrameEvent audioFrameEvent) => audioFrameEvent;
        public virtual SkillEffectFrameEvent BeforeSkillEffectFrameEvent(SkillEffectFrameEvent effectFrameEvent) => effectFrameEvent;
        public virtual SkillDetectionFrameEvent BeforeSkillDetectionFrameEvent(SkillDetectionFrameEvent detectionFrameEvent) => detectionFrameEvent;

        public virtual void OnTick(int frameIndex)
        {
        }

        public virtual void OnAttackDetection(IHitTarget hitTarget, AttackData attackData)
        {
            // 防止重复命中
            if (m_HitTargets.Add(hitTarget))
            {
                OnHitTarget(hitTarget, attackData);
            }
        }

        public virtual void OnHitTarget(IHitTarget hitTarget, AttackData attackData)
        {
            if (attackData.e.attackHitConfig != null)
            {
                PlayHitEffect(attackData);
            }
            hitTarget.BeHit(attackData);
        }

        private void PlayHitEffect(AttackData attackData)
        {
            var hitConfig = attackData.e.attackHitConfig;
            if (hitConfig.hitEffectPrefab != null)
            {
                var hitEffect = PoolSystem.GetGameObject(hitConfig.hitEffectPrefab.name);
                if (hitEffect == null)
                {
                    hitEffect = Object.Instantiate(hitConfig.hitEffectPrefab);
                }
                hitEffect.transform.position = attackData.hitPoint;
                hitEffect.transform.rotation = Quaternion.LookRotation(attackData.hitNormal);
                hitEffect.GetComponent<EffectController>().Init();
            }
            if (hitConfig.hitAudioClip != null)
            {
                AudioManager.PlayOneShot(hitConfig.hitAudioClip, attackData.hitPoint);
            }
        }

        public virtual void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
        }

        public virtual void AfterSkillCustomEventFrameEvent(SkillCustomEventFrameEvent customEventFrameEvent)
        {
            switch (customEventFrameEvent.eventType)
            {
                case SkillEventType.UnFreezeRelease:
                    m_SkillBrain.SetCanReleaseFlag(true);
                    break;
                case SkillEventType.LockRotation:
                    m_CanRotate = false;
                    break;
                case SkillEventType.UnlockRotation:
                    m_CanRotate = true;
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