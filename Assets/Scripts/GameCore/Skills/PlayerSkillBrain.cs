using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using Data.GameCore;
using GameCore.Character.Player;
using GameCore.Skills.SkillBehaviour;

namespace GameCore.Skills
{
    public sealed class PlayerSkillBrain : SkillBrainBase
    {
        /// <summary>
        /// 攻击是否保持连段的共享数据的 key
        /// </summary>
        public const string continuous_attack_mode_data_key = "ContinuousAttackMode";

        private PlayerController m_PlayerController;

        public void Init(PlayerControllerBase playerControllerBase, SkillLearnedDatas learnedDatas)
        {
            m_PlayerController = playerControllerBase as PlayerController;
            if (m_PlayerController != null)
            {
                skillPlayer.Init(m_PlayerController.animationController, m_PlayerController.transform);
            }
            canReleaseSkill = true;
            var skillConfigs = PlayerManager.instance.GetSkillConfigList();
            foreach (var item in learnedDatas.learnedSkillsDic.Dictionary)
            {
                AddSkill(playerControllerBase, skillConfigs, item.Key, item.Value);
            }
        }

        public void AddSkill(PlayerControllerBase playerControllerBase, List<SkillConfig> skillConfigs, int skillIndex, SkillLearnedData skillLearnedData)
        {
            var skillConfig = skillConfigs[skillIndex];
            var skillBehaviour = skillConfig.skillBehaviour.DeepCopy();
            skillBehaviour.Init(playerControllerBase, skillConfig, this, skillPlayer, skillIndex);
            ((PlayerSkillBehaviourBase) skillBehaviour).InitSkillLearnedData(skillLearnedData);
            skillBehaviours.Add(skillBehaviour);
        }
    }
}