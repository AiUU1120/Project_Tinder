/*
 * @Author: AiUU
 * @Description: UnityChan移动状态
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Data.Config;
using Data.Enums.GameCore;
using FrameTools.ResourceSystem;
using UnityEngine;

namespace GameCore.Character.UnityChan.State
{
    public sealed class UnityChanSkillState : UnityChanStateBase
    {
        private SkillConfig m_SkillConfig;

        public override void Enter()
        {
            base.Enter();
            if (isPass)
            {
                return;
            }
            m_SkillConfig = ResourceManager.LoadAsset<SkillConfig>("SkillConfig_Test 1");
            unityChanController.skillPlayer.PlaySkill(m_SkillConfig, OnSkillEnd, OnWeaponDetection, OnRootMotion);
        }

        public override void Update()
        {
            base.Update();
            if (CheckStateChange())
            {
                return;
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (isPass)
            {
                return;
            }
        }

        protected override bool CheckStateChange()
        {
            return false;
        }

        private void OnSkillEnd()
        {
            unityChanController.ChangeState(PlayerMotionState.Idle);
        }

        // TODO: 没有实际技能行为
        private void OnWeaponDetection(Collider obj)
        {
            Debug.Log(obj.name);
        }

        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            unityChanController.characterController.Move(deltaPosition);
            unityChanController.transform.rotation *= deltaRotation;
        }
    }
}