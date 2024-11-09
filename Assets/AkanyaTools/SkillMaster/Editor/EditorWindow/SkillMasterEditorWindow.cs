/*
* @Author: AiUU
* @Description: SkillMaster 技能编辑器窗口
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Runtime.Data.Config;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public sealed partial class SkillMasterEditorWindow : UnityEditor.EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset;

        public static SkillMasterEditorWindow instance { get; private set; }

        public bool isInEditorScene
        {
            get
            {
                var curScenePath = SceneManager.GetActiveScene().path;
                return curScenePath == skill_master_scene_path;
            }
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        [MenuItem("AkanyaTech/SkillMaster &K")]
        public static void ShowExample()
        {
            var wnd = GetWindow<SkillMasterEditorWindow>();
            wnd.titleContent = new GUIContent("SkillMaster");
        }

        public void CreateGUI()
        {
            instance = this;

            SkillConfig.SetOnValidate(ForceRefreshView);

            var root = rootVisualElement;

            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            InitTopMenu();
            InitTimeLine();
            InitConsole();
            InitContent();

            if (skillConfig != null)
            {
                m_SkillConfigObjField.value = skillConfig;
                curFrameCount = skillConfig.frameCount;
            }
            else
            {
                curFrameCount = 100;
            }

            if (curPreviewCharacterPrefab != null)
            {
                m_PreviewCharacterPrefabObjField.value = curPreviewCharacterPrefab;
            }

            if (curPreviewCharacterObj != null)
            {
                m_PreviewCharacterObjObjField.value = curPreviewCharacterObj;
            }

            curSelectedFrameIndex = 0;
        }

        /// <summary>
        /// 强制刷新整个编辑器视图
        /// </summary>
        private void ForceRefreshView()
        {
            var tempConfig = skillConfig;
            m_SkillConfigObjField.value = null;
            m_SkillConfigObjField.value = tempConfig;
        }

        private void OnDisable()
        {
            if (skillConfig != null)
            {
                SaveConfig();
            }
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}