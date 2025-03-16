/*
 * @Author: AiUU
 * @Description: 技能学习面板
 * @AkanyaTech.Tinder
 */

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.UISystem;
using Data;
using Data.GameCore;
using GameCore.Character.Player;
using JKFrame;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace GameCore.UI.PnlLearnSkill
{
    [UIWindowData(nameof(PnlLearnSkill), false, nameof(PnlLearnSkill), 1)]
    public sealed class PnlLearnSkill : GameCoreUIWindowBase
    {
        [SerializeField]
        private Transform m_ItemRoot;

        [SerializeField]
        private GameObject m_ItemPrefab;

        [SerializeField]
        private Button m_BtnBack;

        [SerializeField]
        private Button m_BtnLearn;

        [SerializeField]
        private Button m_BtnEquip;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillPoint;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillDesc;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillAtk;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillCD;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillPrice;

        [SerializeField]
        private TextMeshProUGUI m_TxtSkillEquip;

        private List<PnlLearnSkillSkillItem> m_SkillItems = new();

        private ItemInfo m_CurSelectItemInfo;

        private SkillLearnedDatas m_SkillLearnedDatas;

        private sealed class ItemInfo
        {
            public int skillIndex;
            public SkillConfig skillConfig;
            public SkillLearnedData skillLearnedData;
            public PnlLearnSkillSkillItem skillItem;
        }

        public override void Init()
        {
            m_BtnBack.onClick.AddListener(OnBtnBackClick);
            m_BtnLearn.onClick.AddListener(OnBtnLearnClick);
            m_BtnEquip.onClick.AddListener(OnBtnEquipClick);
        }

        public void Init(SkillLearnedDatas skillLearnedDatas)
        {
            m_SkillLearnedDatas = skillLearnedDatas;
            var skillConfigs = PlayerManager.instance.GetSkillConfigList();
            for (var i = 0; i < skillConfigs.Count; i++)
            {
                if (i < 2)
                {
                    continue;
                }
                var item = CreateItem();
                m_SkillLearnedDatas.learnedSkillsDic.Dictionary.TryGetValue(i, out var skillLearnedData);
                item.Init(skillConfigs[i], skillLearnedData);
                var itemInfo = new ItemInfo() { skillIndex = i, skillLearnedData = skillLearnedData, skillConfig = skillConfigs[i], skillItem = item };
                item.OnClickDown(OnSelectItem, itemInfo);
                m_SkillItems.Add(item);
                if (i == 2)
                {
                    OnSelectItem(null, itemInfo);
                }
            }
            UpdateSkillPoint(m_SkillLearnedDatas.skillPoint);
        }

        private PnlLearnSkillSkillItem CreateItem() => Instantiate(m_ItemPrefab, m_ItemRoot).GetComponent<PnlLearnSkillSkillItem>();

        private void UpdateSkillPoint(int count)
        {
            m_TxtSkillPoint.text = count.ToString();
        }

        private void UpdateSkillInfo(ItemInfo info)
        {
            var level = info.skillLearnedData?.level ?? 1;
            m_TxtSkillDesc.text = info.skillConfig.skillDescription;
            m_TxtSkillPrice.text = $"消耗点数: {info.skillConfig.skillPrice}";
            m_TxtSkillCD.text = $"冷却时间: {info.skillConfig.GetCDByLevel(level):F1} 秒";
            m_TxtSkillAtk.text = $"威力: {info.skillConfig.GetAtkByLevel(level)}";
            if (info.skillLearnedData != null && info.skillLearnedData.level >= info.skillConfig.maxLevel)
            {
                m_BtnLearn.interactable = false;
                m_TxtSkillPrice.text = "已满级";
            }
            else if (m_SkillLearnedDatas.skillPoint < info.skillConfig.skillPrice)
            {
                m_BtnLearn.interactable = false;
            }
            else
            {
                m_BtnLearn.interactable = true;
            }
            if (info.skillLearnedData == null || info.skillLearnedData.level < 1)
            {
                m_BtnEquip.interactable = false;
                m_TxtSkillEquip.text = "未获得";
            }
            else if (DataManager.playerData.shortcutSkillData.skillIndex == info.skillIndex)
            {
                m_BtnEquip.interactable = false;
                m_TxtSkillEquip.text = "已装备";
            }
            else
            {
                m_BtnEquip.interactable = true;
                m_TxtSkillEquip.text = "装备";
            }
        }

        #region Callback

        private void OnBtnBackClick()
        {
            UISystem.Close<PnlLearnSkill>();
        }

        private void OnBtnLearnClick()
        {
            if (!m_SkillLearnedDatas.learnedSkillsDic.Dictionary.TryGetValue(m_CurSelectItemInfo.skillIndex, out var skillLearnedData))
            {
                skillLearnedData = new SkillLearnedData
                {
                    level = 1
                };
                m_CurSelectItemInfo.skillLearnedData = skillLearnedData;
                m_SkillLearnedDatas.learnedSkillsDic.Dictionary.Add(m_CurSelectItemInfo.skillIndex, skillLearnedData);
                PlayerManager.instance.AddSkill(m_CurSelectItemInfo.skillIndex, skillLearnedData);
            }
            else
            {
                skillLearnedData.level++;
            }
            m_SkillLearnedDatas.skillPoint -= m_CurSelectItemInfo.skillConfig.skillPrice;
            UpdateSkillPoint(m_SkillLearnedDatas.skillPoint);
            m_CurSelectItemInfo.skillItem.Init(m_CurSelectItemInfo.skillConfig, skillLearnedData);
            UpdateSkillInfo(m_CurSelectItemInfo);
            DataManager.playerData.skillLearnedDatas = m_SkillLearnedDatas;
        }

        private void OnSelectItem(PointerEventData data, ItemInfo info)
        {
            if (m_CurSelectItemInfo == info)
            {
                return;
            }
            info.skillItem.Select();
            m_CurSelectItemInfo?.skillItem.UnSelect();
            m_CurSelectItemInfo = info;
            UpdateSkillInfo(info);
        }

        private void OnBtnEquipClick()
        {
            if (m_CurSelectItemInfo == null)
            {
                return;
            }
            DataManager.playerData.shortcutSkillData.skillIndex = m_CurSelectItemInfo.skillIndex;
            UISystem.GetWindow<PnlGameMain.PnlGameMain>().Init(DataManager.playerData.shortcutSkillData);
            UpdateSkillInfo(m_CurSelectItemInfo);
        }

        #endregion
    }
}