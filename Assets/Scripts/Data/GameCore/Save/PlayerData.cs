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
        /// <summary>
        /// 技能学习数据
        /// </summary>
        public SkillLearnedDatas skillLearnedDatas;

        /// <summary>
        /// 快捷栏技能数据
        /// </summary>
        public ShortcutSkillData shortcutSkillData;
    }
}