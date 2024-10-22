﻿using System;
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
        private IntegerField m_DurationField;

        private FloatField m_TransitionField;

        private Label m_ClipFrameLabel;

        private Label m_IsLoopLabel;

        private Toggle m_RootMotionToggle;

        private int m_OldDurationValue;

        private int m_OldTransitionValue;

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
            m_RootMotionToggle = new Toggle("Apply Root Motion")
            {
                value = item.animationEvent.applyRootMotion
            };
            m_RootMotionToggle.RegisterValueChangedCallback(OnRootMotionToggleValueChanged);
            m_Root.Add(m_RootMotionToggle);

            // 轨道长度
            m_DurationField = new IntegerField("Duration Frame")
            {
                value = item.animationEvent.durationFrame
            };
            m_DurationField.RegisterCallback<FocusInEvent>(OnDurationFieldFocusIn);
            m_DurationField.RegisterCallback<FocusOutEvent>(OnDurationFieldFocusOut);
            m_Root.Add(m_DurationField);

            // 过渡时间
            m_TransitionField = new FloatField("Transition Time")
            {
                value = item.animationEvent.transitionTime
            };
            m_TransitionField.RegisterCallback<FocusInEvent>(OnTransitionFieldFocusIn);
            m_TransitionField.RegisterCallback<FocusOutEvent>(OnTransitionFieldFocusOut);
            m_Root.Add(m_TransitionField);

            // 动画信息
            var clipFrameCount = (int) (item.animationEvent.animationClip.length * item.animationEvent.animationClip.frameRate);
            m_ClipFrameLabel = new Label("Frame Count: " + clipFrameCount);
            m_Root.Add(m_ClipFrameLabel);
            m_IsLoopLabel = new Label("Is Loop: " + item.animationEvent.animationClip.isLooping);
            m_Root.Add(m_IsLoopLabel);

            // 删除
            var deleteBtn = new Button
            {
                text = "Delete",
                style =
                {
                    backgroundColor = new Color(1f, 0f, 0f, 0.5f)
                },
                clickable = new Clickable(OnDeleteBtnClick),
            };
            m_Root.Add(deleteBtn);
        }

        #region Callback

        private void OnAnimationClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            var clip = evt.newValue as AnimationClip;
            Debug.Assert(clip != null, nameof(clip) + " != null");
            m_ClipFrameLabel.text = "Frame Count: " + (int) (clip.length * clip.frameRate);
            m_IsLoopLabel.text = "Is Loop: " + clip.isLooping;

            ((AnimationTrackItem) s_CurTrackItem).animationEvent.animationClip = clip;
            SkillMasterEditorWindow.instance.SaveConfig();

            s_CurTrackItem.RefreshView();
        }

        private void OnRootMotionToggleValueChanged(ChangeEvent<bool> evt)
        {
            ((AnimationTrackItem) s_CurTrackItem).animationEvent.applyRootMotion = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnDurationFieldFocusIn(FocusInEvent evt)
        {
            m_OldDurationValue = m_DurationField.value;
        }

        private void OnDurationFieldFocusOut(FocusOutEvent evt)
        {
            if (m_OldDurationValue == m_DurationField.value)
            {
                return;
            }
            // 安全校验
            if (((AnimationTrack) s_CurTrack).CheckFrame(m_TrackItemFrameIndex + m_DurationField.value, m_TrackItemFrameIndex, false))
            {
                ((AnimationTrackItem) s_CurTrackItem).animationEvent.durationFrame = m_DurationField.value;
                ((AnimationTrackItem) s_CurTrackItem)?.CheckBoundaryOverflow();
                SkillMasterEditorWindow.instance.SaveConfig();
                s_CurTrackItem?.RefreshView();
            }
            else
            {
                m_DurationField.value = m_OldDurationValue;
            }
        }

        private void OnTransitionFieldFocusIn(FocusInEvent evt)
        {
            m_OldTransitionValue = m_DurationField.value;
        }

        private void OnTransitionFieldFocusOut(FocusOutEvent evt)
        {
            if (Math.Abs(m_OldTransitionValue - m_TransitionField.value) < 0.00001f)
            {
                return;
            }
            ((AnimationTrackItem) s_CurTrackItem).animationEvent.transitionTime = m_TransitionField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnDeleteBtnClick()
        {
            s_CurTrack.DeleteTrackItem(m_TrackItemFrameIndex);
            Selection.activeObject = null;
        }

        #endregion
    }
}