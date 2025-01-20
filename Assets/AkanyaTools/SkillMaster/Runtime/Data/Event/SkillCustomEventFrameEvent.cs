/*
 * @Author: AiUU
 * @Description: SkillMaster 自定义事件帧事件
 * @AkanyaTech.SkillMaster
 */

namespace AkanyaTools.SkillMaster.Runtime.Data.Event
{
    public sealed class SkillCustomEventFrameEvent : SkillFrameEventBase
    {
        public SkillEventType eventType;

        public string customEventName;

        public int intParam;

        public float floatParam;

        public string stringParam;

        public UnityEngine.Object objParam;
    }

    public enum SkillEventType
    {
        Custom,
        UnFreezeRelease,
    }
}