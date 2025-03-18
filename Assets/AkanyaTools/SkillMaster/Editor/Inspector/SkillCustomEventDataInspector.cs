/*
 * @Author: AiUU
 * @Description: SkillMaster 自定义事件数据监视器绘制
 * @AkanyaTech.SkillMaster
 */

using System;
using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Track.CustomEventTrack;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed class SkillCustomEventDataInspector : SkillEventDataInspectorBase<CustomEventTrackItem, CustomEventTrack>
    {
        private List<string> m_CustomEventTypeSelections;

        public override void OnDraw()
        {
            // 事件类型
            m_CustomEventTypeSelections = new List<string>(Enum.GetNames(typeof(SkillEventType)));
            var eventTypeDropdownField = new DropdownField("Event Type", m_CustomEventTypeSelections, (int) m_TrackItem.customEventEvent.eventType);
            eventTypeDropdownField.RegisterValueChangedCallback(OnEventTypeDropdownFieldValueChanged);
            m_Root.Add(eventTypeDropdownField);

            if (m_TrackItem.customEventEvent.eventType == SkillEventType.Custom)
            {
                // 事件名称
                var nameField = new TextField("Event Name")
                {
                    value = m_TrackItem.customEventEvent.customEventName
                };
                nameField.RegisterValueChangedCallback(OnCustomEventNameFieldValueChanged);
                m_Root.Add(nameField);
            }

            // 事件参数
            var intParamField = new IntegerField("Int Param")
            {
                value = m_TrackItem.customEventEvent.intParam
            };
            intParamField.RegisterValueChangedCallback(OnCustomEventIntParamFieldValueChanged);
            m_Root.Add(intParamField);

            var floatParamField = new FloatField("Float Param")
            {
                value = m_TrackItem.customEventEvent.floatParam
            };
            floatParamField.RegisterValueChangedCallback(OnCustomEventFloatParamFieldValueChanged);
            m_Root.Add(floatParamField);

            var stringParamField = new TextField("String Param")
            {
                value = m_TrackItem.customEventEvent.stringParam
            };
            stringParamField.RegisterValueChangedCallback(OnCustomEventStringParamFieldValueChanged);
            m_Root.Add(stringParamField);

            var objParamField = new ObjectField("Object Param")
            {
                objectType = typeof(UnityEngine.Object),
                allowSceneObjects = false,
                value = m_TrackItem.customEventEvent.objParam
            };
            objParamField.RegisterValueChangedCallback(OnCustomEventObjectParamFieldValueChanged);
            m_Root.Add(objParamField);

            // 删除
            var deleteBtn = new Button
            {
                text = "Delete",
                style =
                {
                    backgroundColor = new Color(1f, 0f, 0f, 0.5f)
                },
                clickable = new Clickable(OnCustomEventDeleteBtnClick),
            };
            m_Root.Add(deleteBtn);
        }

        #region Callback

        private void OnEventTypeDropdownFieldValueChanged(ChangeEvent<string> evt)
        {
            var curItem = m_TrackItem;
            curItem.customEventEvent.eventType = (SkillEventType) m_CustomEventTypeSelections.IndexOf(evt.newValue);
            if (curItem.customEventEvent.eventType != SkillEventType.Custom)
            {
                curItem.customEventEvent.customEventName = "";
            }
            SkillMasterEditorWindow.instance.SaveConfig();
            SkillMasterInspector.instance.Refresh();
        }

        private void OnCustomEventNameFieldValueChanged(ChangeEvent<string> evt)
        {
            m_TrackItem.customEventEvent.customEventName = evt.newValue;
        }

        private void OnCustomEventObjectParamFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            m_TrackItem.customEventEvent.objParam = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnCustomEventStringParamFieldValueChanged(ChangeEvent<string> evt)
        {
            m_TrackItem.customEventEvent.stringParam = evt.newValue;
        }

        private void OnCustomEventFloatParamFieldValueChanged(ChangeEvent<float> evt)
        {
            m_TrackItem.customEventEvent.floatParam = evt.newValue;
        }

        private void OnCustomEventIntParamFieldValueChanged(ChangeEvent<int> evt)
        {
            m_TrackItem.customEventEvent.intParam = evt.newValue;
        }

        private void OnCustomEventDeleteBtnClick(EventBase obj)
        {
            m_Track.DeleteTrackItem(m_ItemFrameIndex);
            Selection.activeObject = null;
        }

        #endregion
    }
}