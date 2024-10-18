/*
* @Author: AiUU
* @Description: SkillMaster 单行轨道样式
* @AkanyaTech.SkillMaster
*/

using FrameTools.Extension;
using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style.Common
{
    public sealed class SingleLineTrackStyle : TrackStyleBase
    {
        private const string menu_asset_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/Common/SingleLineTrackMenu.uxml";

        private const string track_asset_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/Common/SingleLineTrackContent.uxml";

        public void Init(VisualElement menuParent, VisualElement contentParent, string title)
        {
            this.menuParent = menuParent;
            this.contentParent = contentParent;
            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menu_asset_path).Instantiate().Query().ToList()[1];
            menuParent.Add(menuRoot);
            contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_asset_path).Instantiate().Query().ToList()[1];
            contentParent.Add(contentRoot);
            titleLabel = menuRoot.NiceQ<Label>("Title");
            titleLabel.text = title;
        }
    }
}