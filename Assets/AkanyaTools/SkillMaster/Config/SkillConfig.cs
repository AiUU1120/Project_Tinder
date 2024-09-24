/*
* @Author: AiUU
* @Description: 技能配置
* @AkanyaTech.SkillMaster
*/

using FrameTools.Base.Config;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Config
{
    [CreateAssetMenu(fileName = "SkillConfig_", menuName = "SkillMaster/Config/SkillConfig")]
    public sealed class SkillConfig : ConfigBase
    {
        public string skillName;

        public int maxFrameCount = 100;
    }
}