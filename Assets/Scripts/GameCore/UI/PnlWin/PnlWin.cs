using AkanyaTools.UISystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameCore.UI.PnlWin
{
    [UIWindowData(nameof(PnlWin), false, nameof(PnlWin), 1)]
    public sealed class PnlWin : GameCoreUIWindowBase
    {
        [SerializeField]
        private Button m_BtnRestart;

        public override void Init()
        {
            m_BtnRestart.onClick.AddListener(OnBtnRestartClick);
        }

        private void OnBtnRestartClick()
        {
            SceneManager.LoadScene("GameMain");
        }
    }
}