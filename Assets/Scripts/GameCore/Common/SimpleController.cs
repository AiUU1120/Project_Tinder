/*
* @Author: AiUU
* @Description: UnityChan简易角色控制器类
* @AkanyaTech.Tinder
*/

using Data.Enums.GameCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCore.Common
{
    public sealed class SimpleController : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private CharacterController m_CharacterController;

        [SerializeField]
        private float m_WalkSpeed = 2.5f;

        [SerializeField]
        private float m_RunSpeed = 5.5f;

        private Transform m_CameraTrans;

        private PlayerPostureState m_PlayerPostureState = PlayerPostureState.Stand;

        private PlayerMotionState m_PlayerMotionState = PlayerMotionState.Idle;

        private readonly float m_StandingThreshold = 0f;

        private readonly float m_HoveringThreshld = 1f;

        private Vector2 m_MoveInput;

        private Vector3 m_PlayerMoveDir;

        private bool m_IsRunning;

        private bool m_IsJumping;

        private static class AnimationHash
        {
            public static readonly int posture = Animator.StringToHash("Posture");
            public static readonly int moveSpeed = Animator.StringToHash("Move Speed");
            public static readonly int turnSpeed = Animator.StringToHash("Turn Speed");
        }

        private void Awake()
        {
            if (Camera.main != null)
            {
                m_CameraTrans = Camera.main.transform;
            }
        }

        private void Update()
        {
            CalculateInputDirection();
            SwitchPlayerState();
            SetupAnimator();
        }

        private void SwitchPlayerState()
        {
            if (m_MoveInput.magnitude <= 0.1f)
            {
                m_PlayerMotionState = PlayerMotionState.Idle;
            }
            else if (!m_IsRunning)
            {
                m_PlayerMotionState = PlayerMotionState.Move;
            }
            else
            {
                m_PlayerMotionState = PlayerMotionState.Dash;
            }
        }

        private void SetupAnimator()
        {
            if (m_PlayerPostureState == PlayerPostureState.Stand)
            {
                m_Animator.SetFloat(AnimationHash.posture, m_StandingThreshold, 0.1f, Time.deltaTime);
                switch (m_PlayerMotionState)
                {
                    case PlayerMotionState.Idle:
                    {
                        m_Animator.SetFloat(AnimationHash.moveSpeed, 0, 0.1f, Time.deltaTime);
                        break;
                    }
                    case PlayerMotionState.Move:
                    {
                        m_Animator.SetFloat(AnimationHash.moveSpeed, m_MoveInput.magnitude * m_WalkSpeed, 0.1f, Time.deltaTime);
                        break;
                    }
                    case PlayerMotionState.Dash:
                    {
                        m_Animator.SetFloat(AnimationHash.moveSpeed, m_MoveInput.magnitude * m_RunSpeed, 0.1f, Time.deltaTime);
                        break;
                    }
                }
            }
            // 计算转向方向与当前方向夹角
            var rad = Mathf.Atan2(m_PlayerMoveDir.x, m_PlayerMoveDir.z);
            m_Animator.SetFloat(AnimationHash.turnSpeed, rad, 0.1f, Time.deltaTime);
            transform.Rotate(0, rad * 200 * Time.deltaTime, 0);
        }

        private void OnAnimatorMove()
        {
            m_CharacterController.Move(m_Animator.deltaPosition);
        }

        /// <summary>
        /// 根据相机方向计算玩家输入移动方向
        /// </summary>
        private void CalculateInputDirection()
        {
            // 计算世界坐标系下相机前向方向向量在y平面上投影
            var forward = m_CameraTrans.forward;
            var camForwardProjection = new Vector3(forward.x, 0, forward.z).normalized;
            // 玩家移动方向即为 相机前向 * 前后输入 + 相机侧向 * 左右输入
            m_PlayerMoveDir = camForwardProjection * m_MoveInput.y + m_CameraTrans.right * m_MoveInput.x;
            // 转换到玩家本地坐标系
            m_PlayerMoveDir = transform.InverseTransformDirection(m_PlayerMoveDir);
        }

        #region 输入相关

        public void GetMoveInput(InputAction.CallbackContext ctx)
        {
            m_MoveInput = ctx.ReadValue<Vector2>();
        }

        public void GetRunInput(InputAction.CallbackContext ctx)
        {
            m_IsRunning = ctx.ReadValueAsButton();
        }

        #endregion
    }
}