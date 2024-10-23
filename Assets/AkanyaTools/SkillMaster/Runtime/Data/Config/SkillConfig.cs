/*
* @Author: AiUU
* @Description: 技能配置
* @AkanyaTech.SkillMaster
*/

using System;
using AkanyaTools.Base.Config;
using Sirenix.Serialization;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data.Config
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

        [OdinSerialize]
        public SkillAnimationData skillAnimationData = new();

        [OdinSerialize]
        public SkillAudioData skillAudioData = new();

        [OdinSerialize]
        public SkillEffectData skillEffectData = new();

#if UNITY_EDITOR
        /// <summary>
        /// 设置 SkillConfig OnValidate 回调
        /// </summary>
        /// <param name="action"></param>
        public static void SetOnValidate(Action action)
        {
            s_OnValidate = action;
        }

        private static Action s_OnValidate;

        private void OnValidate()
        {
            s_OnValidate?.Invoke();
        }
#endif
    }
}