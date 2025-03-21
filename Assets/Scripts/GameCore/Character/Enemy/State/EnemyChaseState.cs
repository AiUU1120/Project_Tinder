/*
 * @Author: AiUU
 * @Description: 敌人追击状态
 * @AkanyaTech.Tinder
 */

using Data.GameCore.Enums;
using UnityEngine;

namespace GameCore.Character.Enemy.State
{
    public sealed class EnemyChaseState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Chase");
            m_EnemyController.PlayAnimation("Chase", mixingTime: 0.3f);
            m_IsFighting = true;
        }

        public override void Update()
        {
            base.Update();
            if (CheckStateChange())
            {
                return;
            }
            m_EnemyController.MoveToPlayer();
        }

        protected override bool CheckStateChange()
        {
            if (m_EnemyController.LostPlayer())
            {
                m_IsFighting = false;
                m_EnemyController.ChangeState(EnemyMotionState.Idle);
                return true;
            }
            if (!m_EnemyController.PlayerInChaseRange())
            {
                m_EnemyController.ChangeState(EnemyMotionState.Idle);
                return true;
            }
            return false;
        }
    }
}