/*
 * @Author: AiUU
 * @Description: Player 顶层管理类
 * @AkanyaTech.Tinder
 */

using System.Collections.Generic;
using AkanyaTools.ResourceSystem;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.UISystem;
using Common.System;
using Data;
using Data.GameCore;
using Data.GameCore.Config;
using FrameTools.Base.Singleton;
using GameCore.UI.PnlGameMain;
using GameCore.UI.PnlLearnSkill;
using UnityEngine;

namespace GameCore.Character.Player
{
    public sealed class PlayerManager : SingletonMono<PlayerManager>
    {
        [SerializeField]
        private PlayerController m_PlayerController;

        public WeaponConfig weaponConfig { get; private set; }

        public void Init()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            weaponConfig = ResourceManager.LoadAsset<WeaponConfig>("WeaponConfig_Katana");
            m_PlayerController.Init(weaponConfig, DataManager.playerData);
            UISystem.Show<PnlGameMain>().Init(DataManager.playerData.shortcutSkillData);
        }

        private void Update()
        {
            if (InputManager.instance.isSkillMenu && UISystem.GetWindow<PnlLearnSkill>() == null)
            {
                UISystem.Show<PnlLearnSkill>().Init(DataManager.playerData.skillLearnedDatas);
            }
        }

        /// <summary>
        /// 获取当前武器的所有技能配置 注意不包含轻重普攻
        /// </summary>
        /// <returns></returns>
        public List<SkillConfig> GetSkillConfigList() => weaponConfig.skillConfigs;

        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="skillIndex"></param>
        /// <param name="skillLearnedData"></param>
        public void AddSkill(int skillIndex, SkillLearnedData skillLearnedData)
        {
            m_PlayerController.skillBrain.AddSkill(m_PlayerController, GetSkillConfigList(), skillIndex, skillLearnedData);
        }
    }
}