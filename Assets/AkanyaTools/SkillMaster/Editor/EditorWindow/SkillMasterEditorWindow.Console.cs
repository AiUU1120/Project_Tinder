/*
* @Author: AiUU
* @Description: SkillMaster 编辑器播放控制台
* @AkanyaTech.SkillMaster
*/

using System;
using FrameTools.Extension;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public partial class SkillMasterEditorWindow
    {
        private Button m_PreFrameBtn;

        private Button m_PlayBtn;

        private Button m_NextFrameBtn;

        private IntegerField m_CurFrameIntField;

        private IntegerField m_FrameCountIntField;

        private bool m_IsPlaying;

        public bool isPlaying
        {
            get => m_IsPlaying;
            private set
            {
                m_IsPlaying = value;
                if (m_IsPlaying)
                {
                    m_StartTime = DateTime.Now;
                    m_StartFrameIndex = curSelectedFrameIndex;
                    foreach (var track in m_TrackList)
                    {
                        track.OnPlay(m_StartFrameIndex);
                    }
                }
                else
                {
                    foreach (var track in m_TrackList)
                    {
                        track.OnStop();
                    }
                }
            }
        }

        private DateTime m_StartTime;

        private int m_StartFrameIndex;

        private void Update()
        {
            if (!isPlaying)
            {
                return;
            }
            var time = DateTime.Now.Subtract(m_StartTime).TotalSeconds;
            var frameRate = skillConfig ? skillConfig.frameRate : SkillMasterEditorConfig.default_frame_rate;
            curSelectedFrameIndex = (int) (time * frameRate + m_StartFrameIndex);
            if (curSelectedFrameIndex == curFrameCount)
            {
                isPlaying = false;
            }
        }

        private void InitConsole()
        {
            m_PreFrameBtn = rootVisualElement.NiceQ<Button>("PreFrameBtn");
            m_PreFrameBtn.clicked += OnPreFrameBtnClick;

            m_PlayBtn = rootVisualElement.NiceQ<Button>("PlayBtn");
            m_PlayBtn.clicked += OnPlayBtnClick;

            m_NextFrameBtn = rootVisualElement.NiceQ<Button>("NextFrameBtn");
            m_NextFrameBtn.clicked += OnNextFrameBtnClick;

            m_CurFrameIntField = rootVisualElement.NiceQ<IntegerField>("CurFrameIntField");
            m_CurFrameIntField.RegisterCallback<FocusOutEvent>(OnCurFrameIntFieldFocusOut);

            m_FrameCountIntField = rootVisualElement.NiceQ<IntegerField>("FrameCountIntField");
            m_FrameCountIntField.RegisterCallback<FocusOutEvent>(OnFrameCountIntFieldFocusOut);
        }

        /// <summary>
        /// 驱动技能表现
        /// </summary>
        private void TickSkill()
        {
            if (skillConfig == null || curPreviewCharacterObj == null)
            {
                return;
            }
            foreach (var track in m_TrackList)
            {
                track.TickView(curSelectedFrameIndex);
            }
        }

        #region Callback

        private void OnPreFrameBtnClick()
        {
            isPlaying = false;
            curSelectedFrameIndex--;
        }

        private void OnPlayBtnClick()
        {
            isPlaying = !isPlaying;
        }

        private void OnNextFrameBtnClick()
        {
            isPlaying = false;
            curSelectedFrameIndex++;
        }

        private void OnCurFrameIntFieldFocusOut(FocusOutEvent evt)
        {
            if (curSelectedFrameIndex == m_CurFrameIntField.value)
            {
                return;
            }
            curSelectedFrameIndex = m_CurFrameIntField.value;
        }

        private void OnFrameCountIntFieldFocusOut(FocusOutEvent evt)
        {
            if (curFrameCount == m_FrameCountIntField.value)
            {
                return;
            }
            curFrameCount = m_FrameCountIntField.value;
        }

        #endregion
    }
}