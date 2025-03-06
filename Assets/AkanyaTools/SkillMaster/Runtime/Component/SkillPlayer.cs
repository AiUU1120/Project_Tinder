/*
 * @Author: AiUU
 * @Description: 技能播放器组件
 * @AkanyaTech.SkillMaster
 */

using System.Collections;
using System.Collections.Generic;
using AkanyaTools.AudioSystem;
using AkanyaTools.PlayableKami;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using AkanyaTools.SkillMaster.Runtime.Tool;
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

        private SkillClip m_SkillClip;

        private int m_CurFrameIndex;

        private float m_FrameRate;

        private float m_PlayTotalTime;

        private SkillBehaviourBase m_CurSkillBehaviour;

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

            if (targetFrameIndex >= m_SkillClip.frameCount)
            {
                isPlaying = false;
                m_CurSkillBehaviour.OnSkillClipEnd();
                Clear();
            }
        }

        public void StartPlaySkillConfig(SkillBehaviourBase skillBehaviour)
        {
            m_CurSkillBehaviour = skillBehaviour;
        }

        /// <summary>
        /// 播放技能片段
        /// </summary>
        /// <param name="skillClip">技能片段</param>
        public void PlaySkillClip(SkillClip skillClip)
        {
            m_SkillClip = skillClip;
            m_CurFrameIndex = -1;
            m_FrameRate = skillClip.frameRate;
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
            m_CurSkillBehaviour.OnTick(m_CurFrameIndex);
            // 驱动自定义事件
            TickSkillCustomEvent();
            // 驱动动画
            TickSkillAnimation();
            // 驱动音效
            TickSkillAudio();
            // 驱动特效
            TickSkillEffect();
            // 驱动伤害检测
            TickSkillDetection();
        }

        private void TickSkillCustomEvent()
        {
            if (m_SkillClip.skillCustomEventData.frameData.TryGetValue(m_CurFrameIndex, out var frameData))
            {
                frameData = m_CurSkillBehaviour?.BeforeSkillCustomEventFrameEvent(frameData);
                if (frameData != null)
                {
                    m_CurSkillBehaviour?.AfterSkillCustomEventFrameEvent(frameData);
                }
            }
        }

        private void TickSkillAnimation()
        {
            if (m_SkillClip.skillAnimationData.frameData.TryGetValue(m_CurFrameIndex, out var frameData))
            {
                frameData = m_CurSkillBehaviour?.BeforeSkillAnimationFrameEvent(frameData);
                if (frameData == null)
                {
                    return;
                }
                m_AnimationController.PlaySingleAnimation(frameData.animationClip, speed: 1f, blockSameAnim: false, mixingTime: frameData.transitionTime);
                if (frameData.applyRootMotion)
                {
                    m_AnimationController.SetOnRootMotion(m_CurSkillBehaviour.OnRootMotion);
                }
                else
                {
                    m_AnimationController.ClearOnRootMotion();
                }
                m_CurSkillBehaviour?.AfterSkillAnimationFrameEvent(frameData);
            }
        }

        private void TickSkillAudio()
        {
            foreach (var data in m_SkillClip.skillAudioData.frameData)
            {
                var frameData = data;
                frameData = m_CurSkillBehaviour?.BeforeSkillAudioFrameEvent(frameData);
                if (frameData == null)
                {
                    continue;
                }
                if (frameData.audioClip != null && frameData.frameIndex == m_CurFrameIndex)
                {
                    AudioManager.PlayOneShot(frameData.audioClip, transform.position, volumeScale: frameData.volume);
                }
                m_CurSkillBehaviour?.AfterSkillAudioFrameEvent(frameData);
            }
        }

        private void TickSkillEffect()
        {
            foreach (var data in m_SkillClip.skillEffectData.frameData)
            {
                var frameData = data;
                frameData = m_CurSkillBehaviour?.BeforeSkillEffectFrameEvent(frameData);
                if (frameData == null)
                {
                    continue;
                }
                if (frameData.effectPrefab != null && frameData.frameIndex == m_CurFrameIndex)
                {
                    var effectObj = PoolSystem.GetGameObject(frameData.effectPrefab.name);
                    if (effectObj == null)
                    {
                        effectObj = Instantiate(frameData.effectPrefab);
                        effectObj.name = frameData.effectPrefab.name;
                    }
                    effectObj.transform.position = modelTransform.TransformPoint(frameData.positionOffset);
                    effectObj.transform.rotation = Quaternion.Euler(modelTransform.eulerAngles + frameData.rotation);
                    effectObj.transform.localScale = frameData.scale;
                    if (frameData.autoDestroy)
                    {
                        StartCoroutine(AutoDestroyEffectGameObject(effectObj, (float) frameData.durationFrame / m_SkillClip.frameRate));
                    }
                }
                m_CurSkillBehaviour?.AfterSkillEffectFrameEvent(frameData);
            }
#if UNITY_EDITOR
            if (m_DrawAttackDetectionGizmos)
            {
                m_DebugSkillDetectionFrameEvents.Clear();
            }
#endif
        }

        private void TickSkillDetection()
        {
            foreach (var data in m_SkillClip.skillDetectionData.frameData)
            {
                var frameData = data;
                frameData = m_CurSkillBehaviour?.BeforeSkillDetectionFrameEvent(frameData);
                if (frameData == null)
                {
                    continue;
                }
                var detectionType = frameData.GetDetectionType();
                // 武器需要关注第一帧和结束帧
                if (detectionType == DetectionType.Weapon)
                {
                    if (frameData.frameIndex == m_CurFrameIndex)
                    {
                        // 驱动武器开启
                        var weaponDetectionData = (WeaponDetectionData) frameData.detectionData;
                        if (m_SkillWeaponsDic.TryGetValue(weaponDetectionData.weaponName, out var weapon))
                        {
                            weapon.StartDetection();
                        }
                        else
                        {
                            Debug.LogError($"SkillMaster: Can't find weapon {weaponDetectionData.weaponName}!");
                        }
                    }
                    if (m_CurFrameIndex == frameData.frameIndex + frameData.durationFrame)
                    {
                        // 武器关闭
                        var weaponDetectionData = (WeaponDetectionData) frameData.detectionData;
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
                    if (m_CurFrameIndex >= frameData.frameIndex && m_CurFrameIndex <= frameData.frameIndex + frameData.durationFrame)
                    {
                        var cols = SkillDetectionTool.ShapeDetection(transform, frameData.detectionData, detectionType, atkDetectionLayerMask);
                        if (cols == null)
                        {
                            break;
                        }
                        foreach (var col in cols)
                        {
                            if (col != null)
                            {
                                m_CurSkillBehaviour.OnAttackDetection(col);
                            }
                        }
                    }
                }
                m_CurSkillBehaviour?.AfterSkillDetectionFrameEvent(frameData);
#if UNITY_EDITOR
                if (m_DrawAttackDetectionGizmos)
                {
                    if (m_CurFrameIndex >= frameData.frameIndex && m_CurFrameIndex <= frameData.frameIndex + frameData.durationFrame)
                    {
                        m_DebugSkillDetectionFrameEvents.Add(frameData);
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
            m_CurSkillBehaviour.OnAttackDetection(col);
        }

        private void Clear()
        {
            m_SkillClip = null;
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