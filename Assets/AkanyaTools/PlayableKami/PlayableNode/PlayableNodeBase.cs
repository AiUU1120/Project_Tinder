/*
* @Author: AiUU
* @Description: Playable 节点基类
* @AkanyaTech.PlayableKami
*/

using FrameTools.Extension;

namespace AkanyaTools.PlayableKami.PlayableNode
{
    public abstract class PlayableNodeBase
    {
        /// <summary>
        /// 目标节点的输入端口号
        /// </summary>
        public int inputPort;

        /// <summary>
        /// 设置速度
        /// </summary>
        /// <param name="speed"></param>
        public abstract void SetSpeed(float speed);

        /// <summary>
        /// 回收Node
        /// </summary>
        public virtual void Recycle()
        {
            this.ObjectPushPool();
        }
    }
}