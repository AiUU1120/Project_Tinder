/*
 * @Author: AiUU
 * @Description: SkillMaster 监视器绘制
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track;
using AkanyaTools.SkillMaster.Editor.Track.AnimationTrack;
using AkanyaTools.SkillMaster.Editor.Track.AudioTrack;
using AkanyaTools.SkillMaster.Editor.Track.CustomEventTrack;
using AkanyaTools.SkillMaster.Editor.Track.DetectionTrack;
using AkanyaTools.SkillMaster.Editor.Track.EffectTrack;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    [CustomEditor(typeof(SkillMasterEditorWindow))]
    public sealed partial class SkillMasterInspector : UnityEditor.Editor
    {
        public static TrackItemBase curTrackItem { get; private set; }

        public static SkillMasterInspector instance;

        private static TrackBase s_CurTrack;

        private VisualElement m_Root;

        private int m_TrackItemFrameIndex;

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
            curTrackItem?.OnUnSelect();
            curTrackItem = item;
            curTrackItem.OnSelect();
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
            m_TrackItemFrameIndex = curTrackItem.frameIndex;
            switch (curTrackItem)
            {
                case CustomEventTrackItem customEventTrackItem:
                    DrawCustomEventTrackItem(customEventTrackItem);
                    break;
                case AnimationTrackItem animationItem:
                    DrawAnimationTrackItem(animationItem);
                    break;
                case AudioTrackItem audioItem:
                    DrawAudioTrackItem(audioItem);
                    break;
                case EffectTrackItem effectItem:
                    DrawEffectTrackItem(effectItem);
                    break;
                case DetectionTrackItem detectionItem:
                    DrawDetectionTrackItem(detectionItem);
                    break;
            }
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
            if (curTrackItem == null)
            {
                return;
            }
            curTrackItem.OnUnSelect();
            curTrackItem = null;
            s_CurTrack = null;
        }
    }
}