/*
 * @Author: AiUU
 * @Description: Buff 解析器
 * @AkanyaTech.BuffGuy
 */

using AkanyaTools.BuffGuy.Data;
using UnityEngine;

namespace AkanyaTools.BuffGuy.Core
{
    public abstract class BuffParserBase : MonoBehaviour
    {
        public abstract void Parse(BuffEntity buff, BuffEffectDataBase data);
    }
}