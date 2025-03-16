/*
 * @Author: AiUU
 * @Description: 快捷栏技能数据
 * @AkanyaTech.Tinder
 */

using System;
using UnityEngine.Serialization;

namespace Data.GameCore
{
    [Serializable]
    public class ShortcutSkillData
    {
        [FormerlySerializedAs("skillId")]
        public int skillIndex;
    }
}