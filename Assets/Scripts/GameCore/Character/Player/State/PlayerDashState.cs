/*
 * @Author: AiUU
 * @Description: UnityChan冲刺状态
 * @AkanyaTech.Tinder
 */

using System;
using Common.System;
using Data.GameCore.Enums;
using FrameTools.StateMachine;
using UnityEngine;

namespace GameCore.Character.Player.State
{
    public sealed class PlayerDashState : PlayerStateBase
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
            m_PlayerController.PlayAnimation("Dash", onRootMotion: onRootMotion);
        }

        public override void Update()
        {
            base.Update();
            if (CheckStateChange())
            {
                return;
            }
            if (!m_ApplyRootMotion)
            {
                Move();
            }
            Rotate();
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
            if (!InputManager.instance.isDashing)
            {
                m_PlayerController.ChangeState(PlayerMotionState.Move);
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
            var move = m_PlayerController.dashSpeed * Time.deltaTime * m_PlayerController.playerMoveDir;
            move.y = -9.8f;
            m_PlayerController.characterController.Move(move);
        }

        /// <summary>
        /// 旋转角色（待优化）
        /// </summary>
        private void Rotate()
        {
            // var playerMoveDir = unityChanController.playerMoveDir;
            // if (playerMoveDir.magnitude == 0)
            // {
            //     return;
            // }
            // 先快后慢旋转
            // 转换到玩家本地坐标系
            // m_PlayerMoveDir = transform.InverseTransformDirection(m_PlayerMoveDir).normalized;
            // var rad = Mathf.Atan2(m_PlayerMoveDir.x, m_PlayerMoveDir.z);
            // transform.Rotate(0, rad * 200 * Time.deltaTime, 0);

            // 匀速旋转
            // unityChanController.transform.rotation = Quaternion.Slerp(unityChanController.transform.rotation, Quaternion.LookRotation(playerMoveDir), Time.deltaTime * unityChanController.turnSpeed);
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