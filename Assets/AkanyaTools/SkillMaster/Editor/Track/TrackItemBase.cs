/*
* @Author: AiUU
* @Description: SkillMaster 轨道片段基类
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track
{
    public abstract class TrackItemBase
    {
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

        public virtual void RefreshView()
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
        protected T track;

        /// <summary>
        /// 通常颜色
        /// </summary>
        protected Color normalColor;

        /// <summary>
        /// 选中颜色
        /// </summary>
        protected Color selectedColor;

        public int frameIndex { get; protected set; }

        public Label root { get; protected set; }

        public override void Select()
        {
            SkillMasterEditorWindow.instance.ShowTrackItemInInspector(this, track);
        }

        public override void OnSelect()
        {
            root.style.backgroundColor = selectedColor;
        }

        public override void OnUnSelect()
        {
            root.style.backgroundColor = normalColor;
        }
    }
}