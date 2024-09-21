﻿using FrameTools.Extension;

namespace FrameTools.StateMachine
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class StateBase
    {
        protected FrameTools.StateMachine.StateMachine stateMachine;

        /// <summary>
        /// 初始化内部数据，系统使用
        /// </summary>
        /// <param name="sm"></param>
        public void InitInternalData(FrameTools.StateMachine.StateMachine sm)
        {
            stateMachine = sm;
        }

        /// <summary>
        /// 初始化状态
        /// 只在状态第一次创建时执行
        /// </summary>
        /// <param name="owner">宿主</param>
        public virtual void Init(IStateMachineOwner owner)
        {
        }

        /// <summary>
        /// 反初始化
        /// 不再使用时候，放回对象池时调用
        /// 把一些引用置空，防止不能被GC
        /// </summary>
        public virtual void UnInit()
        {
            stateMachine = null;
            // 放回对象池
            this.ObjectPushPool();
        }

        /// <summary>
        /// 状态进入
        /// 每次进入都会执行
        /// </summary>
        public virtual void Enter()
        {
        }

        /// <summary>
        /// 状态退出
        /// </summary>
        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public bool TryGetShareData<T>(string key, out T data) => stateMachine.TryGetShareData<T>(key, out data);

        public void AddShareData(string key, object data)
        {
            stateMachine.AddShareData(key, data);
        }

        public void RemoveShareData(string key)
        {
            stateMachine.RemoveShareData(key);
        }

        public void UpdateShareData(string key, object data)
        {
            stateMachine.UpdateShareData(key, data);
        }

        public void CleanShareData()
        {
            stateMachine.CleanShareData();
        }

        public bool ContainsShareData(string key) => stateMachine.ContainsShareData(key);
    }
}