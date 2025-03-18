using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public struct AttackData
    {
        public SkillDetectionFrameEvent e;

        public ISkillCharacter atkSource;

        public Vector3 hitPoint;

        public Vector3 hitNormal;

        public float atkValue;
    }
}