/*
* @Author: AiUU
* @Description: SkillMaster 轨道片段样式
* @AkanyaTech.SkillMaster
*/

using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style
{
    public abstract class TrackItemStyleBase
    {
        public Label root { get; protected set; }

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        /// <param name="color">目标颜色</param>
        public virtual void SetBgColor(Color color)
        {
            root.style.backgroundColor = color;
        }

        /// <summary>
        /// 设置宽度
        /// </summary>
        /// <param name="width">宽度</param>
        public virtual void SetWidth(float width)
        {
            root.style.width = width;
        }

        /// <summary>
        /// 设置 X 坐标
        /// </summary>
        /// <param name="x"></param>
        public virtual void SetPositionX(float x)
        {
            var pos = root.transform.position;
            pos.x = x;
            root.transform.position = pos;
        }
    }
}