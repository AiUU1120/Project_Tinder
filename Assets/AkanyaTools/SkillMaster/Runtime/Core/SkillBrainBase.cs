/*
 * @Author: AiUU
 * @Description: 技能控制基类
 * @AkanyaTech.SkillMaster
 */

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Enum;
using FrameTools.Extension;
using FrameTools.ResourceSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Core
{
    public abstract class SkillBrainBase : MonoBehaviour
    {
        [SerializeField]
        protected SkillPlayer skillPlayer;

        [SerializeField]
        protected SkillConfig normalAttackConfig;

        [SerializeField]
        protected List<SkillConfig> skillConfigs = new();

        [ShowInInspector]
        protected List<SkillBehaviourBase> skillBehaviours;

        private readonly Dictionary<string, ISkillShareData> m_ShareDataDic = new();

        protected interface ISkillShareData
        {
        }

        protected sealed class SkillShareData<T> : ISkillShareData
        {
            public T data;
        }

        public virtual void Init(PlayerControllerBase playerController)
        {
            skillBehaviours = new List<SkillBehaviourBase>(skillConfigs.Count);
            foreach (var skillConfig in skillConfigs)
            {
                var skillBehaviour = skillConfig.skillBehaviour.DeepCopy();
                skillBehaviour.Init(playerController, skillConfig, this, skillPlayer);
                skillBehaviours.Add(skillBehaviour);
            }
        }

        protected virtual void Update()
        {
            foreach (var skillBehaviour in skillBehaviours)
            {
                skillBehaviour.Update();
            }
        }

        /// <summary>
        /// 对技能的消耗代价做实际扣除
        /// </summary>
        /// <param name="costType"></param>
        /// <param name="costValue"></param>
        public virtual void ApplyCost(SkillCostType costType, float costValue)
        {
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="index">索引</param>
        public virtual void ReleaseSkill(int index)
        {
            skillBehaviours[index].Release();
        }

        /// <summary>
        /// 添加技能共享数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void AddSkillShareData<T>(string key, T value)
        {
            var skillShareData = GetSkillShareData<T>(key);
            skillShareData.data = value;
            m_ShareDataDic.Add(key, skillShareData);
        }

        /// <summary>
        /// 添加或更新技能共享数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void AddOrUpdateSkillShareData<T>(string key, T value)
        {
            if (m_ShareDataDic.TryGetValue(key, out var data))
            {
                ((SkillShareData<T>) data).data = value;
            }
            else
            {
                AddSkillShareData(key, value);
            }
        }

        /// <summary>
        /// 尝试获取技能共享数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryGetSkillShareData<T>(string key, out T data)
        {
            if (m_ShareDataDic.TryGetValue(key, out var value))
            {
                data = ((SkillShareData<T>) value).data;
                return true;
            }
            data = default;
            return false;
        }

        public bool ContainsSkillShareData(string key) => m_ShareDataDic.ContainsKey(key);

        /// <summary>
        /// 移除指定技能共享数据
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSkillShareData(string key)
        {
            if (m_ShareDataDic.TryGetValue(key, out var data))
            {
                DestroySkillShareData(data);
                // ?
                m_ShareDataDic.Remove(key);
            }
        }

        /// <summary>
        /// 清除所有技能共享数据
        /// </summary>
        public void ClearSkillShareData()
        {
            foreach (var data in m_ShareDataDic.Values)
            {
                DestroySkillShareData(data);
            }
            m_ShareDataDic.Clear();
        }

        protected SkillShareData<T> GetSkillShareData<T>(string key) => ResourceManager.GetOrNew<SkillShareData<T>>();

        protected void DestroySkillShareData(ISkillShareData obj)
        {
            obj.ObjectPushPool();
        }
    }
}