/*
* @Author: AiUU
* @Description: SkillMaster 技能编辑器窗口
* @AkanyaTech.SkillMaster
*/

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public sealed partial class SkillMasterEditorWindow : UnityEditor.EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;

        public static SkillMasterEditorWindow instance { get; private set; }

        [MenuItem("AkanyaTech/SkillMaster &K")]
        public static void ShowExample()
        {
            var wnd = GetWindow<SkillMasterEditorWindow>();
            wnd.titleContent = new GUIContent("SkillMaster");
        }

        public void CreateGUI()
        {
            instance = this;

            var root = rootVisualElement;

            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            InitTopMenu();
            InitTimeLine();
            InitConsole();
            InitContent();

            if (m_SkillConfig != null)
            {
                m_SkillConfigObjField.value = m_SkillConfig;
                curFrameCount = m_SkillConfig.maxFrameCount;
            }
            else
            {
                curFrameCount = 100;
            }
            curSelectedFrameIndex = 0;
        }
    }
}