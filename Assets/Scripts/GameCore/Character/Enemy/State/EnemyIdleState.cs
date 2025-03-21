/*
 * @Author: AiUU
 * @Description: 敌人待机状态
 * @AkanyaTech.Tinder
 */

using Data.GameCore.Enums;
using UnityEngine;

namespace GameCore.Character.Enemy.State
{
    public sealed class EnemyIdleState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Idle");
            m_EnemyController.StopMove();
            m_EnemyController.PlayAnimation("Idle", mixingTime: 0.3f);
        }

        public override void Update()
        {
            base.Update();
            if (CheckStateChange())
            {
                return;
            }
            m_EnemyController.characterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        }

        protected override bool CheckStateChange()
        {
            if (m_EnemyController.isDead)
            {
                m_EnemyController.ChangeState(EnemyMotionState.Die);
                return true;
            }
            if (m_EnemyController.PlayerInAttackRange())
            {
                if (CheckAndEnterSkillState())
                {
                    m_EnemyController.ChangeState(EnemyMotionState.Skill);
                    return true;
                }
                m_EnemyController.OnSkillRotate();
            }
            else if ((m_EnemyController.PlayerInSight() && m_EnemyController.PlayerInChaseRange()) || (m_IsFighting && m_EnemyController.PlayerInChaseRange()))
            {
                m_EnemyController.ChangeState(EnemyMotionState.Chase);
                return true;
            }
            return false;
        }
    }
}