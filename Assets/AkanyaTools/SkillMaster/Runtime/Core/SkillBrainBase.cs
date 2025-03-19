/*
 * @Author: AiUU
 * @Description: 技能控制基类
 * @AkanyaTech.SkillMaster
 */

using System;
using System.Collections.Generic;
using AkanyaTools.ResourceSystem;
using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Enum;
using FrameTools.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Core
{
    public abstract class SkillBrainBase : MonoBehaviour
    {
        [SerializeField]
        protected SkillPlayer m_SkillPlayer;

        [ShowInInspector]
        protected List<SkillBehaviourBase> m_SkillBehaviours = new();

        /// <summary>
        /// 是否可以释放技能
        /// </summary>
        public virtual bool canReleaseSkill { get; protected set; }

        public int lastReleaseSkillIndex { get; protected set; } = -1;

        public int skillCount => m_SkillBehaviours.Count;

        public virtual SkillBehaviourBase curSkillBehaviour => m_SkillBehaviours.Find(s => s.skillIndex == lastReleaseSkillIndex);

        private readonly Dictionary<string, ISkillShareData> m_ShareDataDic = new();

        private readonly Dictionary<string, ISharedDataEventData> m_ShareDataEventDic = new();

        protected interface ISkillShareData
        {
        }

        protected sealed class SkillShareData<T> : ISkillShareData
        {
            public T data;
        }

        private interface ISharedDataEventData
        {
            public void InvokeRemove();
        }

        private sealed class SharedDataEventData<T> : ISharedDataEventData
        {
            public Action<T> onCreate;

            public Action<T> onChanged;

            public Action onRemove;

            public void InvokeCreate(T data) => onCreate?.Invoke(data);

            public void InvokeChanged(T data) => onChanged?.Invoke(data);

            public void InvokeRemove() => onRemove?.Invoke();
        }

        public virtual void Init(ISkillCharacter skillOwner)
        {
            if (skillOwner != null)
            {
                m_SkillPlayer.Init(skillOwner, skillOwner.animationController, skillOwner.transform);
            }
            canReleaseSkill = true;
        }

        protected virtual void Update()
        {
            foreach (var skillBehaviour in m_SkillBehaviours)
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
            // TODO: 和上一层对接
            Debug.Log($"释放技能的代价: {costType} - {costValue}");
        }

        /// <summary>
        /// 自下而上 询问上级是否满足代价
        /// </summary>
        /// <param name="costType"></param>
        /// <param name="costValue"></param>
        /// <returns></returns>
        public virtual bool CheckCost(SkillCostType costType, float costValue) =>
            // TODO: 和上一层对接
            true;

        /// <summary>
        /// 技能释放检查
        /// </summary>
        /// <param name="skillIndex"></param>
        /// <returns></returns>
        public bool CheckReleaseSkill(int skillIndex)
        {
            var skillBehaviour = m_SkillBehaviours.Find(s => s.skillIndex == skillIndex);
            return canReleaseSkill && skillBehaviour != null && skillBehaviour.CheckRelease();
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="index">skillBehaviours 索引</param>
        public virtual void ReleaseSkill(int index)
        {
            var skillBehaviour = m_SkillBehaviours.Find(s => s.skillIndex == index);
            if (lastReleaseSkillIndex != -1 && lastReleaseSkillIndex != index)
            {
                var lastSkillBehaviour = m_SkillBehaviours.Find(s => s.skillIndex == lastReleaseSkillIndex);
                lastSkillBehaviour.OnSkillBehaviourSwitch();
            }
            lastReleaseSkillIndex = index;
            skillBehaviour.Release();
        }

        /// <summary>
        /// 设置是否可以释放技能标识
        /// </summary>
        /// <param name="flag"></param>
        public virtual void SetCanReleaseFlag(bool flag) => canReleaseSkill = flag;

        #region 技能共享数据相关

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
            if (m_ShareDataEventDic.TryGetValue(key, out var sharedDataEventData))
            {
                ((SharedDataEventData<T>) sharedDataEventData).InvokeCreate(value);
                ((SharedDataEventData<T>) sharedDataEventData).InvokeChanged(value);
            }
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
                if (m_ShareDataEventDic.TryGetValue(key, out var sharedDataEventData))
                {
                    ((SharedDataEventData<T>) sharedDataEventData).InvokeChanged(value);
                }
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
            if (m_ShareDataDic.Remove(key, out var data))
            {
                if (m_ShareDataEventDic.TryGetValue(key, out var sharedDataEventData))
                {
                    sharedDataEventData.InvokeRemove();
                }
                DestroySkillShareData(data);
            }
        }

        /// <summary>
        /// 清除所有技能共享数据
        /// </summary>
        public void ClearSkillShareData()
        {
            foreach (var data in m_ShareDataDic)
            {
                DestroySkillShareData(data.Value);
                if (m_ShareDataEventDic.TryGetValue(data.Key, out var sharedDataEventData))
                {
                    sharedDataEventData.InvokeRemove();
                }
            }
            m_ShareDataDic.Clear();
        }

        /// <summary>
        /// 从对象池获取共享数据实例
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected SkillShareData<T> GetSkillShareData<T>(string key) => ResourceManager.GetOrNew<SkillShareData<T>>();

        /// <summary>
        /// 将共享数据实例回收到对象池
        /// </summary>
        /// <param name="obj"></param>
        protected void DestroySkillShareData(ISkillShareData obj)
        {
            obj.ObjectPushPool();
        }

        /// <summary>
        /// 添加共享数据创建事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void AddSharedDataCreateEventListener<T>(string key, Action<T> action)
        {
            if (!m_ShareDataEventDic.TryGetValue(key, out var data))
            {
                var eventData = new SharedDataEventData<T>();
                eventData.onCreate += action;
                m_ShareDataEventDic.Add(key, eventData);
            }
            else
            {
                var eventData = (SharedDataEventData<T>) data;
                eventData.onCreate += action;
            }
        }

        /// <summary>
        /// 移除共享数据创建事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void RemoveSharedDataCreateEventListener<T>(string key, Action<T> action)
        {
            if (m_ShareDataEventDic.TryGetValue(key, out var data))
            {
                var eventData = (SharedDataEventData<T>) data;
                eventData.onCreate -= action;
            }
        }

        /// <summary>
        /// 添加共享数据修改事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void AddSharedDataChangedEventListener<T>(string key, Action<T> action)
        {
            if (!m_ShareDataEventDic.TryGetValue(key, out var data))
            {
                var eventData = new SharedDataEventData<T>();
                eventData.onChanged += action;
                m_ShareDataEventDic.Add(key, eventData);
            }
            else
            {
                var eventData = (SharedDataEventData<T>) data;
                eventData.onChanged += action;
            }
        }

        /// <summary>
        /// 移除共享数据修改事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void RemoveSharedDataChangedEventListener<T>(string key, Action<T> action)
        {
            if (m_ShareDataEventDic.TryGetValue(key, out var data))
            {
                var eventData = (SharedDataEventData<T>) data;
                eventData.onChanged -= action;
            }
        }

        /// <summary>
        /// 添加共享数据移除事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void AddSharedDataRemoveEventListener<T>(string key, Action action)
        {
            if (!m_ShareDataEventDic.TryGetValue(key, out var data))
            {
                var eventData = new SharedDataEventData<T>();
                eventData.onRemove += action;
                m_ShareDataEventDic.Add(key, eventData);
            }
            else
            {
                var eventData = (SharedDataEventData<T>) data;
                eventData.onRemove += action;
            }
        }

        /// <summary>
        /// 移除共享数据移除事件监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void RemoveSharedDataRemoveEventListener<T>(string key, Action action)
        {
            if (m_ShareDataEventDic.TryGetValue(key, out var data))
            {
                var eventData = (SharedDataEventData<T>) data;
                eventData.onRemove -= action;
            }
        }

        #endregion
    }
}