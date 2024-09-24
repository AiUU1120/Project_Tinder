/*
* @Author: AiUU
* @Description: SkillMaster 编辑器播放控制台
* @AkanyaTech.SkillMaster
*/

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

        private void InitConsole()
        {
            m_PreFrameBtn = rootVisualElement.NiceQ<Button>("PreFrameBtn");
            m_PreFrameBtn.clicked += OnPreFrameBtnClick;

            m_PlayBtn = rootVisualElement.NiceQ<Button>("PlayBtn");
            m_PreFrameBtn.clicked += OnPlayBtnClick;

            m_NextFrameBtn = rootVisualElement.NiceQ<Button>("NextFrameBtn");
            m_NextFrameBtn.clicked += OnNextFrameBtnClick;

            m_CurFrameIntField = rootVisualElement.NiceQ<IntegerField>("CurFrameIntField");
            m_CurFrameIntField.RegisterValueChangedCallback(OnCurFrameIntFieldValueChanged);

            m_FrameCountIntField = rootVisualElement.NiceQ<IntegerField>("FrameCountIntField");
            m_FrameCountIntField.RegisterValueChangedCallback(OnFrameCountIntFieldValueChanged);
        }

        private void OnPreFrameBtnClick()
        {
            curSelectedFrameIndex--;
        }

        private void OnPlayBtnClick()
        {
        }

        private void OnNextFrameBtnClick()
        {
            curSelectedFrameIndex++;
        }

        private void OnCurFrameIntFieldValueChanged(ChangeEvent<int> evt)
        {
            curSelectedFrameIndex = evt.newValue;
        }

        private void OnFrameCountIntFieldValueChanged(ChangeEvent<int> evt)
        {
            curFrameCount = evt.newValue;
        }
    }
}