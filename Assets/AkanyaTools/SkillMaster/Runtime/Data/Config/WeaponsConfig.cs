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
    [CreateAssetMenu(fileName = "WeaponsConfig", menuName = "SkillMaster/Config/WeaponsConfig")]
    public sealed class WeaponsConfig : ConfigBase
    {
        [OdinSerialize]
        private Dictionary<string, SkillWeapon> m_SkillWeaponsDic = new();

        public Dictionary<string, SkillWeapon> skillWeaponsDic => m_SkillWeaponsDic;
    }
}