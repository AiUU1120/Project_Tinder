/*
 * @Author: AiUU
 * @Description: 敌人技能中枢
 * @AkanyaTech.Tinder
 */

using AkanyaTools.SkillMaster.Runtime.Core;
using Data.GameCore.Config;
using GameCore.Character.Enemy.Skills.SkillBehaviour;

namespace GameCore.Character.Enemy.Skills
{
    public sealed class EnemySkillBrain : SkillBrainBase
    {
        public void Init(EnemyController enemyController, WeaponConfig weaponConfig)
        {
            base.Init(enemyController);
            var skillConfigs = weaponConfig.skillConfigs;
            foreach (var skillConfig in skillConfigs)
            {
                var skillBehaviour = skillConfig.skillBehaviour.DeepCopy();
                ((EnemySkillBehaviourBase) skillBehaviour).Init(enemyController, skillConfig, this, m_SkillPlayer);
                m_SkillBehaviours.Add(skillBehaviour);
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