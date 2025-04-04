/*
 * @Author: AiUU
 * @Description: 角色属性
 * @AkanyaTech.Tinder
 */

using System;
using Data.GameCore.Config;
using Sirenix.Serialization;
using UnityEngine;

namespace Data.GameCore
{
    public class CharacterProperties
    {
        public int curHp;

        public int curMp;

        public IntegerProperties maxHp = new();

        public IntegerProperties maxMp = new();

        public IntegerProperties atk = new();

        public IntegerProperties def = new();

        private Action m_OnHpChange;

        private Action m_OnMpChange;

        public virtual void Init(WeaponConfig weaponConfig)
        {
            maxHp.Init(100, onBaseValueChange: OnMaxHpChange);
            maxMp.Init(100, onBaseValueChange: OnMaxMpChange);
            atk.Init(weaponConfig.baseAtk);
            def.Init(weaponConfig.baseDef);
            curHp = maxHp.baseValue;
            curMp = maxMp.baseValue;
        }

        public virtual void AddHp(int value)
        {
            SetHp(curHp + value);
        }

        public virtual void AddMp(int value)
        {
            SetMp(curMp + value);
        }

        public virtual void SetHp(int value)
        {
            curHp = Mathf.Clamp(value, 0, maxHp.curValue);
            m_OnHpChange?.Invoke();
        }

        public virtual void SetMp(int value)
        {
            curMp = Mathf.Clamp(value, 0, maxMp.curValue);
            m_OnMpChange?.Invoke();
        }

        public void SetOnHpChange(Action onHpChange)
        {
            m_OnHpChange = onHpChange;
        }

        public void SetOnMpChange(Action onMpChange)
        {
            m_OnMpChange = onMpChange;
        }

        protected virtual void OnMaxHpChange(int oldValue, int newValue)
        {
            var percent = curHp / oldValue;
            SetHp(newValue * percent);
        }

        protected virtual void OnMaxMpChange(int oldValue, int newValue)
        {
            var percent = curMp / oldValue;
            SetMp(newValue * percent);
        }

        public sealed class FloatProperties : PropertiesBase
        {
            /// <summary>
            /// 基础值
            /// </summary>
            public float baseValue
            {
                get => m_BaseValue;
                set
                {
                    m_OnBaseValueChange?.Invoke(m_BaseValue, value);
                    if (m_OnCurValueChange != null)
                    {
                        var oldValue = curValue;
                        m_BaseValue = value;
                        m_OnCurValueChange.Invoke(oldValue, curValue);
                    }
                    else
                    {
                        m_BaseValue = value;
                    }
                }
            }

            /// <summary>
            /// 固定加成
            /// </summary>
            public float fixedBonus
            {
                get => m_FixedBonus;
                set
                {
                    m_OnFixedBonusValueChange?.Invoke(m_FixedBonus, value);
                    if (m_OnCurValueChange != null)
                    {
                        var oldValue = curValue;
                        m_FixedBonus = value;
                        m_OnCurValueChange.Invoke(oldValue, curValue);
                    }
                    else
                    {
                        m_FixedBonus = value;
                    }
                }
            }

            /// <summary>
            /// 百分比加成
            /// </summary>
            public float percentBonus
            {
                get => m_PercentBonus;
                set
                {
                    m_OnPercentBonusValueChange?.Invoke(m_PercentBonus, value);
                    if (m_OnCurValueChange != null)
                    {
                        var oldValue = curValue;
                        m_PercentBonus = value;
                        m_OnCurValueChange.Invoke(oldValue, curValue);
                    }
                    else
                    {
                        m_PercentBonus = value;
                    }
                }
            }

            public float curValue => baseValue + fixedBonus + baseValue * percentBonus;

            private Action<float, float> m_OnBaseValueChange;

            private Action<float, float> m_OnCurValueChange;

            private float m_BaseValue;

            private float m_FixedBonus;

            private float m_PercentBonus;

            public void Init(float baseValue, Action<float, float> onBaseValueChange, Action<float, float> onCurValueChange = null, Action<float, float> onFixedBonusValueChange = null,
                Action<float, float> onPercentBonusValueChange = null)
            {
                this.baseValue = baseValue;
                m_OnBaseValueChange = onBaseValueChange;
                m_OnCurValueChange = onCurValueChange;
                this.m_OnFixedBonusValueChange = onFixedBonusValueChange;
                this.m_OnPercentBonusValueChange = onPercentBonusValueChange;
            }
        }

        public sealed class IntegerProperties : PropertiesBase
        {
            /// <summary>
            /// 基础值
            /// </summary>
            public int baseValue
            {
                get => m_BaseValue;
                set
                {
                    m_OnBaseValueChange?.Invoke(m_BaseValue, value);
                    if (m_OnCurValueChange != null)
                    {
                        var oldValue = curValue;
                        m_BaseValue = value;
                        m_OnCurValueChange.Invoke(oldValue, curValue);
                    }
                    else
                    {
                        m_BaseValue = value;
                    }
                }
            }

            /// <summary>
            /// 固定加成
            /// </summary>
            public float fixedBonus
            {
                get => m_FixedBonus;
                set
                {
                    m_OnFixedBonusValueChange?.Invoke(m_FixedBonus, value);
                    if (m_OnCurValueChange != null)
                    {
                        var oldValue = curValue;
                        m_FixedBonus = value;
                        m_OnCurValueChange.Invoke(oldValue, curValue);
                    }
                    else
                    {
                        m_FixedBonus = value;
                    }
                }
            }

            /// <summary>
            /// 百分比加成
            /// </summary>
            public float percentBonus
            {
                get => m_PercentBonus;
                set
                {
                    m_OnPercentBonusValueChange?.Invoke(m_PercentBonus, value);
                    if (m_OnCurValueChange != null)
                    {
                        var oldValue = curValue;
                        m_PercentBonus = value;
                        m_OnCurValueChange.Invoke(oldValue, curValue);
                    }
                    else
                    {
                        m_PercentBonus = value;
                    }
                }
            }

            public int curValue => Mathf.RoundToInt(baseValue + fixedBonus + baseValue * percentBonus);

            private Action<int, int> m_OnBaseValueChange;

            private Action<int, int> m_OnCurValueChange;

            private int m_BaseValue;

            private float m_FixedBonus;

            private float m_PercentBonus;

            public void Init(int baseValue, Action<int, int> onBaseValueChange = null, Action<int, int> onCurValueChange = null, Action<float, float> onFixedBonusValueChange = null,
                Action<float, float> onPercentBonusValueChange = null)
            {
                this.baseValue = baseValue;
                m_OnBaseValueChange = onBaseValueChange;
                m_OnCurValueChange = onCurValueChange;
                this.m_OnFixedBonusValueChange = onFixedBonusValueChange;
                this.m_OnPercentBonusValueChange = onPercentBonusValueChange;
            }
        }

        public abstract class PropertiesBase
        {
            protected Action<float, float> m_OnFixedBonusValueChange;

            protected Action<float, float> m_OnPercentBonusValueChange;
        }
    }
}