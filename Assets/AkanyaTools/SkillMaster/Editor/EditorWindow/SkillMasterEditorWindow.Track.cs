/*
* @Author: AiUU
* @Description: SkillMaster 编辑器主体轨道区域
* @AkanyaTech.SkillMaster
*/

using System;
using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Editor.Track;
using AkanyaTools.SkillMaster.Editor.Track.AnimationTrack;
using AkanyaTools.SkillMaster.Editor.Track.AudioTrack;
using AkanyaTools.SkillMaster.Editor.Track.EffectTrack;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public partial class SkillMasterEditorWindow
    {
        private VisualElement m_TrackMenuList;

        private ScrollView m_MainContentView;

        private VisualElement m_ContentListView;

        private readonly List<TrackBase> m_TrackList = new();

        private Func<int, bool, Vector3> m_OnGetPosFromRootMotion;

        #region 初始化

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

        /// <summary>
        /// 初始化轨道
        /// </summary>
        private void InitTrack()
        {
            if (skillConfig == null)
            {
                return;
            }
            InitAnimationTrack();
            InitAudioTrack();
            InitEffectTrack();
        }

        /// <summary>
        /// 初始化动画轨道
        /// </summary>
        private void InitAnimationTrack()
        {
            var animationTrack = new AnimationTrack();
            animationTrack.Init(m_TrackMenuList, m_ContentListView, m_SkillMasterEditorConfig.frameUnitWidth);
            m_TrackList.Add(animationTrack);
            m_OnGetPosFromRootMotion = animationTrack.GetPosFromRootMotion;
        }

        /// <summary>
        /// 初始化音频轨道
        /// </summary>
        private void InitAudioTrack()
        {
            var audioTrack = new AudioTrack();
            audioTrack.Init(m_TrackMenuList, m_ContentListView, m_SkillMasterEditorConfig.frameUnitWidth);
            m_TrackList.Add(audioTrack);
        }

        /// <summary>
        /// 初始化特效轨道
        /// </summary>
        private void InitEffectTrack()
        {
            var effectTrack = new EffectTrack();
            effectTrack.Init(m_TrackMenuList, m_ContentListView, m_SkillMasterEditorConfig.frameUnitWidth);
            m_TrackList.Add(effectTrack);
        }

        #endregion

        /// <summary>
        /// 刷新轨道
        /// </summary>
        private void RefreshTrack()
        {
            if (skillConfig == null)
            {
                DeleteAllTracks();
                return;
            }
            if (m_TrackList.Count == 0)
            {
                InitTrack();
            }
            foreach (var track in m_TrackList)
            {
                track.RefreshView(m_SkillMasterEditorConfig.frameUnitWidth);
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
        /// 在监视器中显示轨道片段信息
        /// </summary>
        public void ShowTrackItemInInspector(TrackItemBase item, TrackBase track)
        {
            SkillMasterInspector.SetTrackItem(item, track);
            Selection.activeObject = this;
        }

        /// <summary>
        /// 销毁所有轨道
        /// </summary>
        private void DeleteAllTracks()
        {
            foreach (var track in m_TrackList)
            {
                track.Destroy();
            }
            m_TrackList.Clear();
        }

        public Vector3 GetPosFromRootMotion(int frameIndex, bool isResume = false) => m_OnGetPosFromRootMotion.Invoke(frameIndex, isResume);

        #region Callback

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

        #endregion
    }
}