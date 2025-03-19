using System.Collections.Generic;
using AkanyaTools.UISystem;
using Data;
using Data.GameCore.Config;
using GameCore.Character.Player;
using JKFrame;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.UI.PnlChangeWeapon
{
    [UIWindowData(nameof(PnlChangeWeapon), false, nameof(PnlChangeWeapon), 1)]
    public sealed class PnlChangeWeapon : GameCoreUIWindowBase
    {
        [SerializeField]
        private Transform m_ItemRoot;

        [SerializeField]
        private GameObject m_ItemPrefab;

        [SerializeField]
        private Button m_BtnBack;

        [SerializeField]
        private Button m_BtnEquip;

        [SerializeField]
        private TextMeshProUGUI m_TxtWeaponDesc;

        [SerializeField]
        private TextMeshProUGUI m_TxtWeaponEquip;

        private ItemInfo m_CurSelectItemInfo;

        private Dictionary<string, WeaponConfig> m_WeaponDic;

        private sealed class ItemInfo
        {
            public string weaponId;
            public WeaponConfig weaponConfig;
            public PnlChangeWeaponWeaponItem weaponItem;
        }

        public override void Init()
        {
            m_BtnBack.onClick.AddListener(OnBtnBackClick);
            m_BtnEquip.onClick.AddListener(OnBtnEquipClick);
        }

        public void Init(Dictionary<string, WeaponConfig> weaponDic)
        {
            m_WeaponDic = weaponDic;
            foreach (var weapon in m_WeaponDic)
            {
                var item = CreateItem();
                item.Init(weapon.Value);
                var itemInfo = new ItemInfo() { weaponId = weapon.Key, weaponConfig = weapon.Value, weaponItem = item };
                item.OnClickDown(OnSelectItem, itemInfo);
                if (m_CurSelectItemInfo == null)
                {
                    OnSelectItem(null, itemInfo);
                }
            }
        }

        private PnlChangeWeaponWeaponItem CreateItem() => Instantiate(m_ItemPrefab, m_ItemRoot).GetComponent<PnlChangeWeaponWeaponItem>();

        private void UpdateWeaponInfo(ItemInfo info)
        {
            m_TxtWeaponDesc.text = info.weaponConfig.weaponDescription;
            if (DataManager.playerData.curWeaponId == info.weaponId)
            {
                m_BtnEquip.interactable = false;
                m_TxtWeaponEquip.text = "已装备";
            }
            else
            {
                m_BtnEquip.interactable = true;
                m_TxtWeaponEquip.text = "装备";
            }
        }

        #region Callback

        private void OnBtnBackClick()
        {
            UISystem.Close<PnlChangeWeapon>();
        }

        private void OnSelectItem(PointerEventData data, ItemInfo info)
        {
            if (m_CurSelectItemInfo == info)
            {
                return;
            }
            info.weaponItem.Select();
            m_CurSelectItemInfo?.weaponItem.UnSelect();
            m_CurSelectItemInfo = info;
            UpdateWeaponInfo(info);
        }

        private void OnBtnEquipClick()
        {
            if (m_CurSelectItemInfo == null)
            {
                return;
            }
            DataManager.playerData.curWeaponId = m_CurSelectItemInfo.weaponId;
            UpdateWeaponInfo(m_CurSelectItemInfo);
            PlayerManager.instance.ChangeWeapon(m_CurSelectItemInfo.weaponConfig);
        }

        #endregion
    }
}