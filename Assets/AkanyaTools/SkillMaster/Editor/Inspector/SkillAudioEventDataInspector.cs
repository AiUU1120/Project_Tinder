/*
 * @Author: AiUU
 * @Description: SkillMaster 音效数据监视器绘制
 * @AkanyaTech.SkillMaster
 */

using System;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.AudioTrack;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed class SkillAudioEventDataInspector : SkillEventDataInspectorBase<AudioTrackItem, AudioTrack>
    {
        private FloatField m_AudioVolumeField;

        private float m_OldAudioVolumeValue;

        public override void OnDraw()
        {
            // 音效资源
            var audioClipAssetField = new ObjectField("Audio Clip")
            {
                objectType = typeof(AudioClip),
                value = m_TrackItem.audioEvent.audioClip
            };
            audioClipAssetField.RegisterValueChangedCallback(OnAudioClipAssetFieldValueChanged);
            m_Root.Add(audioClipAssetField);

            // 音量输入
            m_AudioVolumeField = new FloatField("Volume")
            {
                value = m_TrackItem.audioEvent.volume
            };
            m_AudioVolumeField.RegisterCallback<FocusInEvent>(OnAudioVolumeFieldFocusIn);
            m_AudioVolumeField.RegisterCallback<FocusOutEvent>(OnAudioVolumeFieldFocusOut);
            m_Root.Add(m_AudioVolumeField);
        }

        #region Callback

        private void OnAudioClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            var clip = evt.newValue as AudioClip;
            m_TrackItem.audioEvent.audioClip = clip;
            SkillMasterEditorWindow.instance.SaveConfig();
            m_TrackItem.ForceRefreshView();
        }

        private void OnAudioVolumeFieldFocusIn(FocusInEvent evt)
        {
            m_OldAudioVolumeValue = m_AudioVolumeField.value;
        }

        private void OnAudioVolumeFieldFocusOut(FocusOutEvent evt)
        {
            if (Math.Abs(m_OldAudioVolumeValue - m_AudioVolumeField.value) < 0.00001f)
            {
                return;
            }
            m_TrackItem.audioEvent.volume = m_AudioVolumeField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        #endregion
    }
}