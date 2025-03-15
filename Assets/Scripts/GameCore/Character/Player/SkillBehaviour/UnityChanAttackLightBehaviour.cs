/*
 * @Author: AiUU
 * @Description: UnityChan 轻攻击行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.GameCore.Enums;
using GameCore.Skills.SkillBehaviour;
using UnityEngine;

namespace GameCore.Character.Player.SkillBehaviour
{
    public class UnityChanAttackLightBehaviour : PlayerSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new UnityChanAttackLightBehaviour();

        public override void Release(bool calCDTimer = true)
        {
            base.Release(calCDTimer);
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[0]);
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            playerController.ChangeState(PlayerMotionState.Idle);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            playerController.characterController.Move(deltaPosition);
            playerController.transform.rotation *= deltaRotation;
        }
    }
}