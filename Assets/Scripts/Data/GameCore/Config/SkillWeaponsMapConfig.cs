/*
 * @Author: AiUU
 * @Description: 武器配置表
 * @AkanyaTech.SkillMaster
 */

using System.Collections.Generic;
using AkanyaTools.Base.Config;
using Sirenix.Serialization;
using UnityEngine;

namespace Data.GameCore.Config
{
    [CreateAssetMenu(fileName = "SkillWeaponsMapConfig_", menuName = "Tinder/Config/GameCore/SkillWeaponsMapConfig")]
    public sealed class SkillWeaponsMapConfig : ConfigBase
    {
        [OdinSerialize]
        private Dictionary<string, WeaponConfig> m_SkillWeaponsDic = new();

        public Dictionary<string, WeaponConfig> skillWeaponsDic => m_SkillWeaponsDic;
    }
}