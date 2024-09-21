/*
* @Author: AiUU
* @Description: 主场景加载初始化脚本
* @AkanyaTech.Tinder
*/

using UnityEngine;

namespace GameCore.Common
{
    public sealed class GameMainEntry : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}