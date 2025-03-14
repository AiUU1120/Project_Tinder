/*
 * @Author: AiUU
 * @Description: UnityChan 轻攻击行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using GameCore.Data.Enums;
using GameCore.Skills.SkillBehaviour;
using UnityEngine;

namespace GameCore.Character.UnityChan.SkillBehaviour
{
    public class UnityChanAttackLightBehaviour : PlayerSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new UnityChanAttackLightBehaviour();

        public override void Release()
        {
            base.Release();
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[0]);
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            unityChanController.ChangeState(PlayerMotionState.Idle);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            unityChanController.characterController.Move(deltaPosition);
            unityChanController.transform.rotation *= deltaRotation;
        }
    }
}