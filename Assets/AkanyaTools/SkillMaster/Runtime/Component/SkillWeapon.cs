/*
 * @Author: AiUU
 * @Description: 技能武器组件
 * @AkanyaTech.SkillMaster
 */

using System;
using AkanyaTools.SkillMaster.Runtime.Data;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Component
{
    public sealed class SkillWeapon : MonoBehaviour
    {
        [SerializeField]
        private Collider m_DetectionCol;

        [SerializeField]
        private Transform m_MainGridPoint;

        [SerializeField]
        private Transform m_SecGridPoint;

        public Transform mainGridPoint => m_MainGridPoint;

        public Transform secGridPoint => m_SecGridPoint;

        private LayerMask m_DetectionLayerMask;

        private Action<IHitTarget, AttackData> m_OnDetection;

        private AttackData m_AttackData;

        public void Init(LayerMask detectionLayerMask, Action<IHitTarget, AttackData> onDetection)
        {
            m_DetectionCol.enabled = false;
            m_DetectionLayerMask = detectionLayerMask;
            m_OnDetection = onDetection;
        }

        public void StartDetection(AttackData attackData)
        {
            m_AttackData = attackData;
            m_DetectionCol.enabled = true;
        }

        public void StopDetection()
        {
            m_DetectionCol.enabled = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if ((m_DetectionLayerMask & 1 << other.gameObject.layer) <= 0)
            {
                return;
            }
            var hitTarget = other.GetComponentInChildren<IHitTarget>();
            if (hitTarget == null)
            {
                return;
            }
            m_AttackData.hitPoint = other.ClosestPoint(transform.position);
            m_AttackData.hitNormal = m_AttackData.hitPoint - other.transform.position;
            m_OnDetection?.Invoke(hitTarget, m_AttackData);
        }
    }
}