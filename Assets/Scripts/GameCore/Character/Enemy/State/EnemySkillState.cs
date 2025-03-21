/*
 * @Author: AiUU
 * @Description: 敌人技能状态
 * @AkanyaTech.Tinder
 */

using UnityEngine;

namespace GameCore.Character.Enemy.State
{
    public sealed class EnemySkillState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Skill");
            m_EnemyController.StopMove();
            PlaySkill();
        }

        protected override bool CheckStateChange() => false;

        public override void Update()
        {
        }

        private void PlaySkill()
        {
            m_EnemyController.skillBrain.ReleaseSkill(m_CurReleaseSkillIndex);
        }
    }
}