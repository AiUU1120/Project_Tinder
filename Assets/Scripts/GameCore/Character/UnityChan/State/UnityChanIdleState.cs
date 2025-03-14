/*
* @Author: AiUU
* @Description: UnityChan站立状态
* @AkanyaTech.Tinder
*/

using Common.System;
using GameCore.Data.Enums;
using UnityEngine;

namespace GameCore.Character.UnityChan.State
{
    public sealed class UnityChanIdleState : UnityChanStateBase
    {
        public override void Enter()
        {
            base.Enter();
            // 播放待机动作
            // if (isPass)
            // {
            //     return;
            // }
            unityChanController.PlayAnimation("Idle", mixingTime: 0.3f);
        }

        public override void Update()
        {
            base.Update();
            if (CheckStateChange())
            {
                return;
            }
            unityChanController.characterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        }

        public override void Exit()
        {
            base.Exit();
            // if (isPass)
            // {
            //     return;
            // }
        }

        protected override bool CheckStateChange()
        {
            if (InputManager.instance.moveInput.magnitude >= 0.1f)
            {
                unityChanController.ChangeState(PlayerMotionState.Move);
                return true;
            }
            if (CheckAndEnterSkillState())
            {
                unityChanController.ChangeState(PlayerMotionState.Skill);
                return true;
            }
            return false;
        }
    }
}