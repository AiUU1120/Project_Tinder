/*
 * @Author: AiUU
 * @Description: 主场景加载初始化脚本
 * @AkanyaTech.Tinder
 */

using Data;
using GameCore.Character.Player;
using UnityEngine;

namespace GameCore.Common
{
    public sealed class GameMainEntry : MonoBehaviour
    {
        private void Start()
        {
#if UNITY_EDITOR
            DataManager.CreateSaveData();
#endif
            PlayerManager.instance.Init();
        }
    }
}