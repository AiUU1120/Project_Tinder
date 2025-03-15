/*
 * @Author: AiUU
 * @Description: 玩家 Buff 解析器
 * @AkanyaTech.Tinder
 */

using AkanyaTools.BuffGuy.Core;
using AkanyaTools.BuffGuy.Data;
using AkanyaTools.BuffGuy.Data.Enum;
using GameCore.Character.Player;
using UnityEngine;

namespace GameCore.Buff
{
    public sealed class PlayerBuffParser : BuffParserBase
    {
        [SerializeField]
        private PlayerController m_PlayerController;

        public override void Parse(BuffEntity buff, BuffEffectDataBase data)
        {
            if (data is SimpleBuffEffectData buffData)
            {
                switch (buffData.type)
                {
                    case BuffEffectType.Hp:
                        Debug.Log("Buff: " + buffData.type + " Value: " + buffData.value);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}