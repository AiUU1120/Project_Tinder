using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCore.Character
{
    public sealed class TestPlaySkill : MonoBehaviour
    {
        [SerializeField]
        private SkillPlayer m_SkillPlayer;

        [FormerlySerializedAs("m_SkillConfig")]
        [SerializeField]
        private SkillClip m_SkillClip;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                // m_SkillPlayer.PlaySkill(m_SkillClip);
            }
        }
    }
}