/*
* @Author: AiUU
* @Description: SkillMaster 轨道样式
* @AkanyaTech.SkillMaster
*/

using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style.Common
{
    public abstract class TrackStyleBase
    {
        public Label titleLabel;

        public VisualElement menuRoot;

        public VisualElement contentRoot;

        public VisualElement menuParent;

        public VisualElement contentParent;

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="obj"></param>
        public virtual void AddItem(VisualElement obj)
        {
            contentRoot.Add(obj);
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="obj"></param>
        public virtual void DeleteItem(VisualElement obj)
        {
            contentRoot.Remove(obj);
        }

        /// <summary>
        /// 轨道销毁
        /// </summary>
        public virtual void Destroy()
        {
            if (menuRoot != null)
            {
                menuParent.Remove(menuRoot);
            }
            if (contentRoot != null)
            {
                contentParent.Remove(contentRoot);
            }
        }
    }
}