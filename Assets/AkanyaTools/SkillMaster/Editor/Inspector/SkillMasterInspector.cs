/*
* @Author: AiUU
* @Description: SkillMaster 监视器绘制
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track;
using AkanyaTools.SkillMaster.Editor.Track.AnimationTrack;
using AkanyaTools.SkillMaster.Editor.Track.AudioTrack;
using AkanyaTools.SkillMaster.Editor.Track.DetectionTrack;
using AkanyaTools.SkillMaster.Editor.Track.EffectTrack;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    [CustomEditor(typeof(SkillMasterEditorWindow))]
    public sealed partial class SkillMasterInspector : UnityEditor.Editor
    {
        public static SkillMasterInspector instance;

        private static TrackItemBase s_CurTrackItem;

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
            switch (s_CurTrackItem)
            {
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
            if (s_CurTrackItem == null)
            {
                return;
            }
            s_CurTrackItem.OnUnSelect();
            s_CurTrackItem = null;
            s_CurTrack = null;
        }
    }
}