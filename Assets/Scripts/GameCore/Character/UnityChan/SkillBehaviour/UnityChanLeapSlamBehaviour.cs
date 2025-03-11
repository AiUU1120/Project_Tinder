/*
 * @Author: AiUU
 * @Description: UnityChan 技能 飞跃重击 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.Enums.GameCore;
using GameCore.Skills;
using GameCore.Skills.SkillBehaviour;
using UnityEngine;

namespace GameCore.Character.UnityChan.SkillBehaviour
{
    public class UnityChanLeapSlamBehaviour : PlayerSkillBehaviourBase
    {
        private readonly float m_StandingTime = 5;

        private int m_CurAttackIndex = -1;

        public override SkillBehaviourBase DeepCopy() => new UnityChanLeapSlamBehaviour() { };

        public override void Release()
        {
            base.Release();
            m_CurAttackIndex++;
            if (m_CurAttackIndex == skillConfig.clips.Length - 1)
            {
                cdTimer = cdTime;
            }
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[m_CurAttackIndex]);
            skillBrain.AddOrUpdateSkillShareData(PlayerSkillBrain.continuous_attack_mode_data_key, true);
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
            return checkCd && base.CheckRelease();
        }

        public override void Update()
        {
            base.Update();
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
                    cdTimer = cdTime;
                    m_CurAttackIndex = -1;
                }
            }
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            unityChanController.ChangeState(PlayerMotionState.Idle);
        }

        // TODO: 暂时没有实际行为
        public override void OnAttackDetection(Collider obj)
        {
            Debug.Log(obj.name);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            unityChanController.characterController.Move(deltaPosition);
            unityChanController.transform.rotation *= deltaRotation;
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