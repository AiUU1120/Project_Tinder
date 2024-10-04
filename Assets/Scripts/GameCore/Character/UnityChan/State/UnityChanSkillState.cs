/*
* @Author: AiUU
* @Description: UnityChan移动状态
* @AkanyaTech.Tinder
*/

using AkanyaTools.SkillMaster.Scripts.Config;
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
            m_SkillConfig = ResourceManager.LoadAsset<SkillConfig>("SkillConfig_Test");
            unityChanController.skillPlayer.PlaySkill(m_SkillConfig, OnSkillEnd, OnRootMotion);
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

        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            unityChanController.characterController.Move(deltaPosition);
            unityChanController.transform.rotation *= deltaRotation;
        }
    }
}