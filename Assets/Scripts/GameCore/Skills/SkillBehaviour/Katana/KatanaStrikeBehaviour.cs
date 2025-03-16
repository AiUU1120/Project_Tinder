/*
 * @Author: AiUU
 * @Description: UnityChan 技能 突袭 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.GameCore.Enums;
using UnityEngine;

namespace GameCore.Skills.SkillBehaviour.Katana
{
    public class KatanaStrikeBehaviour : PlayerSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new KatanaStrikeBehaviour();

        public override void Release(bool calCDTimer = true)
        {
            base.Release(calCDTimer);
            cdTimer = GetCDTime();
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[0]);
        }

        public override bool CheckRelease() => cdTimer <= 0 && base.CheckRelease();

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
            playerController.characterController.Move(new Vector3(deltaPosition.x * 2.5f, deltaPosition.y, deltaPosition.z * 2.5f));
            playerController.transform.rotation *= deltaRotation;
        }
    }
}