/*
 * @Author: AiUU
 * @Description: 敌人死亡状态
 * @AkanyaTech.Tinder
 */

namespace GameCore.Character.Enemy.State
{
    public sealed class EnemyDieState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            m_EnemyController.PlayAnimation("Die", mixingTime: 0.3f);
        }

        protected override bool CheckStateChange() => false;
    }
}