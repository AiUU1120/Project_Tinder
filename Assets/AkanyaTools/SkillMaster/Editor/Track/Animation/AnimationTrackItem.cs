/*
* @Author: AiUU
* @Description: SkillMaster 动画轨道片段
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Scripts.Event;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Animation
{
    public sealed class AnimationTrackItem : TrackItemBase
    {
        public Label root { get; private set; }

        private const string track_item_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/Animation/AnimationTrackItem.uxml";

        private AnimationTrack m_AnimationTrack;

        private int m_FrameIndex;

        private float m_FrameUnitWidth;

        private SkillAnimationFrameEvent m_AnimationEvent;

        private VisualElement m_MainDragArea;

        private VisualElement m_AnimationEndLine;

        private Color m_NormalColor;

        private Color m_SelectedColor;

        private bool m_IsMouseDrag;

        private float m_StartDragPosX;

        private int m_StartDragFrame;

        public void Init(AnimationTrack animationTrack, VisualElement parent, int startFrameIndex, float frameUnitWidth, SkillAnimationFrameEvent e)
        {
            m_AnimationTrack = animationTrack;
            m_FrameIndex = startFrameIndex;
            m_FrameUnitWidth = frameUnitWidth;
            m_AnimationEvent = e;
            root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_item_path).Instantiate().Query<Label>();
            m_MainDragArea = root.NiceQ<VisualElement>("MainDragArea");
            m_AnimationEndLine = root.NiceQ<VisualElement>("AnimationEndLine");
            parent.Add(root);

            m_NormalColor = root.resolvedStyle.backgroundColor;
            m_SelectedColor = new Color(m_NormalColor.r, m_NormalColor.g, m_NormalColor.b, 1f);

            m_MainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            m_MainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            m_MainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);
            m_MainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);

            RefreshView(frameUnitWidth);
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        /// <param name="frameUnitWidth"></param>
        public void RefreshView(float frameUnitWidth)
        {
            m_FrameUnitWidth = frameUnitWidth;
            // 更新显示文本为动画片段名
            root.text = m_AnimationEvent.animationClip.name;
            // 位置计算
            var mainPos = root.transform.position;
            mainPos.x = m_FrameIndex * m_FrameUnitWidth;
            root.transform.position = mainPos;
            // 宽度计算
            root.style.width = m_AnimationEvent.durationFrame * frameUnitWidth;
            // 计算动画结束线位置
            var animationClipFrameCount = (int) (m_AnimationEvent.animationClip.length * m_AnimationEvent.animationClip.frameRate);
            if (animationClipFrameCount <= m_AnimationEvent.durationFrame)
            {
                m_AnimationEndLine.style.display = DisplayStyle.Flex;
                var linePos = m_AnimationEndLine.transform.position;
                linePos.x = animationClipFrameCount * frameUnitWidth - 1;
                m_AnimationEndLine.transform.position = linePos;
            }
            // 长度大于持续时间则不显示
            else
            {
                m_AnimationEndLine.style.display = DisplayStyle.None;
            }
        }

        private void ApplyDrag()
        {
            if (m_StartDragFrame == m_FrameIndex)
            {
                return;
            }
            m_AnimationTrack.SetFrameIndex(m_StartDragFrame, m_FrameIndex);
        }

        #region Callback

        private void OnMouseDown(MouseDownEvent evt)
        {
            root.style.backgroundColor = m_SelectedColor;
            m_IsMouseDrag = true;
            m_StartDragPosX = evt.mousePosition.x;
            m_StartDragFrame = m_FrameIndex;
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
            root.style.backgroundColor = m_NormalColor;
            if (m_IsMouseDrag)
            {
                ApplyDrag();
            }
            m_IsMouseDrag = false;
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (m_IsMouseDrag)
            {
                var offsetPos = evt.mousePosition.x - m_StartDragPosX;
                var offsetFrame = Mathf.RoundToInt(offsetPos / m_FrameUnitWidth);
                var targetFrame = m_StartDragFrame + offsetFrame;
                if (targetFrame < 0)
                {
                    return;
                }

                bool checkDrag;
                // 考虑拖动后是否与已有片段位置冲突
                if (offsetFrame < 0)
                {
                    checkDrag = m_AnimationTrack.CheckFrame(targetFrame);
                }
                else if (offsetFrame > 0)
                {
                    checkDrag = m_AnimationTrack.CheckFrame(targetFrame + m_AnimationEvent.durationFrame);
                }
                else
                {
                    return;
                }

                if (!checkDrag)
                {
                    return;
                }
                m_FrameIndex = targetFrame;
                // 超过右边界则拓展
                if (m_FrameIndex + m_AnimationEvent.durationFrame > SkillMasterEditorWindow.instance.curFrameCount)
                {
                    SkillMasterEditorWindow.instance.curFrameCount = m_FrameIndex + m_AnimationEvent.durationFrame;
                }
                RefreshView(m_FrameUnitWidth);
            }
        }

        #endregion
    }
}