/*
* @Author: AiUU
* @Description: SkillMaster 编辑器顶部菜单
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Config;
using FrameTools.Extension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public partial class SkillMasterEditorWindow
    {
        private const string skill_master_scene_path = "Assets/FrameTools/SkillMaster/SkillMasterScene.unity";

        private const string preview_character_parent_name = "PreviewCharacterRoot";

        private string m_OldScenePath;

        private Button m_LoadEditorSceneBtn;

        private Button m_LoadOldSceneBtn;

        private Button m_ShowSkillBasicInfoBtn;

        private ObjectField m_PreviewCharacterObjField;

        private ObjectField m_SkillConfigObjField;

        private GameObject m_CurPreviewCharacterObj;

        private SkillConfig m_SkillConfig;

        private void InitTopMenu()
        {
            m_LoadEditorSceneBtn = rootVisualElement.NiceQ<Button>("LoadEditorSceneBtn");
            m_LoadEditorSceneBtn.clicked += OnLoadEditorSceneBtnClick;

            m_LoadOldSceneBtn = rootVisualElement.NiceQ<Button>("LoadOldSceneBtn");
            m_LoadOldSceneBtn.clicked += OnLoadOldSceneBtnClick;

            m_ShowSkillBasicInfoBtn = rootVisualElement.NiceQ<Button>("ShowSkillBasicInfoBtn");
            m_ShowSkillBasicInfoBtn.clicked += OnShowSkillBasicInfoBtnClick;

            m_PreviewCharacterObjField = rootVisualElement.NiceQ<ObjectField>("PreviewCharacterObjField");
            m_PreviewCharacterObjField.RegisterValueChangedCallback(OnPreviewCharacterObjFieldValueChanged);

            m_SkillConfigObjField = rootVisualElement.NiceQ<ObjectField>("SkillConfigObjField");
            m_SkillConfigObjField.RegisterValueChangedCallback(OnSkillConfigObjFieldValueChanged);
        }

        /// <summary>
        /// 加载编辑器场景
        /// </summary>
        private void OnLoadEditorSceneBtnClick()
        {
            var curScenePath = SceneManager.GetActiveScene().path;
            if (curScenePath == skill_master_scene_path)
            {
                Debug.LogWarning("SkillMaster: 已在编辑器场景!");
                return;
            }
            m_OldScenePath = curScenePath;
            EditorSceneManager.OpenScene(skill_master_scene_path);
        }

        /// <summary>
        /// 返回旧场景
        /// </summary>
        private void OnLoadOldSceneBtnClick()
        {
            if (string.IsNullOrEmpty(m_OldScenePath))
            {
                Debug.LogWarning("SkillMaster: 没有找到上一场景!");
                return;
            }
            var curScenePath = SceneManager.GetActiveScene().path;
            if (curScenePath == m_OldScenePath)
            {
                return;
            }
            EditorSceneManager.OpenScene(m_OldScenePath);
        }

        /// <summary>
        /// 显示技能基本信息
        /// </summary>
        private void OnShowSkillBasicInfoBtnClick()
        {
            if (m_SkillConfig != null)
            {
                Selection.activeObject = m_SkillConfig;
            }
            else
            {
                Debug.LogWarning("SkillMaster: 技能配置为空!");
            }
        }

        /// <summary>
        /// 预览角色修改
        /// </summary>
        /// <param name="evt"></param>
        private void OnPreviewCharacterObjFieldValueChanged(ChangeEvent<Object> evt)
        {
            if (evt.newValue == null)
            {
                return;
            }
            var curScenePath = SceneManager.GetActiveScene().path;
            if (curScenePath != skill_master_scene_path)
            {
                Debug.LogWarning("SkillMaster: 请勿在非 SkillMaster 编辑场景下实例化演示角色!");
                return;
            }
            if (m_CurPreviewCharacterObj != null)
            {
                DestroyImmediate(m_CurPreviewCharacterObj);
            }
            var parent = GameObject.Find(preview_character_parent_name).transform;
            if (parent == null)
            {
                var go = new GameObject(preview_character_parent_name);
                Instantiate(go, Vector3.zero, Quaternion.identity);
            }
            else if (parent != null && parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
            m_CurPreviewCharacterObj = Instantiate(evt.newValue as GameObject, Vector3.zero, Quaternion.identity, parent);
            m_CurPreviewCharacterObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        /// <summary>
        /// 技能配置修改
        /// </summary>
        /// <param name="evt"></param>
        private void OnSkillConfigObjFieldValueChanged(ChangeEvent<Object> evt)
        {
            m_SkillConfig = evt.newValue as SkillConfig;
            if (m_SkillConfig == null)
            {
                Debug.LogError("SkillConfig 数据不匹配!");
                return;
            }
            curFrameCount = m_SkillConfig.maxFrameCount;
        }
    }
}