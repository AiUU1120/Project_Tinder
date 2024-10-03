/*
* @Author: AiUU
* @Description: 技能配置
* @AkanyaTech.SkillMaster
*/

using System;
using System.Collections.Generic;
using AkanyaTools.SkillMaster.Scripts.Event;
using FrameTools.Base.Config;
using Sirenix.Serialization;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Scripts.Config
{
    [CreateAssetMenu(fileName = "SkillConfig_", menuName = "SkillMaster/Config/SkillConfig")]
    public sealed class SkillConfig : ConfigBase
    {
        [Tooltip("技能名称")]
        public string skillName;

        [Tooltip("帧总数")]
        public int frameCount = 100;

        [Tooltip("帧率")]
        public int frameRate = 30;

        [NonSerialized, OdinSerialize]
        public SkillAnimationData skillAnimationData = new();
    }

    /// <summary>
    /// 技能动画数据
    /// </summary>
    [Serializable]
    public class SkillAnimationData
    {
        /// <summary>
        /// 动画帧事件数据
        /// key - 帧数
        /// value - 事件数据
        /// </summary>
        [NonSerialized, OdinSerialize]
        public Dictionary<int, SkillAnimationFrameEvent> frameData = new();
    }
}