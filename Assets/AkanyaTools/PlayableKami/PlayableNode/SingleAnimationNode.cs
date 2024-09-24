/*
* @Author: AiUU
* @Description: 单个 Playable 动画节点类
* @AkanyaTech.PlayableKami
*/

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AkanyaTools.PlayableKami.PlayableNode
{
    public sealed class SingleAnimationNode : PlayableNodeBase
    {
        private AnimationClipPlayable m_ClipPlayable;

        public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, AnimationClip clip, float speed, int inputPortIdx)
        {
            m_ClipPlayable = AnimationClipPlayable.Create(graph, clip);
            m_ClipPlayable.SetSpeed(speed);
            inputPort = inputPortIdx;
            graph.Connect(m_ClipPlayable, 0, outputMixer, inputPortIdx);
        }

        public AnimationClip GetAnimationClip() => m_ClipPlayable.GetAnimationClip();

        public override void SetSpeed(float speed)
        {
            m_ClipPlayable.SetSpeed(speed);
        }
    }
}