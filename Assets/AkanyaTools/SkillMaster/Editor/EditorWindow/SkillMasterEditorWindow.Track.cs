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
using AkanyaTools.SkillMaster.Editor.Track.CustomEventTrack;
using AkanyaTools.SkillMaster.Editor.Track.DetectionTrack;
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

        private ScrollView m_TrackMenuView;

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
            m_TrackMenuView = rootVisualElement.NiceQ<ScrollView>("TrackMenuScrollView");
            m_MainContentView.verticalScroller.valueChanged += (value) => { m_TrackMenuView.verticalScroller.value = value; };
            m_TrackMenuView.verticalScroller.valueChanged += (value) => { m_MainContentView.verticalScroller.value = value; };

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
            InitCustomEventTrack();
            InitAnimationTrack();
            InitDetectionTrack();
            InitEffectTrack();
            InitAudioTrack();
        }

        /// <summary>
        /// 初始化自定义事件轨道
        /// </summary>
        private void InitCustomEventTrack()
        {
            var customEventTrack = new CustomEventTrack();
            customEventTrack.Init(m_TrackMenuList, m_ContentListView, m_SkillMasterEditorConfig.frameUnitWidth);
            m_TrackList.Add(customEventTrack);
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

        /// <summary>
        /// 初始化判定轨道
        /// </summary>
        private void InitDetectionTrack()
        {
            var detectionTrack = new DetectionTrack();
            detectionTrack.Init(m_TrackMenuList, m_ContentListView, m_SkillMasterEditorConfig.frameUnitWidth);
            m_TrackList.Add(detectionTrack);
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
    }
}