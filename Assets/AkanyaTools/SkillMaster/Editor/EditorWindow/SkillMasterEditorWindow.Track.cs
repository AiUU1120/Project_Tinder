/*
* @Author: AiUU
* @Description: SkillMaster 编辑器主体轨道区域
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Editor.Track;
using AkanyaTools.SkillMaster.Editor.Track.AnimationTrack;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public partial class SkillMasterEditorWindow
    {
        private VisualElement m_TrackMenuList;

        private ScrollView m_MainContentView;

        private VisualElement m_ContentListView;

        private readonly List<TrackBase> m_TrackList = new();

        /// <summary>
        /// 初始化主体轨道区域
        /// </summary>
        private void InitContent()
        {
            m_TrackMenuList = rootVisualElement.NiceQ<VisualElement>("TrackMenuList");

            m_MainContentView = rootVisualElement.NiceQ<ScrollView>("MainContentView");
            m_MainContentView.verticalScroller.valueChanged += OnMainContentViewScrollerValueChanged;

            m_ContentListView = rootVisualElement.NiceQ<VisualElement>("ContentListView");
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
            animationTrack.Init(m_TrackMenuList, m_ContentListView, m_SkillMasterEditorConfig.frameUnitWidth);
            m_TrackList.Add(animationTrack);
        }

        /// <summary>
        /// 在监视器中显示轨道片段信息
        /// </summary>
        public void ShowTrackItemInInspector(TrackItemBase item, TrackBase track)
        {
            SkillMasterInspector.SetTrackItem(item, track);
            Selection.activeObject = this;
        }

        /// <summary>
        /// 同步 Track 主体与 TrackMenu 位置
        /// </summary>
        /// <param name="obj"></param>
        private void OnMainContentViewScrollerValueChanged(float obj)
        {
            var pos = m_TrackMenuList.transform.position;
            pos.y = m_ContentContainer.transform.position.y;
            m_TrackMenuList.transform.position = pos;
        }
    }
}