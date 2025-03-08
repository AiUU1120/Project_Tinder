/*
 * @Author: AiUU
 * @Description: UnityChan 技能 飞跃重击 行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.Enums.GameCore;
using UnityEngine;

namespace GameCore.Character.UnityChan.SkillBehaviour
{
    public class UnityChanLeapSlamBehaviour : UnityChanSkillBehaviourBase
    {
        public float cdTime = 10;

        protected float cdTimer;

        public override SkillBehaviourBase DeepCopy() => new UnityChanLeapSlamBehaviour() { cdTime = cdTime };

        public override void Release()
        {
            base.Release();
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[0]);
            cdTimer = cdTime;
        }

        public override bool CheckRelease() => cdTimer <= 0 && base.CheckRelease();

        public override void Update()
        {
            base.Update();
            cdTimer -= Time.deltaTime;
            if (cdTimer < 0)
            {
                cdTimer = 0;
            }
        }

        public override void OnSkillClipEnd()
        {
            unityChanController.ChangeState(PlayerMotionState.Idle);
        }

        // TODO: 暂时没有实际行为
        public override void OnAttackDetection(Collider obj)
        {
            Debug.Log(obj.name);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            unityChanController.characterController.Move(deltaPosition);
            unityChanController.transform.rotation *= deltaRotation;
        }
    }
}