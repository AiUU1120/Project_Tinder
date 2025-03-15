/*
 * @Author: AiUU
 * @Description: UnityChan 重攻击行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.GameCore.Enums;
using GameCore.Skills.SkillBehaviour;
using UnityEngine;

namespace GameCore.Character.Player.SkillBehaviour
{
    public class UnityChanAttackHeavyBehaviour : PlayerSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new UnityChanAttackHeavyBehaviour();

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
            playerController.characterController.Move(new Vector3(deltaPosition.x, -9.8f * Time.deltaTime, deltaPosition.z));
            playerController.transform.rotation *= deltaRotation;
        }
    }
}