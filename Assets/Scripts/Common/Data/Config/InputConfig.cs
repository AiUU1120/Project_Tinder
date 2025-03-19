/*
 * @Author: AiUU
 * @Description: 输入配置
 * @AkanyaTech.Tinder
 */

using System.Collections.Generic;
using AkanyaTools.Base.Config;
using Common.System;
using Sirenix.Serialization;
using UnityEngine;

namespace Common.Data.Config
{
    [CreateAssetMenu(fileName = "InputConfig_", menuName = "Tinder/Config/System/InputConfig")]
    public sealed class InputConfig : ConfigBase
    {
        [OdinSerialize]
        public readonly Dictionary<InputManager.InputType, InputManager.KeyBase> inputConfigDic = new()
        {
            { InputManager.InputType.Move, new InputManager.Vector2Key() },
            { InputManager.InputType.Run, new InputManager.BoolKey() },
            { InputManager.InputType.Jump, new InputManager.BoolKey() },
            { InputManager.InputType.Dodge, new InputManager.BoolKey() },
            { InputManager.InputType.AttackLight, new InputManager.BoolKey() },
            { InputManager.InputType.AttackHeavy, new InputManager.BoolKey() },
            { InputManager.InputType.Special, new InputManager.BoolKey() },
            { InputManager.InputType.SkillMenu, new InputManager.BoolKey() },
            { InputManager.InputType.WeaponMenu, new InputManager.BoolKey() },
        };
    }
}