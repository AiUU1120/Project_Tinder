/*
 * @Author: AiUU
 * @Description: UnityChan 技能 飞跃重击 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data;
using Data.GameCore.Enums;
using UnityEngine;

namespace GameCore.Skills.SkillBehaviour.Katana
{
    public class KatanaLeapSlamBehaviour : PlayerSkillBehaviourBase
    {
        private readonly float m_StandingTime = 5;

        private int m_CurAttackIndex = -1;

        public override SkillBehaviourBase DeepCopy() => new KatanaLeapSlamBehaviour();

        public override void Release(bool calCDTimer = true)
        {
            base.Release(false);
            m_CurAttackIndex++;
            m_CDTimer = m_CurAttackIndex == m_SkillConfig.clips.Length - 1 ? GetCDTime() : m_StandingTime;
            m_SkillPlayer.StartPlaySkillBehaviour(this);
            m_SkillPlayer.PlaySkillClip(m_SkillConfig.clips[m_CurAttackIndex]);
            // skillBrain.AddOrUpdateSkillShareData(PlayerSkillBrain.continuous_attack_mode_data_key, true);
        }

        public override bool CheckRelease()
        {
            var checkCd = true;
            if (m_CurAttackIndex == -1)
            {
                checkCd = m_CDTimer <= 0;
            }
            else if (m_CurAttackIndex == m_SkillConfig.clips.Length - 1)
            {
                checkCd = m_CDTimer <= 0;
            }
            return checkCd && base.CheckReleaseCost();
        }

        public override void UpdateCDTimer()
        {
            if (m_IsPlaying)
            {
                // 技能播放到最后一段
                if (m_CurAttackIndex == m_SkillConfig.clips.Length - 1)
                {
                    m_CDTimer = Mathf.Clamp(m_CDTimer - Time.deltaTime, 0, float.MaxValue);
                }
                return;
            }
            m_CDTimer = Mathf.Clamp(m_CDTimer - Time.deltaTime, 0, float.MaxValue);
            if (m_CurAttackIndex != -1)
            {
                if (m_CDTimer <= 0)
                {
                    m_CDTimer = GetCDTime();
                    m_CurAttackIndex = -1;
                }
            }
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            m_SkillOwner.ChangeToIdleState();
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            m_SkillOwner.OnSkillMove(deltaPosition);
            m_SkillOwner.OnSkillRotate(deltaRotation);
        }

        public override void OnSkillBehaviourSwitchOrClipEnd()
        {
            base.OnSkillBehaviourSwitchOrClipEnd();
            if (m_CurAttackIndex == m_SkillConfig.clips.Length - 1)
            {
                m_CurAttackIndex = -1;
            }
            else
            {
                m_CDTimer = m_StandingTime;
            }
        }
    }
}