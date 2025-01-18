/*
* @Author: AiUU
* @Description: 音频管理器
* @AkanyaTech.FrameTools
*/

using System;
using FrameTools.AudioSystem;
using JKFrame;
using UnityEngine;

namespace AkanyaTools.AudioSystem
{
    public static class AudioManager
    {
        private static AudioModule s_AudioModule;

        public static void Init()
        {
            s_AudioModule = JKFrameRoot.RootTransform.GetComponentInChildren<AudioModule>();
            s_AudioModule.Init();
        }

        /// <summary>
        /// 全局音量
        /// </summary>
        public static float globalVolume
        {
            get => s_AudioModule.globalVolume;
            set => s_AudioModule.globalVolume = value;
        }

        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public static float bgVolume
        {
            get => s_AudioModule.bgVolume;
            set => s_AudioModule.bgVolume = value;
        }

        /// <summary>
        /// 效果音音量
        /// </summary>
        public static float effectVolume
        {
            get => s_AudioModule.effectVolume;
            set => s_AudioModule.effectVolume = value;
        }

        /// <summary>
        /// 静音
        /// </summary>
        public static bool isMute
        {
            get => s_AudioModule.isMute;
            set => s_AudioModule.isMute = value;
        }

        /// <summary>
        /// 循环
        /// </summary>
        public static bool isLoop
        {
            get => s_AudioModule.isLoop;
            set => s_AudioModule.isLoop = value;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public static bool isPause
        {
            get => s_AudioModule.isPause;
            set => s_AudioModule.isPause = value;
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clip">音乐片段</param>
        /// <param name="loop">是否循环</param>
        /// <param name="volume">音量 -1代表不设置 采用当前音量</param>
        /// <param name="fadeOutTime">渐出音量花费的时间</param>
        /// <param name="fadeInTime">渐入音量花费的时间</param>
        public static void PlayBgAudio(AudioClip clip, bool loop = true, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
            => s_AudioModule.PlayBgAudio(clip, loop, volume, fadeOutTime, fadeInTime);

        /// <summary>
        /// 使用音效数组播放背景音乐 自动循环
        /// </summary>
        /// <param name="clips">音频片段数组</param>
        /// <param name="volume">音量 -1代表不设置 采用当前音量</param>
        /// <param name="fadeOutTime">渐出音量花费的时间</param>
        /// <param name="fadeInTime">渐入音量花费的时间</param>
        public static void PlayBgAudioWithClips(AudioClip[] clips, float volume = -1, float fadeOutTime = 0, float fadeInTime = 0)
            => s_AudioModule.PlayBgAudioWithClips(clips, volume, fadeOutTime, fadeInTime);

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public static void StopBgAudio() => s_AudioModule.StopBgAudio();

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public static void PauseBgAudio() => s_AudioModule.PauseBgAudio();

        /// <summary>
        /// 继续播放背景音乐
        /// </summary>
        public static void UnPauseBgAudio() => s_AudioModule.UnPauseBgAudio();

        /// <summary>
        /// 播放一次特效音乐 并且绑定在某个游戏物体身上
        /// 游戏物体销毁时 会瞬间解除绑定 回收音效播放器
        /// </summary>
        /// <param name="clip">音效片段</param>
        /// <param name="autoReleaseClip">播放完毕时候自动回收audioClip</param>
        /// <param name="component">挂载组件</param>
        /// <param name="volumeScale">音量 0-1</param>
        /// <param name="is3d">是否3D</param>
        /// <param name="callBack">回调函数-在音乐播放完成后执行</param>
        public static void PlayOneShot(AudioClip clip, UnityEngine.Component component = null, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
            => s_AudioModule.PlayOneShot(clip, component, autoReleaseClip, volumeScale, is3d, callBack);

        /// <summary>
        /// 播放一次特效音乐
        /// </summary>
        /// <param name="clip">音效片段</param>
        /// <param name="position">播放的位置</param>
        /// <param name="autoReleaseClip">播放完毕时候自动回收audioClip</param>
        /// <param name="volumeScale">音量 0-1</param>
        /// <param name="is3d">是否3D</param>
        /// <param name="callBack">回调函数-在音乐播放完成后执行</param>
        public static void PlayOneShot(AudioClip clip, Vector3 position, bool autoReleaseClip = false, float volumeScale = 1, bool is3d = true, Action callBack = null)
            => s_AudioModule.PlayOneShot(clip, position, autoReleaseClip, volumeScale, is3d, callBack);
    }
}