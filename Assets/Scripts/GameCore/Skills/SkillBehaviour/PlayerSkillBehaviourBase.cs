using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using GameCore.Character.UnityChan;

namespace GameCore.Skills.SkillBehaviour
{
    public abstract class PlayerSkillBehaviourBase : SkillBehaviourBase
    {
        protected UnityChanController unityChanController;

        public override void Init(PlayerControllerBase playerController, SkillConfig skillConfig, SkillBrainBase skillBrain, SkillPlayer skillPlayer)
        {
            base.Init(playerController, skillConfig, skillBrain, skillPlayer);
            unityChanController = playerController as UnityChanController;
        }

        protected override void RotateOnUpdate()
        {
            if (canRotate)
            {
                unityChanController.Rotate();
            }
        }
    }
}