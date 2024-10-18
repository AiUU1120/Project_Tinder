/*
* @Author: AiUU
* @Description: SkillMaster 编辑器窗口时间线
* @AkanyaTech.SkillMaster
*/

using FrameTools.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public partial class SkillMasterEditorWindow
    {
        private readonly SkillMasterEditorConfig m_SkillMasterEditorConfig = new();

        private IMGUIContainer m_Timeline;

        private IMGUIContainer m_SelectLine;

        private VisualElement m_ContentContainer;

        private VisualElement m_ContentViewport;

        private int m_CurSelectedFrameIndex;

        /// <summary>
        /// 当前选中帧索引
        /// </summary>
        private int curSelectedFrameIndex
        {
            get => m_CurSelectedFrameIndex;
            set
            {
                var oldIndex = m_CurSelectedFrameIndex;
                // 选中帧超出范围 更新最大帧
                if (value > curFrameCount)
                {
                    curFrameCount = value;
                }
                m_CurSelectedFrameIndex = Mathf.Clamp(value, 0, curFrameCount);
                m_CurFrameIntField.value = m_CurSelectedFrameIndex;
                if (oldIndex == m_CurSelectedFrameIndex)
                {
                    return;
                }
                RefreshTimelineView();
                TickSkill();
            }
        }

        private int m_CurFrameCount;

        /// <summary>
        /// 当前帧总数 影响 Content 大小
        /// </summary>
        public int curFrameCount
        {
            get => m_CurFrameCount;
            set
            {
                m_CurFrameCount = value;
                m_FrameCountIntField.value = curFrameCount;

                if (skillConfig != null)
                {
                    skillConfig.frameCount = curFrameCount;
                }
                UpdateContentSize();
            }
        }

        private float curSelectedFramePosX => curSelectedFrameIndex * m_SkillMasterEditorConfig.frameUnitWidth;

        /// <summary>
        /// 当前内容区域偏移坐标
        /// </summary>
        private float curContentOffsetPosX => Mathf.Abs(m_ContentContainer.transform.position.x);

        private bool m_IsTimelineMouseEnter;

        private void InitTimeLine()
        {
            // 主容器
            var mainContentView = rootVisualElement.NiceQ<ScrollView>("MainContentView");
            m_ContentContainer = mainContentView.NiceQ<VisualElement>("unity-content-container");
            m_ContentViewport = mainContentView.NiceQ<VisualElement>("unity-content-viewport");

            // 时间轴
            m_Timeline = rootVisualElement.NiceQ<IMGUIContainer>("Timeline");
            m_Timeline.onGUIHandler = OnDrawTimeLine;
            m_Timeline.RegisterCallback<WheelEvent>(OnTimeLineWheel);
            m_Timeline.RegisterCallback<MouseDownEvent>(OnTimeLineMouseDown);
            m_Timeline.RegisterCallback<MouseMoveEvent>(OnTimeLineMouseMove);
            m_Timeline.RegisterCallback<MouseUpEvent>(OnTimeLineMouseUp);
            m_Timeline.RegisterCallback<MouseOutEvent>(OnTimeLineMouseOut);

            // 选中线条
            m_SelectLine = rootVisualElement.NiceQ<IMGUIContainer>("SelectLine");
            m_SelectLine.onGUIHandler = OnDrawSelectLine;
        }

        /// <summary>
        /// 根据鼠标坐标获取帧索引
        /// </summary>
        /// <returns></returns>
        private int GetFrameIndexByMousePos(float x) => GetFrameIndexByPos(x + curContentOffsetPosX);

        /// <summary>
        /// 根据帧坐标获取帧索引
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int GetFrameIndexByPos(float x) => Mathf.RoundToInt(x / m_SkillMasterEditorConfig.frameUnitWidth);

        /// <summary>
        /// 更新脏标识 让 Editor 刷新
        /// </summary>
        private void RefreshTimelineView()
        {
            m_Timeline.MarkDirtyLayout();
            m_SelectLine.MarkDirtyLayout();
        }

        #region CallBack

        /// <summary>
        /// 绘制 Timeline 刻度
        /// </summary>
        private void OnDrawTimeLine()
        {
            Handles.BeginGUI();
            Handles.color = Color.cyan;
            var rect = m_Timeline.contentRect;
            // 起始索引
            var index = Mathf.CeilToInt(curContentOffsetPosX / m_SkillMasterEditorConfig.frameUnitWidth);
            var tickStep = (SkillMasterEditorConfig.max_frame_width_level + 1 - (m_SkillMasterEditorConfig.frameUnitWidth / SkillMasterEditorConfig.standard_frame_unit_width)) / 2;
            tickStep = tickStep > 0 ? tickStep : 1;
            // 计算起始点偏移
            var startOffset = index > 0 ? m_SkillMasterEditorConfig.frameUnitWidth - (curContentOffsetPosX % m_SkillMasterEditorConfig.frameUnitWidth) : 0;
            for (var i = startOffset; i < rect.width; i += m_SkillMasterEditorConfig.frameUnitWidth)
            {
                // 绘制长线
                if (index % tickStep == 0)
                {
                    Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
                    var indexStr = index.ToString();
                    GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
                }
                else
                {
                    Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
                }
                index++;
            }
            Handles.EndGUI();
        }

        /// <summary>
        /// 滚轮缩放 控制 Timeline 刻度缩放
        /// </summary>
        /// <param name="evt"></param>
        private void OnTimeLineWheel(WheelEvent evt)
        {
            var delta = (int) evt.delta.y;
            m_SkillMasterEditorConfig.frameUnitWidth = Mathf.Clamp(m_SkillMasterEditorConfig.frameUnitWidth - delta,
                (int) (SkillMasterEditorConfig.min_frame_width_level * SkillMasterEditorConfig.standard_frame_unit_width),
                SkillMasterEditorConfig.max_frame_width_level * SkillMasterEditorConfig.standard_frame_unit_width);
            RefreshTimelineView();
            UpdateContentSize();
            RefreshTrack();
        }

        /// <summary>
        /// 鼠标点击 更新选择线位置
        /// </summary>
        /// <param name="evt"></param>
        private void OnTimeLineMouseDown(MouseDownEvent evt)
        {
            m_IsTimelineMouseEnter = true;
            isPlaying = false;
            var newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
            if (curSelectedFrameIndex == newValue)
            {
                return;
            }
            curSelectedFrameIndex = GetFrameIndexByMousePos(evt.localMousePosition.x);
        }

        private void OnTimeLineMouseUp(MouseUpEvent evt)
        {
            m_IsTimelineMouseEnter = false;
        }

        /// <summary>
        /// 鼠标长按 拖动选择线
        /// </summary>
        /// <param name="evt"></param>
        private void OnTimeLineMouseMove(MouseMoveEvent evt)
        {
            if (!m_IsTimelineMouseEnter)
            {
                return;
            }
            var newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
            if (curSelectedFrameIndex == newValue)
            {
                return;
            }
            curSelectedFrameIndex = GetFrameIndexByMousePos(evt.localMousePosition.x);
        }

        private void OnTimeLineMouseOut(MouseOutEvent evt)
        {
            m_IsTimelineMouseEnter = false;
        }

        /// <summary>
        /// 绘制选中线
        /// </summary>
        private void OnDrawSelectLine()
        {
            // 防止绘制溢出
            if (curSelectedFramePosX < curContentOffsetPosX)
            {
                return;
            }
            Handles.BeginGUI();
            Handles.color = Color.magenta;
            var x = curSelectedFramePosX - curContentOffsetPosX;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, m_ContentViewport.contentRect.height + m_Timeline.contentRect.height));
            Handles.EndGUI();
        }

        #endregion
    }
}