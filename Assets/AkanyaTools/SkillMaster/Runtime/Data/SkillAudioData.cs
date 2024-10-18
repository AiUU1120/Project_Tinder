/*
* @Author: AiUU
* @Description: SkillMaster 技能音效数据
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Event;

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public sealed class SkillAudioData
    {
        /// <summary>
        /// 音效帧事件数据
        /// </summary>
        public List<SkillAudioFrameEvent> frameData = new();
    }
}