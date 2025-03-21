using AkanyaTools.SkillMaster.Runtime.Core;

namespace GameCore.Character.Enemy.Skills.SkillBehaviour.Mace
{
    public class EnemyMaceAttackBehaviour : EnemySkillBehaviourBase
    {
        public override SkillBehaviourBase DeepCopy() => new EnemyMaceAttackBehaviour();

        public override void Release(bool calCDTimer = true)
        {
            base.Release(calCDTimer);
            m_SkillPlayer.StartPlaySkillBehaviour(this);
            m_SkillPlayer.PlaySkillClip(m_SkillConfig.clips[0]);
        }

        public override void OnSkillClipEnd()
        {
            base.OnSkillClipEnd();
            m_SkillOwner.ChangeToIdleState();
        }
    }
}