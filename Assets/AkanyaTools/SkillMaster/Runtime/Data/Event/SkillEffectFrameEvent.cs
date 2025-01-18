/*
* @Author: AiUU
* @Description: SkillMaster 技能特效帧事件
* @AkanyaTech.SkillMaster
*/

using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Data.Event
{
    public sealed class SkillEffectFrameEvent : SkillFrameEventBase
    {
#if UNITY_EDITOR
        public string trackName = "Effect Track";
#endif

        public int frameIndex = -1;

        public GameObject effectPrefab;

        public Vector3 positionOffset;

        public Vector3 rotation;

        public Vector3 scale;

        public int durationFrame;

        public bool autoDestroy;
    }
}