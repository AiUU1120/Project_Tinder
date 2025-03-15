using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using Data.GameCore;
using GameCore.Character.Player;

namespace GameCore.Skills.SkillBehaviour
{
    public abstract class PlayerSkillBehaviourBase : SkillBehaviourBase
    {
        protected PlayerController playerController;

        protected SkillLearnedData skillLearnedData = new();

        public override void Init(PlayerControllerBase playerControllerBase, SkillConfig skillConfig, SkillBrainBase skillBrain, SkillPlayer skillPlayer)
        {
            base.Init(playerControllerBase, skillConfig, skillBrain, skillPlayer);
            playerController = playerControllerBase as PlayerController;
        }

        public void InitSkillLearnedData(SkillLearnedData skillLearnedData)
        {
            this.skillLearnedData = skillLearnedData;
        }

        protected override void RotateOnUpdate()
        {
            if (canRotate)
            {
                playerController.Rotate();
            }
        }

        public override float GetCDTime() => skillConfig.GetCDByLevel(skillLearnedData.level);
    }
}