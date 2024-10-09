/*
* @Author: AiUU
* @Description: SkillMaster 监视器绘制
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track;
using AkanyaTools.SkillMaster.Editor.Track.AnimationTrack;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    [CustomEditor(typeof(SkillMasterEditorWindow))]
    public sealed class SkillMasterInspector : UnityEditor.Editor
    {
        public static SkillMasterInspector instance;

        private static TrackItemBase s_CurTrackItem;

        private static TrackBase s_CurTrack;

        private VisualElement m_Root;

        private IntegerField m_DurationField;

        private FloatField m_TransitionField;

        private Label m_ClipFrameLabel;

        private Label m_IsLoopLabel;

        private int m_TrackItemFrameIndex;

        private Toggle m_RootMotionToggle;

        public override VisualElement CreateInspectorGUI()
        {
            instance = this;
            m_Root = new VisualElement();
            Refresh();
            return m_Root;
        }

        /// <summary>
        /// 选中片段 刷新监视器显示内容
        /// </summary>
        /// <param name="item"></param>
        /// <param name="track"></param>
        public static void SetTrackItem(TrackItemBase item, TrackBase track)
        {
            s_CurTrackItem?.OnUnSelect();
            s_CurTrackItem = item;
            s_CurTrackItem.OnSelect();
            s_CurTrack = track;
            // 避免已经打开了监视器时数据不刷新
            if (instance != null)
            {
                instance.Refresh();
            }
        }

        public void SetTrackItemFrameIndex(int index)
        {
            m_TrackItemFrameIndex = index;
        }

        /// <summary>
        /// 刷新监视器显示内容
        /// </summary>
        private void Refresh()
        {
            Clear();
            if (s_CurTrackItem is AnimationTrackItem item)
            {
                DrawAnimationTrackItem(item);
            }
        }

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
            m_DurationField.RegisterValueChangedCallback(OnDurationFieldValueChanged);
            m_Root.Add(m_DurationField);

            // 过渡时间
            m_TransitionField = new FloatField("Transition Time")
            {
                value = item.animationEvent.transitionTime
            };
            m_TransitionField.RegisterValueChangedCallback(OnTransitionFieldValueChanged);
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

        /// <summary>
        /// 清除监视器显示内容
        /// </summary>
        private void Clear()
        {
            m_Root?.Clear();
        }

        private void OnDestroy()
        {
            if (s_CurTrackItem == null)
            {
                return;
            }
            s_CurTrackItem.OnUnSelect();
            s_CurTrackItem = null;
            s_CurTrack = null;
        }

        #region Callback

        private void OnAnimationClipAssetFieldValueChanged(ChangeEvent<Object> evt)
        {
            var clip = evt.newValue as AnimationClip;
            Debug.Assert(clip != null, nameof(clip) + " != null");
            m_ClipFrameLabel.text = "Frame Count: " + (int) (clip.length * clip.frameRate);
            m_IsLoopLabel.text = "Is Loop: " + clip.isLooping;

            ((AnimationTrackItem) s_CurTrackItem).animationEvent.animationClip = clip;
            SkillMasterEditorWindow.instance.SaveConfig();

            s_CurTrackItem.RefreshView();
        }

        private void OnDurationFieldValueChanged(ChangeEvent<int> evt)
        {
            var value = evt.newValue;
            // 安全校验
            if (((AnimationTrack) s_CurTrack).CheckFrame(m_TrackItemFrameIndex + value, m_TrackItemFrameIndex, false))
            {
                ((AnimationTrackItem) s_CurTrackItem).animationEvent.durationFrame = value;
                ((AnimationTrackItem) s_CurTrackItem)?.CheckBoundaryOverflow();
                SkillMasterEditorWindow.instance.SaveConfig();
                s_CurTrackItem?.RefreshView();
            }
            else
            {
                m_DurationField.value = evt.previousValue;
            }
        }

        private void OnTransitionFieldValueChanged(ChangeEvent<float> evt)
        {
            ((AnimationTrackItem) s_CurTrackItem).animationEvent.transitionTime = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnDeleteBtnClick()
        {
            s_CurTrack.DeleteTrackItem(m_TrackItemFrameIndex);
            Selection.activeObject = null;
        }

        private void OnRootMotionToggleValueChanged(ChangeEvent<bool> evt)
        {
            ((AnimationTrackItem) s_CurTrackItem).animationEvent.applyRootMotion = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        #endregion
    }
}