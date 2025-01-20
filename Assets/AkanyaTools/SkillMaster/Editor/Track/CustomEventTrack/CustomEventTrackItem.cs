/*
 * @Author: AiUU
 * @Description: SkillMaster 自定义事件轨道片段
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Editor.Track.Style;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.CustomEventTrack
{
    public sealed class CustomEventTrackItem : TrackItemBase<CustomEventTrack>
    {
        public static CustomEventTrackItem curSelectedTrackItem;

        public SkillCustomEventFrameEvent customEventEvent { get; private set; }

        private CustomEventTrackItemStyle m_TrackItemStyle;

        private bool m_IsMouseDrag;

        private float m_StartDragPosX;

        private float m_StartDragItemPosX;

        private int m_StartDragFrame;

        public void Init(CustomEventTrack customEventTrack, TrackStyleBase parentTrackStyle, int startFrameIndex, float frameUnitWidth, SkillCustomEventFrameEvent e)
        {
            track = customEventTrack;
            frameIndex = startFrameIndex;
            this.frameUnitWidth = frameUnitWidth;
            customEventEvent = e;

            m_TrackItemStyle = new CustomEventTrackItemStyle();
            m_TrackItemStyle.Init(parentTrackStyle);
            itemStyle = m_TrackItemStyle;

            normalColor = m_TrackItemStyle.root.resolvedStyle.backgroundColor;
            selectedColor = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);

            OnUnSelect();

            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);

            RefreshView(frameUnitWidth);
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        /// <param name="frameUnitWidth"></param>
        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            // 位置计算
            m_TrackItemStyle.SetPositionX(frameIndex * this.frameUnitWidth - frameUnitWidth * 0.5f);
            m_TrackItemStyle.SetWidth(frameUnitWidth);
        }

        /// <summary>
        /// 应用拖拽
        /// </summary>
        private void ApplyDrag()
        {
            if (m_StartDragFrame == frameIndex)
            {
                RefreshView(frameUnitWidth);
                return;
            }
            RefreshView(frameUnitWidth);
            track.SetFrameIndex(m_StartDragFrame, frameIndex);
            SkillMasterInspector.instance.SetTrackItemFrameIndex(frameIndex);
        }

        #region Callback

        private void OnMouseDown(MouseDownEvent evt)
        {
            m_IsMouseDrag = true;
            m_TrackItemStyle.root.BringToFront();
            m_StartDragPosX = evt.mousePosition.x;
            m_StartDragItemPosX = m_TrackItemStyle.root.transform.position.x;
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

            if (targetFrame < 0)
            {
                return;
            }

            if (track.CheckFrame(targetFrame))
            {
                frameIndex = targetFrame;
            }
            m_TrackItemStyle.SetPositionX(m_StartDragItemPosX + offsetPos);
        }

        public override void OnSelect()
        {
            curSelectedTrackItem = this;
            base.OnSelect();
        }

        public override void OnUnSelect()
        {
            curSelectedTrackItem = null;
            base.OnUnSelect();
        }

        #endregion
    }
}