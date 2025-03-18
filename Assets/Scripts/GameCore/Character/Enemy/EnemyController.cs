/*
 * @Author: AiUU
 * @Description: 敌人控制器
 * @AkanyaTech.Tinder
 */

using AkanyaTools.PlayableKami;
using AkanyaTools.SkillMaster.Runtime.Data;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;

namespace GameCore.Character.Enemy
{
    public sealed class EnemyController : MonoBehaviour, ISkillCharacter
    {
        public AnimationController animationController { get; }

        public void BeHit(AttackData attackData)
        {
            Debug.Log($"我被打了 {attackData.atkValue} 点伤害");
        }

        public int GetAtkValue(SkillDetectionFrameEvent e) => 0;

        public void OnSkillRotate()
        {
        }

        public void ChangeToIdleState()
        {
        }

        public void OnSkillMove(Vector3 deltaPosition)
        {
        }

        public void OnSkillRotate(Quaternion deltaRotation)
        {
        }
    }
}