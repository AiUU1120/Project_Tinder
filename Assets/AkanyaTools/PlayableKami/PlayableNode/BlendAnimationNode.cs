/*
* @Author: AiUU
* @Description: 混合 Playable 动画节点类
* @AkanyaTech.PlayableKami
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AkanyaTools.PlayableKami.PlayableNode
{
    public sealed class BlendAnimationNode : PlayableNodeBase
    {
        private AnimationMixerPlayable m_BlendMixerPlayable;

        private readonly List<AnimationClipPlayable> m_BlendAnimClipPlayables = new(10);

        public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, List<AnimationClip> clips, float speed, int inputPortIdx)
        {
            m_BlendMixerPlayable = AnimationMixerPlayable.Create(graph, clips.Count);
            inputPort = inputPortIdx;
            graph.Connect(m_BlendMixerPlayable, 0, outputMixer, inputPortIdx);
            for (var i = 0; i < clips.Count; i++)
            {
                CreateAndConnectBlendPlayable(graph, clips[i], i, speed);
            }
        }

        public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, AnimationClip clip1, AnimationClip clip2, float speed, int inputPortIdx)
        {
            m_BlendMixerPlayable = AnimationMixerPlayable.Create(graph, 2);
            inputPort = inputPortIdx;
            graph.Connect(m_BlendMixerPlayable, 0, outputMixer, inputPortIdx);
            CreateAndConnectBlendPlayable(graph, clip1, 0, speed);
            CreateAndConnectBlendPlayable(graph, clip2, 1, speed);
        }

        /// <summary>
        /// 设置混合权重
        /// </summary>
        /// <param name="weights">权重数组</param>
        public void SetBlendAnimationWeight(IReadOnlyList<float> weights)
        {
            for (var i = 0; i < m_BlendAnimClipPlayables.Count; i++)
            {
                m_BlendMixerPlayable.SetInputWeight(i, weights[i]);
            }
        }

        /// <summary>
        /// 设置混合权重
        /// </summary>
        /// <param name="weightTo">后一个动画权重</param>
        public void SetBlendAnimationWeight(float weightTo)
        {
            m_BlendMixerPlayable.SetInputWeight(0, 1 - weightTo);
            m_BlendMixerPlayable.SetInputWeight(1, weightTo);
        }

        public override void SetSpeed(float speed)
        {
            foreach (var clip in m_BlendAnimClipPlayables)
            {
                clip.SetSpeed(speed);
            }
        }

        public override void Recycle()
        {
            m_BlendAnimClipPlayables.Clear();
            base.Recycle();
        }

        /// <summary>
        /// 创建并连接 AnimationClipPlayable
        /// </summary>
        /// <returns></returns>
        private AnimationClipPlayable CreateAndConnectBlendPlayable(PlayableGraph graph, AnimationClip clip, int index, float speed)
        {
            var clipPlayable = AnimationClipPlayable.Create(graph, clip);
            clipPlayable.SetSpeed(speed);
            m_BlendAnimClipPlayables.Add(clipPlayable);
            graph.Connect(clipPlayable, 0, m_BlendMixerPlayable, index);
            return clipPlayable;
        }
    }
}