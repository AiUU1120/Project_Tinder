using System;
using System.Collections.Generic;
using FrameTools.ResourceSystem;
using JKFrame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AkanyaTools.UISystem
{
    /// <summary>
    /// UI根节点
    /// </summary>
    public sealed class UISystem : MonoBehaviour
    {
        private static UISystem s_Instance;

        public static void Init()
        {
            s_Instance = FrameRoot.RootTransform.GetComponentInChildren<UISystem>();
        }

        #region 内部类

        [Serializable]
        private class UILayer
        {
            public Transform root;
            public bool enableMask = true;
            public Image maskImage;
            private int m_Count;

            public void OnWindowShow()
            {
                m_Count += 1;
                Update();
            }

            public void OnWindowClose()
            {
                m_Count -= 1;
                Update();
            }

            private void Update()
            {
                if (enableMask == false)
                {
                    return;
                }
                maskImage.raycastTarget = m_Count != 0;
                var posIndex = root.childCount - 2;
                maskImage.transform.SetSiblingIndex(posIndex < 0 ? 0 : posIndex);
            }

            public void Reset()
            {
                m_Count = 0;
                Update();
            }
        }

        #endregion

        private static Dictionary<string, UIWindowData> s_UIWindowDataDic => FrameRoot.toolsSetting.uiWindowDataDic;

        [SerializeField]
        private UILayer[] m_UILayers;

        [SerializeField]
        private RectTransform m_DragLayer;

        /// <summary>
        /// 拖拽层，位于所有UI的最上层
        /// </summary>
        public static RectTransform dragLayer => s_Instance.m_DragLayer;

        private static UILayer[] uiLayers => s_Instance.m_UILayers;

        [SerializeField]
        private GameObject m_UITipsItemPrefab;

        [SerializeField]
        private RectTransform m_UITipsItemParent;

        #region 动态加载/移除窗口数据

        // UI系统的窗口数据中主要包含：预制体路径、是否缓存、当前窗口对象实例等重要信息
        // 为了方便使用，所以窗口数据必须先存放于UIWindowDataDic中，才能通过UI系统显示、关闭等

        /// <summary>
        /// 初始化UI元素数据
        /// 只执行OnInit，不执行OnShow
        /// 会自动SetActive(false)
        /// </summary>
        /// <param name="windowKey">自定义的名称，可以是资源路径或类型名称或其他自定义</param>
        /// <param name="windowData">窗口的重要数据</param>
        /// <param name="instantiateAtOnce">是否立刻实例化，前提是有缓存必要</param>
        public static void AddUIWindowData(string windowKey, UIWindowData windowData, bool instantiateAtOnce = false)
        {
            if (s_UIWindowDataDic.TryAdd(windowKey, windowData))
            {
                if (instantiateAtOnce)
                {
                    if (windowData.isCache)
                    {
                        var window = ResourceManager.InstantiateGameObject<UI_WindowBase>(windowData.assetPath, uiLayers[windowData.layerNum].root, windowKey);
                        windowData.instance = window;
                        window.Init();
                        window.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning("UIWindowData中的isCache=false，但instantiateAtOnce=true!提前实例化对于不需要缓存的窗口来说没有意义");
                    }
                }
            }
        }

        /// <summary>
        /// 初始化UI元素数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="windowData"></param>
        /// <param name="instantiateAtOnce">
        /// 立刻实例化，但是：
        /// 只执行OnInit，不执行OnShow
        /// 会自动SetActive(false)
        /// </param>
        public static void AddUIWindowData(Type type, UIWindowData windowData, bool instantiateAtOnce = false)
        {
            AddUIWindowData(type.Name, windowData, instantiateAtOnce);
        }

        /// <summary>
        /// 初始化UI元素数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="windowData"></param>
        /// <param name="instantiateAtOnce">
        /// 立刻实例化，但是：
        /// 只执行OnInit，不执行OnShow
        /// 会自动SetActive(false)
        /// </param>
        public static void AddUIWindowData<T>(UIWindowData windowData, bool instantiateAtOnce = false)
        {
            AddUIWindowData(typeof(T), windowData, instantiateAtOnce);
        }

        /// <summary>
        /// 获取UI窗口数据
        /// </summary>
        /// <param name="windowKey"></param>
        /// <returns>可能为Null</returns>
        public static UIWindowData GetUIWindowData(string windowKey) => s_UIWindowDataDic.GetValueOrDefault(windowKey);

        public static UIWindowData GetUIWindowData(Type windowType) => GetUIWindowData(windowType.Name);

        public static UIWindowData GetUIWindowData<T>() => GetUIWindowData(typeof(T));

        /// <summary>
        /// 尝试获取UI窗口数据
        /// </summary>
        /// <param name="windowKey"></param>
        /// <param name="windowData"></param>
        public static bool TryGetUIWindowData(string windowKey, out UIWindowData windowData) => s_UIWindowDataDic.TryGetValue(windowKey, out windowData);

        /// <summary>
        /// 移除UI窗口数据,已存在的窗口会被强行删除
        /// </summary>
        /// <param name="windowKey"></param>
        /// <returns></returns>
        public static bool RemoveUIWindowData(string windowKey)
        {
            if (TryGetUIWindowData(windowKey, out var windowData))
            {
                if (windowData.instance != null)
                {
                    Destroy(windowData.instance.gameObject);
                }
            }
            return s_UIWindowDataDic.Remove(windowKey);
        }

        /// <summary>
        /// 清除所有UI窗口数据
        /// </summary>
        public static void ClearUIWindowData()
        {
            var enumerator = s_UIWindowDataDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Destroy(enumerator.Current.Value.instance.gameObject);
            }
            s_UIWindowDataDic.Clear();
        }

        #endregion

        #region UI窗口生命周期管理

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T Show<T>(int layer = -1) where T : UI_WindowBase => Show(typeof(T), layer) as T;

        /// <summary>
        /// 显示窗口 异步
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <param name="callback"></param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync<T>(Action<T> callback = null, int layer = -1) where T : UI_WindowBase
        {
            ShowAsync(typeof(T), (window) => { callback?.Invoke((T) window); }, layer);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <typeparam name="T">要返回的窗口类型</typeparam>
        /// <param name="windowKey">窗口的Key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static T Show<T>(string windowKey, int layer = -1) where T : UI_WindowBase => Show(windowKey, layer) as T;

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static UI_WindowBase Show(Type type, int layer = -1) => Show(type.Name, layer);

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="callback"></param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync(Type type, Action<UI_WindowBase> callback = null, int layer = -1)
        {
            ShowAsync(type.Name, callback, layer);
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="windowKey">窗口的key</param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static UI_WindowBase Show(string windowKey, int layer = -1)
        {
            if (s_UIWindowDataDic.TryGetValue(windowKey, out var windowData))
            {
                return Show(windowData, windowKey, layer);
            }
            // 资源库中没有意味着不允许显示
            Debug.LogWarning($"不存在{windowKey}的UIWindowData");
            return null;
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="windowKey">窗口的key</param>
        /// <param name="callback"></param>
        /// <param name="layer">层级 -1等于不设置</param>
        public static void ShowAsync(string windowKey, Action<UI_WindowBase> callback = null, int layer = -1)
        {
            if (s_UIWindowDataDic.TryGetValue(windowKey, out var windowData))
            {
                ShowAsync(windowData, windowKey, callback, layer);
            }
            else
            {
                Debug.LogWarning($"不存在{windowKey}的UIWindowData"); // 资源库中没有意味着不允许显示
            }
        }

        private static UI_WindowBase Show(UIWindowData windowData, string windowKey, int layer = -1)
        {
            var layerNum = layer == -1 ? windowData.layerNum : layer;
            // 实例化实例或者获取到实例，保证窗口实例存在
            if (windowData.instance != null)
            {
                // 原本就激活使用状态，避免内部计数问题，进行一次层关闭
                if (windowData.instance.isEnable)
                {
                    uiLayers[windowData.layerNum].OnWindowClose();
                }
                windowData.instance.gameObject.SetActive(true);
                windowData.instance.transform.SetParent(uiLayers[layerNum].root);
                windowData.instance.transform.SetAsLastSibling();
                windowData.instance.ShowGeneralLogic(layerNum);
            }
            else
            {
                var window = ResourceManager.InstantiateGameObject<UI_WindowBase>(windowData.assetPath, uiLayers[layerNum].root, windowKey);
                windowData.instance = window;
                window.Init();
                window.ShowGeneralLogic(layerNum);
            }
            windowData.layerNum = layerNum;
            uiLayers[layerNum].OnWindowShow();
            return windowData.instance;
        }

        private static void ShowAsync(UIWindowData windowData, string windowKey, Action<UI_WindowBase> callback = null, int layer = -1)
        {
            var layerNum = layer == -1 ? windowData.layerNum : layer;
            // 实例化实例或者获取到实例，保证窗口实例存在
            if (windowData.instance != null)
            {
                // 原本就激活使用状态，避免内部计数问题，进行一次层关闭
                if (windowData.instance.isEnable)
                {
                    uiLayers[windowData.layerNum].OnWindowClose();
                }
                windowData.instance.gameObject.SetActive(true);
                windowData.instance.transform.SetParent(uiLayers[layerNum].root);
                windowData.instance.transform.SetAsLastSibling();
                windowData.instance.ShowGeneralLogic(layerNum);
                callback?.Invoke(windowData.instance);
            }
            else
            {
                ResourceManager.InstantiateGameObjectAsync<UI_WindowBase>(windowData.assetPath,
                    (window) =>
                    {
                        windowData.instance = window;
                        window.Init();
                        window.ShowGeneralLogic(layerNum);
                        callback?.Invoke(window);
                    }
                    , uiLayers[layerNum].root, windowKey);
            }
            windowData.layerNum = layerNum;
            uiLayers[layerNum].OnWindowShow();
        }

        #endregion

        #region 获取与销毁窗口

        /// <summary>
        /// 获取窗口
        /// </summary>
        /// <param name="windowKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static UI_WindowBase GetWindow(string windowKey)
        {
            if (s_UIWindowDataDic.TryGetValue(windowKey, out var windowData))
            {
                return windowData.instance;
            }
            return null;
        }

        /// <summary>
        /// 获取窗口
        /// </summary>
        /// <param name="windowKey">窗口Key</param>
        /// <returns>没找到会为Null</returns>
        public static T GetWindow<T>(string windowKey) where T : UI_WindowBase => GetWindow(windowKey) as T;

        /// <summary>
        /// 获取窗口
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static T GetWindow<T>() where T : UI_WindowBase => GetWindow(typeof(T).Name) as T;

        /// <summary>
        /// 获取窗口
        /// </summary>
        /// <returns>没找到会为Null</returns>
        public static UI_WindowBase GetWindow(Type windowType) => GetWindow(windowType.Name);

        /// <summary>
        /// 获取窗口
        /// </summary>
        /// <param name="windowType"></param>
        /// <returns>没找到会为Null</returns>
        public static T GetWindow<T>(Type windowType) where T : UI_WindowBase => GetWindow(windowType.Name) as T;

        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="windowKey"></param>
        /// <param name="window"></param>
        public static bool TryGetWindow(string windowKey, out UI_WindowBase window)
        {
            s_UIWindowDataDic.TryGetValue(windowKey, out var windowData);
            window = windowData?.instance;
            return window != null;
        }

        /// <summary>
        /// 尝试获取窗口
        /// </summary>
        /// <param name="windowKey"></param>
        /// <param name="window"></param>
        public static bool TryGetWindow<T>(string windowKey, out T window) where T : UI_WindowBase
        {
            s_UIWindowDataDic.TryGetValue(windowKey, out var windowData);
            window = windowData?.instance as T;
            return window != null;
        }

        /// <summary>
        /// 销毁窗口
        /// </summary>
        public static void DestroyWindow(string windowKey)
        {
            var window = GetWindow(windowKey);
            if (window != null)
            {
                DestroyImmediate(window.gameObject);
            }
        }

        #endregion

        #region 关闭窗口

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        public static void Close<T>()
        {
            Close(typeof(T));
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public static void Close(Type type)
        {
            Close(type.Name);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="windowKey"></param>
        public static void Close(string windowKey)
        {
            if (TryGetUIWindowData(windowKey, out var windowData))
            {
                if (windowData.instance != null && CloseWindow(windowData))
                {
                    uiLayers[windowData.layerNum].OnWindowClose();
                }
                else
                {
                    Debug.LogWarning("您需要关闭的窗口不存在或已经关闭");
                }
            }
            else
            {
                Debug.LogWarning("未查询到UIWindowData");
            }
        }

        /// <summary>
        /// 尝试关闭窗口
        /// </summary>
        /// <typeparam name="T">窗口类型</typeparam>
        public static void TryClose<T>()
        {
            TryClose(typeof(T));
        }

        /// <summary>
        /// 尝试关闭窗口
        /// </summary>
        public static void TryClose(Type type)
        {
            TryClose(type.Name);
        }

        /// <summary>
        /// 尝试关闭窗口
        /// </summary>
        public static bool TryClose(string windowKey)
        {
            if (TryGetUIWindowData(windowKey, out var windowData))
            {
                if (windowData.instance != null && CloseWindow(windowData))
                {
                    uiLayers[windowData.layerNum].OnWindowClose();
                    return true;
                }
                return false;
            }
            return false;
        }

        private static bool CloseWindow(UIWindowData windowData)
        {
            if (windowData.instance.isEnable)
            {
                windowData.instance.CloseGeneralLogic();
                // 缓存则隐藏
                if (windowData.isCache)
                {
                    windowData.instance.transform.SetAsFirstSibling();
                    windowData.instance.gameObject.SetActive(false);
                }
                // 不缓存则销毁
                else
                {
#if ENABLE_ADDRESSABLES
                    ResourceManager.UnloadInstance(windowData.instance.gameObject);
#endif
                    DestroyImmediate(windowData.instance.gameObject);
                    windowData.instance = null;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 关闭全部窗口
        /// </summary>
        public static void CloseAllWindow()
        {
            // 处理缓存中所有状态的逻辑
            foreach (var item in s_UIWindowDataDic.Values)
            {
                if (item.instance != null && item.instance.gameObject.activeInHierarchy)
                {
                    CloseWindow(item);
                }
            }
            foreach (var layer in uiLayers)
            {
                layer.Reset();
            }
        }

        #endregion

        #region UITips

        public static void AddTips(string tips)
        {
            var item = PoolSystem.GetGameObject<UITipsItem>(s_Instance.m_UITipsItemPrefab.name, s_Instance.m_UITipsItemParent);
            if (item == null)
            {
                item = GameObject.Instantiate(s_Instance.m_UITipsItemPrefab, s_Instance.m_UITipsItemParent).GetComponent<UITipsItem>();
            }
            item.Init(tips);
        }

        #endregion

        #region 工具

        private static List<RaycastResult> s_RaycastResultList = new List<RaycastResult>();

        /// <summary>
        /// 检查鼠标是否在UI上,会屏蔽名称为Mask的物体
        /// </summary>
        public static bool CheckMouseOnUI()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            return CheckPositionOnUI(Input.mousePosition);
#else
            return CheckPositionOnUI(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
#endif
        }

        private static UnityEngine.EventSystems.EventSystem s_EventSystem;
        private static PointerEventData s_PointerEventData;

        /// <summary>
        /// 检查一个坐标是否在UI上,会屏蔽名称为Mask的物体
        /// </summary>
        public static bool CheckPositionOnUI(Vector2 pos)
        {
            if (s_EventSystem == null)
            {
                s_EventSystem = UnityEngine.EventSystems.EventSystem.current;
                s_PointerEventData = new PointerEventData(s_EventSystem);
            }
            s_PointerEventData.position = pos;
            // 射线去检测有没有除了Mask以外的任何UI物体
            s_EventSystem.RaycastAll(s_PointerEventData, s_RaycastResultList);
            for (var i = 0; i < s_RaycastResultList.Count; i++)
            {
                // 是UI，同时还不是Mask作用的物体
                if (s_RaycastResultList[i].gameObject.name != "Mask")
                {
                    s_RaycastResultList.Clear();
                    return true;
                }
            }
            s_RaycastResultList.Clear();
            return false;
        }

        #endregion
    }
}