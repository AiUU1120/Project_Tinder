/*
* @Author: AiUU
* @Description: SkillMaster 技能编辑器窗口
* @AkanyaTech.FrameTools
*/

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrameTools.SkillMaster.Editor.EditorWindow
{
    public sealed partial class SkillMasterEditorWindow : UnityEditor.EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;

        [MenuItem("AkanyaTech/SkillMaster &K")]
        public static void ShowExample()
        {
            var wnd = GetWindow<SkillMasterEditorWindow>();
            wnd.titleContent = new GUIContent("SkillMaster");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            InitTopMenu();
            InitTimeLine();
        }
    }
}