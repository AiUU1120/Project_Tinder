/*
* @Author: AiUU
* @Description: SkillMaster 轨道基类
* @AkanyaTech.SkillMaster
*/

using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track
{
    public abstract class TrackBase
    {
        protected float frameUnitWidth;

        public virtual void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
        {
            frameUnitWidth = frameWidth;
        }

        /// <summary>
        /// 驱动轨道显示
        /// </summary>
        public virtual void TickView(int frameIndex)
        {
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        protected void RefreshView()
        {
            RefreshView(frameUnitWidth);
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        /// <param name="frameUnitWidth"></param>
        public virtual void RefreshView(float frameUnitWidth)
        {
            this.frameUnitWidth = frameUnitWidth;
        }

        public virtual void OnPlay(int startFrameIndex)
        {
        }

        public virtual void OnStop()
        {
        }

        /// <summary>
        /// 删除片段
        /// </summary>
        public virtual void DeleteTrackItem(int frameIndex)
        {
        }

        /// <summary>
        /// 配置修改时
        /// </summary>
        public virtual void OnConfigChanged()
        {
        }

        /// <summary>
        /// 销毁轨道
        /// </summary>
        public virtual void Destroy()
        {
        }

        public virtual void DrawGizmos()
        {
        }
    }
}