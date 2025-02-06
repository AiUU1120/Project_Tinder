/*
 * @Author: AiUU
 * @Description: 技能片段
 * @AkanyaTech.SkillMaster
 */

using System;
using AkanyaTools.Base.Config;
using Sirenix.Serialization;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data.Config
{
    [CreateAssetMenu(fileName = "SkillClip_", menuName = "SkillMaster/Config/SkillClip")]
    public sealed class SkillClip : ConfigBase
    {
        [Tooltip("技能名称")]
        public string skillName;

        [Tooltip("帧总数")]
        public int frameCount = 100;

        [Tooltip("帧率")]
        public int frameRate = 30;

        [OdinSerialize]
        public SkillCustomEventData skillCustomEventData = new();

        [OdinSerialize]
        public SkillAnimationData skillAnimationData = new();

        [OdinSerialize]
        public SkillDetectionData skillDetectionData = new();

        [OdinSerialize]
        public SkillEffectData skillEffectData = new();

        [OdinSerialize]
        public SkillAudioData skillAudioData = new();

#if UNITY_EDITOR
        /// <summary>
        /// 设置 SkillClip OnValidate 回调
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