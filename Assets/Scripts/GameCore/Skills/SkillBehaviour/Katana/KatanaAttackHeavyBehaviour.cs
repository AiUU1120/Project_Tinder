/*
 * @Author: AiUU
 * @Description: 武士刀 Katana 重攻击行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using UnityEngine;

namespace GameCore.Skills.SkillBehaviour.Katana
{
    public class KatanaAttackHeavyBehaviour : PlayerSkillBehaviourBase
    {
        [SerializeField]
        private int m_ComboCount;

        private int m_CurAttackIndex = -1;

        private int m_LastAttackIndex = -1;

        protected override bool autoUpdateSlot => false;

        public override SkillBehaviourBase DeepCopy() => new KatanaAttackHeavyBehaviour() { m_ComboCount = m_ComboCount };

        public override void Release(bool calCDTimer = true)
        {
            base.Release(false);
            m_SkillBrain.TryGetSkillShareData(PlayerSkillBrain.continuous_attack_mode_data_key, out bool canContinuousAttack);
            if (canContinuousAttack)
            {
                m_CurAttackIndex = m_LastAttackIndex;
            }
            m_CurAttackIndex++;
            if (m_CurAttackIndex > m_ComboCount - 1 || m_CurAttackIndex > m_SkillConfig.clips.Length - 1)
            {
                m_CurAttackIndex = 0;
            }
            m_SkillPlayer.StartPlaySkillBehaviour(this);
            m_SkillPlayer.PlaySkillClip(m_SkillConfig.clips[m_CurAttackIndex]);
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            m_SkillOwner.ChangeToIdleState();
        }

        public override void OnSkillBehaviourSwitchOrClipEnd()
        {
            base.OnSkillBehaviourSwitchOrClipEnd();
            m_LastAttackIndex = m_CurAttackIndex;
            m_CurAttackIndex = -1;
            m_SkillBrain.AddOrUpdateSkillShareData(PlayerSkillBrain.continuous_attack_mode_data_key, false);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            m_SkillOwner.OnSkillMove(deltaPosition);
            m_SkillOwner.OnSkillRotate(deltaRotation);
        }
    }
}