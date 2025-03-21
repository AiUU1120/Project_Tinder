/*
 * @Author: AiUU
 * @Description: Katana 技能 突袭 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using UnityEngine;

namespace GameCore.Character.Player.Skills.SkillBehaviour.Katana
{
    public class KatanaStrikeBehaviour : PlayerSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new KatanaStrikeBehaviour();

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
            var pos = new Vector3(deltaPosition.x * 2.5f, deltaPosition.y, deltaPosition.z * 2.5f);
            m_SkillOwner.OnSkillMove(pos);
            m_SkillOwner.OnSkillRotate(deltaRotation);
        }
    }
}