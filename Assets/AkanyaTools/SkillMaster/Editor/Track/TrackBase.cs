/*
* @Author: AiUU
* @Description: SkillMaster 轨道基类
* @AkanyaTech.SkillMaster
*/

using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track
{
    public abstract class TrackBase
    {
        protected abstract string menuAssetPath { get; }

        protected abstract string trackAssetPath { get; }

        protected float frameUnitWidth;

        protected VisualElement menuParent;

        protected VisualElement trackParent;

        protected VisualElement menu;

        protected VisualElement track;

        public virtual void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
        {
            this.menuParent = menuParent;
            this.trackParent = trackParent;
            this.frameUnitWidth = frameWidth;
            menu = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuAssetPath).Instantiate().Query().ToList()[1];
            menuParent.Add(menu);
            track = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query().ToList()[1];
            trackParent.Add(track);
        }

        /// <summary>
        /// 刷新视图
        /// </summary>
        public void RefreshView()
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
    }
}