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
            protected bool isCache;

            [SerializeField]
            protected float cacheTime;

            protected T value;

            protected float lastInputTime;

            public virtual T GetState() => default;

            public void SetResultValue(T value)
            {
                this.value = value;
            }

            public override void Init()
            {
                lastInputTime = float.MinValue;
            }

            public override void Update()
            {
            }

            public void ResetCacheTimer()
            {
                lastInputTime = float.MinValue;
            }
        }

        public sealed class BoolKey : Key<bool>
        {
            public override bool GetState()
            {
                if (!isCache)
                {
                    return value;
                }
                return value || (Time.time - lastInputTime) < cacheTime;
            }

            public override void Update()
            {
                if (!isCache)
                {
                    return;
                }
                if (value)
                {
                    lastInputTime = Time.time;
                }
            }
        }

        public sealed class Vector2Key : Key<Vector2>
        {
            public override Vector2 GetState() => value;
        }

        public enum InputType
        {
            Move,
            Run,
            Jump,
            AttackLight,
            AttackHeavy,
            Special,
            SkillMenu
        }

        public Vector2 moveInput => GetInputValue<Vector2>(InputType.Move);

        public bool isDashing => GetInputValue<bool>(InputType.Run);

        public bool isJumping => GetInputValue<bool>(InputType.Jump);

        public bool isAttackLight => GetInputValue<bool>(InputType.AttackLight);

        public bool isAttackHeavy => GetInputValue<bool>(InputType.AttackHeavy);

        public bool isSpecial => GetInputValue<bool>(InputType.Special);

        public bool isSkillMenu => GetInputValue<bool>(InputType.SkillMenu);

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

        public void SetSkillMenuInput(InputAction.CallbackContext ctx)
        {
            SetInputValue(InputType.SkillMenu, ctx.ReadValueAsButton());
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