/*
* @Author: AiUU
* @Description: SkillMaster 音频轨道
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.AudioTrack
{
    public sealed class AudioTrack : TrackBase
    {
        public SkillAudioData audioData => SkillMasterEditorWindow.instance.skillConfig.skillAudioData;

        private MultiLineTrackStyle m_TrackStyle;

        private readonly List<AudioTrackItem> m_TrackItems = new();

        #region 初始化

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameUnitWidth)
        {
            base.Init(menuParent, trackParent, frameUnitWidth);
            m_TrackStyle = new MultiLineTrackStyle();
            m_TrackStyle.Init(menuParent, trackParent, "Audio", AddSubTrack, DeleteSubTrack, SwapSubTrack, UpdateSubTrackName);
            RefreshView();
        }

        #endregion

        public override void TickView(int frameIndex)
        {
            if (!SkillMasterEditorWindow.instance.isPlaying)
            {
                return;
            }
            foreach (var data in audioData.frameData)
            {
                if (data.audioClip != null && data.frameIndex == frameIndex)
                {
                    EditorAudioHelper.PlayAudioClip(data.audioClip, 0);
                }
            }
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

            foreach (var data in audioData.frameData)
            {
                CreateTrackItem(data);
            }
        }

        public override void OnPlay(int startFrameIndex)
        {
            foreach (var data in audioData.frameData)
            {
                if (data.audioClip == null)
                {
                    continue;
                }
                var clipFrameCount = (int) (data.audioClip.length * SkillMasterEditorWindow.instance.skillConfig.frameRate);
                var clipEndFrameIndex = clipFrameCount + data.frameIndex;
                // 播放帧在 clip 中间
                if (data.frameIndex < startFrameIndex && clipEndFrameIndex > startFrameIndex)
                {
                    var progress = (float) (startFrameIndex - data.frameIndex) / clipFrameCount;
                    EditorAudioHelper.PlayAudioClip(data.audioClip, progress);
                }
                // 播放帧在 clip 头部
                else if (data.frameIndex == startFrameIndex)
                {
                    EditorAudioHelper.PlayAudioClip(data.audioClip, 0);
                }
            }
        }

        private void CreateTrackItem(SkillAudioFrameEvent e)
        {
            var item = new AudioTrackItem();
            item.Init(this, frameUnitWidth, e, m_TrackStyle.AddSubTrack());
            m_TrackItems.Add(item);
        }

        private void AddSubTrack()
        {
            var e = new SkillAudioFrameEvent();
            audioData.frameData.Add(e);
            CreateTrackItem(e);
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private bool DeleteSubTrack(int index)
        {
            if (index < 0 || index >= audioData.frameData.Count)
            {
                return false;
            }
            audioData.frameData.RemoveAt(index);
            m_TrackItems.RemoveAt(index);
            SkillMasterEditorWindow.instance.SaveConfig();
            return true;
        }

        private void SwapSubTrack(int index1, int index2)
        {
            if (index1 < 0 || index1 >= audioData.frameData.Count || index2 < 0 || index2 >= audioData.frameData.Count)
            {
                return;
            }
            (audioData.frameData[index1], audioData.frameData[index2]) = (audioData.frameData[index2], audioData.frameData[index1]);
        }

        private void UpdateSubTrackName(MultiLineTrackStyle.SubTrackStyle subTrackStyle, string name)
        {
            audioData.frameData[subTrackStyle.GetIndex()].trackName = name;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        public override void Destroy()
        {
            m_TrackStyle.Destroy();
        }
    }
}