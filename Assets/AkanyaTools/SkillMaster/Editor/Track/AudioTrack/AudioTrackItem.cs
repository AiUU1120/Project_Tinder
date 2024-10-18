/*
* @Author: AiUU
* @Description: SkillMaster 音效轨道片段
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Event;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.AudioTrack
{
    public sealed class AudioTrackItem : TrackItemBase<AudioTrack>
    {
        public SkillAudioFrameEvent audioEvent { get; private set; }

        private MultiLineTrackStyle.SubTrackStyle m_SubTrackStyle;

        private AudioTrackItemStyle m_TrackItemStyle;

        private bool m_IsMouseDrag;

        private float m_StartDragPosX;

        private int m_StartDragFrame;

        public void Init(AudioTrack track, float frameUnitWidth, SkillAudioFrameEvent e, MultiLineTrackStyle.SubTrackStyle subTrackStyle)
        {
            this.track = track;
            frameIndex = e.frameIndex;
            audioEvent = e;
            m_SubTrackStyle = subTrackStyle;

            SetTrackName(e.trackName);

            m_TrackItemStyle = new AudioTrackItemStyle();
            itemStyle = m_TrackItemStyle;
            m_TrackItemStyle.Init(frameUnitWidth, audioEvent, m_SubTrackStyle);

            normalColor = m_TrackItemStyle.root.resolvedStyle.backgroundColor;
            selectedColor = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);

            OnUnSelect();

            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);

            m_SubTrackStyle.trackRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            m_SubTrackStyle.trackRoot.RegisterCallback<DragExitedEvent>(OnDragExited);

            RefreshView(frameUnitWidth);
        }

        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            m_TrackItemStyle.RefreshView(frameUnitWidth, audioEvent);
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
            // SkillMasterInspector.instance.SetTrackItemFrameIndex(frameIndex);
        }

        /// <summary>
        /// 检查边界溢出
        /// </summary>
        private void CheckBoundaryOverflow()
        {
            var frameCount = (int) (audioEvent.audioClip.length * SkillMasterEditorWindow.instance.skillConfig.frameRate);
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
            audioEvent.frameIndex = frameIndex;
            RefreshView(frameUnitWidth);
        }

        /// <summary>
        /// 监听用户音效拖拽
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            var objs = DragAndDrop.objectReferences;
            var clip = objs[0] as AudioClip;
            if (clip != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        /// <summary>
        /// 监听用户松手 放入音效片段
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragExited(DragExitedEvent evt)
        {
            var objs = DragAndDrop.objectReferences;
            var clip = objs[0] as AudioClip;
            if (clip == null)
            {
                return;
            }
            // 放置音效资源
            var selectFrameIndex = SkillMasterEditorWindow.instance.GetFrameIndexByPos(evt.localMousePosition.x);
            if (selectFrameIndex < 0)
            {
                return;
            }
            audioEvent.audioClip = clip;
            audioEvent.frameIndex = selectFrameIndex;
            audioEvent.playCount = 1;
            audioEvent.volume = 1;
            frameIndex = selectFrameIndex;
            RefreshView();
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        #endregion
    }
}