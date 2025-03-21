/*
 * @Author: AiUU
 * @Description: 敌人状态基类
 * @AkanyaTech.Tinder
 */

using AkanyaTools.AudioSystem;
using FrameTools.StateMachine;

namespace GameCore.Character.Enemy.State
{
    public abstract class EnemyStateBase : StateBase
    {
        protected EnemyController m_EnemyController;

        protected static int m_CurReleaseSkillIndex;

        protected static bool m_IsFighting;

        public override void Init(IStateMachineOwner owner)
        {
            m_EnemyController = owner as EnemyController;
        }

        protected bool CheckAndEnterSkillState()
        {
            if (m_EnemyController.skillBrain.CheckReleaseSkill(0))
            {
                m_CurReleaseSkillIndex = 0;
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
            var clips = m_EnemyController.weaponConfig.footStepAudioClips;
            var index = UnityEngine.Random.Range(0, clips.Length);
            AudioManager.PlayOneShot(clips[index], m_EnemyController.transform.position, volumeScale: 0.2f);
        }
    }
}