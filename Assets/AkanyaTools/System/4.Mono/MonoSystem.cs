using System;
using System.Collections;
using System.Collections.Generic;
using AkanyaTools;
using UnityEngine;

namespace JKFrame
{
    public sealed class MonoSystem : MonoBehaviour
    {
        private MonoSystem()
        {
        }

        private static MonoSystem s_Instance;
        private Action m_UpdateEvent;
        private Action m_LateUpdateEvent;
        private Action m_FixedUpdateEvent;

        public static void Init()
        {
            s_Instance = FrameRoot.RootTransform.GetComponent<MonoSystem>();
            s_Instance.m_UpdateEvent = null;
            s_Instance.m_LateUpdateEvent = null;
            s_Instance.m_FixedUpdateEvent = null;
        }

        #region 生命周期函数

        /// <summary>
        /// 添加Update监听
        /// </summary>
        /// <param name="action"></param>
        public static void AddUpdateListener(Action action)
        {
            s_Instance.m_UpdateEvent += action;
        }

        /// <summary>
        /// 移除Update监听
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveUpdateListener(Action action)
        {
            s_Instance.m_UpdateEvent -= action;
        }

        /// <summary>
        /// 添加LateUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void AddLateUpdateListener(Action action)
        {
            s_Instance.m_LateUpdateEvent += action;
        }

        /// <summary>
        /// 移除LateUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveLateUpdateListener(Action action)
        {
            s_Instance.m_LateUpdateEvent -= action;
        }

        /// <summary>
        /// 添加FixedUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void AddFixedUpdateListener(Action action)
        {
            s_Instance.m_FixedUpdateEvent += action;
        }

        /// <summary>
        /// 移除FixedUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveFixedUpdateListener(Action action)
        {
            s_Instance.m_FixedUpdateEvent -= action;
        }

        private void Update()
        {
            m_UpdateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            m_LateUpdateEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            m_FixedUpdateEvent?.Invoke();
        }

        #endregion

        #region 协程

        private Dictionary<object, List<Coroutine>> m_CoroutineDic = new();
        private static ObjectPoolModule s_PoolModule = new();

        /// <summary>
        /// 启动一个协程序
        /// </summary>
        public static Coroutine Start_Coroutine(IEnumerator coroutine) => s_Instance.StartCoroutine(coroutine);

        /// <summary>
        /// 启动一个协程序并且绑定某个对象
        /// </summary>
        public static Coroutine Start_Coroutine(object obj, IEnumerator coroutine)
        {
            var _coroutine = s_Instance.StartCoroutine(coroutine);
            if (!s_Instance.m_CoroutineDic.TryGetValue(obj, out var coroutineList))
            {
                coroutineList = s_PoolModule.GetObject<List<Coroutine>>() ?? new List<Coroutine>();
                s_Instance.m_CoroutineDic.Add(obj, coroutineList);
            }
            coroutineList.Add(_coroutine);
            return _coroutine;
        }

        /// <summary>
        /// 停止一个协程序并基于某个对象
        /// </summary>
        public static void Stop_Coroutine(object obj, Coroutine routine)
        {
            if (s_Instance.m_CoroutineDic.TryGetValue(obj, out var coroutineList))
            {
                s_Instance.StopCoroutine(routine);
                coroutineList.Remove(routine);
            }
        }

        /// <summary>
        /// 停止一个协程序
        /// </summary>
        public static void Stop_Coroutine(Coroutine routine)
        {
            s_Instance.StopCoroutine(routine);
        }

        /// <summary>
        /// 停止某个对象的全部协程
        /// </summary>
        public static void StopAllCoroutine(object obj)
        {
            if (s_Instance.m_CoroutineDic.Remove(obj, out var coroutineList))
            {
                for (var i = 0; i < coroutineList.Count; i++)
                {
                    s_Instance.StopCoroutine(coroutineList[i]);
                }
                coroutineList.Clear();
                s_PoolModule.PushObject(coroutineList);
            }
        }

        /// <summary>
        /// 整个系统全部协程都会停止
        /// </summary>
        public static void StopAllCoroutine()
        {
            // 全部数据都会无效
            foreach (List<Coroutine> item in s_Instance.m_CoroutineDic.Values)
            {
                item.Clear();
                s_PoolModule.PushObject(item);
            }
            s_Instance.m_CoroutineDic.Clear();
            s_Instance.StopAllCoroutines();
        }

        #endregion
    }
}