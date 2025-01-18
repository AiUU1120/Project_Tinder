/*
* @Author: AiUU
* @Description: SkillMaster 特效轨道片段样式
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style
{
    public sealed class EffectTrackItemStyle : TrackItemStyleBase
    {
        public VisualElement mainDragArea { get; private set; }

        private const string track_item_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/TrackItem/MultiLineSubTrackItem.uxml";

        private Label m_TitleLabel;

        public void Init(float frameUnitWidth, SkillEffectFrameEvent e, MultiLineTrackStyle.SubTrackStyle subTrackStyle)
        {
            m_TitleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_item_path).Instantiate().Query<Label>();
            root = m_TitleLabel;
            subTrackStyle.AddItem(root);

            mainDragArea = root.NiceQ<VisualElement>("MainDragArea");
        }

        public void RefreshView(float frameUnitWidth, SkillEffectFrameEvent e)
        {
            if (e.effectPrefab == null)
            {
                SetVisible(false);
                return;
            }
            SetVisible(true);
            SetTitle(e.effectPrefab.name);
            SetWidth(frameUnitWidth * e.durationFrame);
            SetPositionX(frameUnitWidth * e.frameIndex);
        }

        private void SetTitle(string title)
        {
            m_TitleLabel.text = title;
        }
    }
}