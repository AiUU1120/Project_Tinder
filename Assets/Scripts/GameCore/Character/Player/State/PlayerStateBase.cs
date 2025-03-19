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
        protected PlayerController m_PlayerController;

        protected static int m_CurReleaseSkillIndex;

        // TODO: 旁通？
        // /// <summary>
        // /// 是否旁通 (不执行状态逻辑)
        // /// </summary>
        // protected bool isPass { get; private set; }

        public override void Init(IStateMachineOwner owner)
        {
            m_PlayerController = owner as PlayerController;
        }

        public override void Enter()
        {
            base.Enter();
            // isPass = CheckStateChange();
        }

        protected bool CheckAndEnterSkillState()
        {
            if (InputManager.instance.isAttackLight && m_PlayerController.skillBrain.CheckReleaseSkill(0))
            {
                m_CurReleaseSkillIndex = 0;
                return true;
            }
            if (InputManager.instance.isAttackHeavy && m_PlayerController.skillBrain.CheckReleaseSkill(1))
            {
                m_CurReleaseSkillIndex = 1;
                return true;
            }
            if (InputManager.instance.isSpecial)
            {
                if (!m_PlayerController.skillBrain.CheckReleaseSkill(DataManager.playerData.shortcutSkillDataDic.Dictionary[DataManager.playerData.curWeaponId].skillIndex))
                {
                    return false;
                }
                m_CurReleaseSkillIndex = DataManager.playerData.shortcutSkillDataDic.Dictionary[DataManager.playerData.curWeaponId].skillIndex;
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
            var clips = m_PlayerController.weaponConfig.footStepAudioClips;
            var index = UnityEngine.Random.Range(0, clips.Length);
            AudioManager.PlayOneShot(clips[index], m_PlayerController.transform.position, volumeScale: 0.2f);
        }
    }
}