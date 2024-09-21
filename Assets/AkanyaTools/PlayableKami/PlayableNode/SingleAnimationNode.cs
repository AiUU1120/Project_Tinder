/*
* @Author: AiUU
* @Description: 单个 Playable 动画节点类
* @AkanyaTech.FrameTools
*/

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FrameTools.PlayableKami.PlayableNode
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