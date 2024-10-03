/*
* @Author: AiUU
* @Description: SkillMaster 动画轨道片段
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Scripts.Event;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Animation
{
    public sealed class AnimationTrackItem : TrackItemBase<AnimationTrack>
    {
        public SkillAnimationFrameEvent animationEvent { get; private set; }

        private const string track_item_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/Animation/AnimationTrackItem.uxml";

        private VisualElement m_MainDragArea;

        private VisualElement m_AnimationEndLine;

        private bool m_IsMouseDrag;

        private float m_StartDragPosX;

        private int m_StartDragFrame;

        public void Init(AnimationTrack animationTrack, VisualElement parent, int startFrameIndex, float frameUnitWidth, SkillAnimationFrameEvent e)
        {
            track = animationTrack;
            frameIndex = startFrameIndex;
            this.frameUnitWidth = frameUnitWidth;
            animationEvent = e;
            root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_item_path).Instantiate().Query<Label>();
            m_MainDragArea = root.NiceQ<VisualElement>("MainDragArea");
            m_AnimationEndLine = root.NiceQ<VisualElement>("AnimationEndLine");
            parent.Add(root);

            normalColor = root.resolvedStyle.backgroundColor;
            selectedColor = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);

            OnUnSelect();

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
        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            // 更新显示文本为动画片段名
            root.text = animationEvent.animationClip.name;
            // 位置计算
            var mainPos = root.transform.position;
            mainPos.x = frameIndex * this.frameUnitWidth;
            root.transform.position = mainPos;
            // 宽度计算
            root.style.width = animationEvent.durationFrame * frameUnitWidth;
            // 计算动画结束线位置
            var animationClipFrameCount = (int) (animationEvent.animationClip.length * animationEvent.animationClip.frameRate);
            if (animationClipFrameCount <= animationEvent.durationFrame)
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
            if (m_IsMouseDrag)
            {
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
        }

        public override void OnConfigChanged()
        {
            animationEvent = track.animationData.frameData[frameIndex];
        }

        #endregion
    }
}