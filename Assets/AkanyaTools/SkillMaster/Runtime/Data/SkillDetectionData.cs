/*
* @Author: AiUU
* @Description: SkillMaster 技能判定数据
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Data.Event;

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public sealed class SkillDetectionData
    {
        /// <summary>
        /// 判定帧事件数据
        /// </summary>
        public readonly List<SkillDetectionFrameEvent> frameData = new();
    }
}