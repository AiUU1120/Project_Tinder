/*
 * @Author: AiUU
 * @Description: 玩家镜头控制相关
 * @AkanyaTech.Tinder
 */

using Cinemachine;
using FrameTools.Base.Singleton;
using UnityEngine;

namespace GameCore.Common
{
    public sealed class PlayerCamera : SingletonMono<PlayerCamera>
    {
        [SerializeField]
        private CinemachineInputProvider m_InputProvider;

        public void LockCamera()
        {
            m_InputProvider.enabled = false;
        }

        public void UnlockCamera()
        {
            m_InputProvider.enabled = true;
        }
    }
}