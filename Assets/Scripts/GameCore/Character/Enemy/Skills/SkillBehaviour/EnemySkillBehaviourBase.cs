using AkanyaTools.SkillMaster.Runtime.Core;

namespace GameCore.Character.Enemy.Skills.SkillBehaviour
{
    public abstract class EnemySkillBehaviourBase : SkillBehaviourBase
    {
        protected override void RotateOnUpdate()
        {
            if (m_CanRotate)
            {
                m_SkillOwner.OnSkillRotate();
            }
        }
    }
}