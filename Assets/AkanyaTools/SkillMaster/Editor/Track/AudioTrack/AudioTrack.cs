/*
* @Author: AiUU
* @Description: SkillMaster 音频轨道
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.Track.Style;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.AudioTrack
{
    public sealed class AudioTrack : TrackBase
    {
        private MultiLineTrackStyle m_TrackStyle;

        #region 初始化

        public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
        {
            base.Init(menuParent, trackParent, frameWidth);
            m_TrackStyle = new MultiLineTrackStyle();
            m_TrackStyle.Init(menuParent, trackParent, "Audio", AddSubTrack, DeleteSubTrack);
        }

        #endregion

        private bool AddSubTrack()
        {
            return true;
        }

        private bool DeleteSubTrack(int index)
        {
            return true;
        }

        public override void Destroy()
        {
            m_TrackStyle.Destroy();
        }
    }
}