/*
 * @Author: AiUU
 * @Description: SkillMaster 技能自定义事件数据
 * @AkanyaTech.SkillMaster
 */

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Data.Event;

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public sealed class SkillCustomEventData
    {
        /// <summary>
        /// 自定义事件帧事件数据
        /// key - 帧数
        /// value - 事件数据
        /// </summary>
        public Dictionary<int, SkillCustomEventFrameEvent> frameData = new();
    }
}