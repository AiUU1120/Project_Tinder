/*
* @Author: AiUU
* @Description: SkillMaster 技能音效帧事件
* @AkanyaTech.SkillMaster
*/

using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data.Event
{
    public sealed class SkillAudioFrameEvent : SkillFrameEventBase
    {
#if UNITY_EDITOR
        public string trackName = "Audio Track";
#endif

        public int frameIndex = -1;

        public AudioClip audioClip;

        public int playCount;

        public float volume = 1;
    }
}