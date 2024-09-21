/*
* @Author: AiUU
* @Description: 单例组件基类
* @AkanyaTech.FrameTools
*/

using UnityEngine;

namespace FrameTools.Base.Singleton
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        public static T instance;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
        }
    }
}