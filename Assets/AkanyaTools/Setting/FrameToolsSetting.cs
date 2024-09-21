/*
* @Author: AiUU
* @Description: 框架设置
* @AkanyaTech.FrameTools
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using FrameTools.Base;
using JKFrame;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace FrameTools.Setting
{
    /// <summary>
    /// 资源系统类型
    /// </summary>
    public enum ResourcesSystemType
    {
        Resources,
        Addressables
    }

    /// <summary>
    /// 存档系统类型
    /// </summary>
    public enum SaveSystemType
    {
        Binary,
        Json
    }

    /// <summary>
    /// 框架的设置
    /// </summary>
    [CreateAssetMenu(fileName = "FrameToolsSetting", menuName = "FrameTools/FrameToolsSetting")]
    public sealed class FrameToolsSetting : SerializedScriptableObject
    {
        [LabelText("资源管理方式")]
#if UNITY_EDITOR
        [OnValueChanged(nameof(SetResourcesSystemType))]
#endif
        public ResourcesSystemType resourcesSystemType = ResourcesSystemType.Addressables;

#if UNITY_EDITOR
        [LabelText("存档方式"), Tooltip("修改类型会导致之前的存档丢失"), OnValueChanged(nameof(SetSaveSystemType))]
#endif
        public SaveSystemType saveSystemType = SaveSystemType.Json;

        [LabelText("二进制序列化器")]
        public IBinarySerializer binarySerializer;

        [LabelText("日志设置")]
        public LogSetting logConfig = new();

        [LabelText("UI窗口数据(无需手动填写)")]
        public Dictionary<string, UIWindowData> uiWindowDataDic = new();

        /// <summary>
        /// 日志设置
        /// </summary>
        public sealed class LogSetting
        {
            [LabelText("启用日志"), OnValueChanged(nameof(EnableLogValueChanged))]
            public bool enableLog = true;

            [LabelText("写入时间"), OnValueChanged(nameof(EnableLogValueChanged))]
            public bool writeTime = true;

            [LabelText("写入线程ID"), OnValueChanged(nameof(EnableLogValueChanged))]
            public bool writeThreadID = false;

            [LabelText("写入堆栈"), OnValueChanged(nameof(EnableLogValueChanged))]
            public bool writeTrace = true;

            [LabelText("保存日志文件"), OnValueChanged(nameof(EnableLogValueChanged))]
            public bool enableSave = false;

            [LabelText("需要保存的日志类型"), HideIf("CheckSaveState"), EnumFlags, OnValueChanged(nameof(EnableLogValueChanged))]
            public JK.Log.LogType saveLogTypes;

            [LabelText("保存路径,相对persistentDataPath的路径"), HideIf("CheckSaveState"), OnValueChanged(nameof(EnableLogValueChanged))]
            public string savePath = "/Log";

            [LabelText("自定义的文件名"), HideIf("CheckSaveState"), OnValueChanged(nameof(EnableLogValueChanged))]
            [InfoBox("如果填写，则会导致每次保存都是覆盖式的；如果不填写，则每次自动保存为时间命名的文件")]
            public string customSaveFileName = string.Empty;

#if UNITY_EDITOR
            public void InitOnEditor()
            {
                EnableLogValueChanged();
            }

            [Button("打开日志")]
            private void OpenLog()
            {
                var path = Application.persistentDataPath + savePath;
                path = path.Replace("/", "\\");
                global::System.Diagnostics.Process.Start("explorer.exe", path);
            }

            private bool CheckSaveState() => !enableSave;

            private void EnableLogValueChanged()
            {
                if (enableLog)
                {
                    AddScriptCompilationSymbol("ENABLE_LOG");
                }
                else
                {
                    RemoveScriptCompilationSymbol("ENABLE_LOG");
                }
            }
#endif
        }

#if UNITY_EDITOR
        [Button("重置")]
        public void Reset()
        {
            logConfig = new LogSetting();
            logConfig.InitOnEditor();
            SetResourcesSystemType();
            SetSaveSystemType();
            InitUIWindowDataDicOnEditor();
        }

        public void InitOnEditor()
        {
            logConfig?.InitOnEditor();
            SetResourcesSystemType();
            InitUIWindowDataDicOnEditor();
        }

        /// <summary>
        /// 修改资源管理系统的类型
        /// </summary>
        public void SetResourcesSystemType()
        {
            switch (resourcesSystemType)
            {
                case ResourcesSystemType.Resources:
                    RemoveScriptCompilationSymbol("ENABLE_ADDRESSABLES");
                    // 查找资源R.cs，如果有需要删除
                    var path = Application.dataPath + "/JKFrame//Scripts/2.System/3.Res/R.cs";
                    if (global::System.IO.File.Exists(path))
                    {
                        AssetDatabase.DeleteAsset("Assets/JKFrame//Scripts/2.System/3.Res/R.cs");
                    }
                    break;
                case ResourcesSystemType.Addressables:
                    AddScriptCompilationSymbol("ENABLE_ADDRESSABLES");
                    break;
            }
        }

        /// <summary>
        /// 修改存档系统的类型
        /// </summary>
        private void SetSaveSystemType()
        {
            // 清空存档
            SaveSystem.DeleteAll();
        }

        /// <summary>
        /// 增加预处理指令
        /// </summary>
        public static void AddScriptCompilationSymbol(string name)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var group = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (!group.Contains(name))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, group + ";" + name);
            }
        }

        /// <summary>
        /// 移除预处理指令
        /// </summary>
        public static void RemoveScriptCompilationSymbol(string name)
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var group = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (group.Contains(name))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, group.Replace(";" + name, string.Empty));
            }
        }

        private void InitUIWindowDataDicOnEditor()
        {
            uiWindowDataDic.Clear();
            // 获取所有程序集
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var baseType = typeof(UI_WindowBase);
            // 遍历程序集
            foreach (var assembly in asms)
            {
                // 遍历程序集下的每一个类型
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (!baseType.IsAssignableFrom(type) || type.IsAbstract)
                        {
                            continue;
                        }
                        var attributes = type.GetCustomAttributes<UIWindowDataAttribute>();
                        foreach (var attribute in attributes)
                        {
                            uiWindowDataDic.Add(attribute.windowKey, new UIWindowData(attribute.isCache, attribute.assetPath, attribute.layerNum));
                        }
                    }
                }
                catch (Exception)
                {
                    Debug.LogError("InitUIWindowDataDicOnEditor ERROR");
                }
            }
        }
#endif
    }
}