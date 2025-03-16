/*
 * @Author: AiUU
 * @Description: 主游戏中 UI 窗口基类
 * @AkanyaTech.Tinder
 */

using AkanyaTools.UISystem;
using Common.System;
using GameCore.Common;

namespace GameCore.UI
{
    public abstract class GameCoreUIWindowBase : UI_WindowBase
    {
        public override void OnShow()
        {
            InputManager.instance.isUIControl = true;
            PlayerCamera.instance.LockCamera();
        }

        public override void OnClose()
        {
            InputManager.instance.isUIControl = false;
            PlayerCamera.instance.UnlockCamera();
        }
    }
}