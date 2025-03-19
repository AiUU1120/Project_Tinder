/*
 * @Author: AiUU
 * @Description: 大剑技能 毁灭一击 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using UnityEngine;

namespace GameCore.Skills.SkillBehaviour.GreatSword
{
    public class GreatSwordSmashBehaviour : PlayerSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new GreatSwordSmashBehaviour();

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
            m_SkillOwner.OnSkillMove(deltaPosition);
            m_SkillOwner.OnSkillRotate(deltaRotation);
        }
    }
}