/*
* @Author: AiUU
* @Description: 单例基类
* @AkanyaTech.FrameTools
*/

namespace FrameTools.Base.Singleton
{
    /// <summary>
    /// 单例基类
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T s_Instance;
        public static T instance => s_Instance ??= new T();
    }
}