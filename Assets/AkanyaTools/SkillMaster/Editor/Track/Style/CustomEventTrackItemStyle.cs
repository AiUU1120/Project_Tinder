/*
 * @Author: AiUU
 * @Description: SkillMaster 自定义事件轨道片段样式
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style
{
    public sealed class CustomEventTrackItemStyle : TrackItemStyleBase
    {
        public VisualElement mainDragArea { get; private set; }

        private const string track_item_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/TrackItem/CustomEventTrackItem.uxml";

        public void Init(TrackStyleBase trackStyle)
        {
            root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_item_path).Instantiate().Query<Label>();
            trackStyle.AddItem(root);
            mainDragArea = root.NiceQ<VisualElement>("MainDragArea");
        }
    }
}