﻿/*
* @Author: AiUU
* @Description: SkillMaster 动画轨道
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Scripts.Config;
using AkanyaTools.SkillMaster.Scripts.Event;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Animation
{
    public sealed class AnimationTrack : TrackBase
    {
        protected override string menuAssetPath => "Assets/AkanyaTools/SkillMaster/Editor/Track/Animation/AnimationTrackMenu.uxml";

        protected override string trackAssetPath => "Assets/AkanyaTools/SkillMaster/Editor/Track/Animation/AnimationTrack.uxml";

        private readonly Dictionary<int, AnimationTrackItem> m_TrackItemDic = new();

        private SkillAnimationData animationData => SkillMasterEditorWindow.instance.skillConfig.skillAnimationData;

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
        {
            base.Init(menuParent, trackParent, frameWidth);
            track.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            track.RegisterCallback<DragExitedEvent>(OnDragExited);
            RefreshView();
        }

        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            // 销毁已有
            foreach (var item in m_TrackItemDic)
            {
                track.Remove(item.Value.root);
            }
            m_TrackItemDic.Clear();

            if (SkillMasterEditorWindow.instance.skillConfig == null)
            {
                return;
            }

            // 根据数据绘制片段
            foreach (var item in animationData.frameData)
            {
                var trackItem = new AnimationTrackItem();
                trackItem.Init(this, track, item.Key, frameUnitWidth, item.Value);
                m_TrackItemDic.Add(item.Key, trackItem);
            }
        }

        /// <summary>
        /// 检查目标帧是否在已有片段内
        /// </summary>
        /// <param name="targetFrame"></param>
        /// <returns></returns>
        public bool CheckFrame(int targetFrame)
        {
            foreach (var item in animationData.frameData)
            {
                if (targetFrame > item.Key && targetFrame < item.Value.durationFrame + item.Key)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 修改索引
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public void SetFrameIndex(int oldIndex, int newIndex)
        {
            if (!animationData.frameData.Remove(oldIndex, out var e))
            {
                return;
            }
            animationData.frameData.Add(newIndex, e);
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        #region Callback

        /// <summary>
        /// 监听用户动画拖拽
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            var objs = DragAndDrop.objectReferences;
            var clip = objs[0] as AnimationClip;
            if (clip != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        /// <summary>
        /// 监听用户松手 放入动画片段
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragExited(DragExitedEvent evt)
        {
            var objs = DragAndDrop.objectReferences;
            var clip = objs[0] as AnimationClip;
            if (clip != null)
            {
                // 放置动画资源
                var selectFrameIndex = SkillMasterEditorWindow.instance.GetFrameIndexByPos(evt.localMousePosition.x);
                var canPlace = true;
                var durationFrame = -1;
                var clipFrameCount = (int) (clip.length * clip.frameRate);
                var nextTrackItem = -1;
                var curOffset = int.MaxValue;

                foreach (var item in SkillMasterEditorWindow.instance.skillConfig.skillAnimationData.frameData)
                {
                    // 不允许选中帧在 trackItem 中间
                    if (selectFrameIndex > item.Key && selectFrameIndex < item.Value.durationFrame + item.Key)
                    {
                        canPlace = false;
                        break;
                    }
                    // 找到右侧最近的 trackItem
                    if (item.Key > selectFrameIndex)
                    {
                        var offset = item.Key - selectFrameIndex;
                        if (offset < curOffset)
                        {
                            curOffset = offset;
                            nextTrackItem = item.Key;
                        }
                    }
                }
                if (canPlace)
                {
                    // 考虑不能覆盖右边片段
                    if (nextTrackItem != -1)
                    {
                        var offset = clipFrameCount - curOffset;
                        durationFrame = offset < 0 ? clipFrameCount : curOffset;
                    }
                    // 右边没有片段
                    else
                    {
                        durationFrame = clipFrameCount;
                    }
                    // 构建动画数据
                    var animationEvent = new SkillAnimationFrameEvent()
                    {
                        animationClip = clip,
                        durationFrame = durationFrame,
                        transitionTime = 0.25f,
                    };
                    // 保存新增动画数据
                    SkillMasterEditorWindow.instance.skillConfig.skillAnimationData.frameData.Add(selectFrameIndex, animationEvent);
                    SkillMasterEditorWindow.instance.SaveConfig();

                    RefreshView();
                }
            }
        }

        #endregion
    }
}