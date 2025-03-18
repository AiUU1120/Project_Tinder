/*
 * @Author: AiUU
 * @Description: 武器配置表
 * @AkanyaTech.SkillMaster
 */

using System.Collections.Generic;
using AkanyaTools.Base.Config;
using AkanyaTools.SkillMaster.Runtime.Component;
using Sirenix.Serialization;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data.Config
{
    [CreateAssetMenu(fileName = "SkillWeaponsMapConfig_", menuName = "SkillMaster/Config/SkillWeaponsMapConfig")]
    public sealed class SkillWeaponsMapConfig : ConfigBase
    {
        [OdinSerialize]
        private Dictionary<string, GameObject> m_SkillWeaponsDic = new();

        public Dictionary<string, GameObject> skillWeaponsDic => m_SkillWeaponsDic;
    }
}