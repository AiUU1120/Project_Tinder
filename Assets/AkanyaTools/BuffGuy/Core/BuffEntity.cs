/*
 * @Author: AiUU
 * @Description: Buff 实体
 * @AkanyaTech.BuffGuy
 */

using System;
using AkanyaTools.BuffGuy.Data.Config;
using FrameTools.Extension;
using UnityEngine;

namespace AkanyaTools.BuffGuy.Core
{
    public sealed class BuffEntity
    {
        public BuffConfig config { get; private set; }

        public float destroyTimer { get; private set; }

        public float tickTimer { get; private set; }

        public int layer { get; private set; }

        private Action<BuffEntity> m_OnBuffStart;

        private Action<BuffEntity> m_OnBuffTick;

        private Action<BuffEntity> m_OnBuffEnd;

        public void Init(BuffConfig config, Action<BuffEntity> onBuffStart, Action<BuffEntity> onBuffTick, Action<BuffEntity> onBuffEnd)
        {
            this.config = config;
            m_OnBuffStart = onBuffStart;
            m_OnBuffTick = onBuffTick;
            m_OnBuffEnd = onBuffEnd;
        }

        public void Start()
        {
            destroyTimer = config.durationTime;
            tickTimer = config.tickTime;
            layer = 1;
            m_OnBuffStart?.Invoke(this);
        }

        public void Update()
        {
            if (m_OnBuffTick != null)
            {
                tickTimer -= Time.deltaTime;
                if (tickTimer <= 0)
                {
                    m_OnBuffTick.Invoke(this);
                    // 重置 tickTimer 这里加上本身是为了减小总体误差
                    tickTimer = config.tickTime + tickTimer;
                }
            }
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
            {
                m_OnBuffEnd?.Invoke(this);
            }
        }

        public void Stop()
        {
            config = null;
            m_OnBuffStart = null;
            m_OnBuffTick = null;
            m_OnBuffEnd = null;
            this.ObjectPushPool();
        }

        public void AddLayer(int layer)
        {
            if (config.canStack)
            {
                this.layer = Mathf.Clamp(this.layer + layer, 0, config.maxLayer);
            }
            destroyTimer = config.durationTime;
        }
    }
}