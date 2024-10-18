/*
* @Author: AiUU
* @Description: 编辑器音频工具类
* @AkanyaTech.EditorHelper
*/

using System.Reflection;
using UnityEngine;

public static class EditorAudioHelper
{
    private static readonly MethodInfo s_PlayClipMethodInfo;

    private static readonly MethodInfo s_StopAllClipMethodInfo;

    static EditorAudioHelper()
    {
        var asm = typeof(UnityEditor.AudioImporter).Assembly;
        var utilClass = asm.GetType("UnityEditor.AudioUtil");

        s_PlayClipMethodInfo = utilClass.GetMethod("PlayPreviewClip", BindingFlags.Static | BindingFlags.Public, null,
            new[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
        s_StopAllClipMethodInfo = utilClass.GetMethod("StopAllPreviewClips", BindingFlags.Static | BindingFlags.Public);
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="startProgress">0 ~ 1</param>
    public static void PlayAudioClip(AudioClip clip, float startProgress)
    {
        s_PlayClipMethodInfo.Invoke(clip, new object[] { clip, (int) (startProgress * clip.frequency * clip.length), false });
    }

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public static void StopAllAudioClip()
    {
        s_StopAllClipMethodInfo.Invoke(null, null);
    }
}