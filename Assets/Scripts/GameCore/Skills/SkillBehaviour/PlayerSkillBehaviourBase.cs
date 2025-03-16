using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.UISystem;
using Data;
using Data.GameCore;
using GameCore.Character.Player;
using GameCore.UI.PnlGameMain;

namespace GameCore.Skills.SkillBehaviour
{
    public abstract class PlayerSkillBehaviourBase : SkillBehaviourBase
    {
        protected PlayerController playerController;

        protected SkillLearnedData skillLearnedData = new();

        protected virtual bool autoUpdateSlot => true;

        public override void Init(PlayerControllerBase playerControllerBase, SkillConfig skillConfig, SkillBrainBase skillBrain, SkillPlayer skillPlayer, int skillIndex = -1)
        {
            base.Init(playerControllerBase, skillConfig, skillBrain, skillPlayer, skillIndex);
            playerController = playerControllerBase as PlayerController;
        }

        public void InitSkillLearnedData(SkillLearnedData skillLearnedData)
        {
            this.skillLearnedData = skillLearnedData;
        }

        public override void Update()
        {
            base.Update();
            if (autoUpdateSlot)
            {
                UpdateSkillSlot();
            }
        }

        protected override void RotateOnUpdate()
        {
            if (canRotate)
            {
                playerController.Rotate();
            }
        }

        public override float GetCDTime() => skillConfig.GetCDByLevel(skillLearnedData.level);

        private void UpdateSkillSlot()
        {
            if (skillIndex == DataManager.playerData.shortcutSkillData.skillIndex)
            {
                OnUpdateSkillSlot();
            }
        }

        protected virtual void OnUpdateSkillSlot()
        {
            UISystem.GetWindow<PnlGameMain>().UpdateCDMask(cdTimer / skillConfig.GetCDByLevel(skillLearnedData.level));
        }
    }
}