/*
 * @Author: AiUU
 * @Description: UnityChan移动状态
 * @AkanyaTech.Tinder
 */

using System;
using Common.System;
using Data.GameCore.Enums;
using FrameTools.StateMachine;
using UnityEngine;

namespace GameCore.Character.Player.State
{
    public sealed class PlayerMoveState : PlayerStateBase
    {
        private bool m_ApplyRootMotion;

        #region 生命周期

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            m_ApplyRootMotion = m_PlayerController.weaponConfig.applyRootMotion;
        }

        public override void Enter()
        {
            base.Enter();
            // if (isPass)
            // {
            //     return;
            // }
            m_PlayerController.AddAnimationEvent("FootStep", OnFootStep);
            Action<Vector3, Quaternion> onRootMotion = m_ApplyRootMotion ? OnRootMotion : null;
            m_PlayerController.PlayBlendAnimation("Walk", "Run", onRootMotion: onRootMotion);
        }

        public override void Update()
        {
            base.Update();
            if (CheckStateChange())
            {
                return;
            }
            m_PlayerController.SetBlendAnimationWeight(InputManager.instance.moveInput.magnitude);
            Rotate();
            if (!m_ApplyRootMotion)
            {
                Move();
            }
        }

        public override void Exit()
        {
            base.Exit();
            // if (isPass)
            // {
            //     return;
            // }
            m_PlayerController.ClearOnRootMotion();
            m_PlayerController.RemoveAnimationEvent("FootStep", OnFootStep);
        }

        #endregion

        protected override bool CheckStateChange()
        {
            // 输入值接近 0 时回到 idle 状态
            if (InputManager.instance.moveInput.magnitude <= 0.1f)
            {
                m_PlayerController.ChangeState(PlayerMotionState.Idle);
                return true;
            }
            if (InputManager.instance.isDashing)
            {
                m_PlayerController.ChangeState(PlayerMotionState.Dash);
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

        /// <summary>
        /// 移动角色 (不启用根运动)
        /// </summary>
        private void Move()
        {
            var move = m_PlayerController.moveSpeed * Time.deltaTime * m_PlayerController.playerMoveDir;
            move.y = -9.8f;
            m_PlayerController.characterController.Move(move);
        }

        /// <summary>
        /// 旋转角色
        /// </summary>
        private void Rotate()
        {
            m_PlayerController.Rotate();
        }

        /// <summary>
        /// 根运动回调
        /// </summary>
        /// <param name="deltaPosition"></param>
        /// <param name="deltaRotation"></param>
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y = -9.8f * Time.deltaTime;
            m_PlayerController.characterController.Move(deltaPosition);
        }
    }
}