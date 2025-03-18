/*
 * @Author: AiUU
 * @Description: 特效管理器
 * @AkanyaTech.SkillMaster
 */

using FrameTools.Extension;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Component
{
    public class EffectController : MonoBehaviour
    {
        public float destroyTime;

        private float m_DestroyTimer;

        public void Init()
        {
            m_DestroyTimer = destroyTime;
        }

        private void Update()
        {
            m_DestroyTimer -= Time.deltaTime;
            if (m_DestroyTimer <= 0)
            {
                this.GameObjectPushPool();
            }
        }
    }
}