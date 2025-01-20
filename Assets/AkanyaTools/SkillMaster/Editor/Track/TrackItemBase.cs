/*
 * @Author: AiUU
 * @Description: SkillMaster 轨道片段基类
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Editor.Track
{
    public abstract class TrackItemBase
    {
        public int frameIndex { get; protected set; }

        protected float frameUnitWidth;

        /// <summary>
        /// 选中
        /// </summary>
        public abstract void Select();

        /// <summary>
        /// 选中回调
        /// </summary>
        public abstract void OnSelect();

        /// <summary>
        /// 取消选中回调
        /// </summary>
        public abstract void OnUnSelect();

        /// <summary>
        /// 配置发生改变时
        /// </summary>
        public virtual void OnConfigChanged()
        {
        }

        public void ForceRefreshView()
        {
            RefreshView(frameUnitWidth);
        }

        public virtual void RefreshView(float frameUnitWidth)
        {
            this.frameUnitWidth = frameUnitWidth;
        }
    }

    public abstract class TrackItemBase<T> : TrackItemBase where T : TrackBase
    {
        public TrackItemStyleBase itemStyle { get; protected set; }

        protected T track;

        /// <summary>
        /// 通常颜色
        /// </summary>
        protected Color normalColor;

        /// <summary>
        /// 选中颜色
        /// </summary>
        protected Color selectedColor;

        public override void Select()
        {
            SkillMasterEditorWindow.instance.ShowTrackItemInInspector(this, track);
        }

        public override void OnSelect()
        {
            itemStyle.SetBgColor(selectedColor);
        }

        public override void OnUnSelect()
        {
            itemStyle.SetBgColor(normalColor);
        }
    }
}