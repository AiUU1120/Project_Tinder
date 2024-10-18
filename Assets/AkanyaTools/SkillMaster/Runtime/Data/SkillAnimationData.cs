/*
* @Author: AiUU
* @Description: SkillMaster 技能动画数据
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Event;

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public sealed class SkillAnimationData
    {
        /// <summary>
        /// 动画帧事件数据
        /// key - 帧数
        /// value - 事件数据
        /// </summary>
        public readonly Dictionary<int, SkillAnimationFrameEvent> frameData = new();
    }
}