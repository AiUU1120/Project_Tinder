/*
* @Author: AiUU
* @Description: Playable 动画控制器
* @AkanyaTech.FrameTools
*/

using System;
using System.Collections;
using System.Collections.Generic;
using FrameTools.PlayableKami.PlayableNode;
using FrameTools.ResourceSystem;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace FrameTools.PlayableKami
{
    public sealed class AnimationController : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private string m_GraphName = "";

        /// <summary>
        /// 动画速度
        /// </summary>
        public float playSpeed
        {
            get => m_PlaySpeed;
            set
            {
                m_PlaySpeed = value;
                m_CurrNode.SetSpeed(value);
            }
        }

        private PlayableGraph m_PlayableGraph;

        private AnimationMixerPlayable m_MixerPlayable;

        private PlayableNodeBase m_PreNode;

        private PlayableNodeBase m_CurrNode;

        private int m_InputPortFrom = 0;

        private int m_InputPortTo = 1;

        private Coroutine m_TransitionCoroutine;

        private readonly Dictionary<string, Action> m_AnimEventDic = new();

        private Action<Vector3, Quaternion> m_OnRootMotion;

        private float m_PlaySpeed;

        private void Start()
        {
            if (m_GraphName == "")
            {
                m_GraphName = GetHashCode().ToString();
            }
            // 创建图
            m_PlayableGraph = PlayableGraph.Create(m_GraphName);
            // 设置图的时间模式为 Time.time
            m_PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            // 创建混合器
            m_MixerPlayable = AnimationMixerPlayable.Create(m_PlayableGraph, 3);
            // 创建输出
            var playableOutput = AnimationPlayableOutput.Create(m_PlayableGraph, "Animation", m_Animator);
            // 让混合器连接输出
            playableOutput.SetSourcePlayable(m_MixerPlayable);
        }

        #region Animation

        /// <summary>
        /// 播放单个动画
        /// </summary>
        /// <param name="clip">要播放的动画片段</param>
        /// <param name="onRootMotion">根运动回调</param>
        /// <param name="speed">动画播放速度</param>
        /// <param name="blockSameAnim">是否忽略相同动画</param>
        /// <param name="mixingTime">过渡时间</param>
        public void PlaySingleAnimation(AnimationClip clip, Action<Vector3, Quaternion> onRootMotion = null, float speed = 1f, bool blockSameAnim = true, float mixingTime = 0.25f)
        {
            if (onRootMotion != null)
            {
                SetOnRootMotion(onRootMotion);
            }
            SingleAnimationNode singleAnimationNode;
            // 首次播放 不需要过渡
            if (m_CurrNode == null)
            {
                singleAnimationNode = ResourceManager.GetOrNew<SingleAnimationNode>();
                singleAnimationNode.Init(m_PlayableGraph, m_MixerPlayable, clip, speed, m_InputPortFrom);
                m_MixerPlayable.SetInputWeight(m_InputPortFrom, 1);
            }
            else
            {
                if (blockSameAnim && m_CurrNode is SingleAnimationNode preNode && preNode.GetAnimationClip() == clip)
                {
                    return;
                }
                RecycleNode(m_PreNode);
                singleAnimationNode = ResourceManager.GetOrNew<SingleAnimationNode>();
                singleAnimationNode.Init(m_PlayableGraph, m_MixerPlayable, clip, speed, m_InputPortTo);
                m_PreNode = m_CurrNode;
                StartTransitAnimation(mixingTime);
            }
            m_CurrNode = singleAnimationNode;
            playSpeed = speed;
            if (!m_PlayableGraph.IsPlaying())
            {
                m_PlayableGraph.Play();
            }
        }

        /// <summary>
        /// 播放混合动画
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="onRootMotion">根运动回调</param>
        /// <param name="speed">动画播放速度</param>
        /// <param name="mixingTime">过渡时间</param>
        public void PlayBlendAnimation(List<AnimationClip> clips, Action<Vector3, Quaternion> onRootMotion = null, float speed = 1f, float mixingTime = 0.25f)
        {
            if (onRootMotion != null)
            {
                SetOnRootMotion(onRootMotion);
            }
            var blendAnimNode = ResourceManager.GetOrNew<BlendAnimationNode>();
            if (m_CurrNode == null)
            {
                blendAnimNode.Init(m_PlayableGraph, m_MixerPlayable, clips, speed, m_InputPortFrom);
                m_MixerPlayable.SetInputWeight(m_InputPortFrom, 1);
            }
            else
            {
                RecycleNode(m_PreNode);
                blendAnimNode.Init(m_PlayableGraph, m_MixerPlayable, clips, speed, m_InputPortTo);
                m_PreNode = m_CurrNode;
                StartTransitAnimation(mixingTime);
            }
            m_CurrNode = blendAnimNode;
            playSpeed = speed;
            if (!m_PlayableGraph.IsPlaying())
            {
                m_PlayableGraph.Play();
            }
        }

        /// <summary>
        /// 播放混合动画
        /// </summary>
        /// <param name="clip1">动画1</param>
        /// <param name="clip2">动画2</param>
        /// <param name="onRootMotion">跟运动回调</param>
        /// <param name="speed">动画播放速度</param>
        /// <param name="mixingTime">过渡时间</param>
        public void PlayBlendAnimation(AnimationClip clip1, AnimationClip clip2, Action<Vector3, Quaternion> onRootMotion = null, float speed = 1f, float mixingTime = 0.25f)
        {
            if (onRootMotion != null)
            {
                SetOnRootMotion(onRootMotion);
            }
            var blendAnimNode = ResourceManager.GetOrNew<BlendAnimationNode>();
            if (m_CurrNode == null)
            {
                blendAnimNode.Init(m_PlayableGraph, m_MixerPlayable, clip1, clip2, speed, m_InputPortFrom);
                m_MixerPlayable.SetInputWeight(m_InputPortFrom, 1);
            }
            else
            {
                RecycleNode(m_PreNode);
                blendAnimNode.Init(m_PlayableGraph, m_MixerPlayable, clip1, clip2, speed, m_InputPortTo);
                m_PreNode = m_CurrNode;
                StartTransitAnimation(mixingTime);
            }
            m_CurrNode = blendAnimNode;
            playSpeed = speed;
            if (!m_PlayableGraph.IsPlaying())
            {
                m_PlayableGraph.Play();
            }
        }

        /// <summary>
        /// 设置混合权重
        /// </summary>
        /// <param name="weights">权重数组</param>
        public void SetBlendAnimationWeight(IReadOnlyList<float> weights)
        {
            if (m_CurrNode is BlendAnimationNode node)
            {
                node.SetBlendAnimationWeight(weights);
            }
            else
            {
                Debug.LogError("该 node 不是混合动画节点!");
            }
        }

        /// <summary>
        /// 设置混合权重
        /// </summary>
        /// <param name="weightTo">后一个动画权重</param>
        public void SetBlendAnimationWeight(float weightTo)
        {
            if (m_CurrNode is BlendAnimationNode node)
            {
                node.SetBlendAnimationWeight(weightTo);
            }
            else
            {
                Debug.LogError("该 node 不是混合动画节点!");
            }
        }

        /// <summary>
        /// 开始过渡
        /// </summary>
        /// <param name="mixingTime">过渡时间</param>
        private void StartTransitAnimation(float mixingTime)
        {
            if (m_TransitionCoroutine != null)
            {
                StopCoroutine(m_TransitionCoroutine);
            }
            // 开始过渡
            m_TransitionCoroutine = StartCoroutine(TransitAnimation(mixingTime));
        }

        /// <summary>
        /// 动画过渡
        /// </summary>
        /// <param name="mixingTime">混合时间</param>
        /// <returns></returns>
        private IEnumerator TransitAnimation(float mixingTime)
        {
            // 交换端口号 由于端口号已被交换 以下操作颠倒
            (m_InputPortFrom, m_InputPortTo) = (m_InputPortTo, m_InputPortFrom);

            // 硬切
            if (mixingTime <= 0.00001f)
            {
                m_MixerPlayable.SetInputWeight(m_InputPortTo, 0);
                m_MixerPlayable.SetInputWeight(m_InputPortFrom, 1);
            }

            var curWeight = 1f;
            var speed = 1 / mixingTime;

            while (curWeight > 0)
            {
                curWeight = Mathf.Clamp01(curWeight - Time.deltaTime * speed);
                m_MixerPlayable.SetInputWeight(m_InputPortTo, curWeight);
                m_MixerPlayable.SetInputWeight(m_InputPortFrom, 1 - curWeight);
                yield return null;
            }
            m_TransitionCoroutine = null;
        }

        /// <summary>
        /// 回收 Node
        /// </summary>
        /// <param name="node"></param>
        private void RecycleNode(PlayableNodeBase node)
        {
            if (node == null)
            {
                return;
            }
            m_PlayableGraph.Disconnect(m_MixerPlayable, node.inputPort);
            node.Recycle();
        }

        #endregion

        #region Animation Event

        private void AnimationEvent(string eventName)
        {
            if (m_AnimEventDic.TryGetValue(eventName, out var e))
            {
                e?.Invoke();
            }
        }

        /// <summary>
        /// 添加动画事件
        /// </summary>
        /// <param name="eventName">事件 key</param>
        /// <param name="action">具体 Action</param>
        public void AddAnimationEvent(string eventName, Action action)
        {
            if (m_AnimEventDic.ContainsKey(eventName))
            {
                m_AnimEventDic[eventName] += action;
            }
            else
            {
                m_AnimEventDic.Add(eventName, action);
            }
        }

        /// <summary>
        /// 移除 key 下所有 Action
        /// </summary>
        /// <param name="eventName">事件 key</param>
        public void RemoveAnimationEvent(string eventName)
        {
            m_AnimEventDic.Remove(eventName);
        }

        /// <summary>
        /// 移除 key 下指定 Action
        /// </summary>
        /// <param name="eventName">事件 key</param>
        /// <param name="action">具体 Action</param>
        public void RemoveAnimationEvent(string eventName, Action action)
        {
            if (m_AnimEventDic.ContainsKey(eventName))
            {
                m_AnimEventDic[eventName] -= action;
            }
            else
            {
                Debug.LogWarning($"尝试移除不存在的事件 {eventName}");
            }
        }

        /// <summary>
        /// 清除动画控制器中所有动画事件
        /// </summary>
        public void ClearAllAnimationEvent()
        {
            m_AnimEventDic.Clear();
        }

        #endregion

        #region Root Motion

        private void OnAnimatorMove()
        {
            m_OnRootMotion?.Invoke(m_Animator.deltaPosition, m_Animator.deltaRotation);
        }

        /// <summary>
        /// 设置根运动回调
        /// </summary>
        /// <param name="onRootMotion"></param>
        public void SetOnRootMotion(Action<Vector3, Quaternion> onRootMotion)
        {
            m_OnRootMotion = onRootMotion;
        }

        /// <summary>
        /// 清理根运动回调
        /// </summary>
        public void ClearOnRootMotion()
        {
            m_OnRootMotion = null;
        }

        #endregion

        private void OnDisable()
        {
            m_PlayableGraph.Stop();
        }

        private void OnDestroy()
        {
            m_PlayableGraph.Destroy();
            ClearOnRootMotion();
        }
    }
}