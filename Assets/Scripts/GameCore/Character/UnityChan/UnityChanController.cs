/*
* @Author: AiUU
* @Description: UnityChan角色控制器类
* @AkanyaTech.Tinder
*/

using System;
using AkanyaTools.PlayableKami;
using AkanyaTools.SkillMaster.Runtime.Component;
using GameCore.Character.UnityChan.State;
using GameCore.Common;
using Data.Config;
using Data.Enums.GameCore;
using FrameTools.ResourceSystem;
using FrameTools.StateMachine;
using UnityEngine;

namespace GameCore.Character.UnityChan
{
    public sealed class UnityChanController : MonoBehaviour, IStateMachineOwner
    {
        [Header("组件")]
        [SerializeField]
        private CharacterController m_CharacterController;

        [SerializeField]
        private AnimationController m_AnimationController;

        [SerializeField]
        private SkillPlayer m_SkillPlayer;

        [SerializeField]
        private InputController m_InputController;

        [SerializeField]
        private CharacterConfig m_CharacterConfig;

        [Space]
        [Header("属性")]
        [SerializeField]
        [Tooltip("移动速度 仅未开启根运动时有效")]
        private float m_MoveSpeed = 6.5f;

        [SerializeField]
        [Tooltip("冲刺速度 仅未开启根运动时有效")]
        private float m_DashSpeed = 7.3f;

        [SerializeField]
        private float m_TurnSpeed = 7f;

        public SkillPlayer skillPlayer => m_SkillPlayer;

        public InputController input => m_InputController;

        public CharacterController characterController => m_CharacterController;

        public CharacterConfig characterConfig => m_CharacterConfig;

        public float moveSpeed => m_MoveSpeed;

        public float dashSpeed => m_DashSpeed;

        public float turnSpeed => m_TurnSpeed;

        public Vector3 playerMoveDir { get; private set; }

        private Transform m_CameraTrans;

        private StateMachine m_StateMachine;

        private PlayerPostureState m_CurrPlayerPostureState = PlayerPostureState.Stand;

        private PlayerMotionState m_CurrPlayerMotionState = PlayerMotionState.Idle;

        private readonly float m_StandingThreshold = 0f;

        private readonly float m_HoveringThreshld = 1f;

        private static class AnimationHash
        {
            public static readonly int posture = Animator.StringToHash("Posture");
            public static readonly int moveSpeed = Animator.StringToHash("Move Speed");
            public static readonly int turnSpeed = Animator.StringToHash("Turn Speed");
        }

        #region 初始化

        private void Start()
        {
            if (Camera.main != null)
            {
                m_CameraTrans = Camera.main.transform;
            }
            skillPlayer.Init(m_AnimationController);
            InitStateMachine();
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void InitStateMachine()
        {
            m_StateMachine = ResourceManager.GetOrNew<StateMachine>();
            m_StateMachine.Init<UnityChanIdleState>(this);
        }

        #endregion

        private void Update()
        {
            CalculateMoveInputDirection();
        }

        #region 状态机与动画

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="playerMotionState">目标状态</param>
        public void ChangeState(PlayerMotionState playerMotionState)
        {
            m_CurrPlayerMotionState = playerMotionState;
            switch (playerMotionState)
            {
                case PlayerMotionState.Idle:
                    m_StateMachine.ChangeState<UnityChanIdleState>();
                    break;
                case PlayerMotionState.Move:
                    m_StateMachine.ChangeState<UnityChanMoveState>();
                    break;
                case PlayerMotionState.Dash:
                    m_StateMachine.ChangeState<UnityChanDashState>();
                    break;
                case PlayerMotionState.Skill:
                    m_StateMachine.ChangeState<UnityChanSkillState>();
                    break;
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="clipName">动画名称 (根据配置表) </param>
        /// <param name="onRootMotion">根运动回调</param>
        /// <param name="blockSameAnim">是否忽略相同动画</param>
        /// <param name="mixingTime">过渡时间</param>
        /// <param name="speed">动画播放速度</param>
        public void PlayAnimation(string clipName, Action<Vector3, Quaternion> onRootMotion = null, float speed = 1f, bool blockSameAnim = false, float mixingTime = 0.25f)
        {
            m_AnimationController.PlaySingleAnimation(m_CharacterConfig.GetCommonAnimByName(clipName), onRootMotion: onRootMotion, speed: speed, blockSameAnim: blockSameAnim, mixingTime: mixingTime);
        }

        /// <summary>
        /// 播放混合动画
        /// </summary>
        /// <param name="clip1Name">动画1名称 (根据配置表) </param>
        /// <param name="clip2Name">动画2名称 (根据配置表) </param>
        /// <param name="onRootMotion">根运动回调</param>
        /// <param name="speed">动画播放速度</param>
        /// <param name="mixingTime">过渡时间</param>
        public void PlayBlendAnimation(string clip1Name, string clip2Name, Action<Vector3, Quaternion> onRootMotion = null, float speed = 1f, float mixingTime = 0.25f)
        {
            m_AnimationController.PlayBlendAnimation(m_CharacterConfig.GetCommonAnimByName(clip1Name), m_CharacterConfig.GetCommonAnimByName(clip2Name), onRootMotion: onRootMotion, speed: speed,
                mixingTime: mixingTime);
        }

        /// <summary>
        /// 设置动画混合权重
        /// </summary>
        /// <param name="weightTo">后一个动画权重</param>
        public void SetBlendAnimationWeight(float weightTo)
        {
            m_AnimationController.SetBlendAnimationWeight(weightTo);
        }

        /// <summary>
        /// 清理根运动回调
        /// </summary>
        public void ClearOnRootMotion()
        {
            m_AnimationController.ClearOnRootMotion();
        }

        /// <summary>
        /// 添加动画事件
        /// </summary>
        /// <param name="eventName">事件 key</param>
        /// <param name="action">具体 Action</param>
        public void AddAnimationEvent(string eventName, Action action)
        {
            m_AnimationController.AddAnimationEvent(eventName, action);
        }

        /// <summary>
        /// 移除 key 下所有 Action
        /// </summary>
        /// <param name="eventName">事件 key</param>
        public void RemoveAnimationEvent(string eventName)
        {
            m_AnimationController.RemoveAnimationEvent(eventName);
        }

        /// <summary>
        /// 移除 key 下指定 Action
        /// </summary>
        /// <param name="eventName">事件 key</param>
        /// <param name="action">具体 Action</param>
        public void RemoveAnimationEvent(string eventName, Action action)
        {
            m_AnimationController.RemoveAnimationEvent(eventName, action);
        }

        /// <summary>
        /// 清除动画控制器中所有动画事件
        /// </summary>
        public void ClearAllAnimationEvent()
        {
            m_AnimationController.ClearAllAnimationEvent();
        }

        #endregion

        #region 内部计算

        /// <summary>
        /// 根据相机方向计算玩家输入移动方向
        /// </summary>
        private void CalculateMoveInputDirection()
        {
            // 计算世界坐标系下相机前向方向向量在y平面上投影
            var forward = m_CameraTrans.forward;
            var camForwardProjection = new Vector3(forward.x, 0, forward.z).normalized;
            // 玩家移动方向即为 相机前向 * 前后输入 + 相机侧向 * 左右输入
            playerMoveDir = camForwardProjection * m_InputController.moveInput.y + m_CameraTrans.right * m_InputController.moveInput.x;
        }

        #endregion
    }
}