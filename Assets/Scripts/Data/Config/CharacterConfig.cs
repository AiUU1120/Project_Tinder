/*
* @Author: AiUU
* @Description: 角色配置类
* @AkanyaTech.Tinder
*/

using System.Collections.Generic;
using FrameTools.Base.Config;
using UnityEngine;

namespace Data.Config
{
    [CreateAssetMenu(fileName = "CharacterConfig_", menuName = "Tinder/Config/CharacterConfig")]
    public sealed class CharacterConfig : ConfigBase
    {
        [Header("动画")]
        public bool applyRootMotion;

        public Dictionary<string, AnimationClip> commonAnimationDic = new();

        [Space]
        [Header("音效")]
        public AudioClip[] footStepAudioClips;

        /// <summary>
        /// 根据名称获取常规动画
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns></returns>
        public AnimationClip GetCommonAnimByName(string clipName)
        {
            if (commonAnimationDic.TryGetValue(clipName, out var clip))
            {
                return clip;
            }
            Debug.LogError($"没有找到 {clipName} 动画片段！");
            return null;
        }
    }
}