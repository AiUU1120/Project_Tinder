using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Core;

namespace GameCore.Character.UnityChan
{
    public sealed class UnityChanSkillBrain : SkillBrainBase
    {
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