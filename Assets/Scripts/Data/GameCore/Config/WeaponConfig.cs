/*
 * @Author: AiUU
 * @Description: 角色配置类
 * @AkanyaTech.Tinder
 */

using System.Collections.Generic;
using AkanyaTools.Base.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using UnityEngine;

namespace Data.GameCore.Config
{
    [CreateAssetMenu(fileName = "WeaponConfig_", menuName = "Tinder/Config/GameCore/WeaponConfig")]
    public sealed class WeaponConfig : ConfigBase
    {
        public string weaponName;

        public Sprite icon;

        [Multiline]
        public string weaponDescription;

        public GameObject weaponPrefab;

        public bool isDoubleWeapon;

        [Header("属性")]
        public int baseAtk;

        public int baseDef;

        [Space]
        [Header("全部技能")]
        public List<SkillConfig> skillConfigs = new();

        [Space]
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