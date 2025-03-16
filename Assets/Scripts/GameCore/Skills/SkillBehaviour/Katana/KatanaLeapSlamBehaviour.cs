/*
 * @Author: AiUU
 * @Description: UnityChan 技能 飞跃重击 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
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
            cdTimer = m_CurAttackIndex == skillConfig.clips.Length - 1 ? GetCDTime() : m_StandingTime;
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[m_CurAttackIndex]);
            // skillBrain.AddOrUpdateSkillShareData(PlayerSkillBrain.continuous_attack_mode_data_key, true);
        }

        public override bool CheckRelease()
        {
            var checkCd = true;
            if (m_CurAttackIndex == -1)
            {
                checkCd = cdTimer <= 0;
            }
            else if (m_CurAttackIndex == skillConfig.clips.Length - 1)
            {
                checkCd = cdTimer <= 0;
            }
            return checkCd && base.CheckReleaseCost();
        }

        public override void UpdateCDTimer()
        {
            if (isPlaying)
            {
                // 技能播放到最后一段
                if (m_CurAttackIndex == skillConfig.clips.Length - 1)
                {
                    cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
                }
                return;
            }
            cdTimer = Mathf.Clamp(cdTimer - Time.deltaTime, 0, float.MaxValue);
            if (m_CurAttackIndex != -1)
            {
                if (cdTimer <= 0)
                {
                    cdTimer = GetCDTime();
                    m_CurAttackIndex = -1;
                }
            }
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            playerController.ChangeState(PlayerMotionState.Idle);
        }

        // TODO: 暂时没有实际行为
        public override void OnAttackDetection(Collider obj)
        {
            Debug.Log(obj.name);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            playerController.characterController.Move(deltaPosition);
            playerController.transform.rotation *= deltaRotation;
        }

        public override void OnSkillBehaviourSwitchOrClipEnd()
        {
            base.OnSkillBehaviourSwitchOrClipEnd();
            if (m_CurAttackIndex == skillConfig.clips.Length - 1)
            {
                m_CurAttackIndex = -1;
            }
            else
            {
                cdTimer = m_StandingTime;
            }
        }
    }
}