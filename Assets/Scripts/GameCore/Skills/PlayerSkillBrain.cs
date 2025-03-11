using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;
using GameCore.Character.UnityChan;

namespace GameCore.Skills
{
    public sealed class PlayerSkillBrain : SkillBrainBase
    {
        /// <summary>
        /// 攻击是否保持连段的共享数据的 key
        /// </summary>
        public const string continuous_attack_mode_data_key = "ContinuousAttackMode";

        private UnityChanController m_UnityChanController;

        public override void Init(PlayerControllerBase playerController)
        {
            base.Init(playerController);
            m_UnityChanController = playerController as UnityChanController;
            if (m_UnityChanController != null)
            {
                skillPlayer.Init(m_UnityChanController.animationController, m_UnityChanController.transform);
            }
        }
    }
}