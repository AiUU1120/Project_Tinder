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
using Data.GameCore;
using Data.GameCore.Config;
using FrameTools.Base.Singleton;
using GameCore.UI.PnlChangeWeapon;
using GameCore.UI.PnlGameMain;
using GameCore.UI.PnlLearnSkill;
using UnityEngine;

namespace GameCore.Character.Player
{
    public sealed class PlayerManager : SingletonMono<PlayerManager>
    {
        [SerializeField]
        private PlayerController m_PlayerController;

        public void Init()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            m_PlayerController.Init(DataManager.playerData);
            UISystem.Show<PnlGameMain>().Init(DataManager.playerData.shortcutSkillDataDic.Dictionary[DataManager.playerData.curWeaponId]);
        }

        private void Update()
        {
            if (InputManager.instance.isSkillMenu && UISystem.GetWindow<PnlLearnSkill>() == null)
            {
                UISystem.Show<PnlLearnSkill>().Init(DataManager.playerData.skillLearnedDatasDic.Dictionary[DataManager.playerData.curWeaponId]);
            }
            else if (InputManager.instance.isWeaponMenu && UISystem.GetWindow<PnlChangeWeapon>() == null)
            {
                UISystem.Show<PnlChangeWeapon>().Init(m_PlayerController.skillWeaponsMapConfig.skillWeaponsDic);
            }
        }

        /// <summary>
        /// 获取当前武器的所有技能配置 注意不包含轻重普攻
        /// </summary>
        /// <returns></returns>
        public List<SkillConfig> GetSkillConfigList() => m_PlayerController.weaponConfig.skillConfigs;

        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="skillIndex"></param>
        /// <param name="skillLearnedData"></param>
        public void AddSkill(int skillIndex, SkillLearnedData skillLearnedData)
        {
            m_PlayerController.skillBrain.AddSkill(m_PlayerController, GetSkillConfigList(), skillIndex, skillLearnedData);
        }

        public void ChangeWeapon(WeaponConfig weaponConfig)
        {
            m_PlayerController.ChangeWeapon(weaponConfig, DataManager.playerData.skillLearnedDatasDic.Dictionary[DataManager.playerData.curWeaponId]);
            UISystem.Show<PnlGameMain>().Init(DataManager.playerData.shortcutSkillDataDic.Dictionary[DataManager.playerData.curWeaponId]);
        }
    }
}