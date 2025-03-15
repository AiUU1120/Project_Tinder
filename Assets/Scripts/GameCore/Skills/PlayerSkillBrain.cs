using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
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

        private PlayerController m_UnityChanController;

        public void Init(PlayerControllerBase playerController, SkillLearnedDatas learnedDatas)
        {
            m_UnityChanController = playerController as PlayerController;
            if (m_UnityChanController != null)
            {
                skillPlayer.Init(m_UnityChanController.animationController, m_UnityChanController.transform);
            }
            canReleaseSkill = true;
            skillBehaviours = new List<SkillBehaviourBase>(learnedDatas.learnedSkillsDic.Dictionary.Count + 2);
            InitNormalAttack();
            foreach (var item in learnedDatas.learnedSkillsDic.Dictionary)
            {
                var skillBehaviour = skillConfigs[item.Key + 2].skillBehaviour.DeepCopy();
                skillBehaviour.Init(playerController, skillConfigs[item.Key + 2], this, skillPlayer);
                ((PlayerSkillBehaviourBase) skillBehaviour).InitSkillLearnedData(item.Value);
                skillBehaviours.Add(skillBehaviour);
            }
        }

        private void InitNormalAttack()
        {
            var skillBehaviour0 = skillConfigs[0].skillBehaviour.DeepCopy();
            skillBehaviour0.Init(m_UnityChanController, skillConfigs[0], this, skillPlayer);
            skillBehaviours.Add(skillBehaviour0);
            var skillBehaviour1 = skillConfigs[1].skillBehaviour.DeepCopy();
            skillBehaviour1.Init(m_UnityChanController, skillConfigs[1], this, skillPlayer);
            skillBehaviours.Add(skillBehaviour1);
        }
    }
}