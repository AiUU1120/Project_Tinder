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
    public sealed partial class SkillMasterInspector
    {
        private List<string> m_CustomEventTypeSelections;

        /// <summary>
        /// 绘制监视器显示内容
        /// </summary>
        /// <param name="item"></param>
        private void DrawCustomEventTrackItem(CustomEventTrackItem item)
        {
            // 事件类型
            m_CustomEventTypeSelections = new List<string>(Enum.GetNames(typeof(SkillEventType)));
            var eventTypeDropdownField = new DropdownField("Event Type", m_CustomEventTypeSelections, (int) item.customEventEvent.eventType);
            eventTypeDropdownField.RegisterValueChangedCallback(OnEventTypeDropdownFieldValueChanged);
            m_Root.Add(eventTypeDropdownField);

            if (item.customEventEvent.eventType == SkillEventType.Custom)
            {
                // 事件名称
                var nameField = new TextField("Event Name")
                {
                    value = item.customEventEvent.customEventName
                };
                nameField.RegisterValueChangedCallback(OnCustomEventNameFieldValueChanged);
                m_Root.Add(nameField);
            }

            // 事件参数
            var intParamField = new IntegerField("Int Param")
            {
                value = item.customEventEvent.intParam
            };
            intParamField.RegisterValueChangedCallback(OnCustomEventIntParamFieldValueChanged);
            m_Root.Add(intParamField);

            var floatParamField = new FloatField("Float Param")
            {
                value = item.customEventEvent.floatParam
            };
            floatParamField.RegisterValueChangedCallback(OnCustomEventFloatParamFieldValueChanged);
            m_Root.Add(floatParamField);

            var stringParamField = new TextField("String Param")
            {
                value = item.customEventEvent.stringParam
            };
            stringParamField.RegisterValueChangedCallback(OnCustomEventStringParamFieldValueChanged);
            m_Root.Add(stringParamField);

            var objParamField = new ObjectField("Object Param")
            {
                objectType = typeof(UnityEngine.Object),
                allowSceneObjects = false,
                value = item.customEventEvent.objParam
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
            var curItem = (CustomEventTrackItem) curTrackItem;
            curItem.customEventEvent.eventType = (SkillEventType) m_CustomEventTypeSelections.IndexOf(evt.newValue);
            if (curItem.customEventEvent.eventType != SkillEventType.Custom)
            {
                curItem.customEventEvent.customEventName = "";
            }
            SkillMasterEditorWindow.instance.SaveConfig();
            Refresh();
        }

        private void OnCustomEventNameFieldValueChanged(ChangeEvent<string> evt)
        {
            ((CustomEventTrackItem) curTrackItem).customEventEvent.customEventName = evt.newValue;
        }

        private void OnCustomEventObjectParamFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            ((CustomEventTrackItem) curTrackItem).customEventEvent.objParam = evt.newValue;
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        private void OnCustomEventStringParamFieldValueChanged(ChangeEvent<string> evt)
        {
            ((CustomEventTrackItem) curTrackItem).customEventEvent.stringParam = evt.newValue;
        }

        private void OnCustomEventFloatParamFieldValueChanged(ChangeEvent<float> evt)
        {
            ((CustomEventTrackItem) curTrackItem).customEventEvent.floatParam = evt.newValue;
        }

        private void OnCustomEventIntParamFieldValueChanged(ChangeEvent<int> evt)
        {
            ((CustomEventTrackItem) curTrackItem).customEventEvent.intParam = evt.newValue;
        }

        private void OnCustomEventDeleteBtnClick(EventBase obj)
        {
            s_CurTrack.DeleteTrackItem(m_TrackItemFrameIndex);
            Selection.activeObject = null;
        }

        #endregion
    }
}