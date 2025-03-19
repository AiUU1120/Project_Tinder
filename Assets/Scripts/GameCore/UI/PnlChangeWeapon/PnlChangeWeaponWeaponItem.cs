using Data.GameCore.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI.PnlChangeWeapon
{
    public sealed class PnlChangeWeaponWeaponItem : MonoBehaviour
    {
        [SerializeField]
        private Image m_ImgBg;

        [SerializeField]
        private Image m_ImgIcon;

        [SerializeField]
        private TextMeshProUGUI m_TxtWeaponName;

        [SerializeField]
        private Color m_NormalColor = Color.white;

        [SerializeField]
        private Color m_SelectedColor = Color.yellow;

        public void Init(WeaponConfig weapon)
        {
            m_TxtWeaponName.text = weapon.weaponName;
            m_ImgIcon.sprite = weapon.icon;
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