/*
 * @Author: AiUU
 * @Description: 武士刀 Katana 轻攻击行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.GameCore.Enums;
using UnityEngine;

namespace GameCore.Skills.SkillBehaviour.Katana
{
    public class KatanaAttackLightBehaviour : PlayerSkillBehaviourBase
    {
        [SerializeField]
        private int m_ComboCount;

        private int m_CurAttackIndex = -1;

        private int m_LastAttackIndex = -1;

        protected override bool autoUpdateSlot => false;

        public override SkillBehaviourBase DeepCopy() => new KatanaAttackLightBehaviour() { m_ComboCount = m_ComboCount };

        public override void Release(bool calCDTimer = true)
        {
            base.Release(false);
            skillBrain.TryGetSkillShareData(PlayerSkillBrain.continuous_attack_mode_data_key, out bool canContinuousAttack);
            if (canContinuousAttack)
            {
                m_CurAttackIndex = m_LastAttackIndex;
            }
            m_CurAttackIndex++;
            if (m_CurAttackIndex > m_ComboCount - 1 || m_CurAttackIndex > skillConfig.clips.Length - 1)
            {
                m_CurAttackIndex = 0;
            }
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[m_CurAttackIndex]);
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            playerController.ChangeState(PlayerMotionState.Idle);
        }

        public override void OnSkillBehaviourSwitchOrClipEnd()
        {
            base.OnSkillBehaviourSwitchOrClipEnd();
            m_LastAttackIndex = m_CurAttackIndex;
            m_CurAttackIndex = -1;
            skillBrain.AddOrUpdateSkillShareData(PlayerSkillBrain.continuous_attack_mode_data_key, false);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            playerController.characterController.Move(new Vector3(deltaPosition.x, deltaPosition.y, deltaPosition.z));
            playerController.transform.rotation *= deltaRotation;
        }
    }
}