using System;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.AnimationTrack;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed partial class SkillMasterInspector
    {
        private IntegerField m_AnimationDurationField;

        private FloatField m_AnimationTransitionField;

        private Label m_AnimationClipFrameLabel;

        private Label m_AnimationIsLoopLabel;

        private Toggle m_AnimationRootMotionToggle;

        private int m_OldAnimationDurationValue;

        private int m_OldAnimationTransitionValue;

        /// <summary>
        /// 绘制监视器显示内容
        /// </summary>
        /// <param name="item"></param>
        private void DrawAnimationTrackItem(AnimationTrackItem item)
        {
            m_TrackItemFrameIndex = item.frameIndex;

            // 动画资源
            var animationClipAssetField = new ObjectField("Animation Clip")
            {
                objectType = typeof(AnimationClip),
                value = item.animationEvent.animationClip
            };
            animationClipAssetField.RegisterValueChangedCallback(OnAnimationClipAssetFieldValueChanged);
            m_Root.Add(animationClipAssetField);

            // 根运动
            m_AnimationRootMotionToggle = new Toggle("Apply Root Motion")
            {
                value = item.animationEvent.applyRootMotion
            };
            m_AnimationRootMotionToggle.RegisterValueChangedCallback(OnAnimationRootMotionToggleValueChanged);
            m_Root.Add(m_AnimationRootMotionToggle);

            // 轨道长度
            m_AnimationDurationField = new IntegerField("Duration Frame")
            {
                value = item.animationEvent.durationFrame
            };
            m_AnimationDurationField.RegisterCallback<FocusInEvent>(OnAnimationDurationFieldFocusIn);
            m_AnimationDurationField.RegisterCallback<FocusOutEvent>(OnAnimationDurationFieldFocusOut);
            m_Root.Add(m_AnimationDurationField);

            // 过渡时间
            m_AnimationTransitionField = new FloatField("Transition Time")
            {
                value = item.animationEvent.transitionTime
            };
            m_AnimationTransitionField.RegisterCallback<FocusInEvent>(OnAnimationTransitionFieldFocusIn);
            m_AnimationTransitionField.RegisterCallback<FocusOutEvent>(OnAnimationTransitionFieldFocusOut);
            m_Root.Add(m_AnimationTransitionField);

            // 动画信息
            var clipFrameCount = (int) (item.animationEvent.animationClip.length * item.animationEvent.animationClip.frameRate);
            m_AnimationClipFrameLabel = new Label("Frame Count: " + clipFrameCount);
            m_Root.Add(m_AnimationClipFrameLabel);
            m_AnimationIsLoopLabel = new Label("Is Loop: " + item.animationEvent.animationClip.isLooping);
            m_Root.Add(m_AnimationIsLoopLabel);

            // 删除
            var deleteBtn = new Button
            {
                text = "Delete",
                style =
                {
                    backgroundColor = new Color(1f, 0f, 0f, 0.5f)
                },
                clickable = new Clickable(OnAnimationDeleteBtnClick),
            };
            m_Root.Add(deleteBtn);

            // 设置持续帧数至选中帧
            var setFrameBtn = new Button
            {
                text = "Set Duration Frame To Selected Frame",
                style =
                {
                    backgroundColor = new Color(1f, 0f, 0f, 0.5f)
                },
                clickable = new Clickable(OnAnimationSetFrameBtnClick),
            };
            m_Root.Add(setFrameBtn);
        }

        #region Callback

        private void OnAnimationClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            var clip = evt.newValue as AnimationClip;
            Debug.Assert(clip != null, nameof(clip) + " != null");
            m_AnimationClipFrameLabel.text = "Frame Count: " + (int) (clip.length * clip.frameRate);
            m_AnimationIsLoopLabel.text = "Is Loop: " + clip.isLooping;

            ((AnimationTrackItem) curTrackItem).animationEvent.animationClip = clip;
            SkillMasterEditorWindow.instance.SaveConfig();

            curTrackItem.ForceRefreshView();
        }

        private void OnAnimationRootMotionToggleValueChanged(ChangeEvent<bool> evt)
        {
            ((AnimationTrackItem) curTrackItem).animationEvent.applyRootMotion = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnAnimationDurationFieldFocusIn(FocusInEvent evt)
        {
            m_OldAnimationDurationValue = m_AnimationDurationField.value;
        }

        private void OnAnimationDurationFieldFocusOut(FocusOutEvent evt)
        {
            if (m_OldAnimationDurationValue == m_AnimationDurationField.value)
            {
                return;
            }
            // 安全校验
            if (((AnimationTrack) s_CurTrack).CheckFrame(m_TrackItemFrameIndex + m_AnimationDurationField.value, m_TrackItemFrameIndex, false))
            {
                ((AnimationTrackItem) curTrackItem).animationEvent.durationFrame = m_AnimationDurationField.value;
                ((AnimationTrackItem) curTrackItem)?.CheckBoundaryOverflow();
                SkillMasterEditorWindow.instance.SaveConfig();
                curTrackItem?.ForceRefreshView();
            }
            else
            {
                m_AnimationDurationField.value = m_OldAnimationDurationValue;
            }
        }

        private void OnAnimationTransitionFieldFocusIn(FocusInEvent evt)
        {
            m_OldAnimationTransitionValue = m_AnimationDurationField.value;
        }

        private void OnAnimationTransitionFieldFocusOut(FocusOutEvent evt)
        {
            if (Math.Abs(m_OldAnimationTransitionValue - m_AnimationTransitionField.value) < 0.00001f)
            {
                return;
            }
            ((AnimationTrackItem) curTrackItem).animationEvent.transitionTime = m_AnimationTransitionField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnAnimationDeleteBtnClick()
        {
            s_CurTrack.DeleteTrackItem(m_TrackItemFrameIndex);
            Selection.activeObject = null;
        }

        private void OnAnimationSetFrameBtnClick()
        {
            OnAnimationDurationFieldFocusIn(null);
            m_AnimationDurationField.value = SkillMasterEditorWindow.instance.curSelectedFrameIndex - ((AnimationTrackItem) curTrackItem).frameIndex;
            OnAnimationDurationFieldFocusOut(null);
        }

        #endregion
    }
}