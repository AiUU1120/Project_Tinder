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
        /// 技能名
        /// </summary>
        public string skillName;

        /// <summary>
        /// 技能图标
        /// </summary>
        public Sprite icon;

        /// <summary>
        /// 备用图标
        /// </summary>
        public Sprite[] icons;

        /// <summary>
        /// 全部技能片段
        /// </summary>
        public SkillClip[] clips;

        /// <summary>
        /// 技能行为 运行逻辑
        /// </summary>
        public SkillBehaviourBase skillBehaviour;

        /// <summary>
        /// 技能基本CD时间
        /// </summary>
        public float baseCD;

        /// <summary>
        /// 技能基础攻击力
        /// </summary>
        public int baseAtk;

        /// <summary>
        /// 技能消耗代价
        /// </summary>
        public Dictionary<SkillCostType, float> releaseCostDic = new();

        /// <summary>
        /// 是否是被动技能
        /// </summary>
        public bool isPassive;

        /// <summary>
        /// 技能描述
        /// </summary>
        [Multiline]
        public string skillDescription;

        /// <summary>
        /// 习得所需点数
        /// </summary>
        public int skillPrice;

        /// <summary>
        /// 技能最大等级
        /// </summary>
        public int maxLevel;

        /// <summary>
        /// CD升级时减少的比例
        /// </summary>
        public float cdReduceRate = 0.9f;

        /// <summary>
        /// 攻击力升级时增加的比例
        /// </summary>
        public float atkIncreaseRate = 1.2f;

        public int GetAtkByLevel(int level) => level <= 0 ? baseAtk : Mathf.RoundToInt(baseAtk * Mathf.Pow(atkIncreaseRate, level - 1));

        public float GetCDByLevel(int level)
        {
            if (level <= 0)
            {
                return baseCD;
            }
            var cd = baseCD * Mathf.Pow(cdReduceRate, level - 1);
            return (float) System.Math.Round(cd, 1);
        }
    }
}