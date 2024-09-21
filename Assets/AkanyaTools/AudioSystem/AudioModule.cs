using System;
using System.Collections;
using System.Collections.Generic;
using FrameTools.Extension;
using FrameTools.ResourceSystem;
using JKFrame;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace FrameTools.AudioSystem
{
    public sealed class AudioModule : MonoBehaviour
    {
        private static GameObjectPoolModule s_PoolModule;

        [FormerlySerializedAs("BGAudioSource")]
        [SerializeField, LabelText("背景音乐播放器")]
        private AudioSource m_BgAudioSource;

        [FormerlySerializedAs("EffectAudioPlayPrefab")]
        [SerializeField, LabelText("效果播放器预制体")]
        private GameObject m_EffectAudioPlayPrefab;

        [SerializeField, LabelText("对象池预设播放器数量")]
        private int m_EffectAudioDefaultQuantity = 20;

        // 场景中生效的所有特效音乐播放器
        private List<AudioSource> m_AudioPlayList;

        #region 音量、播放控制

        [SerializeField, Range(0, 1), OnValueChanged(nameof(UpdateAllAudioPlay))]
        private float m_GlobalVolume;

        public float globalVolume
        {
            get => m_GlobalVolume;
            set
            {
                if (Math.Abs(m_GlobalVolume - value) < 0.001)
                {
                    return;
                }
                m_GlobalVolume = value;
                UpdateAllAudioPlay();
            }
        }

        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged(nameof(UpdateBgAudioPlay))]
        private float m_BgVolume;

        public float bgVolume
        {
            get => m_BgVolume;
            set
            {
                if (Math.Abs(m_BgVolume - value) < 0.001)
                {
                    return;
                }
                m_BgVolume = value;
                UpdateBgAudioPlay();
            }
        }

        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged(nameof(UpdateEffectAudioPlay))]
        private float m_EffectVolume;

        public float effectVolume
        {
            get => m_EffectVolume;
            set
            {
                if (Math.Abs(m_EffectVolume - value) < 0.001)
                {
                    return;
                }
                m_EffectVolume = value;
                UpdateEffectAudioPlay();
            }
        }

        [SerializeField]
        [OnValueChanged(nameof(UpdateMute))]
        private bool m_IsMute;

        public bool isMute
        {
            get => m_IsMute;
            set
            {
                if (m_IsMute == value)
                {
                    return;
                }
                m_IsMute = value;
                UpdateMute();
            }
        }

        [SerializeField]
        [OnValueChanged(nameof(UpdateLoop))]
        private bool m_IsLoop = true;

        public bool isLoop
        {
            get => m_IsLoop;
            set
            {
                if (m_IsLoop == value)
                {
                    return;
                }
                m_IsLoop = value;
                UpdateLoop();
            }
        }

        [SerializeField]
        [OnValueChanged(nameof(UpdatePause))]
        private bool m_IsPause;

        public bool isPause
        {
            get => m_IsPause;
            set
            {
                if (m_IsPause == value)
                {
                    return;
                }
                m_IsPause = value;
                UpdatePause();
            }
        }

        /// <summary>
        /// 更新全部播放器类型
        /// </summary>
        private void UpdateAllAudioPlay()
        {
            UpdateBgAudioPlay();
            UpdateEffectAudioPlay();
        }

        /// <summary>
        /// 更新背景音乐
        /// </summary>
        private void UpdateBgAudioPlay()
        {
            m_BgAudioSource.volume = m_BgVolume * m_GlobalVolume;
        }

        /// <summary>
        /// 更新特效音乐播放器
        /// </summary>
        private void UpdateEffectAudioPlay()
        {
            if (m_AudioPlayList == null)
            {
                return;
            }
            // 倒序遍历
            for (var i = m_AudioPlayList.Count - 1; i >= 0; i--)
            {
                if (m_AudioPlayList[i] != null)
                {
                    SetEffectAudioPlay(m_AudioPlayList[i]);
                }
                else
                {
                    m_AudioPlayList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 设置特效音乐播放器
        /// </summary>
        private void SetEffectAudioPlay(AudioSource audioPlay, float spatial = -1)
        {
            audioPlay.mute = m_IsMute;
            audioPlay.volume = m_EffectVolume * m_GlobalVolume;
            if (spatial > -1)
            {
                audioPlay.spatialBlend = spatial;
            }
            if (m_IsPause)
            {
                audioPlay.Pause();
            }
            else
            {
                audioPlay.UnPause();
            }
        }

        /// <summary>
        /// 更新全局音乐静音情况
        /// </summary>
        private void UpdateMute()
        {
            m_BgAudioSource.mute = m_IsMute;
            UpdateEffectAudioPlay();
        }

        /// <summary>
        /// 更新背景音乐循环
        /// </summary>
        private void UpdateLoop()
        {
            m_BgAudioSource.loop = m_IsLoop;
        }

        /// <summary>
        /// 更新背景音乐暂停
        /// </summary>
        private void UpdatePause()
        {
            if (m_IsPause)
            {
                m_BgAudioSource.Pause();
            }
            else
            {
                m_BgAudioSource.UnPause();
            }
        }

        #endregion

        public void Init()
        {
            var poolRoot = new GameObject("AudioPlayerPoolRoot").transform;
            poolRoot.SetParent(transform);
            s_PoolModule = new GameObjectPoolModule();
            s_PoolModule.Init(poolRoot);
            s_PoolModule.InitObjectPool(m_EffectAudioPlayPrefab, -1, m_EffectAudioDefaultQuantity);
            m_AudioPlayList = new List<AudioSource>(m_EffectAudioDefaultQuantity);
            m_AudioPlayRoot = new GameObject("AudioPlayRoot").transform;
            m_AudioPlayRoot.SetParent(transform);
            UpdateAllAudioPlay();
        }

        #region 背景音乐

        private static Coroutine s_FadeCoroutine;

        public void PlayBgAudio(AudioClip clip, bool loop = true, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
        {
            isLoop = loop;
            if (volume > -1)
            {
                bgVolume = volume;
            }
            s_FadeCoroutine = StartCoroutine(DoVolumeFade(clip, fadeOutTime, fadeInTime));
        }

        private IEnumerator DoVolumeFade(AudioClip clip, float fadeOutTime, float fadeInTime)
        {
            float currTime = 0;
            if (fadeOutTime <= 0)
            {
                fadeOutTime = 0.0001f;
            }
            if (fadeInTime <= 0)
            {
                fadeInTime = 0.0001f;
            }

            // 淡出
            while (currTime < fadeOutTime)
            {
                yield return CoroutineTool.WaitForFrames();
                if (!m_IsPause)
                {
                    currTime += Time.deltaTime;
                }
                var ratio = Mathf.Lerp(1, 0, currTime / fadeOutTime);
                m_BgAudioSource.volume = m_BgVolume * m_GlobalVolume * ratio;
            }

            m_BgAudioSource.clip = clip;
            m_BgAudioSource.Play();
            currTime = 0;

            // 淡入
            while (currTime < fadeInTime)
            {
                yield return CoroutineTool.WaitForFrames();
                if (!m_IsPause)
                {
                    currTime += Time.deltaTime;
                }
                var ratio = Mathf.InverseLerp(0, 1, currTime / fadeInTime);
                m_BgAudioSource.volume = m_BgVolume * m_GlobalVolume * ratio;
            }
            s_FadeCoroutine = null;
        }

        private static Coroutine s_BgWithClipsCoroutine;

        /// <summary>
        /// 使用音效数组播放背景音乐，自动循环
        /// </summary>
        public void PlayBgAudioWithClips(AudioClip[] clips, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
        {
            s_BgWithClipsCoroutine = MonoSystem.Start_Coroutine(DoPlayBgAudioWithClips(clips, volume));
        }

        private IEnumerator DoPlayBgAudioWithClips(IReadOnlyList<AudioClip> clips, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
        {
            if (volume > -1)
            {
                bgVolume = volume;
            }
            var currIndex = 0;
            while (true)
            {
                var clip = clips[currIndex];
                s_FadeCoroutine = StartCoroutine(DoVolumeFade(clip, fadeOutTime, fadeInTime));
                var time = clip.length;
                // 时间只要还好 一直检测
                while (time > 0)
                {
                    yield return CoroutineTool.WaitForFrames();
                    if (!m_IsPause)
                    {
                        time -= Time.deltaTime;
                    }
                }
                // 到达这里说明倒计时结束 修改索引号 继续外侧While循环
                currIndex++;
                if (currIndex >= clips.Count)
                {
                    currIndex = 0;
                }
            }
        }

        public void StopBgAudio()
        {
            if (s_BgWithClipsCoroutine != null)
            {
                MonoSystem.Stop_Coroutine(s_BgWithClipsCoroutine);
            }
            if (s_FadeCoroutine != null)
            {
                MonoSystem.Stop_Coroutine(s_FadeCoroutine);
            }
            m_BgAudioSource.Stop();
            m_BgAudioSource.clip = null;
        }

        public void PauseBgAudio()
        {
            isPause = true;
        }

        public void UnPauseBgAudio()
        {
            isPause = false;
        }

        #endregion

        #region 特效音乐

        private Transform m_AudioPlayRoot;

        /// <summary>
        /// 获取音乐播放器
        /// </summary>
        /// <returns></returns>
        private AudioSource GetAudioPlay(bool is3D = true)
        {
            // 从对象池中获取播放器
            var audioPlay = s_PoolModule.GetObject("AudioPlay", m_AudioPlayRoot);
            if (audioPlay.IsNull())
            {
                audioPlay = Instantiate(m_EffectAudioPlayPrefab, m_AudioPlayRoot);
                audioPlay.name = m_EffectAudioPlayPrefab.name;
            }
            var audioSource = audioPlay.GetComponent<AudioSource>();
            SetEffectAudioPlay(audioSource, is3D ? 1f : 0f);
            m_AudioPlayList.Add(audioSource);
            return audioSource;
        }

        /// <summary>
        /// 回收播放器
        /// </summary>
        private void RecycleAudioPlay(AudioSource audioSource, AudioClip clip, bool autoReleaseClip, Action callBak)
        {
            StartCoroutine(DoRecycleAudioPlay(audioSource, clip, autoReleaseClip, callBak));
        }

        private IEnumerator DoRecycleAudioPlay(AudioSource audioSource, AudioClip clip, bool autoReleaseClip, Action callBak)
        {
            // 延迟 Clip的长度（秒）
            yield return CoroutineTool.WaitForSeconds(clip.length);
            // 放回池子
            if (audioSource == null)
            {
                yield break;
            }
            m_AudioPlayList.Remove(audioSource);
            s_PoolModule.PushObject(audioSource.gameObject);
            if (autoReleaseClip)
            {
                ResourceManager.UnloadAsset(clip);
            }
            callBak?.Invoke();
        }

        public void PlayOneShot(AudioClip clip, UnityEngine.Component component = null, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
        {
            // 初始化音乐播放器
            var audioSource = GetAudioPlay(is3d);
            if (component == null)
            {
                audioSource.transform.SetParent(null);
            }
            else
            {
                var sourceTransform = audioSource.transform;
                sourceTransform.SetParent(component.transform);
                sourceTransform.localPosition = Vector3.zero;
                // 宿主销毁时，释放父物体
                component.OnDestroy(OnOwnerDestroy, audioSource);
            }
            // 播放一次音效
            audioSource.PlayOneShot(clip, volumeScale);
            // 播放器回收以及回调函数
            callBack += () => PlayOverRemoveOwnerDestroyAction(component); // 播放结束时移除宿主销毁Action
            RecycleAudioPlay(audioSource, clip, autoReleaseClip, callBack);
        }

        /// <summary>
        /// 宿主销毁时，提前回收
        /// </summary>
        /// <param name="go"></param>
        /// <param name="audioSource"></param>
        private void OnOwnerDestroy(GameObject go, AudioSource audioSource)
        {
            audioSource.transform.SetParent(m_AudioPlayRoot);
        }

        /// <summary>
        /// 播放结束时移除宿主销毁Action
        /// </summary>
        /// <param name="owner"></param>
        private void PlayOverRemoveOwnerDestroyAction(UnityEngine.Component owner)
        {
            if (owner != null)
            {
                owner.RemoveOnDestroy<AudioSource>(OnOwnerDestroy);
            }
        }

        public void PlayOneShot(AudioClip clip, Vector3 position, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
        {
            // 初始化音乐播放器
            var audioSource = GetAudioPlay(is3d);
            audioSource.transform.position = position;
            // 播放一次音效
            audioSource.PlayOneShot(clip, volumeScale);
            // 播放器回收以及回调函数
            RecycleAudioPlay(audioSource, clip, autoReleaseClip, callBack);
        }

        #endregion
    }
}