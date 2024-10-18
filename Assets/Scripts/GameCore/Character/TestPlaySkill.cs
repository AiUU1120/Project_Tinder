using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using UnityEngine;

namespace GameCore.Character
{
    public sealed class TestPlaySkill : MonoBehaviour
    {
        [SerializeField]
        private SkillPlayer m_SkillPlayer;

        [SerializeField]
        private SkillConfig m_SkillConfig;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                // m_SkillPlayer.PlaySkill(m_SkillConfig);
            }
        }
    }
}