/*
* @Author: AiUU
* @Description: SkillMaster 单行轨道样式
* @AkanyaTech.SkillMaster
*/

using UnityEditor;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style
{
    public sealed class SingleLineTrackStyle : TrackStyleBase
    {
        private const string menu_asset_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/SingleLineTrackMenu.uxml";

        private const string track_asset_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/SingleLineTrackContent.uxml";

        public void Init(VisualElement menuParent, VisualElement contentParent, string title)
        {
            this.menuParent = menuParent;
            this.contentParent = contentParent;
            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menu_asset_path).Instantiate().Query().ToList()[2];
            menuParent.Add(menuRoot);
            contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(track_asset_path).Instantiate().Query().ToList()[1];
            contentParent.Add(contentRoot);
            titleLabel = (Label) menuRoot;
            titleLabel.text = title;
        }
    }
}