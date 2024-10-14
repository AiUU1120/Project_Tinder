/*
* @Author: AiUU
* @Description: SkillMaster 编辑器顶部菜单
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Scripts.Config;
using FrameTools.Extension;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public partial class SkillMasterEditorWindow
    {
        public GameObject curPreviewCharacterObj { get; private set; }

        public GameObject curPreviewCharacterPrefab { get; private set; }

        public SkillConfig skillConfig { get; private set; }

        private const string skill_master_scene_path = "Assets/AkanyaTools/SkillMaster/Static Resources/SkillMasterScene.unity";

        private const string default_character_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Models/YBot/ybot.fbx";

        private const string preview_character_parent_name = "PreviewCharacterRoot";

        private string m_OldScenePath;

        private Button m_LoadEditorSceneBtn;

        private Button m_LoadOldSceneBtn;

        private Button m_ShowSkillBasicInfoBtn;

        private ObjectField m_PreviewCharacterPrefabObjField;

        private ObjectField m_PreviewCharacterObjObjField;

        private ObjectField m_SkillConfigObjField;

        private void InitTopMenu()
        {
            m_LoadEditorSceneBtn = rootVisualElement.NiceQ<Button>("LoadEditorSceneBtn");
            m_LoadEditorSceneBtn.clicked += OnLoadEditorSceneBtnClick;

            m_LoadOldSceneBtn = rootVisualElement.NiceQ<Button>("LoadOldSceneBtn");
            m_LoadOldSceneBtn.clicked += OnLoadOldSceneBtnClick;

            m_ShowSkillBasicInfoBtn = rootVisualElement.NiceQ<Button>("ShowSkillBasicInfoBtn");
            m_ShowSkillBasicInfoBtn.clicked += OnShowSkillBasicInfoBtnClick;

            m_PreviewCharacterPrefabObjField = rootVisualElement.NiceQ<ObjectField>("PreviewCharacterPrefabObjField");
            m_PreviewCharacterPrefabObjField.RegisterValueChangedCallback(OnPreviewCharacterPrefabObjFieldValueChanged);
            // m_PreviewCharacterObjField.value = AssetDatabase.LoadAssetAtPath<GameObject>(default_character_path);

            m_PreviewCharacterObjObjField = rootVisualElement.NiceQ<ObjectField>("PreviewCharacterObjObjField");
            m_PreviewCharacterObjObjField.RegisterValueChangedCallback(OnPreviewCharacterObjObjFieldValueChanged);

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
                Debug.LogWarning("SkillMaster: 已经在旧场景!");
                return;
            }
            EditorSceneManager.OpenScene(m_OldScenePath);
        }

        /// <summary>
        /// 显示技能基本信息
        /// </summary>
        private void OnShowSkillBasicInfoBtnClick()
        {
            if (skillConfig != null)
            {
                Selection.activeObject = skillConfig;
            }
            else
            {
                Debug.LogWarning("SkillMaster: 技能配置为空!");
            }
        }

        /// <summary>
        /// 预览角色预制体修改
        /// </summary>
        /// <param name="evt"></param>
        private void OnPreviewCharacterPrefabObjFieldValueChanged(ChangeEvent<Object> evt)
        {
            // 避免在非编辑器场景下操作
            var curScenePath = SceneManager.GetActiveScene().path;
            if (curScenePath != skill_master_scene_path)
            {
                m_PreviewCharacterPrefabObjField.value = null;
                return;
            }

            if (evt.newValue == null)
            {
                return;
            }

            if (evt.newValue == curPreviewCharacterPrefab)
            {
                return;
            }

            curPreviewCharacterPrefab = evt.newValue as GameObject;

            // 删除现有预览角色
            if (curPreviewCharacterObj != null)
            {
                DestroyImmediate(curPreviewCharacterObj);
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
            curPreviewCharacterObj = Instantiate(evt.newValue as GameObject, Vector3.zero, Quaternion.identity, parent);
            curPreviewCharacterObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            m_PreviewCharacterObjObjField.value = curPreviewCharacterObj;
        }

        /// <summary>
        /// 预览角色修改
        /// </summary>
        /// <param name="evt"></param>
        private void OnPreviewCharacterObjObjFieldValueChanged(ChangeEvent<Object> evt)
        {
            curPreviewCharacterObj = evt.newValue as GameObject;
        }

        /// <summary>
        /// 技能配置修改
        /// </summary>
        /// <param name="evt"></param>
        private void OnSkillConfigObjFieldValueChanged(ChangeEvent<Object> evt)
        {
            skillConfig = evt.newValue as SkillConfig;
            curSelectedFrameIndex = 0;
            curFrameCount = skillConfig == null ? 100 : skillConfig.frameCount;
            RefreshTrack();
        }

        /// <summary>
        /// 保存配置变更
        /// </summary>
        public void SaveConfig()
        {
            if (skillConfig == null)
            {
                return;
            }
            EditorUtility.SetDirty(skillConfig);
            AssetDatabase.SaveAssetIfDirty(skillConfig);
            ReReferenceData();
        }

        /// <summary>
        /// 重新引用数据
        /// </summary>
        private void ReReferenceData()
        {
            foreach (var track in m_TrackList)
            {
                track.OnConfigChanged();
            }
        }
    }
}