/*
 * @Author: AiUU
 * @Description: Unity酱状态基类
 * @AkanyaTech.Tinder
 */

using AkanyaTools.AudioSystem;
using Common.System;
using Data;
using FrameTools.StateMachine;

namespace GameCore.Character.Player.State
{
    public abstract class PlayerStateBase : StateBase
    {
        protected PlayerController unityChanController;

        protected static int curReleaseSkillIndex;

        // TODO: 旁通？
        // /// <summary>
        // /// 是否旁通 (不执行状态逻辑)
        // /// </summary>
        // protected bool isPass { get; private set; }

        public override void Init(IStateMachineOwner owner)
        {
            unityChanController = owner as PlayerController;
        }

        public override void Enter()
        {
            base.Enter();
            // isPass = CheckStateChange();
        }

        protected bool CheckAndEnterSkillState()
        {
            if (InputManager.instance.isAttackLight && unityChanController.skillBrain.CheckReleaseSkill(0))
            {
                curReleaseSkillIndex = 0;
                return true;
            }
            if (InputManager.instance.isAttackHeavy && unityChanController.skillBrain.CheckReleaseSkill(1))
            {
                curReleaseSkillIndex = 1;
                return true;
            }
            if (InputManager.instance.isSpecial)
            {
                if (!unityChanController.skillBrain.CheckReleaseSkill(DataManager.playerData.shortcutSkillData.skillIndex))
                {
                    return false;
                }
                curReleaseSkillIndex = DataManager.playerData.shortcutSkillData.skillIndex;
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
            var clips = unityChanController.weaponConfig.footStepAudioClips;
            var index = UnityEngine.Random.Range(0, clips.Length);
            AudioManager.PlayOneShot(clips[index], unityChanController.transform.position, volumeScale: 0.2f);
        }
    }
}