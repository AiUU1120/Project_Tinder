/*
* @Author: AiUU
* @Description: Unity酱状态基类
* @AkanyaTech.Tinder
*/

using FrameTools.StateMachine;

namespace GameCore.Character.UnityChan.State
{
    public abstract class UnityChanStateBase : StateBase
    {
        protected UnityChanController unityChanController;

        /// <summary>
        /// 是否旁通 (不执行状态逻辑)
        /// </summary>
        protected bool isPass { get; private set; }

        public override void Init(IStateMachineOwner owner)
        {
            unityChanController = owner as UnityChanController;
        }

        public override void Enter()
        {
            base.Enter();
            isPass = CheckStateChange();
        }

        /// <summary>
        /// 检查状态切换
        /// </summary>
        /// <returns></returns>
        protected abstract bool CheckStateChange();
    }
}