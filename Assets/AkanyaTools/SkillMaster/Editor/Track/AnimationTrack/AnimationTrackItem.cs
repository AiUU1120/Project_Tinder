/*
 * @Author: AiUU
 * @Description: SkillMaster 动画轨道片段
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Editor.Track.Style;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.AnimationTrack
{
    public sealed class AnimationTrackItem : TrackItemBase<AnimationTrack>
    {
        public SkillAnimationFrameEvent animationEvent { get; private set; }

        private AnimationTrackItemStyle m_TrackItemStyle;

        private bool m_IsMouseDrag;

        private float m_StartDragPosX;

        private int m_StartDragFrame;

        public void Init(AnimationTrack animationTrack, TrackStyleBase parentTrackStyle, int startFrameIndex, float frameUnitWidth, SkillAnimationFrameEvent e)
        {
            track = animationTrack;
            frameIndex = startFrameIndex;
            this.frameUnitWidth = frameUnitWidth;
            animationEvent = e;

            m_TrackItemStyle = new AnimationTrackItemStyle();
            m_TrackItemStyle.Init(parentTrackStyle, startFrameIndex, frameUnitWidth);
            itemStyle = m_TrackItemStyle;

            normalColor = new Color(track.themeColor.r, track.themeColor.g, track.themeColor.b, 0.7f);
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
            // 更新显示文本为动画片段名
            m_TrackItemStyle.SetTitle(animationEvent.animationClip.name);
            // 位置计算
            m_TrackItemStyle.SetPositionX(frameIndex * this.frameUnitWidth);
            // 宽度计算
            m_TrackItemStyle.SetWidth(animationEvent.durationFrame * frameUnitWidth);
            // 计算动画结束线位置
            var animationClipFrameCount = (int) (animationEvent.animationClip.length * animationEvent.animationClip.frameRate);
            if (animationClipFrameCount <= animationEvent.durationFrame)
            {
                m_TrackItemStyle.animationEndLine.style.display = DisplayStyle.Flex;
                var linePos = m_TrackItemStyle.animationEndLine.transform.position;
                linePos.x = animationClipFrameCount * frameUnitWidth - 1;
                m_TrackItemStyle.animationEndLine.transform.position = linePos;
            }
            // 长度大于持续时间则不显示
            else
            {
                m_TrackItemStyle.animationEndLine.style.display = DisplayStyle.None;
            }
            track.TickView(SkillMasterEditorWindow.instance.curSelectedFrameIndex);
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
            track.SetFrameIndex(m_StartDragFrame, frameIndex);
            SkillMasterInspector.instance.SetTrackItemFrameIndex(frameIndex);
        }

        /// <summary>
        /// 检查边界溢出
        /// </summary>
        public void CheckBoundaryOverflow()
        {
            // 超过右边界则拓展
            if (frameIndex + animationEvent.durationFrame > SkillMasterEditorWindow.instance.curFrameCount)
            {
                SkillMasterEditorWindow.instance.curFrameCount = frameIndex + animationEvent.durationFrame;
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
            if (targetFrame < 0)
            {
                return;
            }

            bool checkDrag;
            // 考虑拖动后是否与已有片段位置冲突
            if (offsetFrame < 0)
            {
                checkDrag = track.CheckFrame(targetFrame, m_StartDragFrame, true);
            }
            else if (offsetFrame > 0)
            {
                checkDrag = track.CheckFrame(targetFrame + animationEvent.durationFrame, m_StartDragFrame, false);
            }
            else
            {
                return;
            }

            if (!checkDrag)
            {
                return;
            }
            frameIndex = targetFrame;
            CheckBoundaryOverflow();
            RefreshView(frameUnitWidth);
        }

        public override void OnConfigChanged()
        {
            animationEvent = track.animationData.frameData[frameIndex];
        }

        #endregion
    }
}