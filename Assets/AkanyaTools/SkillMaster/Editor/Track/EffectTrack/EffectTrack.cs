/*
* @Author: AiUU
* @Description: SkillMaster 特效轨道
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.EffectTrack
{
    public sealed class EffectTrack : TrackBase
    {
        public SkillEffectData effectData => SkillMasterEditorWindow.instance.skillConfig.skillEffectData;

        public static Transform effectRoot { get; private set; }

        private MultiLineTrackStyle m_TrackStyle;

        private readonly List<EffectTrackItem> m_TrackItems = new();

        #region 初始化

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameUnitWidth)
        {
            base.Init(menuParent, trackParent, frameUnitWidth);
            m_TrackStyle = new MultiLineTrackStyle();
            m_TrackStyle.Init(menuParent, trackParent, "Effect", AddSubTrack, DeleteSubTrack, SwapSubTrack, UpdateSubTrackName);
            if (SkillMasterEditorWindow.instance.isInEditorScene)
            {
                effectRoot = GameObject.Find("PreviewEffectsRoot").transform;
                effectRoot.position = Vector3.zero;
                effectRoot.rotation = Quaternion.identity;
                for (var i = effectRoot.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(effectRoot.GetChild(i).gameObject);
                }
            }
            RefreshView();
        }

        #endregion

        public override void TickView(int frameIndex)
        {
            foreach (var item in m_TrackItems)
            {
                item.TickView(frameIndex);
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

            foreach (var data in effectData.frameData)
            {
                CreateTrackItem(data);
            }
        }

        public override void OnPlay(int startFrameIndex)
        {
            // foreach (var data in audioData.frameData)
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

        private void CreateTrackItem(SkillEffectFrameEvent e)
        {
            var item = new EffectTrackItem();
            item.Init(this, frameUnitWidth, e, m_TrackStyle.AddSubTrack());
            m_TrackItems.Add(item);
        }

        private void AddSubTrack()
        {
            var e = new SkillEffectFrameEvent();
            effectData.frameData.Add(e);
            CreateTrackItem(e);
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private bool DeleteSubTrack(int index)
        {
            if (index < 0 || index >= effectData.frameData.Count)
            {
                return false;
            }
            effectData.frameData.RemoveAt(index);
            m_TrackItems[index].ClearEffectPreviewObj();
            m_TrackItems.RemoveAt(index);
            SkillMasterEditorWindow.instance.SaveConfig();
            return true;
        }

        private void SwapSubTrack(int index1, int index2)
        {
            if (index1 < 0 || index1 >= effectData.frameData.Count || index2 < 0 || index2 >= effectData.frameData.Count)
            {
                return;
            }
            (effectData.frameData[index1], effectData.frameData[index2]) = (effectData.frameData[index2], effectData.frameData[index1]);
        }

        private void UpdateSubTrackName(MultiLineTrackStyle.SubTrackStyle subTrackStyle, string name)
        {
            effectData.frameData[subTrackStyle.GetIndex()].trackName = name;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        public override void Destroy()
        {
            foreach (var item in m_TrackItems)
            {
                item.ClearEffectPreviewObj();
            }
            m_TrackStyle.Destroy();
        }
    }
}