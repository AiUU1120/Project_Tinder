/*
 * @Author: AiUU
 * @Description: SkillMaster 特效数据监视器绘制
 * @AkanyaTech.SkillMaster
 */

using System;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.EffectTrack;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public class SkillEffectEventDataInspector : SkillEventDataInspectorBase<EffectTrackItem, EffectTrack>
    {
        private IntegerField m_EffectDurationFrameField;

        private float m_OldEffectDurationTimeValue;

        public override void OnDraw()
        {
            // 预制体资源
            var effectPrefabField = new ObjectField("Effect Prefab")
            {
                objectType = typeof(GameObject),
                value = m_TrackItem.effectEvent.effectPrefab
            };
            effectPrefabField.RegisterValueChangedCallback(OnEffectPrefabFieldValueChanged);
            m_Root.Add(effectPrefabField);

            // 偏移坐标
            var offsetField = new Vector3Field("Position Offset")
            {
                value = m_TrackItem.effectEvent.positionOffset
            };
            offsetField.RegisterValueChangedCallback(OnEffectOffsetFieldValueChanged);
            m_Root.Add(offsetField);

            // 旋转
            var rotationField = new Vector3Field("Rotation")
            {
                value = m_TrackItem.effectEvent.rotation
            };
            rotationField.RegisterValueChangedCallback(OnEffectRotationFieldFieldValueChanged);
            m_Root.Add(rotationField);

            // 缩放
            var scaleField = new Vector3Field("Scale")
            {
                value = m_TrackItem.effectEvent.scale
            };
            scaleField.RegisterValueChangedCallback(OnEffectScaleFieldFieldValueChanged);
            m_Root.Add(scaleField);

            // 自动销毁
            var autoDestroyToggle = new Toggle("Auto Destroy")
            {
                value = m_TrackItem.effectEvent.autoDestroy
            };
            autoDestroyToggle.RegisterValueChangedCallback(OnEffectAutoDestroyToggleValueChanged);
            m_Root.Add(autoDestroyToggle);

            // 持续时间
            m_EffectDurationFrameField = new IntegerField("Duration Frame")
            {
                value = m_TrackItem.effectEvent.durationFrame
            };
            m_EffectDurationFrameField.RegisterCallback<FocusInEvent>(OnEffectDurationFrameFieldFocusIn);
            m_EffectDurationFrameField.RegisterCallback<FocusOutEvent>(OnEffectDurationFrameFieldFocusOut);
            m_Root.Add(m_EffectDurationFrameField);

            // 计算时间
            var calculateBtn = new Button(CalculateEffectDurationTime)
            {
                text = "Calculate Duration Time"
            };
            m_Root.Add(calculateBtn);

            // 计算时间
            var applyModelTransBtn = new Button(ApplyEffectModelTransform)
            {
                text = "Apply Effect Model Transform"
            };
            m_Root.Add(applyModelTransBtn);

            // 设置持续帧数至选中帧
            var setFrameBtn = new Button
            {
                text = "Set Duration Frame To Selected Frame",
                style =
                {
                    backgroundColor = new Color(1f, 0f, 0f, 0.5f)
                },
                clickable = new Clickable(OnEffectSetFrameBtnClick),
            };
            m_Root.Add(setFrameBtn);
        }

        private void CalculateEffectDurationTime()
        {
            var item = m_TrackItem;
            var particles = item.effectEvent.effectPrefab.GetComponentsInChildren<ParticleSystem>();
            var maxDurationTime = -1f;
            foreach (var p in particles)
            {
                if (p.main.duration > maxDurationTime)
                {
                    maxDurationTime = p.main.duration;
                }
            }
            item.effectEvent.durationFrame = (int) (maxDurationTime * SkillMasterEditorWindow.instance.skillClip.frameRate);
            m_EffectDurationFrameField.value = item.effectEvent.durationFrame;
            SkillMasterEditorWindow.instance.SaveConfig();
            m_TrackItem.ForceRefreshView();
        }

        private void ApplyEffectModelTransform()
        {
            m_TrackItem.ApplyEffectModelTransform();
            SkillMasterInspector.instance.Refresh();
        }

        #region Callback

        private void OnEffectPrefabFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            var prefab = evt.newValue as GameObject;
            m_TrackItem.effectEvent.effectPrefab = prefab;
            CalculateEffectDurationTime();
            SkillMasterEditorWindow.instance.SaveConfig();
            m_TrackItem.ForceRefreshView();
            SkillMasterEditorWindow.instance.TickSkill();
        }

        private void OnEffectOffsetFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            m_TrackItem.effectEvent.positionOffset = evt.newValue;
            m_TrackItem.ForceRefreshView();
            SkillMasterEditorWindow.instance.TickSkill();
        }

        private void OnEffectRotationFieldFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            m_TrackItem.effectEvent.rotation = evt.newValue;
            m_TrackItem.ForceRefreshView();
            SkillMasterEditorWindow.instance.TickSkill();
        }

        private void OnEffectScaleFieldFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            m_TrackItem.effectEvent.scale = evt.newValue;
            m_TrackItem.ForceRefreshView();
            SkillMasterEditorWindow.instance.TickSkill();
        }

        private void OnEffectAutoDestroyToggleValueChanged(ChangeEvent<bool> evt)
        {
            m_TrackItem.effectEvent.autoDestroy = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnEffectDurationFrameFieldFocusIn(FocusInEvent evt)
        {
            m_OldEffectDurationTimeValue = m_EffectDurationFrameField.value;
        }

        private void OnEffectDurationFrameFieldFocusOut(FocusOutEvent evt)
        {
            if (Math.Abs(m_EffectDurationFrameField.value - m_OldEffectDurationTimeValue) < 0.00001f)
            {
                return;
            }
            m_TrackItem.effectEvent.durationFrame = m_EffectDurationFrameField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
            m_TrackItem.ForceRefreshView();
            SkillMasterEditorWindow.instance.TickSkill();
        }

        private void OnEffectSetFrameBtnClick()
        {
            OnEffectDurationFrameFieldFocusIn(null);
            m_TrackItem.effectEvent.durationFrame = m_EffectDurationFrameField.value;
            OnEffectDurationFrameFieldFocusOut(null);
        }

        #endregion
    }
}