/*
* @Author: AiUU
* @Description: UnityChan冲刺状态
* @AkanyaTech.Tinder
*/

using System;
using AkanyaTools.AudioSystem;
using Data.Enums.GameCore;
using FrameTools.AudioSystem;
using FrameTools.StateMachine;
using UnityEngine;

namespace GameCore.Character.UnityChan.State
{
    public sealed class UnityChanDashState : UnityChanStateBase
    {
        private bool m_ApplyRootMotion;

        #region 生命周期

        public override void Init(IStateMachineOwner owner)
        {
            base.Init(owner);
            m_ApplyRootMotion = unityChanController.characterConfig.applyRootMotion;
        }

        public override void Enter()
        {
            base.Enter();
            if (isPass)
            {
                return;
            }
            unityChanController.AddAnimationEvent("FootStep", OnFootStep);
            Action<Vector3, Quaternion> onRootMotion = m_ApplyRootMotion ? OnRootMotion : null;
            unityChanController.PlayAnimation("Dash", onRootMotion: onRootMotion);
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
            if (isPass)
            {
                return;
            }
            unityChanController.ClearOnRootMotion();
            unityChanController.RemoveAnimationEvent("FootStep", OnFootStep);
        }

        #endregion

        protected override bool CheckStateChange()
        {
            // 输入值接近 0 时回到 idle 状态
            if (unityChanController.input.moveInput.magnitude <= 0.1f)
            {
                unityChanController.ChangeState(PlayerMotionState.Idle);
                return true;
            }
            if (!unityChanController.input.isDashing)
            {
                unityChanController.ChangeState(PlayerMotionState.Move);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移动角色 (不启用根运动)
        /// </summary>
        private void Move()
        {
            var move = unityChanController.dashSpeed * Time.deltaTime * unityChanController.playerMoveDir;
            move.y = -9.8f;
            unityChanController.characterController.Move(move);
        }

        /// <summary>
        /// 旋转角色（待优化）
        /// </summary>
        private void Rotate()
        {
            var playerMoveDir = unityChanController.playerMoveDir;
            if (playerMoveDir.magnitude == 0)
            {
                return;
            }
            // 先快后慢旋转
            // 转换到玩家本地坐标系
            // m_PlayerMoveDir = transform.InverseTransformDirection(m_PlayerMoveDir).normalized;
            // var rad = Mathf.Atan2(m_PlayerMoveDir.x, m_PlayerMoveDir.z);
            // transform.Rotate(0, rad * 200 * Time.deltaTime, 0);

            // 匀速旋转
            unityChanController.transform.rotation = Quaternion.Slerp(unityChanController.transform.rotation, Quaternion.LookRotation(playerMoveDir), Time.deltaTime * unityChanController.turnSpeed);
        }

        /// <summary>
        /// 根运动回调
        /// </summary>
        /// <param name="deltaPosition"></param>
        /// <param name="deltaRotation"></param>
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            deltaPosition.y = -9.8f * Time.deltaTime;
            unityChanController.characterController.Move(deltaPosition);
        }

        private void OnFootStep()
        {
            var clips = unityChanController.characterConfig.footStepAudioClips;
            var index = UnityEngine.Random.Range(0, clips.Length);
            AudioManager.PlayOneShot(clips[index], unityChanController.transform.position, volumeScale: 0.2f);
        }
    }
}