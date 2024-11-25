using System;
using System.Collections.Generic;
using System.Linq;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.DetectionTrack;
using AkanyaTools.SkillMaster.Runtime.Component;
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
                    var weaponData = (WeaponDetectionData) detectionItem.detectionEvent.detectionData;
                    var skillPlayer = SkillMasterEditorWindow.instance.curPreviewCharacterObj.GetComponent<SkillPlayer>();
                    var weaponDropdownField = new DropdownField("Weapon")
                    {
                        choices = skillPlayer.skillWeaponsDic.Keys.ToList(),
                    };
                    if (!string.IsNullOrEmpty(weaponData.weaponName) && skillPlayer.skillWeaponsDic.ContainsKey(weaponData.weaponName))
                    {
                        weaponDropdownField.value = weaponData.weaponName;
                    }
                    weaponDropdownField.RegisterValueChangedCallback(OnWeaponDropdownFieldValueChanged);
                    m_Root.Add(weaponDropdownField);
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
                case DetectionType.Sector:
                    var sectorData = (SectorDetectionData) detectionItem.detectionEvent.detectionData;
                    var sectorDetectionPositionField = new Vector3Field("Position")
                    {
                        value = sectorData.position
                    };
                    sectorDetectionPositionField.RegisterValueChangedCallback(OnShapeDetectionPositionFieldValueChanged);
                    m_Root.Add(sectorDetectionPositionField);
                    var sectorDetectionRotationField = new Vector3Field("Rotation")
                    {
                        value = sectorData.rotation
                    };
                    sectorDetectionRotationField.RegisterValueChangedCallback(OnSectorDetectionRotationFieldValueChanged);
                    m_Root.Add(sectorDetectionRotationField);
                    var sectorDetectionOuterRadiusField = new FloatField("OuterRadius")
                    {
                        value = sectorData.outerRadius
                    };
                    sectorDetectionOuterRadiusField.RegisterValueChangedCallback(OnSectorDetectionOuterRadiusFieldValueChanged);
                    m_Root.Add(sectorDetectionOuterRadiusField);
                    var sectorDetectionInnerRadiusField = new FloatField("InnerRadius")
                    {
                        value = sectorData.innerRadius
                    };
                    sectorDetectionInnerRadiusField.RegisterValueChangedCallback(OnSectorDetectionInnerRadiusFieldValueChanged);
                    m_Root.Add(sectorDetectionInnerRadiusField);
                    var sectorDetectionHeightField = new FloatField("Height")
                    {
                        value = sectorData.height
                    };
                    sectorDetectionHeightField.RegisterValueChangedCallback(OnSectorDetectionHeightFieldValueChanged);
                    m_Root.Add(sectorDetectionHeightField);
                    var sectorDetectionAngleField = new FloatField("Angle")
                    {
                        value = sectorData.angle
                    };
                    sectorDetectionAngleField.RegisterValueChangedCallback(OnSectorDetectionAngleFieldValueChanged);
                    m_Root.Add(sectorDetectionAngleField);
                    break;
            }
        }

        #region Callback

        private void OnWeaponDropdownFieldValueChanged(ChangeEvent<string> evt)
        {
            var data = (WeaponDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.weaponName = evt.newValue;
        }

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
            ((DetectionTrackItem) curTrackItem).detectionEvent.durationFrame = m_DurationFrameField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
            curTrackItem?.ForceRefreshView();
        }

        private void OnDetectionDropdownFieldValueChanged(ChangeEvent<string> evt)
        {
            var curItem = (DetectionTrackItem) curTrackItem;
            curItem.detectionEvent.detectionType = (DetectionType) m_DetectionSelections.IndexOf(evt.newValue);
            SkillMasterEditorWindow.instance.SaveConfig();
            Refresh();
        }

        private void OnShapeDetectionPositionFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (ShapeDetectionDataBase) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.position = evt.newValue;
        }

        private void OnBoxDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (BoxDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.rotation = evt.newValue;
        }

        private void OnBoxDetectionScaleFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (BoxDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.scale = evt.newValue;
        }

        private void OnSphereDetectionRadiusFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SphereDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.radius = evt.newValue;
        }

        private void OnSectorDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (SectorDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.rotation = evt.newValue;
        }

        private void OnSectorDetectionOuterRadiusFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.outerRadius = evt.newValue;
            if (data.outerRadius <= data.innerRadius)
            {
                data.innerRadius = data.outerRadius - 0.1f;
                Refresh();
            }
        }

        private void OnSectorDetectionInnerRadiusFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.innerRadius = evt.newValue;
            if (data.outerRadius <= data.innerRadius)
            {
                data.innerRadius = data.outerRadius - 0.1f;
                Refresh();
            }
        }

        private void OnSectorDetectionHeightFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.height = evt.newValue;
            if (data.height <= 0)
            {
                data.height = 0.1f;
                Refresh();
            }
        }

        private void OnSectorDetectionAngleFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
            data.angle = evt.newValue;
            if (data.angle < 0)
            {
                data.angle = 0.1f;
                Refresh();
            }
            else if (data.angle > 360)
            {
                data.angle = 360;
                Refresh();
            }
        }

        #endregion
    }
}