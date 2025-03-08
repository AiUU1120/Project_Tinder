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
        /// <summary>
        /// 自定义事件
        /// </summary>
        Custom,

        /// <summary>
        /// 取消后摇
        /// </summary>
        UnFreezeRelease,

        /// <summary>
        /// 锁定旋转
        /// </summary>
        LockRotation,

        /// <summary>
        /// 解锁旋转
        /// </summary>
        UnlockRotation,
    }
}