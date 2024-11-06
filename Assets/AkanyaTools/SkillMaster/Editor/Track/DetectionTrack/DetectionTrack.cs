/*
* @Author: AiUU
* @Description: SkillMaster 判定轨道
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.DetectionTrack
{
    public sealed class DetectionTrack : TrackBase
    {
        public SkillDetectionData detectionData => SkillMasterEditorWindow.instance.skillConfig.skillDetectionData;

        public Color themeColor => m_ThemeColor;

        private MultiLineTrackStyle m_TrackStyle;

        private readonly List<DetectionTrackItem> m_TrackItems = new();

        private readonly Color m_ThemeColor = new(0.14f, 0.78f, 0.59f, 1f);

        #region 初始化

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameUnitWidth)
        {
            base.Init(menuParent, trackParent, frameUnitWidth);
            m_TrackStyle = new MultiLineTrackStyle();
            m_TrackStyle.Init(menuParent, trackParent, "Detection", AddSubTrack, DeleteSubTrack, SwapSubTrack, UpdateSubTrackName, m_ThemeColor);
            RefreshView();
        }

        #endregion

        public override void TickView(int frameIndex)
        {
            // if (!SkillMasterEditorWindow.instance.isPlaying)
            // {
            //     return;
            // }
            // foreach (var data in detectionData.frameData)
            // {
            //     if (data.audioClip != null && data.frameIndex == frameIndex)
            //     {
            //         EditorAudioHelper.PlayAudioClip(data.audioClip, 0);
            //     }
            // }
        }

        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            foreach (var item in m_TrackItems)
            {
                item.Destroy();
            }
            m_TrackItems.Clear();

            if (SkillMasterEditorWindow.instance.skillConfig == null)
            {
                return;
            }

            foreach (var data in detectionData.frameData)
            {
                CreateTrackItem(data);
            }
        }

        public override void OnPlay(int startFrameIndex)
        {
            // foreach (var data in detectionData.frameData)
            // {
            //     if (data.audioClip == null)
            //     {
            //         continue;
            //     }
            //     var clipFrameCount = (int) (data.audioClip.length * SkillMasterEditorWindow.instance.skillConfig.frameRate);
            //     var clipEndFrameIndex = clipFrameCount + data.frameIndex;
            //     // 播放帧在 clip 中间
            //     if (data.frameIndex < startFrameIndex && clipEndFrameIndex > startFrameIndex)
            //     {
            //         var progress = (float) (startFrameIndex - data.frameIndex) / clipFrameCount;
            //         EditorAudioHelper.PlayAudioClip(data.audioClip, progress);
            //     }
            //     // 播放帧在 clip 头部
            //     else if (data.frameIndex == startFrameIndex)
            //     {
            //         EditorAudioHelper.PlayAudioClip(data.audioClip, 0);
            //     }
            // }
        }

        private void CreateTrackItem(SkillDetectionFrameEvent e)
        {
            var item = new DetectionTrackItem();
            item.Init(this, frameUnitWidth, e, m_TrackStyle.AddSubTrack());
            m_TrackItems.Add(item);
        }

        private void AddSubTrack()
        {
            var e = new SkillDetectionFrameEvent();
            detectionData.frameData.Add(e);
            CreateTrackItem(e);
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private bool DeleteSubTrack(int index)
        {
            if (index < 0 || index >= detectionData.frameData.Count)
            {
                return false;
            }
            detectionData.frameData.RemoveAt(index);
            m_TrackItems.RemoveAt(index);
            SkillMasterEditorWindow.instance.SaveConfig();
            return true;
        }

        private void SwapSubTrack(int index1, int index2)
        {
            if (index1 < 0 || index1 >= detectionData.frameData.Count || index2 < 0 || index2 >= detectionData.frameData.Count)
            {
                return;
            }
            (detectionData.frameData[index1], detectionData.frameData[index2]) = (detectionData.frameData[index2], detectionData.frameData[index1]);
        }

        private void UpdateSubTrackName(MultiLineTrackStyle.SubTrackStyle subTrackStyle, string name)
        {
            detectionData.frameData[subTrackStyle.GetIndex()].trackName = name;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        public override void DrawGizmos()
        {
            var curFrameIndex = SkillMasterEditorWindow.instance.curSelectedFrameIndex;
            foreach (var item in m_TrackItems)
            {
                if (curFrameIndex < item.detectionEvent.frameIndex || curFrameIndex > item.detectionEvent.frameIndex + item.detectionEvent.durationFrame)
                {
                    continue;
                }
                item.DrawGizmos();
            }
        }

        public override void Destroy()
        {
            m_TrackStyle.Destroy();
        }
    }
}