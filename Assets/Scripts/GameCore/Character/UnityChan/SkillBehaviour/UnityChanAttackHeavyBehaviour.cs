/*
 * @Author: AiUU
 * @Description: UnityChan 重攻击行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.Enums.GameCore;
using UnityEngine;

namespace GameCore.Character.UnityChan.SkillBehaviour
{
    public class UnityChanAttackHeavyBehaviour : UnityChanSkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new UnityChanAttackHeavyBehaviour();

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
            unityChanController.characterController.Move(new Vector3(deltaPosition.x, -9.8f * Time.deltaTime, deltaPosition.z));
            unityChanController.transform.rotation *= deltaRotation;
        }
    }
}