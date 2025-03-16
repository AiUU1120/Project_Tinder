/*
 * @Author: AiUU
 * @Description: UnityChan移动状态
 * @AkanyaTech.Tinder
 */

using Common.System;

namespace GameCore.Character.Player.State
{
    public sealed class PlayerSkillState : PlayerStateBase
    {
        public override void Enter()
        {
            base.Enter();
            // if (isPass)
            // {
            //     return;
            // }
            PlaySkill();
        }

        protected override bool CheckStateChange() => false;

        public override void Update()
        {
            if (CheckAndEnterSkillState())
            {
                InputManager.instance.ResetAllCacheTimer();
                PlaySkill();
            }
        }

        private void PlaySkill()
        {
            unityChanController.skillBrain.ReleaseSkill(curReleaseSkillIndex);
        }
    }
}