/*
 * @Author: AiUU
 * @Description: 输入管理器
 * @AkanyaTech.Tinder
 */

using System;
using Common.Data.Config;
using FrameTools.Base.Singleton;
using FrameTools.Extension;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Common.System
{
    public sealed class InputManager : SingletonMono<InputManager>
    {
        [SerializeField]
        private InputConfig m_InputConfig;

        public abstract class KeyBase
        {
            public abstract void Init();

            public abstract void Update();
        }

        [Serializable]
        public class Key<T> : KeyBase
        {
            [SerializeField]
            protected bool m_IsCache;

            [SerializeField]
            protected float m_CacheTime;

            protected T m_Value;

            protected float m_LastInputTime;

            public virtual T GetState() => default;

            public void SetResultValue(T value)
            {
                this.m_Value = value;
            }

            public override void Init()
            {
                m_LastInputTime = float.MinValue;
            }

            public override void Update()
            {
            }

            public void ResetCacheTimer()
            {
                m_LastInputTime = float.MinValue;
            }
        }

        public sealed class BoolKey : Key<bool>
        {
            public override bool GetState()
            {
                if (!m_IsCache)
                {
                    return m_Value;
                }
                return m_Value || (Time.time - m_LastInputTime) < m_CacheTime;
            }

            public override void Update()
            {
                if (!m_IsCache)
                {
                    return;
                }
                if (m_Value)
                {
                    m_LastInputTime = Time.time;
                }
            }
        }

        public sealed class Vector2Key : Key<Vector2>
        {
            public override Vector2 GetState() => m_Value;
        }

        public enum InputType
        {
            Move,
            Run,
            Jump,
            Dodge,
            AttackLight,
            AttackHeavy,
            Special,
            SkillMenu,
            WeaponMenu,
        }

        public Vector2 moveInput => GetInputValue<Vector2>(InputType.Move);

        public bool isDashing => GetInputValue<bool>(InputType.Run);

        public bool isJumping => GetInputValue<bool>(InputType.Jump);

        public bool isAttackLight => GetInputValue<bool>(InputType.AttackLight);

        public bool isAttackHeavy => GetInputValue<bool>(InputType.AttackHeavy);

        public bool isDodge => GetInputValue<bool>(InputType.Dodge);

        public bool isSpecial => GetInputValue<bool>(InputType.Special);

        public bool isSkillMenu => GetInputValue<bool>(InputType.SkillMenu);

        public bool isWeaponMenu => GetInputValue<bool>(InputType.WeaponMenu);

        public bool isUIControl
        {
            get => m_IsUIControl;
            set
            {
                m_IsUIControl = value;
                if (m_IsUIControl)
                {
                    FrameToolsExtension.UnlockCursor();
                }
                else
                {
                    FrameToolsExtension.LockCursor();
                }
            }
        }

        private bool m_IsUIControl;

        protected override void Awake()
        {
            base.Awake();
            foreach (var key in m_InputConfig.inputConfigDic)
            {
                key.Value.Init();
            }
        }

        private void Update()
        {
            foreach (var key in m_InputConfig.inputConfigDic)
            {
                key.Value.Update();
            }
        }

        public void SetMoveInput(InputAction.CallbackContext ctx)
        {
            if (m_IsUIControl)
            {
                return;
            }
            SetInputValue(InputType.Move, ctx.ReadValue<Vector2>());
        }

        public void SetRunInput(InputAction.CallbackContext ctx)
        {
            if (m_IsUIControl)
            {
                return;
            }
            SetInputValue(InputType.Run, ctx.ReadValueAsButton());
        }

        public void SetAttackLightInput(InputAction.CallbackContext ctx)
        {
            if (m_IsUIControl)
            {
                return;
            }
            SetInputValue(InputType.AttackLight, ctx.ReadValueAsButton());
        }

        public void SetAttackHeavyInput(InputAction.CallbackContext ctx)
        {
            if (m_IsUIControl)
            {
                return;
            }
            SetInputValue(InputType.AttackHeavy, ctx.ReadValueAsButton());
        }

        public void SetSpecialInput(InputAction.CallbackContext ctx)
        {
            if (m_IsUIControl)
            {
                return;
            }
            SetInputValue(InputType.Special, ctx.ReadValueAsButton());
        }

        public void SetJumpInput(InputAction.CallbackContext ctx)
        {
            if (m_IsUIControl)
            {
                return;
            }
            SetInputValue(InputType.Jump, ctx.ReadValueAsButton());
        }

        public void SetDodgeInput(InputAction.CallbackContext ctx)
        {
            if (m_IsUIControl)
            {
                return;
            }
            SetInputValue(InputType.Dodge, ctx.ReadValueAsButton());
        }

        public void SetSkillMenuInput(InputAction.CallbackContext ctx)
        {
            SetInputValue(InputType.SkillMenu, ctx.ReadValueAsButton());
        }

        public void SetWeaponMenuInput(InputAction.CallbackContext ctx)
        {
            SetInputValue(InputType.WeaponMenu, ctx.ReadValueAsButton());
        }

        private T GetInputValue<T>(InputType type) where T : struct
        {
            if (m_InputConfig.inputConfigDic.TryGetValue(type, out var key))
            {
                if (key is Key<T> genericKey)
                {
                    return genericKey.GetState();
                }
                Debug.LogError($"Input type {type} is not matched.");
            }
            Debug.LogError("InputType not found");
            return default;
        }

        private void SetInputValue<T>(InputType type, T value) where T : struct
        {
            if (m_InputConfig.inputConfigDic.TryGetValue(type, out var key))
            {
                if (key is Key<T> genericKey)
                {
                    genericKey.SetResultValue(value);
                    return;
                }
                Debug.LogError($"Input type {type} is not matched.");
            }
            Debug.LogError("InputType not found");
        }

        public void ResetAllCacheTimer()
        {
            foreach (var key in m_InputConfig.inputConfigDic)
            {
                if (key.Value is Key<bool> boolKey)
                {
                    boolKey.ResetCacheTimer();
                }
            }
        }
    }
}