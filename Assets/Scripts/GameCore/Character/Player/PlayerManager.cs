/*
 * @Author: AiUU
 * @Description: Player 顶层管理类
 * @AkanyaTech.Tinder
 */

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.UISystem;
using Common.System;
using Data;
using Data.GameCore.Config;
using FrameTools.Base.Singleton;
using FrameTools.ResourceSystem;
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
        }

        private void Update()
        {
            if (InputManager.instance.isSkillMenu && UISystem.GetWindow<PnlLearnSkill>() == null)
            {
                UISystem.Show<PnlLearnSkill>().Init(DataManager.playerData.skillLearnedDatas);
            }
        }

        /// <summary>
        /// 获取当前武器的所有技能配置
        /// </summary>
        /// <returns></returns>
        public List<SkillConfig> GetSkillConfigList() => weaponConfig.skillConfigs;
    }
}