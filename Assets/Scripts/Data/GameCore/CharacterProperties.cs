/*
 * @Author: AiUU
 * @Description: 角色属性
 * @AkanyaTech.Tinder
 */

using System;
using Data.GameCore.Config;
using UnityEngine;

namespace Data.GameCore
{
    public sealed class CharacterProperties
    {
        public float curHp;

        public float curMp;

        public IntegerProperties maxHp = new();

        public IntegerProperties maxMp = new();

        public IntegerProperties atk = new();

        public IntegerProperties def = new();

        public void Init(WeaponConfig weaponConfig)
        {
            maxHp.Init(100, onBaseValueChange: OnMaxHpChange);
            atk.Init(weaponConfig.baseAtk);
            def.Init(weaponConfig.baseDef);
        }

        private void OnMaxHpChange(int oldValue, int newValue)
        {
            var percent = curHp / oldValue;
            curHp = newValue * percent;
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
                    onFixedBonusValueChange?.Invoke(m_FixedBonus, value);
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
                    onPercentBonusValueChange?.Invoke(m_PercentBonus, value);
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
                this.onFixedBonusValueChange = onFixedBonusValueChange;
                this.onPercentBonusValueChange = onPercentBonusValueChange;
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
                    onFixedBonusValueChange?.Invoke(m_FixedBonus, value);
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
                    onPercentBonusValueChange?.Invoke(m_PercentBonus, value);
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
                this.onFixedBonusValueChange = onFixedBonusValueChange;
                this.onPercentBonusValueChange = onPercentBonusValueChange;
            }
        }

        public abstract class PropertiesBase
        {
            protected Action<float, float> onFixedBonusValueChange;

            protected Action<float, float> onPercentBonusValueChange;
        }
    }
}