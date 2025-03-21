/*
 * @Author: AiUU
 * @Description: 玩家技能中枢
 * @AkanyaTech.Tinder
 */

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Enum;
using Data.GameCore;
using Data.GameCore.Config;
using GameCore.Character.Player.Skills.SkillBehaviour;
using UnityEngine;

namespace GameCore.Character.Player.Skills
{
    public sealed class PlayerSkillBrain : SkillBrainBase
    {
        /// <summary>
        /// 攻击是否保持连段的共享数据的 key
        /// </summary>
        public const string continuous_attack_mode_data_key = "ContinuousAttackMode";

        public override SkillBehaviourBase curSkillBehaviour => (PlayerSkillBehaviourBase) base.curSkillBehaviour;

        private PlayerController m_PlayerController;

        public void Init(PlayerController playerController)
        {
            base.Init(playerController);
            m_PlayerController = playerController;
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

        public void ChangeWeapon(WeaponConfig weaponConfig, SkillLearnedDatas learnedDatas)
        {
            m_SkillBehaviours.Clear();
            lastReleaseSkillIndex = -1;
            var skillConfigs = weaponConfig.skillConfigs;
            foreach (var item in learnedDatas.learnedSkillsDic.Dictionary)
            {
                AddSkill(m_PlayerController, skillConfigs, item.Key, item.Value);
            }
            if (!weaponConfig.isDoubleWeapon)
            {
                m_SkillPlayer.CreateWeaponOnWeaponPoint(weaponConfig.weaponPrefab);
            }
            else
            {
                m_SkillPlayer.CreateDoubleWeaponOnWeaponPoint(weaponConfig.weaponPrefab);
            }
        }
    }
}