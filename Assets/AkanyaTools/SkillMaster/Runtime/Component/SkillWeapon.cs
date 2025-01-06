/*
 * @Author: AiUU
 * @Description: 技能武器组件
 * @AkanyaTech.SkillMaster
 */

using System;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Component
{
    public sealed class SkillWeapon : MonoBehaviour
    {
        [SerializeField]
        private Collider m_DetectionCol;

        private LayerMask m_DetectionLayerMask;

        private Action<Collider> m_OnDetection;

        public void Init(LayerMask detectionLayerMask, Action<Collider> onDetection)
        {
            m_DetectionCol.enabled = false;
            m_DetectionLayerMask = detectionLayerMask;
            m_OnDetection = onDetection;
        }

        public void StartDetection()
        {
            m_DetectionCol.enabled = true;
        }

        public void StopDetection()
        {
            m_DetectionCol.enabled = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if ((m_DetectionLayerMask & 1 << other.gameObject.layer) > 0)
            {
                m_OnDetection?.Invoke(other);
            }
        }
    }
}