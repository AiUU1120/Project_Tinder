/*
 * @Author: AiUU
 * @Description: 大剑技能 冲锋 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using UnityEngine;

namespace GameCore.Skills.SkillBehaviour.GreatSword
{
    public sealed class GreatSwordStrikeBehaviour : PlayerSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new GreatSwordStrikeBehaviour();

        public override void Release(bool calCDTimer = true)
        {
            base.Release(calCDTimer);
            m_CDTimer = GetCDTime();
            m_SkillPlayer.StartPlaySkillBehaviour(this);
            m_SkillPlayer.PlaySkillClip(m_SkillConfig.clips[0]);
        }

        public override bool CheckRelease() => m_CDTimer <= 0 && base.CheckRelease();

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            m_SkillOwner.ChangeToIdleState();
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            var pos = new Vector3(deltaPosition.x * 1.5f, deltaPosition.y, deltaPosition.z * 1.5f);
            m_SkillOwner.OnSkillMove(pos);
            m_SkillOwner.OnSkillRotate(deltaRotation);
        }
    }
}