/*
 * @Author: AiUU
 * @Description: 技能配置
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.Base.Config;
using AkanyaTools.SkillMaster.Runtime.Behaviour;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data.Config
{
    [CreateAssetMenu(fileName = "SkillConfig_", menuName = "SkillMaster/Config/SkillConfig")]
    public sealed class SkillConfig : ConfigBase
    {
        /// <summary>
        /// 全部技能片段
        /// </summary>
        public SkillClip[] clips;

        /// <summary>
        /// 技能行为 运行逻辑
        /// </summary>
        public SkillBehaviourBase skillBehaviour;
    }
}