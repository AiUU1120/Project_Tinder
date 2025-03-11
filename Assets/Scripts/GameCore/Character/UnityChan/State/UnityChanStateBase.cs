/*
 * @Author: AiUU
 * @Description: Unity酱状态基类
 * @AkanyaTech.Tinder
 */

using AkanyaTools.AudioSystem;
using FrameTools.StateMachine;

namespace GameCore.Character.UnityChan.State
{
    public abstract class UnityChanStateBase : StateBase
    {
        protected UnityChanController unityChanController;

        protected static int curReleaseSkillIndex;

        // TODO: 旁通？
        // /// <summary>
        // /// 是否旁通 (不执行状态逻辑)
        // /// </summary>
        // protected bool isPass { get; private set; }

        public override void Init(IStateMachineOwner owner)
        {
            unityChanController = owner as UnityChanController;
        }

        public override void Enter()
        {
            base.Enter();
            // isPass = CheckStateChange();
        }

        protected bool CheckAndEnterSkillState()
        {
            if (unityChanController.input.isAttackLight && unityChanController.skillBrain.CheckReleaseSkill(0))
            {
                curReleaseSkillIndex = 0;
                return true;
            }
            else if (unityChanController.input.isAttackHeavy && unityChanController.skillBrain.CheckReleaseSkill(1))
            {
                curReleaseSkillIndex = 1;
                return true;
            }
            else if (unityChanController.input.isSpecial && unityChanController.skillBrain.CheckReleaseSkill(2))
            {
                curReleaseSkillIndex = 2;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查状态切换
        /// </summary>
        /// <returns></returns>
        protected abstract bool CheckStateChange();

        /// <summary>
        /// 脚步声方法回调
        /// </summary>
        protected virtual void OnFootStep()
        {
            var clips = unityChanController.characterConfig.footStepAudioClips;
            var index = UnityEngine.Random.Range(0, clips.Length);
            AudioManager.PlayOneShot(clips[index], unityChanController.transform.position, volumeScale: 0.2f);
        }
    }
}