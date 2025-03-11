/*
 * @Author: AiUU
 * @Description: 技能配置
 * @AkanyaTech.SkillMaster
 */

using System.Collections.Generic;
using AkanyaTools.Base.Config;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data.Enum;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data.Config
{
    [CreateAssetMenu(fileName = "SkillConfig_", menuName = "SkillMaster/Config/SkillConfig")]
    public sealed class SkillConfig : ConfigBase
    {
        /// <summary>
        /// 全部技能片段
        /// </summary>
        public SkillClip[] clips;

        /// <summary>
        /// 技能行为 运行逻辑
        /// </summary>
        public SkillBehaviourBase skillBehaviour;

        /// <summary>
        /// 技能CD时间
        /// </summary>
        public float cdTime;

        /// <summary>
        /// 技能消耗代价
        /// </summary>
        public Dictionary<SkillCostType, float> releaseCostDic = new();
    }
}