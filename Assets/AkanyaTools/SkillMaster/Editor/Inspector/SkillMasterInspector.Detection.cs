using System;
using System.Collections.Generic;
using AkanyaTools.SkillMaster.Editor.Track.DetectionTrack;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Inspector
{
    public sealed partial class SkillMasterInspector
    {
        private List<string> m_DetectionSelections;

        private void DrawDetectionTrackItem(DetectionTrackItem detectionItem)
        {
            m_DetectionSelections = new List<string>(Enum.GetNames(typeof(DetectionType)));
            var detectionDropdownField = new DropdownField("Detection Type", m_DetectionSelections, (int) detectionItem.detectionEvent.detectionType);
            detectionDropdownField.RegisterValueChangedCallback(OnDetectionDropdownFieldValueChanged);
            m_Root.Add(detectionDropdownField);
        }

        #region Callback

        private void OnDetectionDropdownFieldValueChanged(ChangeEvent<string> evt)
        {
            var curItem = (DetectionTrackItem) s_CurTrackItem;
            curItem.detectionEvent.detectionType = (DetectionType) m_DetectionSelections.IndexOf(evt.newValue);
        }

        #endregion
    }
}