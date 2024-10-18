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
        private FloatField m_VolumeField;

        private float m_OldVolumeValue;

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
            m_VolumeField = new FloatField("Volume")
            {
                value = item.audioEvent.volume
            };
            m_VolumeField.RegisterCallback<FocusInEvent>(OnVolumeFieldFocusIn);
            m_VolumeField.RegisterCallback<FocusOutEvent>(OnVolumeFieldFocusOut);
            m_Root.Add(m_VolumeField);
        }

        #region Callback

        private void OnAudioClipAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            var clip = evt.newValue as AudioClip;
            // Debug.Assert(clip != null, nameof(clip) + " != null");
            ((AudioTrackItem) s_CurTrackItem).audioEvent.audioClip = clip;
            SkillMasterEditorWindow.instance.SaveConfig();
            s_CurTrackItem.RefreshView();
        }

        private void OnVolumeFieldFocusIn(FocusInEvent evt)
        {
            m_OldVolumeValue = m_VolumeField.value;
        }

        private void OnVolumeFieldFocusOut(FocusOutEvent evt)
        {
            if (Math.Abs(m_OldVolumeValue - m_VolumeField.value) < 0.00001f)
            {
                return;
            }
            ((AudioTrackItem) s_CurTrackItem).audioEvent.volume = m_VolumeField.value;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        #endregion
    }
}