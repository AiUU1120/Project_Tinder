using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Data.Event;

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public class SkillEffectData
    {
        /// <summary>
        /// 特效帧事件数据
        /// </summary>
        public readonly List<SkillEffectFrameEvent> frameData = new();
    }
}