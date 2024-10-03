/*
* @Author: AiUU
* @Description: SkillMaster 动画轨道
* @AkanyaTech.SkillMaster
*/

using System.Collections.Generic;
using System.Linq;
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

        public SkillAnimationData animationData => SkillMasterEditorWindow.instance.skillConfig.skillAnimationData;

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
                CreateTrackItem(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 检查目标帧是否在已有片段内
        /// </summary>
        /// <param name="targetFrame">目标帧</param>
        /// <param name="selfIndex">自身开始帧 不填则不规避自身判断</param>
        /// <param name="isLeft"></param>
        /// <returns></returns>
        public bool CheckFrame(int targetFrame, int selfIndex, bool isLeft)
        {
            foreach (var item in animationData.frameData)
            {
                // 规避自身判断
                if (item.Key == selfIndex)
                {
                    continue;
                }
                // 向左移动
                if (isLeft && selfIndex > item.Key && targetFrame < item.Value.durationFrame + item.Key)
                {
                    return false;
                }
                // 向右移动
                if (!isLeft && selfIndex < item.Key && targetFrame > item.Key)
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
            // 修改数据索引
            m_TrackItemDic.Remove(oldIndex, out var item);
            m_TrackItemDic.Add(newIndex, item);
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        public override void DeleteTrackItem(int frameIndex)
        {
            base.DeleteTrackItem(frameIndex);
            animationData.frameData.Remove(frameIndex);
            if (m_TrackItemDic.Remove(frameIndex, out var item))
            {
                track.Remove(item.root);
            }
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void CreateTrackItem(int frameIndex, SkillAnimationFrameEvent e)
        {
            var trackItem = new AnimationTrackItem();
            trackItem.Init(this, track, frameIndex, frameUnitWidth, e);
            m_TrackItemDic.Add(frameIndex, trackItem);
        }

        public override void TickView(int frameIndex)
        {
            var previewObj = SkillMasterEditorWindow.instance.curPreviewCharacterObj;
            var animator = previewObj.GetComponent<Animator>();
            var frameData = animationData.frameData;

            #region 根运动

            var frameDataSortedDic = new SortedDictionary<int, SkillAnimationFrameEvent>(frameData);
            var keys = frameDataSortedDic.Keys.ToArray();
            var rootMotionTotalPos = Vector3.zero;
            // 按序遍历
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var e = frameDataSortedDic[key];
                // 只考虑应用根运动的动画
                if (!e.applyRootMotion)
                {
                    continue;
                }
                var nextKeyFrameIndex = 0;
                nextKeyFrameIndex = i + 1 < keys.Length ? keys[i + 1] : SkillMasterEditorWindow.instance.skillConfig.frameCount;

                var isBreak = false;
                // 手动点击 Timeline 时特判
                if (nextKeyFrameIndex > frameIndex)
                {
                    nextKeyFrameIndex = frameIndex;
                    isBreak = true;
                }

                // 当前片段持续帧数
                var durationFrameCount = nextKeyFrameIndex - key;
                if (durationFrameCount > 0)
                {
                    var frameCount = e.animationClip.length * SkillMasterEditorWindow.instance.skillConfig.frameRate;
                    // 播放进度
                    var totalProgress = durationFrameCount / frameCount;
                    // 播放次数
                    var playTimes = 0;
                    // 最后一次不完整播放
                    var lastPlayProgress = 0f;
                    // 循环动画采样多次
                    if (e.animationClip.isLooping)
                    {
                        playTimes = (int) totalProgress;
                        lastPlayProgress = totalProgress - playTimes;
                    }
                    else
                    {
                        if (totalProgress >= 1)
                        {
                            playTimes = 1;
                            lastPlayProgress = 0;
                        }
                        else if (totalProgress < 1)
                        {
                            lastPlayProgress = totalProgress;
                            playTimes = 0;
                        }
                    }

                    // 采样计算
                    animator.applyRootMotion = true;
                    if (playTimes >= 1)
                    {
                        e.animationClip.SampleAnimation(previewObj, e.animationClip.length);
                        rootMotionTotalPos += previewObj.transform.position * playTimes;
                    }
                    if (lastPlayProgress > 0)
                    {
                        e.animationClip.SampleAnimation(previewObj, lastPlayProgress * e.animationClip.length);
                        rootMotionTotalPos += previewObj.transform.position;
                    }
                }
                if (isBreak)
                {
                    break;
                }
            }

            #endregion

            #region 姿态

            // curOffset: 当前帧距离最近的动画片段的偏移
            var curOffset = int.MaxValue;
            var animationEventIndex = -1;
            foreach (var data in frameData)
            {
                var offset = frameIndex - data.Key;
                if (offset > 0 && offset < curOffset)
                {
                    curOffset = offset;
                    animationEventIndex = data.Key;
                }
            }
            if (animationEventIndex == -1)
            {
                return;
            }
            var animationEvent = frameData[animationEventIndex];
            var clipFrameCount = animationEvent.animationClip.length * animationEvent.animationClip.frameRate;
            var progress = curOffset / clipFrameCount;
            if (progress > 1 && animationEvent.animationClip.isLooping)
            {
                progress -= (int) progress;
            }
            animator.applyRootMotion = animationEvent.applyRootMotion;
            animationEvent.animationClip.SampleAnimation(previewObj, progress * animationEvent.animationClip.length);

            #endregion

            previewObj.transform.position = rootMotionTotalPos;
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
                    animationData.frameData.Add(selectFrameIndex, animationEvent);
                    SkillMasterEditorWindow.instance.SaveConfig();

                    CreateTrackItem(selectFrameIndex, animationEvent);
                }
            }
        }

        public override void OnConfigChanged()
        {
            foreach (var item in m_TrackItemDic.Values)
            {
                item.OnConfigChanged();
            }
        }

        #endregion
    }
}