/*
* @Author: AiUU
* @Description: SkillMaster 音效轨道片段样式
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style
{
    public sealed class AudioTrackItemStyle : TrackItemStyleBase
    {
        public VisualElement mainDragArea { get; private set; }

        private const string track_item_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/TrackItem/AudioTrackItem.uxml";

        private Label m_TitleLabel;

        public void Init(float frameUnitWidth, SkillAudioFrameEvent e, MultiLineTrackStyle.SubTrackStyle subTrackStyle)
        {
            m_TitleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_item_path).Instantiate().Query<Label>();
            root = m_TitleLabel;
            subTrackStyle.AddItem(root);

            mainDragArea = root.NiceQ<VisualElement>("MainDragArea");
        }

        public void RefreshView(float frameUnitWidth, SkillAudioFrameEvent e)
        {
            if (e.audioClip == null)
            {
                SetVisible(false);
                return;
            }
            SetVisible(true);
            SetTitle(e.audioClip.name);
            SetWidth(frameUnitWidth * e.audioClip.length * SkillMasterEditorWindow.instance.skillConfig.frameRate);
            SetPositionX(frameUnitWidth * e.frameIndex);
        }

        private void SetTitle(string title)
        {
            m_TitleLabel.text = title;
        }
    }
}