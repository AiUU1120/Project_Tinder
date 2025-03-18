/*
 * @Author: AiUU
 * @Description: 主游戏内 UI
 * @AkanyaTech.Tinder
 */

using AkanyaTools.UISystem;
using Data.GameCore;
using GameCore.Character.Player;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI.PnlGameMain
{
    [UIWindowData(nameof(PnlGameMain), true, nameof(PnlGameMain), 0)]
    public sealed class PnlGameMain : GameCoreUIWindowBase
    {
        [SerializeField]
        private Image m_ShortcutSkillIconImg;

        [SerializeField]
        private Image m_CDMaskImg;

        public override void OnShow()
        {
        }

        public void Init(ShortcutSkillData shortcutSkillData)
        {
            var skillConfigs = PlayerManager.instance.GetSkillConfigList();
            if (shortcutSkillData.skillIndex != -1 && skillConfigs.Count > shortcutSkillData.skillIndex)
            {
                var skillConfig = skillConfigs[shortcutSkillData.skillIndex];
                m_ShortcutSkillIconImg.gameObject.SetActive(true);
                m_ShortcutSkillIconImg.sprite = skillConfig.icon;
            }
            else
            {
                m_ShortcutSkillIconImg.gameObject.SetActive(false);
            }
            UpdateCDMask(0);
        }

        /// <summary>
        /// 更新技能 CD 遮罩
        /// </summary>
        /// <param name="fillAmount"></param>
        public void UpdateCDMask(float fillAmount)
        {
            m_CDMaskImg.fillAmount = fillAmount;
        }

        /// <summary>
        /// 更新技能栏状态
        /// </summary>
        /// <param name="canRelease"></param>
        public void UpdateSkillSlotState(bool canRelease)
        {
            m_ShortcutSkillIconImg.color = canRelease ? Color.white : Color.gray;
        }
    }
}