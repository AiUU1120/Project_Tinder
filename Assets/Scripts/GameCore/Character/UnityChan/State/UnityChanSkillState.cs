/*
 * @Author: AiUU
 * @Description: UnityChan移动状态
 * @AkanyaTech.Tinder
 */

namespace GameCore.Character.UnityChan.State
{
    public sealed class UnityChanSkillState : UnityChanStateBase
    {
        public override void Enter()
        {
            base.Enter();
            if (isPass)
            {
                return;
            }
            // m_SkillClip = ResourceManager.LoadAsset<SkillClip>("SkillConfig_Test 1");
            // unityChanController.skillPlayer.PlaySkillClip(m_SkillClip, OnSkillEnd, OnWeaponDetection, OnRootMotion);
            unityChanController.skillBrain.ReleaseSkill(0);
        }

        //
        // public override void Update()
        // {
        //     base.Update();
        //     if (CheckStateChange())
        //     {
        //         return;
        //     }
        // }
        //
        // public override void Exit()
        // {
        //     base.Exit();
        //     if (isPass)
        //     {
        //         return;
        //     }
        // }
        //
        protected override bool CheckStateChange() => false;
    }
}