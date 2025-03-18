/*
 * @Author: AiUU
 * @Description: SkillMaster 检测数据监视器绘制
 * @AkanyaTech.SkillMaster
 */

using System;
using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.DetectionTrack;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed class SkillDetectionEventDataInspector : SkillEventDataInspectorBase<DetectionTrackItem, DetectionTrack>
    {
        private List<string> m_DetectionSelections;

        private IntegerField m_DetectionDurationFrameField;

        private int m_OldDetectionDurationValue;

        public override void OnDraw()
        {
            DrawDetection();
            DrawHitConfig();
        }

        private void DrawDetection()
        {
            m_DetectionDurationFrameField = new IntegerField("Duration Frame")
            {
                value = m_TrackItem.detectionEvent.durationFrame
            };
            m_DetectionDurationFrameField.RegisterCallback<FocusInEvent>(OnDetectionDurationFieldFocusIn);
            m_DetectionDurationFrameField.RegisterCallback<FocusOutEvent>(OnDetectionDurationFieldFocusOut);
            m_Root.Add(m_DetectionDurationFrameField);

            m_DetectionSelections = new List<string>(Enum.GetNames(typeof(DetectionType)));
            var detectionDropdownField = new DropdownField("Detection Type", m_DetectionSelections, (int) m_TrackItem.detectionEvent.detectionType);
            detectionDropdownField.RegisterValueChangedCallback(OnDetectionDropdownFieldValueChanged);
            m_Root.Add(detectionDropdownField);

            // 根据不同的检测类型显示不同的内容
            switch (m_TrackItem.detectionEvent.detectionType)
            {
                case DetectionType.Weapon:
                    // var weaponData = (WeaponDetectionData) detectionItem.detectionEvent.detectionData;
                    // var weaponDropdownField = new DropdownField("Weapon");
                    // if (SkillMasterEditorWindow.instance.curPreviewCharacterObj != null)
                    // {
                    //     var skillPlayer = SkillMasterEditorWindow.instance.curPreviewCharacterObj.GetComponent<SkillPlayer>();
                    //     {
                    //         weaponDropdownField.choices = skillPlayer.skillWeaponsDic.Keys.ToList();
                    //     }
                    // }
                    // if (!string.IsNullOrEmpty(weaponData.weaponName))
                    // {
                    //     weaponDropdownField.value = weaponData.weaponName;
                    // }
                    // weaponDropdownField.RegisterValueChangedCallback(OnWeaponDropdownFieldValueChanged);
                    // m_Root.Add(weaponDropdownField);
                    break;
                case DetectionType.Box:
                    var boxData = (BoxDetectionData) m_TrackItem.detectionEvent.detectionData;
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
                    var sphereData = (SphereDetectionData) m_TrackItem.detectionEvent.detectionData;
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
                    var sectorData = (SectorDetectionData) m_TrackItem.detectionEvent.detectionData;
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

            // 设置持续帧数至选中帧
            var setFrameBtn = new Button
            {
                text = "Set Duration Frame To Selected Frame",
                style =
                {
                    backgroundColor = new Color(1f, 0f, 0f, 0.5f)
                },
                clickable = new Clickable(OnDetectionSetFrameBtnClick),
            };
            m_Root.Add(setFrameBtn);
        }

        private void DrawHitConfig()
        {
            m_Root.Add(new Label());
            m_Root.Add(new Label("====== Hit Config ======"));
            var atkFactorField = new FloatField("Attack Factor")
            {
                value = m_TrackItem.detectionEvent.attackHitConfig.atkFactor
            };
            atkFactorField.RegisterValueChangedCallback(OnAtkFactorFieldValueChanged);
            m_Root.Add(atkFactorField);
            var repelForceField = new Vector3Field("Repel Force")
            {
                value = m_TrackItem.detectionEvent.attackHitConfig.repelForce
            };
            repelForceField.RegisterValueChangedCallback(OnRepelForceFieldValueChanged);
            m_Root.Add(repelForceField);
            var repelTimeField = new FloatField("Repel Time")
            {
                value = m_TrackItem.detectionEvent.attackHitConfig.repelTime
            };
            repelTimeField.RegisterValueChangedCallback(OnRepelTimeFieldValueChanged);
            m_Root.Add(repelTimeField);
            var hitEffectPrefabField = new ObjectField("Hit Effect Prefab")
            {
                objectType = typeof(GameObject),
                value = m_TrackItem.detectionEvent.attackHitConfig.hitEffectPrefab
            };
            hitEffectPrefabField.RegisterValueChangedCallback(OnHitEffectPrefabFieldValueChanged);
            m_Root.Add(hitEffectPrefabField);
            var hitAudioClipField = new ObjectField("Hit Audio Clip")
            {
                objectType = typeof(AudioClip),
                value = m_TrackItem.detectionEvent.attackHitConfig.hitAudioClip
            };
            hitAudioClipField.RegisterValueChangedCallback(OnHitAudioClipFieldValueChanged);
            m_Root.Add(hitAudioClipField);
        }

        #region Callback

        // private void OnWeaponDropdownFieldValueChanged(ChangeEvent<string> evt)
        // {
        //     var data = (WeaponDetectionData) ((DetectionTrackItem) curTrackItem).detectionEvent.detectionData;
        //     // data.weaponName = evt.newValue;
        // }

        private void OnDetectionDurationFieldFocusIn(FocusInEvent evt)
        {
            m_OldDetectionDurationValue = m_DetectionDurationFrameField.value;
        }

        private void OnDetectionDurationFieldFocusOut(FocusOutEvent evt)
        {
            if (m_OldDetectionDurationValue == m_DetectionDurationFrameField.value)
            {
                return;
            }
            m_TrackItem.detectionEvent.durationFrame = m_DetectionDurationFrameField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
            m_TrackItem?.ForceRefreshView();
        }

        private void OnDetectionDropdownFieldValueChanged(ChangeEvent<string> evt)
        {
            var curItem = m_TrackItem;
            curItem.detectionEvent.detectionType = (DetectionType) m_DetectionSelections.IndexOf(evt.newValue);
            SkillMasterEditorWindow.instance.SaveConfig();
            SkillMasterInspector.instance.Refresh();
        }

        private void OnShapeDetectionPositionFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (ShapeDetectionDataBase) m_TrackItem.detectionEvent.detectionData;
            data.position = evt.newValue;
        }

        private void OnBoxDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (BoxDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.rotation = evt.newValue;
        }

        private void OnBoxDetectionScaleFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (BoxDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.scale = evt.newValue;
        }

        private void OnSphereDetectionRadiusFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SphereDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.radius = evt.newValue;
        }

        private void OnSectorDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var data = (SectorDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.rotation = evt.newValue;
        }

        private void OnSectorDetectionOuterRadiusFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.outerRadius = evt.newValue;
            if (data.outerRadius <= data.innerRadius)
            {
                data.innerRadius = data.outerRadius - 0.1f;
                SkillMasterInspector.instance.Refresh();
            }
        }

        private void OnSectorDetectionInnerRadiusFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.innerRadius = evt.newValue;
            if (data.outerRadius <= data.innerRadius)
            {
                data.innerRadius = data.outerRadius - 0.1f;
                SkillMasterInspector.instance.Refresh();
            }
        }

        private void OnSectorDetectionHeightFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.height = evt.newValue;
            if (data.height <= 0)
            {
                data.height = 0.1f;
                SkillMasterInspector.instance.Refresh();
            }
        }

        private void OnSectorDetectionAngleFieldValueChanged(ChangeEvent<float> evt)
        {
            var data = (SectorDetectionData) m_TrackItem.detectionEvent.detectionData;
            data.angle = evt.newValue;
            if (data.angle < 0)
            {
                data.angle = 0.1f;
                SkillMasterInspector.instance.Refresh();
            }
            else if (data.angle > 360)
            {
                data.angle = 360;
                SkillMasterInspector.instance.Refresh();
            }
        }

        private void OnDetectionSetFrameBtnClick()
        {
            OnDetectionDurationFieldFocusIn(null);
            m_DetectionDurationFrameField.value = SkillMasterEditorWindow.instance.curSelectedFrameIndex - m_TrackItem.frameIndex;
            OnDetectionDurationFieldFocusOut(null);
        }

        private void OnAtkFactorFieldValueChanged(ChangeEvent<float> evt)
        {
            m_TrackItem.detectionEvent.attackHitConfig.atkFactor = evt.newValue;
        }

        private void OnRepelForceFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            m_TrackItem.detectionEvent.attackHitConfig.repelForce = evt.newValue;
        }

        private void OnRepelTimeFieldValueChanged(ChangeEvent<float> evt)
        {
            m_TrackItem.detectionEvent.attackHitConfig.repelTime = evt.newValue;
        }

        private void OnHitEffectPrefabFieldValueChanged(ChangeEvent<Object> evt)
        {
            m_TrackItem.detectionEvent.attackHitConfig.hitEffectPrefab = (GameObject) evt.newValue;
        }

        private void OnHitAudioClipFieldValueChanged(ChangeEvent<Object> evt)
        {
            m_TrackItem.detectionEvent.attackHitConfig.hitAudioClip = (AudioClip) evt.newValue;
        }

        #endregion
    }
}