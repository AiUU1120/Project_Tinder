/*
 * @Author: AiUU
 * @Description: 玩家数据类
 * @AkanyaTech.Tinder
 */

using System;

namespace Data.GameCore.Save
{
    [Serializable]
    public class PlayerData
    {
        public string curWeaponId;

        public int skillPoint;

        /// <summary>
        /// 技能学习数据
        /// K - 武器 ID
        /// </summary>
        public Serialized_Dic<string, SkillLearnedDatas> skillLearnedDatasDic;

        /// <summary>
        /// 快捷栏技能数据
        /// K - 武器 ID
        /// </summary>
        public Serialized_Dic<string, ShortcutSkillData> shortcutSkillDataDic;
    }
}