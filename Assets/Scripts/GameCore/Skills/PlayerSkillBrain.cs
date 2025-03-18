using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Enum;
using Data.GameCore;
using GameCore.Character.Player;
using GameCore.Skills.SkillBehaviour;
using UnityEngine;

namespace GameCore.Skills
{
    public sealed class PlayerSkillBrain : SkillBrainBase
    {
        /// <summary>
        /// 攻击是否保持连段的共享数据的 key
        /// </summary>
        public const string continuous_attack_mode_data_key = "ContinuousAttackMode";

        public override SkillBehaviourBase curSkillBehaviour => (PlayerSkillBehaviourBase) base.curSkillBehaviour;

        private PlayerController m_PlayerController;

        public void Init(PlayerController playerController, SkillLearnedDatas learnedDatas)
        {
            base.Init(playerController);
            m_PlayerController = playerController;
            var skillConfigs = PlayerManager.instance.GetSkillConfigList();
            foreach (var item in learnedDatas.learnedSkillsDic.Dictionary)
            {
                AddSkill(playerController, skillConfigs, item.Key, item.Value);
            }
        }

        public void AddSkill(ISkillCharacter skillOwner, List<SkillConfig> skillConfigs, int skillIndex, SkillLearnedData skillLearnedData)
        {
            var skillConfig = skillConfigs[skillIndex];
            var skillBehaviour = skillConfig.skillBehaviour.DeepCopy();
            ((PlayerSkillBehaviourBase) skillBehaviour).Init(skillOwner, skillConfig, this, m_SkillPlayer, skillLearnedData, skillIndex);
            m_SkillBehaviours.Add(skillBehaviour);
        }

        public override bool CheckCost(SkillCostType costType, float costValue)
        {
            switch (costType)
            {
                case SkillCostType.Mp:
                    return m_PlayerController.characterProperties.curMp >= costValue;
            }
            return false;
        }

        public override void ApplyCost(SkillCostType costType, float costValue)
        {
            base.ApplyCost(costType, costValue);
            switch (costType)
            {
                case SkillCostType.Mp:
                    m_PlayerController.characterProperties.AddMp(-Mathf.RoundToInt(costValue));
                    break;
            }
        }
    }
}