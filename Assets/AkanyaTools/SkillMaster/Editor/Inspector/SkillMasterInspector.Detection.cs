using System;
using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.DetectionTrack;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed partial class SkillMasterInspector
    {
        private List<string> m_DetectionSelections;

        private IntegerField m_DurationFrameField;

        private void DrawDetectionTrackItem(DetectionTrackItem detectionItem)
        {
            m_DurationFrameField = new IntegerField("Duration Frame")
            {
                value = detectionItem.detectionEvent.durationFrame
            };
            m_DurationFrameField.RegisterCallback<FocusInEvent>(OnDetectionDurationFieldFocusIn);
            m_DurationFrameField.RegisterCallback<FocusOutEvent>(OnDetectionDurationFieldFocusOut);
            m_Root.Add(m_DurationFrameField);

            m_DetectionSelections = new List<string>(Enum.GetNames(typeof(DetectionType)));
            var detectionDropdownField = new DropdownField("Detection Type", m_DetectionSelections, (int) detectionItem.detectionEvent.detectionType);
            detectionDropdownField.RegisterValueChangedCallback(OnDetectionDropdownFieldValueChanged);
            m_Root.Add(detectionDropdownField);

            // 根据不同的检测类型显示不同的内容
            switch (detectionItem.detectionEvent.detectionType)
            {
                case DetectionType.Weapon:
                    break;
                case DetectionType.Box:
                    var boxData = (BoxDetectionData) detectionItem.detectionEvent.detectionData;
                    var boxDetectionPositionField = new Vector3Field("Position")
                    {
                        value = boxData.position
                    };
                    boxDetectionPositionField.RegisterValueChangedCallback(OnShapeDetectionPositionFieldValueChanged);
                    m_Root.Add(boxDetectionPositionField);
                    var boxDetectionRotationField = new Vector3Field("Rotation")
                    {
                        value = boxData.rotation
                    };
                    boxDetectionRotationField.RegisterValueChangedCallback(OnBoxDetectionRotationFieldValueChanged);
                    m_Root.Add(boxDetectionRotationField);
                    var boxDetectionScaleField = new Vector3Field("Scale")
                    {
                        value = boxData.scale
                    };
                    boxDetectionScaleField.RegisterValueChangedCallback(OnBoxDetectionScaleFieldValueChanged);
                    m_Root.Add(boxDetectionScaleField);
                    break;
                case DetectionType.Sphere:
                    var sphereData = (SphereDetectionData) detectionItem.detectionEvent.detectionData;
                    var sphereDetectionPositionField = new Vector3Field("Position")
                    {
                        value = sphereData.position
                    };
                    sphereDetectionPositionField.RegisterValueChangedCallback(OnShapeDetectionPositionFieldValueChanged);
                    m_Root.Add(sphereDetectionPositionField);
                    var sphereDetectionRadiusField = new FloatField("Radius")
                    {
                        value = sphereData.radius
                    };
                    sphereDetectionRadiusField.RegisterValueChangedCallback(OnSphereDetectionRadiusFieldValueChanged);
                    m_Root.Add(sphereDetectionRadiusField);
                    break;
            }
        }

        #region Callback

        private void OnDetectionDurationFieldFocusIn(FocusInEvent evt)
        {
            m_OldAnimationDurationValue = m_DurationFrameField.value;
        }

        private void OnDetectionDurationFieldFocusOut(FocusOutEvent evt)
        {
            if (m_OldAnimationDurationValue == m_DurationFrameField.value)
            {
                return;
            }
            ((DetectionTrackItem) s_CurTrackItem).detectionEvent.durationFrame = m_DurationFrameField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
            s_CurTrackItem?.ForceRefreshView();
        }

        private void OnDetectionDropdownFieldValueChanged(ChangeEvent<string> evt)
        {
            var curItem = (DetectionTrackItem) s_CurTrackItem;
            curItem.detectionEvent.detectionType = (DetectionType) m_DetectionSelections.IndexOf(evt.newValue);
            SkillMasterEditorWindow.instance.SaveConfig();
            Refresh();
        }

        private void OnShapeDetectionPositionFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (ShapeDetectionDataBase) ((DetectionTrackItem) s_CurTrackItem).detectionEvent.detectionData;
            data.position = evt.newValue;
        }

        private void OnBoxDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (BoxDetectionData) ((DetectionTrackItem) s_CurTrackItem).detectionEvent.detectionData;
            data.rotation = evt.newValue;
        }

        private void OnBoxDetectionScaleFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (BoxDetectionData) ((DetectionTrackItem) s_CurTrackItem).detectionEvent.detectionData;
            data.scale = evt.newValue;
        }

        private void OnSphereDetectionRadiusFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SphereDetectionData) ((DetectionTrackItem) s_CurTrackItem).detectionEvent.detectionData;
            data.radius = evt.newValue;
        }

        #endregion
    }
}