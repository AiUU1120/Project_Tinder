/*
* @Author: AiUU
* @Description: SkillMaster 编辑器主体轨道区域
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.Track;
using AkanyaTools.SkillMaster.Editor.Track.Animation;
using FrameTools.Extension;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public partial class SkillMasterEditorWindow
    {
        private VisualElement m_TrackMenu;

        private VisualElement m_ContentListView;

        private readonly List<TrackBase> m_TrackList = new();

        /// <summary>
        /// 初始化主体轨道区域
        /// </summary>
        private void InitContent()
        {
            m_ContentListView = rootVisualElement.NiceQ<VisualElement>("ContentListView");
            m_TrackMenu = rootVisualElement.NiceQ<VisualElement>("TrackMenu");
            UpdateContentSize();
            InitTrack();
        }

        private void InitTrack()
        {
            InitAnimationTrack();
        }

        /// <summary>
        /// 刷新轨道
        /// </summary>
        private void RefreshTrack()
        {
            foreach (var track in m_TrackList)
            {
                track.RefreshView();
            }
        }

        /// <summary>
        /// 更新 Content 区域尺寸大小
        /// </summary>
        private void UpdateContentSize()
        {
            m_ContentListView.style.width = m_SkillMasterEditorConfig.frameUnitWidth * curFrameCount;
        }

        /// <summary>
        /// 初始化动画轨道
        /// </summary>
        private void InitAnimationTrack()
        {
            var animationTrack = new AnimationTrack();
            animationTrack.Init(m_TrackMenu, m_ContentListView, m_SkillMasterEditorConfig.frameUnitWidth);
            m_TrackList.Add(animationTrack);
        }
    }
}