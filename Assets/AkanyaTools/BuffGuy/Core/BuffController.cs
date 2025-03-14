/*
 * @Author: AiUU
 * @Description: Buff 控制器
 * @AkanyaTech.BuffGuy
 */

using System.Collections.Generic;
using AkanyaTools.BuffGuy.Data.Config;
using FrameTools.ResourceSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkanyaTools.BuffGuy.Core
{
    public sealed class BuffController : MonoBehaviour
    {
        [SerializeField]
        private BuffParserBase m_BuffParser;

        [ShowInInspector]
        private Dictionary<BuffConfig, BuffEntity> m_BuffDic = new();

        // 由于 foreach 不能直接删除元素，所以这里用一个 HashSet 来存储需要删除的 Buff
        private readonly List<BuffEntity> m_DestroyBuffs = new();

        private void Update()
        {
            foreach (var buff in m_BuffDic.Values)
            {
                buff.Update();
                if (buff.destroyTimer <= 0)
                {
                    m_DestroyBuffs.Add(buff);
                }
            }
            foreach (var buff in m_DestroyBuffs)
            {
                m_BuffDic.Remove(buff.config);
                buff.Stop();
            }
            m_DestroyBuffs.Clear();
        }

        /// <summary>
        /// 清理 Buff
        /// </summary>
        public void ClearBuffs()
        {
            foreach (var buff in m_BuffDic.Values)
            {
                buff.Stop();
            }
            m_BuffDic.Clear();
        }

        private void OnBuffStart(BuffEntity buff)
        {
            m_BuffParser.Parse(buff, buff.config.startEffect);
        }

        private void OnBuffTick(BuffEntity buff)
        {
            m_BuffParser.Parse(buff, buff.config.tickEffect);
        }

        private void OnBuffEnd(BuffEntity buff)
        {
            m_BuffParser.Parse(buff, buff.config.endEffect);
        }

#if UNITY_EDITOR
        [Button("AddBuff Test")]
        private BuffEntity AddBuff(BuffConfig config, int layer = 1)
        {
            if (m_BuffDic.TryGetValue(config, out var buff))
            {
                buff.AddLayer(layer);
            }
            else
            {
                buff = ResourceManager.GetOrNew<BuffEntity>();
                buff.Init(config, OnBuffStart, OnBuffTick, OnBuffEnd);
                buff.Start();
                m_BuffDic.Add(config, buff);
            }
            return buff;
        }
#endif
    }
}