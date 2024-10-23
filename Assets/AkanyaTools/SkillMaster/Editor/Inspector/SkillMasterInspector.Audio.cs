using System;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.AudioTrack;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed partial class SkillMasterInspector
    {
        private FloatField m_AudioVolumeField;

        private float m_OldAudioVolumeValue;

        /// <summary>
        /// 绘制监视器显示内容
        /// </summary>
        /// <param name="item"></param>
        private void DrawAudioTrackItem(AudioTrackItem item)
        {
            // 音效资源
            var audioClipAssetField = new ObjectField("Audio Clip")
            {
                objectType = typeof(AudioClip),
                value = item.audioEvent.audioClip
            };
            audioClipAssetField.RegisterValueChangedCallback(OnAudioClipAssetFieldValueChanged);
            m_Root.Add(audioClipAssetField);

            // 音量输入
            m_AudioVolumeField = new FloatField("Volume")
            {
                value = item.audioEvent.volume
            };
            m_AudioVolumeField.RegisterCallback<FocusInEvent>(OnAudioVolumeFieldFocusIn);
            m_AudioVolumeField.RegisterCallback<FocusOutEvent>(OnAudioVolumeFieldFocusOut);
            m_Root.Add(m_AudioVolumeField);
        }

        #region Callback

        private void OnAudioClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            var clip = evt.newValue as AudioClip;
            ((AudioTrackItem) s_CurTrackItem).audioEvent.audioClip = clip;
            SkillMasterEditorWindow.instance.SaveConfig();
            s_CurTrackItem.ForceRefreshView();
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
            ((AudioTrackItem) s_CurTrackItem).audioEvent.volume = m_AudioVolumeField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        #endregion
    }
}