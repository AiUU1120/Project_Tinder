/*
 * @Author: AiUU
 * @Description: UnityChan角色控制器类
 * @AkanyaTech.Tinder
 */

using System;
using AkanyaTools.PlayableKami;
using AkanyaTools.ResourceSystem;
using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using AkanyaTools.StateMachine;
using Common.System;
using Data.GameCore;
using Data.GameCore.Config;
using Data.GameCore.Enums;
using Data.GameCore.Save;
using FrameTools.StateMachine;
using GameCore.Character.Player.State;
using GameCore.Skills;
using GameCore.Skills.SkillBehaviour;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Character.Player
{
    public sealed class PlayerController : PlayerControllerBase, IStateMachineOwner, ISkillCharacter
    {
        [Header("组件")]
        [SerializeField]
        private CharacterController m_CharacterController;

        [SerializeField]
        private AnimationController m_AnimationController;

        [SerializeField]
        private PlayerSkillBrain m_SkillBrain;

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

        public CharacterController characterController => m_CharacterController;

        public AnimationController animationController => m_AnimationController;

        public WeaponConfig weaponConfig { get; private set; }

        public PlayerSkillBrain skillBrain => m_SkillBrain;

        public float moveSpeed => m_MoveSpeed;

        public float dashSpeed => m_DashSpeed;

        public float turnSpeed => m_TurnSpeed;

        [ShowInInspector]
        public CharacterProperties characterProperties { get; private set; }

        public Vector3 playerMoveDir { get; private set; }

        private Transform m_CameraTrans;

        private StateMachine m_StateMachine;

        #region 初始化

        public void Init(WeaponConfig weaponConfig, PlayerData playerData)
        {
            if (Camera.main != null)
            {
                m_CameraTrans = Camera.main.transform;
            }
            this.weaponConfig = weaponConfig;
            skillBrain.Init(this, playerData.skillLearnedDatas);
            characterProperties = new CharacterProperties();
            characterProperties.Init(weaponConfig);
            InitStateMachine();
        }

        /// <summary>
        /// 初始化状态机
        /// </summary>
        private void InitStateMachine()
        {
            m_StateMachine = ResourceManager.GetOrNew<StateMachine>();
            m_StateMachine.Init<PlayerIdleState>(this);
        }

        #endregion

        private void Update()
        {
            CalculateMoveInputDirection();
        }

        /// <summary>
        /// 旋转角色（待优化）
        /// </summary>
        public void Rotate(float rotateSpeed = 0)
        {
            if (rotateSpeed == 0)
            {
                rotateSpeed = turnSpeed;
            }
            // var forward = m_CameraTrans.forward;
            // var camForwardProjection = new Vector3(forward.x, 0, forward.z).normalized;
            // playerMoveDir = camForwardProjection * dir.z + m_CameraTrans.right * dir.x;
            if (playerMoveDir.magnitude == 0)
            {
                return;
            }
            // 匀速旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerMoveDir), Time.deltaTime * rotateSpeed);
        }

        #region 状态机与动画

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="playerMotionState">目标状态</param>
        /// <param name="reCurState">同一状态是否切换</param>
        public void ChangeState(PlayerMotionState playerMotionState, bool reCurState = false)
        {
            // m_CurrPlayerMotionState = playerMotionState;
            switch (playerMotionState)
            {
                case PlayerMotionState.Idle:
                    m_StateMachine.ChangeState<PlayerIdleState>(reCurState);
                    break;
                case PlayerMotionState.Move:
                    m_StateMachine.ChangeState<PlayerMoveState>(reCurState);
                    break;
                case PlayerMotionState.Dash:
                    m_StateMachine.ChangeState<PlayerDashState>(reCurState);
                    break;
                case PlayerMotionState.Skill:
                    m_StateMachine.ChangeState<PlayerSkillState>(reCurState);
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
            m_AnimationController.PlaySingleAnimation(weaponConfig.GetCommonAnimByName(clipName), onRootMotion: onRootMotion, speed: speed, blockSameAnim: blockSameAnim, mixingTime: mixingTime);
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
            m_AnimationController.PlayBlendAnimation(weaponConfig.GetCommonAnimByName(clip1Name), weaponConfig.GetCommonAnimByName(clip2Name), onRootMotion: onRootMotion, speed: speed,
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
            playerMoveDir = camForwardProjection * InputManager.instance.moveInput.y + m_CameraTrans.right * InputManager.instance.moveInput.x;
        }

        #endregion

        public void BeHit(AttackData attackData)
        {
        }

        public int GetAtkValue(SkillDetectionFrameEvent e)
        {
            var curSkill = (PlayerSkillBehaviourBase) skillBrain.curSkillBehaviour;
            var skillAtk = skillBrain.curSkillBehaviour.skillConfig.GetAtkByLevel(curSkill.skillLearnedData.level);
            return Mathf.RoundToInt((characterProperties.atk.curValue + skillAtk) * e.attackHitConfig.atkFactor);
        }

        public void OnSkillRotate()
        {
            Rotate();
        }

        public void ChangeToIdleState()
        {
            ChangeState(PlayerMotionState.Idle);
        }

        public void OnSkillMove(Vector3 deltaPosition)
        {
            m_CharacterController.Move(deltaPosition);
        }

        public void OnSkillRotate(Quaternion deltaRotation)
        {
            transform.rotation *= deltaRotation;
        }
    }
}