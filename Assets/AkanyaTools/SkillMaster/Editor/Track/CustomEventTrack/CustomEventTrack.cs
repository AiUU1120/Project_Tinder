/*
 * @Author: AiUU
 * @Description: SkillMaster 自定义事件轨道
 * @AkanyaTech.SkillMaster
 */

using System;
using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.CustomEventTrack
{
    public sealed class CustomEventTrack : TrackBase
    {
        public SkillCustomEventData customEventData => SkillMasterEditorWindow.instance.skillConfig.skillCustomEventData;

        public Color themeColor => m_ThemeColor;

        private SingleLineTrackStyle m_TrackStyle;

        private readonly Dictionary<int, CustomEventTrackItem> m_TrackItemDic = new();

        private readonly Color m_ThemeColor = new(1f, 0.431f, 0.87f, 1f);

        private DateTime m_StartTime;

        private bool isWaitingDoubleClick;

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameUnitWidth)
        {
            base.Init(menuParent, trackParent, frameUnitWidth);
            m_TrackStyle = new SingleLineTrackStyle();
            m_TrackStyle.Init(menuParent, trackParent, "CustomEvent", m_ThemeColor);
            trackParent.RegisterCallback<MouseDownEvent>(OnMouseDown);
            RefreshView();
        }

        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            // 销毁已有
            foreach (var item in m_TrackItemDic)
            {
                m_TrackStyle.DeleteItem(item.Value.itemStyle.root);
            }
            m_TrackItemDic.Clear();

            if (SkillMasterEditorWindow.instance.skillConfig == null)
            {
                return;
            }

            // 根据数据绘制片段
            foreach (var item in customEventData.frameData)
            {
                CreateTrackItem(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 检查目标帧是否冲突
        /// </summary>
        /// <param name="targetFrame">目标帧</param>
        /// <returns></returns>
        public bool CheckFrame(int targetFrame) => !customEventData.frameData.ContainsKey(targetFrame);

        /// <summary>
        /// 修改索引
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public void SetFrameIndex(int oldIndex, int newIndex)
        {
            if (!customEventData.frameData.Remove(oldIndex, out var e))
            {
                return;
            }
            customEventData.frameData.Add(newIndex, e);
            // 修改数据索引
            m_TrackItemDic.Remove(oldIndex, out var item);
            m_TrackItemDic.Add(newIndex, item);
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void CreateTrackItem(int frameIndex, SkillCustomEventFrameEvent e)
        {
            var trackItem = new CustomEventTrackItem();
            trackItem.Init(this, m_TrackStyle, frameIndex, frameUnitWidth, e);
            m_TrackItemDic.Add(frameIndex, trackItem);
        }

        public override void DeleteTrackItem(int frameIndex)
        {
            base.DeleteTrackItem(frameIndex);
            customEventData.frameData.Remove(frameIndex);
            if (m_TrackItemDic.Remove(frameIndex, out var item))
            {
                m_TrackStyle.DeleteItem(item.itemStyle.root);
            }
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        public override void Destroy()
        {
            m_TrackStyle.Destroy();
        }

        #region Callback

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!isWaitingDoubleClick)
            {
                isWaitingDoubleClick = true;
                m_StartTime = DateTime.Now;
                return;
            }
            if (DateTime.Now - m_StartTime < TimeSpan.FromMilliseconds(300))
            {
                var index = SkillMasterEditorWindow.instance.GetFrameIndexByPos(evt.localMousePosition.x);
                if (customEventData.frameData.ContainsKey(index))
                {
                    m_StartTime = DateTime.Now;
                    return;
                }
                var e = new SkillCustomEventFrameEvent();
                customEventData.frameData.Add(index, e);
                SkillMasterEditorWindow.instance.SaveConfig();
                CreateTrackItem(index, e);
            }
            m_StartTime = DateTime.Now;
        }
    }

    #endregion
}