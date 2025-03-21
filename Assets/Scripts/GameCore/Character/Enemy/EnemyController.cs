/*
 * @Author: AiUU
 * @Description: 敌人控制器
 * @AkanyaTech.Tinder
 */

using System;
using AkanyaTools.PlayableKami;
using AkanyaTools.ResourceSystem;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using AkanyaTools.StateMachine;
using Data.GameCore;
using Data.GameCore.Config;
using Data.GameCore.Enums;
using FrameTools.StateMachine;
using GameCore.Character.Enemy.Skills;
using GameCore.Character.Enemy.State;
using GameCore.UI.PnlEnemyHpBar;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore.Character.Enemy
{
    public sealed class EnemyController : MonoBehaviour, IStateMachineOwner, ISkillCharacter
    {
        [Header("组件")]
        [SerializeField]
        private CharacterController m_CharacterController;

        [SerializeField]
        private AnimationController m_AnimationController;

        [SerializeField]
        private EnemySkillBrain m_SkillBrain;

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

        [SerializeField]
        private WeaponConfig m_WeaponConfig;

        public CharacterController characterController => m_CharacterController;

        public AnimationController animationController => m_AnimationController;

        public WeaponConfig weaponConfig => m_WeaponConfig;

        public EnemySkillBrain skillBrain => m_SkillBrain;

        public float moveSpeed => m_MoveSpeed;

        public float dashSpeed => m_DashSpeed;

        public float turnSpeed => m_TurnSpeed;

        public bool isDead { get; private set; }

        [ShowInInspector]
        public CharacterProperties characterProperties { get; private set; } = new();

        public Vector3 playerMoveDir { get; private set; }

        private StateMachine m_StateMachine;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            m_AnimationController.Init();
            InitStateMachine();
            m_SkillBrain.Init(this, m_WeaponConfig);
            InitProperties();
        }

        private void InitStateMachine()
        {
            m_StateMachine = ResourceManager.GetOrNew<StateMachine>();
            m_StateMachine.Init<EnemyIdleState>(this);
        }

        private void InitProperties()
        {
            characterProperties.Init(weaponConfig);
            characterProperties.SetOnHpChange(OnHpChange);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="enemyMotionState">目标状态</param>
        /// <param name="reCurState">同一状态是否切换</param>
        public void ChangeState(EnemyMotionState enemyMotionState, bool reCurState = false)
        {
            // m_CurrPlayerMotionState = playerMotionState;
            switch (enemyMotionState)
            {
                case EnemyMotionState.Idle:
                    m_StateMachine.ChangeState<EnemyIdleState>(reCurState);
                    break;
                case EnemyMotionState.Patrol:
                    break;
                case EnemyMotionState.Chase:
                    break;
                case EnemyMotionState.BeHit:
                    break;
                case EnemyMotionState.Skill:
                    break;
                case EnemyMotionState.Die:
                    m_StateMachine.ChangeState<EnemyDieState>(reCurState);
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
            m_AnimationController.PlaySingleAnimation(m_WeaponConfig.GetCommonAnimByName(clipName), onRootMotion: onRootMotion, speed: speed, blockSameAnim: blockSameAnim, mixingTime: mixingTime);
        }

        public void BeHit(AttackData attackData)
        {
            Debug.Log($"我被打了 {attackData.atkValue} 点伤害");
            characterProperties.AddHp(-Mathf.RoundToInt(attackData.atkValue));
            if (characterProperties.curHp <= 0)
            {
                Debug.Log("我死了");
                isDead = true;
            }
        }

        public int GetAtkValue(SkillDetectionFrameEvent e) => 0;

        public void OnSkillRotate()
        {
        }

        public void ChangeToIdleState()
        {
        }

        public void OnSkillMove(Vector3 deltaPosition)
        {
        }

        public void OnSkillRotate(Quaternion deltaRotation)
        {
        }

        private void OnHpChange()
        {
            var fillAmount = (float) characterProperties.curHp / characterProperties.maxHp.curValue;
            GetComponentInChildren<PnlEnemyHpBar>().SetHpBar(fillAmount);
        }
    }
}