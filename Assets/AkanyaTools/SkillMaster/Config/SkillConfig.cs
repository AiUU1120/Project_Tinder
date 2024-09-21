/*
* @Author: AiUU
* @Description: 技能配置
* @AkanyaTech.Tinder
*/

using FrameTools.Base.Config;
using UnityEngine;

namespace FrameTools.SkillMaster.Config
{
    [CreateAssetMenu(fileName = "SkillConfig_", menuName = "SkillMaster/Config/SkillConfig")]
    public sealed class SkillConfig : ConfigBase
    {
        public string skillName;
    }
}