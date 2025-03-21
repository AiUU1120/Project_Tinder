using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.UISystem;
using Data;
using Data.GameCore;
using GameCore.UI.PnlGameMain;

namespace GameCore.Character.Player.Skills.SkillBehaviour
{
    public abstract class PlayerSkillBehaviourBase : SkillBehaviourBase
    {
        public SkillLearnedData skillLearnedData => m_SkillLearnedData;

        protected SkillLearnedData m_SkillLearnedData = new();

        protected virtual bool autoUpdateSlot => true;

        public void Init(ISkillCharacter skillOwner, SkillConfig skillConfig, SkillBrainBase skillBrain, SkillPlayer skillPlayer, SkillLearnedData skillLearnedData, int skillIndex = -1)
        {
            base.Init(skillOwner, skillConfig, skillBrain, skillPlayer, skillIndex);
            m_SkillLearnedData = skillLearnedData;
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
            if (m_CanRotate)
            {
                m_SkillOwner.OnSkillRotate();
            }
        }

        public override float GetCDTime() => m_SkillConfig.GetCDByLevel(m_SkillLearnedData.level);

        private void UpdateSkillSlot()
        {
            if (skillIndex == DataManager.playerData.shortcutSkillDataDic.Dictionary[DataManager.playerData.curWeaponId].skillIndex)
            {
                OnUpdateSkillSlot();
            }
        }

        protected virtual void OnUpdateSkillSlot()
        {
            var maxCD = m_SkillConfig.GetCDByLevel(m_SkillLearnedData.level);
            var fillAmount = maxCD == 0 ? 0 : m_CDTimer / maxCD;
            UISystem.GetWindow<PnlGameMain>().UpdateCDMask(fillAmount);
            UISystem.GetWindow<PnlGameMain>().UpdateSkillSlotState(CheckReleaseCost());
        }
    }
}