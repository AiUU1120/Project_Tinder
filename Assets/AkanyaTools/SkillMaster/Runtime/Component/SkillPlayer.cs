/*
 * @Author: AiUU
 * @Description: 技能播放器组件
 * @AkanyaTech.SkillMaster
 */

using System.Collections;
using System.Collections.Generic;
using AkanyaTools.AudioSystem;
using AkanyaTools.PlayableKami;
using AkanyaTools.ResourceSystem;
using AkanyaTools.SkillMaster.Runtime.Core;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Config;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using AkanyaTools.SkillMaster.Runtime.Tool;
using FrameTools.Extension;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Component
{
    public sealed class SkillPlayer : SerializedMonoBehaviour
    {
        [SerializeField]
        private Transform m_WeaponPoint;

        [SerializeField]
        private SkillWeaponsMapConfig m_SkillWeaponsMapConfig;

        [SerializeField]
        private GameObject m_DefaultWeaponPrefab;

        public bool isPlaying { get; private set; }

        public LayerMask atkDetectionLayerMask;

        public Transform modelTransform { get; private set; }

        public Transform weaponPointTransform => m_WeaponPoint;

        public SkillWeapon curWeapon => m_CurWeapon;

        private AnimationController m_AnimationController;

        private SkillClip m_SkillClip;

        private SkillWeapon m_CurWeapon;

        private int m_CurFrameIndex;

        private float m_FrameRate;

        private float m_PlayTotalTime;

        private SkillBehaviourBase m_CurSkillBehaviour;

        private ISkillCharacter m_SkillOwner;

        public void Init(ISkillCharacter skillOwner, AnimationController animationController, Transform modelTransform)
        {
            m_SkillOwner = skillOwner;
            m_AnimationController = animationController;
            this.modelTransform = modelTransform;

#if UNITY_EDITOR
            if (m_DefaultWeaponPrefab != null)
            {
                CreateWeaponOnWeaponPoint();
            }
#endif
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
                Clear();
                isPlaying = false;
                m_CurSkillBehaviour.OnSkillClipEnd();
            }
        }

        /// <summary>
        /// 将武器生成到角色武器抓取点并对齐
        /// </summary>
        public GameObject CreateWeaponOnWeaponPoint(GameObject weaponObj = null)
        {
            if (m_CurWeapon != null)
            {
                DestroyImmediate(m_CurWeapon.gameObject);
            }
            if (m_WeaponPoint == null)
            {
                Debug.LogWarning("未设置角色武器抓取点！");
                return null;
            }
            if (weaponObj == null)
            {
                if (m_DefaultWeaponPrefab == null)
                {
                    Debug.LogWarning("没有可生成的武器，缺乏预制体并传入空对象！");
                    return null;
                }
            }

            var weapon = Instantiate(weaponObj == null ? m_DefaultWeaponPrefab : weaponObj, m_WeaponPoint, true); // 不设置父对象
            m_CurWeapon = weapon.GetComponent<SkillWeapon>();
            m_CurWeapon.Init(atkDetectionLayerMask, OnWeaponDetection);
            var weaponGrabPoint = m_CurWeapon.mainGridPoint;

            // 获取抓取点相对于武器的本地位置和旋转
            var grabLocalPos = weaponGrabPoint.localPosition;
            var grabLocalRot = weaponGrabPoint.localRotation;

            // 计算目标旋转：手部旋转与抓取点本地旋转的逆相乘
            var targetRotation = m_WeaponPoint.rotation * Quaternion.Inverse(grabLocalRot);
            // 计算目标位置：手部位置减去旋转后的本地偏移
            var targetPosition = m_WeaponPoint.position - targetRotation * grabLocalPos;

            // 应用新的旋转和位置
            weapon.transform.rotation = targetRotation;
            weapon.transform.position = targetPosition;

            return weapon;
        }

        public void StartPlaySkillBehaviour(SkillBehaviourBase skillBehaviour)
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
            if (!m_SkillClip.skillCustomEventData.frameData.TryGetValue(m_CurFrameIndex, out var frameData))
            {
                return;
            }
            frameData = m_CurSkillBehaviour?.BeforeSkillCustomEventFrameEvent(frameData);
            if (frameData != null)
            {
                m_CurSkillBehaviour?.AfterSkillCustomEventFrameEvent(frameData);
            }
        }

        private void TickSkillAnimation()
        {
            if (!m_SkillClip.skillAnimationData.frameData.TryGetValue(m_CurFrameIndex, out var frameData))
            {
                return;
            }
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

        private void TickSkillAudio()
        {
            foreach (var data in m_SkillClip.skillAudioData.frameData)
            {
                var frameData = data;
                if (frameData == null)
                {
                    continue;
                }
                if (frameData.frameIndex != m_CurFrameIndex)
                {
                    continue;
                }
                frameData = m_CurSkillBehaviour?.BeforeSkillAudioFrameEvent(frameData);
                if (frameData != null && frameData.audioClip != null)
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
                if (frameData == null)
                {
                    continue;
                }
                if (frameData.frameIndex != m_CurFrameIndex)
                {
                    continue;
                }
                frameData = m_CurSkillBehaviour?.BeforeSkillEffectFrameEvent(frameData);
                if (frameData == null || frameData.effectPrefab == null)
                {
                    continue;
                }
                var effectObj = ResourceManager.GetOrInstantiateGameObject(frameData.effectPrefab);
                effectObj.transform.position = modelTransform.TransformPoint(frameData.positionOffset);
                effectObj.transform.rotation = Quaternion.Euler(modelTransform.eulerAngles + frameData.rotation);
                effectObj.transform.localScale = frameData.scale;
                if (frameData.autoDestroy)
                {
                    StartCoroutine(AutoDestroyEffectGameObject(effectObj, (float) frameData.durationFrame / m_SkillClip.frameRate));
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
                if (frameData == null)
                {
                    continue;
                }
                var detectionType = frameData.GetDetectionType();
                if (detectionType == DetectionType.Weapon)
                {
                    // 第一帧开启触发器
                    if (m_CurFrameIndex == frameData.frameIndex)
                    {
                        if (m_CurWeapon == null)
                        {
                            Debug.LogError("SkillMaster: Can't find weapon!");
                            continue;
                        }
                        frameData = m_CurSkillBehaviour?.BeforeSkillDetectionFrameEvent(frameData);
                        var attackData = new AttackData
                        {
                            e = frameData,
                            atkSource = m_SkillOwner,
                            atkValue = m_SkillOwner.GetAtkValue(frameData),
                        };
                        m_CurWeapon.StartDetection(attackData);
                    }
                    // 结束帧关闭触发器
                    else if (m_CurFrameIndex == frameData.frameIndex + frameData.durationFrame)
                    {
                        m_CurWeapon.StopDetection();
                        m_CurSkillBehaviour?.AfterSkillDetectionFrameEvent(frameData);
                    }
                }
                else
                {
                    if (m_CurFrameIndex < frameData.frameIndex || m_CurFrameIndex > frameData.frameIndex + frameData.durationFrame)
                    {
                        continue;
                    }
                    if (m_CurFrameIndex == frameData.frameIndex)
                    {
                        frameData = m_CurSkillBehaviour?.BeforeSkillDetectionFrameEvent(frameData);
                        if (frameData == null)
                        {
                            continue;
                        }
                    }
                    // 当前帧在范围内
                    var cols = SkillDetectionTool.ShapeDetection(transform, frameData.detectionData, detectionType, atkDetectionLayerMask);
                    if (cols == null)
                    {
                        break;
                    }
                    CheckDetectionCols(cols, frameData);
                    if (m_CurFrameIndex == frameData.frameIndex + frameData.durationFrame)
                    {
                        m_CurSkillBehaviour?.AfterSkillDetectionFrameEvent(frameData);
                    }
                }
#if UNITY_EDITOR
                if (!m_DrawAttackDetectionGizmos || frameData == null)
                {
                    continue;
                }
                if (m_CurFrameIndex >= frameData.frameIndex && m_CurFrameIndex <= frameData.frameIndex + frameData.durationFrame)
                {
                    m_DebugSkillDetectionFrameEvents.Add(frameData);
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

        private void OnWeaponDetection(IHitTarget hitTarget, AttackData attackData)
        {
            m_CurSkillBehaviour.OnAttackDetection(hitTarget, attackData);
        }

        private void Clear()
        {
            m_SkillClip = null;
        }

        private void CheckDetectionCols(Collider[] cols, SkillDetectionFrameEvent frameData)
        {
            foreach (var col in cols)
            {
                if (col == null)
                {
                    continue;
                }
                if ((atkDetectionLayerMask & 1 << col.gameObject.layer) <= 0)
                {
                    continue;
                }
                var hitTarget = col.GetComponentInChildren<IHitTarget>();
                if (hitTarget == null)
                {
                    continue;
                }
                var pos = ((ShapeDetectionDataBase) frameData.detectionData).position;
                var attackData = new AttackData
                {
                    e = frameData,
                    atkSource = m_SkillOwner,
                    hitPoint = col.ClosestPoint(transform.TransformPoint(pos)),
                    hitNormal = col.ClosestPoint(transform.TransformPoint(pos)) - col.gameObject.transform.position,
                    atkValue = m_SkillOwner.GetAtkValue(frameData),
                };
                m_CurSkillBehaviour.OnAttackDetection(hitTarget, attackData);
            }
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
                SkillGizmosTool.DrawDetectionGizmos(e, this, m_CurWeapon);
            }
        }
#endif

        #endregion
    }
}