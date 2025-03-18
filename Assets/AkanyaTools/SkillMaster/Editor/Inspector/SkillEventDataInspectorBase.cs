/*
 * @Author: AiUU
 * @Description: SkillMaster 监视器绘制基类
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Editor.Track;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public abstract class SkillEventDataInspectorBase
    {
        protected VisualElement m_Root;

        protected int m_ItemFrameIndex;

        public void SetFrameIndex(int index)
        {
            m_ItemFrameIndex = index;
        }

        public virtual void Draw(VisualElement root, TrackItemBase trackItem, TrackBase track)
        {
            this.m_Root = root;
            SetFrameIndex(trackItem.frameIndex);
        }
    }

    public abstract class SkillEventDataInspectorBase<TTrackItem, TTrack> : SkillEventDataInspectorBase
        where TTrackItem : TrackItemBase
        where TTrack : TrackBase
    {
        protected TTrackItem m_TrackItem;

        protected TTrack m_Track;

        public override void Draw(VisualElement root, TrackItemBase trackItem, TrackBase track)
        {
            base.Draw(root, trackItem, track);
            this.m_TrackItem = (TTrackItem) trackItem;
            this.m_Track = (TTrack) track;
            OnDraw();
        }

        public abstract void OnDraw();
    }
}