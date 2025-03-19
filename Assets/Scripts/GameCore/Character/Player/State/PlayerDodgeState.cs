/*
 * @Author: AiUU
 * @Description: UnityChan闪避状态
 * @AkanyaTech.Tinder
 */

using Data.GameCore.Enums;
using UnityEngine;

namespace GameCore.Character.Player.State
{
    public sealed class PlayerDodgeState : PlayerStateBase
    {
        #region 生命周期

        public override void Enter()
        {
            base.Enter();
            m_PlayerController.AddAnimationEvent("FootStep", OnFootStep);
            m_PlayerController.PlayAnimation("Dodge", onRootMotion: OnRootMotion);
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
            m_PlayerController.ClearOnRootMotion();
            m_PlayerController.RemoveAnimationEvent("FootStep", OnFootStep);
        }

        #endregion

        protected override bool CheckStateChange()
        {
            if (m_PlayerController.GetCurAnimationProgress() >= 1)
            {
                m_PlayerController.ChangeState(PlayerMotionState.Idle);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根运动回调
        /// </summary>
        /// <param name="deltaPosition"></param>
        /// <param name="deltaRotation"></param>
        private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            var pos = new Vector3(deltaPosition.x * 1.5f, deltaPosition.y, deltaPosition.z * 1.5f);
            m_PlayerController.characterController.Move(pos);
        }
    }
}