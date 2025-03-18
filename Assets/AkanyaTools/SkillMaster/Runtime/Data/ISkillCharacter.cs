using AkanyaTools.PlayableKami;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public interface ISkillCharacter : IHitTarget
    {
        public AnimationController animationController { get; }

        public Transform transform { get; }

        public int GetAtkValue(SkillDetectionFrameEvent e);

        public void OnSkillRotate();

        public void ChangeToIdleState();

        public void OnSkillMove(Vector3 deltaPosition);

        public void OnSkillRotate(Quaternion deltaRotation);
    }
}