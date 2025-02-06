/*
 * @Author: AiUU
 * @Description: 技能行为测试类
 * @AkanyaTech.SkillMaster
 */

using System;

#if UNITY_EDITOR

namespace AkanyaTools.SkillMaster.Runtime.Behaviour
{
    public class SkillTempBehaviour : SkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new SkillTempBehaviour();
    }
}
#endif