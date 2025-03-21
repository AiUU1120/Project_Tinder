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
using GameCore.Character.Player;
using GameCore.UI.PnlEnemyHpBar;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

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

        [SerializeField]
        private NavMeshAgent m_NavMeshAgent;

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

        [Space]
        [Header("AI")]
        [SerializeField]
        private float m_StopDistance;

        [SerializeField]
        private float m_ViewRadius;

        [SerializeField]
        private float m_ViewAngle;

        [SerializeField]
        private float m_ChaseMinDistance;

        public CharacterController characterController => m_CharacterController;

        public AnimationController animationController => m_AnimationController;

        public NavMeshAgent navMeshAgent => m_NavMeshAgent;

        public WeaponConfig weaponConfig => m_WeaponConfig;

        public EnemySkillBrain skillBrain => m_SkillBrain;

        public float moveSpeed => m_MoveSpeed;

        public float dashSpeed => m_DashSpeed;

        public float turnSpeed => m_TurnSpeed;

        public bool isDead { get; private set; }

        [ShowInInspector]
        public CharacterProperties characterProperties { get; private set; } = new();

        private StateMachine m_StateMachine;

        public Transform playerNavTarget { get; private set; }

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
            playerNavTarget = PlayerManager.instance.GetPlayerTransform();
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
                    m_StateMachine.ChangeState<EnemyChaseState>(reCurState);
                    break;
                case EnemyMotionState.BeHit:
                    break;
                case EnemyMotionState.Skill:
                    m_StateMachine.ChangeState<EnemySkillState>(reCurState);
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

        public int GetAtkValue(SkillDetectionFrameEvent e)
        {
            var skillAtk = skillBrain.curSkillBehaviour.skillConfig.baseAtk;
            return Mathf.RoundToInt((characterProperties.atk.curValue + skillAtk) * e.attackHitConfig.atkFactor);
        }

        public void OnSkillRotate()
        {
            var playerDir = (playerNavTarget.position - transform.position).normalized;
            if (playerDir.magnitude == 0)
            {
                return;
            }
            // 匀速旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDir), Time.deltaTime * m_TurnSpeed);
        }

        public void ChangeToIdleState()
        {
            ChangeState(EnemyMotionState.Idle);
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

        #region AI

        public void MoveToPlayer()
        {
            m_NavMeshAgent.isStopped = false;
            m_NavMeshAgent.speed = m_MoveSpeed;
            m_NavMeshAgent.destination = playerNavTarget.position;
        }

        public void StopMove()
        {
            m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.speed = 0;
        }

        /// <summary>
        /// 检测玩家是否在扇形视野内
        /// </summary>
        /// <returns></returns>
        public bool PlayerInSight()
        {
            if (playerNavTarget == null)
            {
                return false;
            }
            var directionToPlayer = playerNavTarget.position - transform.position;
            var distanceToPlayer = directionToPlayer.magnitude;
            // 距离检测
            if (distanceToPlayer > m_ViewRadius)
            {
                return false;
            }
            // 角度检测
            var angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer.normalized);
            if (angleToPlayer > m_ViewAngle * 0.5f)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检测玩家是否在追击范围内
        /// </summary>
        /// <returns></returns>
        public bool PlayerInChaseRange()
        {
            if (playerNavTarget == null)
            {
                return false;
            }
            var directionToPlayer = playerNavTarget.position - transform.position;
            var distanceToPlayer = directionToPlayer.magnitude;
            // 距离检测
            return distanceToPlayer <= m_ViewRadius && distanceToPlayer > m_StopDistance;
        }

        public bool PlayerInAttackRange()
        {
            if (playerNavTarget == null)
            {
                return false;
            }
            var directionToPlayer = playerNavTarget.position - transform.position;
            var distanceToPlayer = directionToPlayer.magnitude;
            // 距离检测
            return distanceToPlayer <= m_ChaseMinDistance;
        }

        public bool LostPlayer()
        {
            if (playerNavTarget == null)
            {
                return true;
            }
            var directionToPlayer = playerNavTarget.position - transform.position;
            var distanceToPlayer = directionToPlayer.magnitude;
            // 距离检测
            return distanceToPlayer > m_ViewRadius;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            var position = transform.position;
            Gizmos.DrawWireSphere(position, m_ViewRadius);
            var forward = transform.forward;
            var leftDir = Quaternion.Euler(0, -m_ViewAngle / 2, 0) * forward;
            var rightDir = Quaternion.Euler(0, m_ViewAngle / 2, 0) * forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(position, leftDir * m_ViewRadius);
            Gizmos.DrawRay(position, rightDir * m_ViewRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, m_ChaseMinDistance);
            Gizmos.DrawWireSphere(position, m_StopDistance);
        }
#endif

        #endregion
    }
}