/*
 * @Author: AiUU
 * @Description: UnityChan移动状态
 * @AkanyaTech.Tinder
 */

namespace GameCore.Character.Player.State
{
    public sealed class UnityChanSkillState : UnityChanStateBase
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
                PlaySkill();
            }
        }

        private void PlaySkill()
        {
            unityChanController.skillBrain.ReleaseSkill(curReleaseSkillIndex);
        }
    }
}