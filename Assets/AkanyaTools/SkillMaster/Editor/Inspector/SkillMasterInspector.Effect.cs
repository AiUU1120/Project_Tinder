using System;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.EffectTrack;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed partial class SkillMasterInspector
    {
        private FloatField m_EffectDurationTimeField;

        private float m_OldEffectDurationTimeValue;

        /// <summary>
        /// 绘制监视器显示内容
        /// </summary>
        /// <param name="item"></param>
        private void DrawEffectTrackItem(EffectTrackItem item)
        {
            // 预制体资源
            var effectPrefabField = new ObjectField("Effect Prefab")
            {
                objectType = typeof(GameObject),
                value = item.effectEvent.effectPrefab
            };
            effectPrefabField.RegisterValueChangedCallback(OnEffectPrefabFieldValueChanged);
            m_Root.Add(effectPrefabField);

            // 偏移坐标
            var offsetField = new Vector3Field("Position Offset")
            {
                value = item.effectEvent.positionOffset
            };
            offsetField.RegisterValueChangedCallback(OnEffectOffsetFieldValueChanged);
            m_Root.Add(offsetField);

            // 旋转
            var rotationField = new Vector3Field("Rotation")
            {
                value = item.effectEvent.rotation
            };
            rotationField.RegisterValueChangedCallback(OnEffectRotationFieldFieldValueChanged);
            m_Root.Add(rotationField);

            // 缩放
            var scaleField = new Vector3Field("Scale")
            {
                value = item.effectEvent.scale
            };
            scaleField.RegisterValueChangedCallback(OnEffectScaleFieldFieldValueChanged);
            m_Root.Add(scaleField);

            // 自动销毁
            var autoDestroyToggle = new Toggle("Auto Destroy")
            {
                value = item.effectEvent.autoDestroy
            };
            autoDestroyToggle.RegisterValueChangedCallback(OnEffectAutoDestroyToggleValueChanged);
            m_Root.Add(autoDestroyToggle);

            // 持续时间
            m_EffectDurationTimeField = new FloatField("Duration Time")
            {
                value = item.effectEvent.durationTime
            };
            m_EffectDurationTimeField.RegisterCallback<FocusInEvent>(OnEffectDurationTimeFieldFocusIn);
            m_EffectDurationTimeField.RegisterCallback<FocusOutEvent>(OnEffectDurationTimeFieldFocusOut);
            m_Root.Add(m_EffectDurationTimeField);

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
        }

        private void CalculateEffectDurationTime()
        {
            var item = (EffectTrackItem) curTrackItem;
            var particles = item.effectEvent.effectPrefab.GetComponentsInChildren<ParticleSystem>();
            var maxDurationTime = -1f;
            foreach (var p in particles)
            {
                if (p.main.duration > maxDurationTime)
                {
                    maxDurationTime = p.main.duration;
                }
            }
            item.effectEvent.durationTime = maxDurationTime;
            m_EffectDurationTimeField.value = item.effectEvent.durationTime;
            SkillMasterEditorWindow.instance.SaveConfig();
            curTrackItem.ForceRefreshView();
        }

        private void ApplyEffectModelTransform()
        {
            ((EffectTrackItem) curTrackItem).ApplyEffectModelTransform();
            Refresh();
        }

        #region Callback

        private void OnEffectPrefabFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            var prefab = evt.newValue as GameObject;
            ((EffectTrackItem) curTrackItem).effectEvent.effectPrefab = prefab;
            CalculateEffectDurationTime();
            SkillMasterEditorWindow.instance.SaveConfig();
            ((EffectTrackItem) curTrackItem).ForceRefreshView();
        }

        private void OnEffectOffsetFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            ((EffectTrackItem) curTrackItem).effectEvent.positionOffset = evt.newValue;
            ((EffectTrackItem) curTrackItem).ForceRefreshView();
        }

        private void OnEffectRotationFieldFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            ((EffectTrackItem) curTrackItem).effectEvent.rotation = evt.newValue;
            ((EffectTrackItem) curTrackItem).ForceRefreshView();
        }

        private void OnEffectScaleFieldFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            ((EffectTrackItem) curTrackItem).effectEvent.scale = evt.newValue;
            ((EffectTrackItem) curTrackItem).ForceRefreshView();
        }

        private void OnEffectAutoDestroyToggleValueChanged(ChangeEvent<bool> evt)
        {
            ((EffectTrackItem) curTrackItem).effectEvent.autoDestroy = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnEffectDurationTimeFieldFocusIn(FocusInEvent evt)
        {
            m_OldEffectDurationTimeValue = m_EffectDurationTimeField.value;
        }

        private void OnEffectDurationTimeFieldFocusOut(FocusOutEvent evt)
        {
            if (Math.Abs(m_EffectDurationTimeField.value - m_OldEffectDurationTimeValue) < 0.00001f)
            {
                return;
            }
            ((EffectTrackItem) curTrackItem).effectEvent.durationTime = m_EffectDurationTimeField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
            curTrackItem.ForceRefreshView();
        }

        #endregion
    }
}