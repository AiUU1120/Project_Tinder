/*
* @Author: AiUU
* @Description: UnityChan站立状态
* @AkanyaTech.Tinder
*/

using Common.System;
using Data.GameCore.Enums;
using UnityEngine;

namespace GameCore.Character.Player.State
{
    public sealed class PlayerIdleState : PlayerStateBase
    {
        public override void Enter()
        {
            base.Enter();
            // 播放待机动作
            // if (isPass)
            // {
            //     return;
            // }
            m_PlayerController.PlayAnimation("Idle", mixingTime: 0.3f);
        }

        public override void Update()
        {
            base.Update();
            if (CheckStateChange())
            {
                return;
            }
            m_PlayerController.characterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
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
                m_PlayerController.ChangeState(PlayerMotionState.Move);
                return true;
            }
            if (InputManager.instance.isDodge)
            {
                m_PlayerController.ChangeState(PlayerMotionState.Dodge);
                return true;
            }
            if (CheckAndEnterSkillState())
            {
                m_PlayerController.ChangeState(PlayerMotionState.Skill);
                return true;
            }
            return false;
        }
    }
}