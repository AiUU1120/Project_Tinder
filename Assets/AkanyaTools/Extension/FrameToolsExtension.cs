/*
* @Author: AiUU
* @Description: 框架拓展方法
* @AkanyaTech.FrameTools
*/

using System;
using System.Collections;
using JKFrame;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrameTools.Extension
{
    public static class FrameToolsExtension
    {
        #region Common

        /// <summary>
        /// 数组相等对比
        /// </summary>
        public static bool ArrayEquals(this object[] objs, object[] other)
        {
            if (other == null || objs.GetType() != other.GetType())
            {
                return false;
            }
            if (objs.Length == other.Length)
            {
                for (var i = 0; i < objs.Length; i++)
                {
                    if (!objs[i].Equals(other[i]))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        #endregion

        #region GameObject

        public static bool IsNull(this GameObject obj) => ReferenceEquals(obj, null);

        #endregion

        #region ResourceSystem

        /// <summary>
        /// GameObject放入对象池
        /// </summary>
        public static void GameObjectPushPool(this GameObject go)
        {
            if (go.IsNull())
            {
                JKLog.Error("将空物体放入对象池");
            }
            else
            {
                PoolSystem.PushGameObject(go);
            }
        }

        /// <summary>
        /// GameObject放入对象池
        /// </summary>
        public static void GameObjectPushPool(this Component com)
        {
            GameObjectPushPool(com.gameObject);
        }

        /// <summary>
        /// 普通类放进池子
        /// </summary>
        public static void ObjectPushPool(this object obj)
        {
            PoolSystem.PushObject(obj);
        }

        #endregion

        #region Mono

        /// <summary>
        /// 添加Update监听
        /// </summary>
        public static void AddUpdate(this object obj, Action action)
        {
            MonoSystem.AddUpdateListener(action);
        }

        /// <summary>
        /// 移除Update监听
        /// </summary>
        public static void RemoveUpdate(this object obj, Action action)
        {
            MonoSystem.RemoveUpdateListener(action);
        }

        /// <summary>
        /// 添加LateUpdate监听
        /// </summary>
        public static void AddLateUpdate(this object obj, Action action)
        {
            MonoSystem.AddLateUpdateListener(action);
        }

        /// <summary>
        /// 移除LateUpdate监听
        /// </summary>
        public static void RemoveLateUpdate(this object obj, Action action)
        {
            MonoSystem.RemoveLateUpdateListener(action);
        }

        /// <summary>
        /// 添加FixedUpdate监听
        /// </summary>
        public static void AddFixedUpdate(this object obj, Action action)
        {
            MonoSystem.AddFixedUpdateListener(action);
        }

        /// <summary>
        /// 移除Update监听
        /// </summary>
        public static void RemoveFixedUpdate(this object obj, Action action)
        {
            MonoSystem.RemoveFixedUpdateListener(action);
        }

        public static Coroutine StartCoroutine(this object obj, IEnumerator routine) => MonoSystem.Start_Coroutine(obj, routine);

        public static void StopCoroutine(this object obj, Coroutine routine)
        {
            MonoSystem.Stop_Coroutine(obj, routine);
        }

        /// <summary>
        /// 关闭全部协程，注意只会关闭调用对象所属的协程
        /// </summary>
        /// <param name="obj"></param>
        public static void StopAllCoroutine(this object obj)
        {
            MonoSystem.StopAllCoroutine(obj);
        }

        #endregion

        #region UI Element

        /// <summary>
        /// 带错误提示的查找
        /// </summary>
        /// <param name="ve"></param>
        /// <param name="name">组件名</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns></returns>
        public static T NiceQ<T>(this VisualElement ve, string name) where T : VisualElement
        {
            var e = ve.Q<T>(name);
            if (e != null)
            {
                return e;
            }
            Debug.LogError($"SkillMaster: 没有找到 {name}!\n请检查是否修改了 UI Builder 组件名称!");
            return null;
        }

        #endregion
    }
}