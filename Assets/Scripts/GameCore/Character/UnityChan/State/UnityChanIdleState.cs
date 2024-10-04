/*
* @Author: AiUU
* @Description: UnityChan站立状态
* @AkanyaTech.Tinder
*/

using Data.Enums.GameCore;

namespace GameCore.Character.UnityChan.State
{
    public sealed class UnityChanIdleState : UnityChanStateBase
    {
        public override void Enter()
        {
            base.Enter();
            // 播放待机动作
            unityChanController.PlayAnimation("Idle", mixingTime: 0.3f);
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
        }

        protected override bool CheckStateChange()
        {
            if (unityChanController.input.moveInput.magnitude >= 0.1f)
            {
                unityChanController.ChangeState(PlayerMotionState.Move);
                return true;
            }
            if (unityChanController.input.isSpecial)
            {
                unityChanController.ChangeState(PlayerMotionState.Skill);
                return true;
            }
            return false;
        }
    }
}