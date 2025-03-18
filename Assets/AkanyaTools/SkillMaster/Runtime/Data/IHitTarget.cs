/*
 * @Author: AiUU
 * @Description: SkillMaster 可击中对象接口
 * @AkanyaTech.SkillMaster
 */

namespace AkanyaTools.SkillMaster.Runtime.Data
{
    public interface IHitTarget
    {
        public void BeHit(AttackData attackData);
    }
}