/*
 * @Author: AiUU
 * @Description: UnityChan 普攻行为
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using Data.Enums.GameCore;
using UnityEngine;

namespace GameCore.Character.UnityChan.Behaviour
{
    public class UnityChanStdAttackBehaviour : SkillBehaviourBase
    {
        private UnityChanController m_UnityChanController;

        public override void Init(PlayerControllerBase playerController, SkillConfig skillConfig, SkillBrainBase skillBrain, SkillPlayer skillPlayer)
        {
            base.Init(playerController, skillConfig, skillBrain, skillPlayer);
            m_UnityChanController = playerController as UnityChanController;
        }

        public override SkillBehaviourBase DeepCopy()
        {
            return new UnityChanStdAttackBehaviour();
        }

        public override void Release()
        {
            skillPlayer.StartPlaySkillConfig(this);
            skillPlayer.PlaySkillClip(skillConfig.clips[0]);
        }

        public override void OnSkillClipEnd()
        {
            m_UnityChanController.ChangeState(PlayerMotionState.Idle);
        }

        // TODO: 没有实际技能行为
        public override void OnAttackDetection(Collider obj)
        {
            Debug.Log(obj.name);
        }

        public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            m_UnityChanController.characterController.Move(deltaPosition);
            m_UnityChanController.transform.rotation *= deltaRotation;
        }
    }
}