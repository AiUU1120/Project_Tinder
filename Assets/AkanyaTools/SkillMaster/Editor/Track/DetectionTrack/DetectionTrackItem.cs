/*
* @Author: AiUU
* @Description: SkillMaster 判定轨道片段
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Editor.Track.Style;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.DetectionTrack
{
    public class DetectionTrackItem : TrackItemBase<DetectionTrack>
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