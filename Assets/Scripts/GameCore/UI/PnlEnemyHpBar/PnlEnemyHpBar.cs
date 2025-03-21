/*
 * @Author: AiUU
 * @Description: 怪物血量条UI
 * @AkanyaTech.Tinder
 */

using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI.PnlEnemyHpBar
{
    public sealed class PnlEnemyHpBar : MonoBehaviour
    {
        [SerializeField]
        private Image m_ImgHpBar;

        private bool m_IsShow;

        private void Start()
        {
            Hide();
        }

        private void LateUpdate()
        {
            if (m_IsShow && Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.forward);
            }
        }

        public void Show()
        {
            m_IsShow = true;
            GetComponent<Canvas>().enabled = true;
        }

        public void Hide()
        {
            m_IsShow = false;
            GetComponent<Canvas>().enabled = false;
        }

        public void SetHpBar(float fillAmount)
        {
            if (fillAmount < 1)
            {
                Show();
            }
            m_ImgHpBar.fillAmount = fillAmount;
        }
    }
}