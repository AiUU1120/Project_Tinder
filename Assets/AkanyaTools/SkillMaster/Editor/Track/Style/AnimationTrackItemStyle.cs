/*
* @Author: AiUU
* @Description: SkillMaster 动画轨道片段样式
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style
{
    public sealed class AnimationTrackItemStyle : TrackItemStyleBase
    {
        public VisualElement mainDragArea { get; private set; }

        public VisualElement animationEndLine { get; private set; }

        private const string track_item_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/TrackItem/AnimationTrackItem.uxml";

        private Label m_TitleLabel;

        public void Init(TrackStyleBase trackStyle, int startFrameIndex, float frameUnitWidth)
        {
            m_TitleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_item_path).Instantiate().Query<Label>();
            root = m_TitleLabel;
            mainDragArea = root.NiceQ<VisualElement>("MainDragArea");
            animationEndLine = root.NiceQ<VisualElement>("AnimationEndLine");
            trackStyle.AddItem(root);
        }

        public void SetTitle(string title)
        {
            m_TitleLabel.text = title;
        }
    }
}