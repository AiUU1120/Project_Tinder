/*
 * @Author: AiUU
 * @Description: Buff 配置类
 * @AkanyaTech.BuffGuy
 */

using AkanyaTools.Base.Config;
using UnityEngine;
using UnityEngine.Serialization;

namespace AkanyaTools.BuffGuy.Data.Config
{
    [CreateAssetMenu(fileName = "BuffConfig_", menuName = "BuffGuy/BuffConfig")]
    public sealed class BuffConfig : ConfigBase
    {
        public string buffName;

        [Multiline]
        public string description;

        public Sprite icon;

        [FormerlySerializedAs("maxStack")]
        public int maxLayer = 1;

        /// <summary>
        /// Buff 是否可以堆叠
        /// </summary>
        public bool canStack => maxLayer > 1;

        public float durationTime;

        public float tickTime;

        /// <summary>
        /// Buff 开始时的效果
        /// </summary>
        public BuffEffectDataBase startEffect;

        /// <summary>
        /// Buff 每次 tick 的效果
        /// </summary>
        public BuffEffectDataBase tickEffect;

        /// <summary>
        /// Buff 结束时的效果
        /// </summary>
        public BuffEffectDataBase endEffect;
    }
}