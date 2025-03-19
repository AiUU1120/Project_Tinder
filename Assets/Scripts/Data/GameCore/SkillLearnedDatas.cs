/*
 * @Author: AiUU
 * @Description: 技能学习数据
 * @AkanyaTech.Tinder
 */

using System;

namespace Data.GameCore
{
    [Serializable]
    public class SkillLearnedDatas
    {
        /// <summary>
        /// Key - 技能 Index
        /// </summary>
        public Serialized_Dic<int, SkillLearnedData> learnedSkillsDic = new()
        {
            Dictionary = { { 0, new SkillLearnedData() { level = 1 } }, { 1, new SkillLearnedData() { level = 1 } } }
        };
    }
}