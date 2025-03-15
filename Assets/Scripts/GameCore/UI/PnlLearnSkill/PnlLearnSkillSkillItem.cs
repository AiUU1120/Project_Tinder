using AkanyaTools.SkillMaster.Runtime.Data.Config;
using Data.GameCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI.PnlLearnSkill
{
    public sealed class PnlLearnSkillSkillItem : MonoBehaviour
    {
        [SerializeField]
        private Image m_ImgBg;

        [SerializeField]
        private Image m_ImgIcon;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillName;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillLevel;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillType;

        [SerializeField]
        private Color m_NormalColor = Color.white;

        [SerializeField]
        private Color m_SelectedColor = Color.yellow;

        public void Init(SkillConfig skillConfig, SkillLearnedData skillLearnedData)
        {
            m_TxtSkillType.text = skillConfig.isPassive ? "被动" : "主动";
            m_TxtSkillName.text = skillConfig.skillName;
            m_ImgIcon.sprite = skillConfig.icon;
            if (skillLearnedData != null)
            {
                m_TxtSkillLevel.text = "Lv." + skillLearnedData.level;
            }
            else
            {
                m_TxtSkillLevel.text = "Lv.0";
            }
        }

        public void Select()
        {
            m_ImgBg.color = m_SelectedColor;
        }

        public void UnSelect()
        {
            m_ImgBg.color = m_NormalColor;
        }
    }
}