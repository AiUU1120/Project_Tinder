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
            for (var i = 0; i < skillConfigs.Count; i++)
            {
                var skillBehaviour = skillConfigs[i].skillBehaviour.DeepCopy();
                ((EnemySkillBehaviourBase) skillBehaviour).Init(enemyController, skillConfigs[i], this, m_SkillPlayer, i);
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