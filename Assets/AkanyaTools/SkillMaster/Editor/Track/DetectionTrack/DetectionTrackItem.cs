﻿/*
* @Author: AiUU
* @Description: SkillMaster 判定轨道片段
* @AkanyaTech.SkillMaster
*/

using System;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Editor.Track.Style;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using AkanyaTools.SkillMaster.Runtime.Tool;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.DetectionTrack
{
    public sealed class DetectionTrackItem : TrackItemBase<DetectionTrack>
    {
        public SkillDetectionFrameEvent detectionEvent { get; private set; }

        private MultiLineTrackStyle.SubTrackStyle m_SubTrackStyle;

        private DetectionTrackItemStyle m_TrackItemStyle;

        private bool m_IsMouseDrag;

        private float m_StartDragPosX;

        private int m_StartDragFrame;

        public void Init(DetectionTrack track, float frameUnitWidth, SkillDetectionFrameEvent e, MultiLineTrackStyle.SubTrackStyle subTrackStyle)
        {
            this.track = track;
            frameIndex = e.frameIndex;
            detectionEvent = e;
            m_SubTrackStyle = subTrackStyle;

            SetTrackName(e.trackName);

            m_TrackItemStyle = new DetectionTrackItemStyle();
            itemStyle = m_TrackItemStyle;
            m_TrackItemStyle.Init(frameUnitWidth, detectionEvent, m_SubTrackStyle);

            normalColor = new Color(this.track.themeColor.r, this.track.themeColor.g, this.track.themeColor.b, 0.7f);
            selectedColor = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);

            OnUnSelect();

            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);

            RefreshView(frameUnitWidth);
        }

        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            m_TrackItemStyle.RefreshView(frameUnitWidth, detectionEvent);
        }

        private void SetTrackName(string name)
        {
            m_SubTrackStyle.SetTrackName(name);
        }

        public void Destroy()
        {
            m_SubTrackStyle.DeleteView();
        }

        /// <summary>
        /// 应用拖拽
        /// </summary>
        private void ApplyDrag()
        {
            if (m_StartDragFrame == frameIndex)
            {
                return;
            }
            SkillMasterInspector.instance.SetTrackItemFrameIndex(frameIndex);
        }

        /// <summary>
        /// 检查边界溢出
        /// </summary>
        private void CheckBoundaryOverflow()
        {
            var frameCount = detectionEvent.durationFrame;
            // 超过右边界则拓展
            if (frameIndex + frameCount > SkillMasterEditorWindow.instance.curFrameCount)
            {
                SkillMasterEditorWindow.instance.curFrameCount = frameIndex + frameCount;
            }
        }

        /// <summary>
        /// 绘制绿框
        /// </summary>
        public void DrawGizmos()
        {
            SkillGizmosTool.DrawDetectionGizmos(detectionEvent, SkillMasterEditorWindow.instance.curPreviewCharacterObj.GetComponent<SkillPlayer>());
        }

        /// <summary>
        /// 绘制 Handle
        /// </summary>
        public void DrawSceneGUI()
        {
            switch (detectionEvent.detectionType)
            {
                case DetectionType.Box:
                    var boxDetectionData = (BoxDetectionData) detectionEvent.detectionData;
                    var boxPosition = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.TransformPoint(boxDetectionData.position);
                    var boxRotation = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.rotation * Quaternion.Euler(boxDetectionData.rotation);
                    EditorGUI.BeginChangeCheck();
                    Handles.TransformHandle(ref boxPosition, ref boxRotation, ref boxDetectionData.scale);
                    // 如果发生了修改
                    if (EditorGUI.EndChangeCheck())
                    {
                        boxDetectionData.position = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.InverseTransformPoint(boxPosition);
                        boxDetectionData.rotation = (Quaternion.Inverse(SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.rotation) * boxRotation).eulerAngles;
                        SkillMasterInspector.SetTrackItem(this, track);
                    }
                    break;
                case DetectionType.Sphere:
                    var sphereDetectionData = (SphereDetectionData) detectionEvent.detectionData;
                    var oldSpherePosition = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.TransformPoint(sphereDetectionData.position);
                    var newSpherePosition = Handles.PositionHandle(oldSpherePosition, Quaternion.identity);
                    var newRadius = Handles.ScaleSlider(sphereDetectionData.radius, newSpherePosition, Vector3.up, Quaternion.identity, sphereDetectionData.radius + 0.2f, 0.1f);
                    if (oldSpherePosition != newSpherePosition || Math.Abs(sphereDetectionData.radius - newRadius) > 0.00001f)
                    {
                        sphereDetectionData.position = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.InverseTransformPoint(newSpherePosition);
                        sphereDetectionData.radius = newRadius;
                        SkillMasterInspector.SetTrackItem(this, track);
                    }
                    break;
                case DetectionType.Sector:
                    var sectorDetectionData = (SectorDetectionData) detectionEvent.detectionData;
                    var sectorPosition = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.TransformPoint(sectorDetectionData.position);
                    var sectorRotation = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.rotation * Quaternion.Euler(sectorDetectionData.rotation);
                    var sectorScale = new Vector3(sectorDetectionData.angle, sectorDetectionData.height, sectorDetectionData.outerRadius);
                    EditorGUI.BeginChangeCheck();
                    Handles.TransformHandle(ref sectorPosition, ref sectorRotation, ref sectorScale);
                    var innerRadiusHandle = Handles.ScaleSlider(sectorDetectionData.innerRadius, sectorPosition, -SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.forward,
                        Quaternion.identity, 0.5f, 0.1f);
                    // 如果发生了修改
                    if (EditorGUI.EndChangeCheck())
                    {
                        sectorDetectionData.position = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.InverseTransformPoint(sectorPosition);
                        sectorDetectionData.rotation = (Quaternion.Inverse(SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform.rotation) * sectorRotation).eulerAngles;
                        sectorDetectionData.angle = sectorScale.x;
                        sectorDetectionData.height = sectorScale.y;
                        sectorDetectionData.outerRadius = sectorScale.z;
                        sectorDetectionData.innerRadius = innerRadiusHandle;
                        SkillMasterInspector.SetTrackItem(this, track);
                    }
                    break;
            }
        }

        #region Callback

        private void OnMouseDown(MouseDownEvent evt)
        {
            m_IsMouseDrag = true;
            m_StartDragPosX = evt.mousePosition.x;
            m_StartDragFrame = frameIndex;
            Select();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (m_IsMouseDrag)
            {
                ApplyDrag();
            }
            m_IsMouseDrag = false;
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            if (m_IsMouseDrag)
            {
                ApplyDrag();
            }
            m_IsMouseDrag = false;
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!m_IsMouseDrag)
            {
                return;
            }
            var offsetPos = evt.mousePosition.x - m_StartDragPosX;
            var offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            var targetFrame = m_StartDragFrame + offsetFrame;
            if (targetFrame < 0 || offsetFrame == 0)
            {
                return;
            }
            frameIndex = targetFrame;
            detectionEvent.frameIndex = frameIndex;
            RefreshView(frameUnitWidth);
        }

        #endregion
    }
}