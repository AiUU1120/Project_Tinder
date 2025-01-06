/*
 * @Author: AiUU
 * @Description: 技能播放器组件
 * @AkanyaTech.SkillMaster
 */

using System;
using System.Collections;
using System.Collections.Generic;
using AkanyaTools.PlayableKami;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using AkanyaTools.SkillMaster.Runtime.Tool;
using FrameTools.AudioSystem;
using FrameTools.Extension;
using JKFrame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Component
{
    public sealed class SkillPlayer : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private Dictionary<string, SkillWeapon> m_SkillWeaponsDic = new();

        public Dictionary<string, SkillWeapon> skillWeaponsDic => m_SkillWeaponsDic;

        public bool isPlaying { get; private set; }

        public LayerMask atkDetectionLayerMask;

        public Transform modelTransform { get; private set; }

        private AnimationController m_AnimationController;

        private SkillConfig m_SkillConfig;

        private int m_CurFrameIndex;

        private float m_FrameRate;

        private float m_PlayTotalTime;

        private Action<Vector3, Quaternion> m_OnRootMotion;

        private Action m_OnSkillEnd;

        private Action<Collider> m_OnWeaponDetection;

        public void Init(AnimationController animationController, Transform modelTransform)
        {
            m_AnimationController = animationController;
            this.modelTransform = modelTransform;
            foreach (var weapon in skillWeaponsDic.Values)
            {
                weapon.Init(atkDetectionLayerMask, OnWeaponDetection);
            }
        }

        private void Update()
        {
            if (!isPlaying)
            {
                return;
            }

            m_PlayTotalTime += Time.deltaTime;
            var targetFrameIndex = (int) (m_PlayTotalTime * m_FrameRate);
            // 追帧
            while (m_CurFrameIndex < targetFrameIndex)
            {
                TickSkill();
            }

            if (targetFrameIndex >= m_SkillConfig.frameCount)
            {
                isPlaying = false;
                m_OnSkillEnd?.Invoke();
                Clear();
            }
        }

        /// <summary>
        /// 播放技能
        /// </summary>
        /// <param name="skillConfig">技能配置</param>
        /// <param name="skillEndAction">技能结束回调</param>
        /// <param name="onWeaponDetection">武器检测回调</param>
        /// <param name="rootMotionAction">根运动回调</param>
        public void PlaySkill(SkillConfig skillConfig, Action skillEndAction, Action<Collider> onWeaponDetection, Action<Vector3, Quaternion> rootMotionAction = null)
        {
            m_SkillConfig = skillConfig;
            m_OnSkillEnd = skillEndAction;
            m_OnWeaponDetection = onWeaponDetection;
            m_OnRootMotion = rootMotionAction;
            m_CurFrameIndex = -1;
            m_FrameRate = skillConfig.frameRate;
            m_PlayTotalTime = 0;
            isPlaying = true;
            TickSkill();
        }

        /// <summary>
        /// 驱动技能
        /// </summary>
        private void TickSkill()
        {
            if (m_AnimationController == null)
            {
                Debug.LogError("SkillMaster: AnimationController is null!");
                return;
            }
            m_CurFrameIndex++;
            // 驱动动画
            if (m_SkillConfig.skillAnimationData.frameData.TryGetValue(m_CurFrameIndex, out var frameData))
            {
                m_AnimationController.PlaySingleAnimation(frameData.animationClip, speed: 1f, blockSameAnim: false, mixingTime: frameData.transitionTime);
                if (frameData.applyRootMotion)
                {
                    m_AnimationController.SetOnRootMotion(m_OnRootMotion);
                }
                else
                {
                    m_AnimationController.ClearOnRootMotion();
                }
            }
            // 驱动音效
            foreach (var data in m_SkillConfig.skillAudioData.frameData)
            {
                if (data.audioClip != null && data.frameIndex == m_CurFrameIndex)
                {
                    AudioManager.PlayOneShot(data.audioClip, transform.position, volumeScale: data.volume);
                }
            }
            // 驱动特效
            foreach (var data in m_SkillConfig.skillEffectData.frameData)
            {
                if (data.effectPrefab != null && data.frameIndex == m_CurFrameIndex)
                {
                    var effectObj = PoolSystem.GetGameObject(data.effectPrefab.name);
                    if (effectObj == null)
                    {
                        effectObj = Instantiate(data.effectPrefab);
                        effectObj.name = data.effectPrefab.name;
                    }
                    effectObj.transform.position = modelTransform.TransformPoint(data.positionOffset);
                    effectObj.transform.rotation = Quaternion.Euler(modelTransform.eulerAngles + data.rotation);
                    effectObj.transform.localScale = data.scale;
                    if (data.autoDestroy)
                    {
                        StartCoroutine(AutoDestroyEffectGameObject(effectObj, data.durationTime));
                    }
                }
            }
#if UNITY_EDITOR
            if (m_DrawAttackDetectionGizmos)
            {
                m_DebugSkillDetectionFrameEvents.Clear();
            }
#endif
            // 驱动伤害检测
            foreach (var data in m_SkillConfig.skillDetectionData.frameData)
            {
                var detectionType = data.GetDetectionType();
                // 武器需要关注第一帧和结束帧
                if (detectionType == DetectionType.Weapon)
                {
                    if (data.frameIndex == m_CurFrameIndex)
                    {
                        // 驱动武器开启
                        var weaponDetectionData = (WeaponDetectionData) data.detectionData;
                        if (m_SkillWeaponsDic.TryGetValue(weaponDetectionData.weaponName, out var weapon))
                        {
                            weapon.StartDetection();
                        }
                        else
                        {
                            Debug.LogError($"SkillMaster: Can't find weapon {weaponDetectionData.weaponName}!");
                        }
                    }
                    if (m_CurFrameIndex == data.frameIndex + data.durationFrame)
                    {
                        // 武器关闭
                        var weaponDetectionData = (WeaponDetectionData) data.detectionData;
                        if (m_SkillWeaponsDic.TryGetValue(weaponDetectionData.weaponName, out var weapon))
                        {
                            weapon.StopDetection();
                        }
                        else
                        {
                            Debug.LogError($"SkillMaster: Can't find weapon {weaponDetectionData.weaponName}!");
                        }
                    }
                }
                else
                {
                    // 当前帧在范围内
                    if (m_CurFrameIndex >= data.frameIndex && m_CurFrameIndex <= data.frameIndex + data.durationFrame)
                    {
                        var cols = SkillDetectionTool.ShapeDetection(transform, data.detectionData, detectionType, atkDetectionLayerMask);
                        if (cols == null)
                        {
                            break;
                        }
                        foreach (var col in cols)
                        {
                            if (col != null)
                            {
                                m_OnWeaponDetection?.Invoke(col);
                            }
                        }
                    }
                }
#if UNITY_EDITOR
                if (m_DrawAttackDetectionGizmos)
                {
                    if (m_CurFrameIndex >= data.frameIndex && m_CurFrameIndex <= data.frameIndex + data.durationFrame)
                    {
                        m_DebugSkillDetectionFrameEvents.Add(data);
                    }
                }
#endif
            }
        }

        /// <summary>
        /// 自动销毁特效物体
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoDestroyEffectGameObject(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            obj.GameObjectPushPool();
        }

        private void OnWeaponDetection(Collider col)
        {
            m_OnWeaponDetection?.Invoke(col);
        }

        private void Clear()
        {
            if (m_OnRootMotion != null)
            {
                m_AnimationController.ClearOnRootMotion();
            }
            m_SkillConfig = null;
            m_OnSkillEnd = null;
            m_OnWeaponDetection = null;
            m_OnRootMotion = null;
        }

        #region Debug

#if UNITY_EDITOR
        [Header("======Debug======")]
        [SerializeField]
        private bool m_DrawAttackDetectionGizmos;

        private List<SkillDetectionFrameEvent> m_DebugSkillDetectionFrameEvents = new();
        private void OnDrawGizmos()
        {
            if (!m_DrawAttackDetectionGizmos || m_DebugSkillDetectionFrameEvents.Count == 0)
            {
                return;
            }
            foreach (var e in m_DebugSkillDetectionFrameEvents)
            {
                SkillGizmosTool.DrawDetectionGizmos(e, this);
            }
        }
#endif

        #endregion
    }
}