/*
 * @Author: AiUU
 * @Description: SkillMaster 特效轨道片段
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Editor.EditorWindow;
using AkanyaTools.SkillMaster.Editor.Inspector;
using AkanyaTools.SkillMaster.Editor.Track.Style;
using AkanyaTools.SkillMaster.Editor.Track.Style.Common;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.EffectTrack
{
    public sealed class EffectTrackItem : TrackItemBase<EffectTrack>
    {
        public SkillEffectFrameEvent effectEvent { get; private set; }

        private MultiLineTrackStyle.SubTrackStyle m_SubTrackStyle;

        private EffectTrackItemStyle m_TrackItemStyle;

        private GameObject m_EffectPreviewObj;

        private bool m_IsMouseDrag;

        private float m_StartDragPosX;

        private int m_StartDragFrame;

        public void Init(EffectTrack track, float frameUnitWidth, SkillEffectFrameEvent e, MultiLineTrackStyle.SubTrackStyle subTrackStyle)
        {
            this.track = track;
            frameIndex = e.frameIndex;
            effectEvent = e;
            m_SubTrackStyle = subTrackStyle;

            SetTrackName(e.trackName);

            m_TrackItemStyle = new EffectTrackItemStyle();
            itemStyle = m_TrackItemStyle;
            m_TrackItemStyle.Init(frameUnitWidth, effectEvent, m_SubTrackStyle);

            normalColor = new Color(this.track.themeColor.r, this.track.themeColor.g, this.track.themeColor.b, 0.7f);
            selectedColor = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);

            OnUnSelect();

            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);
            m_TrackItemStyle.mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);

            m_SubTrackStyle.trackRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            m_SubTrackStyle.trackRoot.RegisterCallback<DragExitedEvent>(OnDragExited);

            RefreshView(frameUnitWidth);
        }

        public void TickView(int frameIndex)
        {
            if (effectEvent.effectPrefab == null || SkillMasterEditorWindow.instance.curPreviewCharacterObj == null)
            {
                return;
            }
            var durationFrame = effectEvent.durationFrame;
            if (effectEvent.frameIndex <= frameIndex && frameIndex <= effectEvent.frameIndex + durationFrame)
            {
                if (m_EffectPreviewObj != null && m_EffectPreviewObj.name != effectEvent.effectPrefab.name)
                {
                    Object.DestroyImmediate(m_EffectPreviewObj);
                    m_EffectPreviewObj = null;
                }
                if (m_EffectPreviewObj == null)
                {
                    var characterTrans = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform;

                    var pos = characterTrans.TransformPoint(effectEvent.positionOffset);
                    var rot = characterTrans.eulerAngles + effectEvent.rotation;

                    m_EffectPreviewObj = Object.Instantiate(effectEvent.effectPrefab, pos, Quaternion.Euler(rot), EffectTrack.effectRoot);
                    m_EffectPreviewObj.transform.localScale = effectEvent.scale;
                    m_EffectPreviewObj.name = effectEvent.effectPrefab.name;
                }
                var particles = m_EffectPreviewObj.GetComponentsInChildren<ParticleSystem>();
                foreach (var particle in particles)
                {
                    var simulateFrame = frameIndex - effectEvent.frameIndex;
                    var time = (float) simulateFrame / SkillMasterEditorWindow.instance.skillConfig.frameRate;
                    particle.Simulate(time, true, true);
                }
            }
            else
            {
                ClearEffectPreviewObj();
            }
        }

        public override void RefreshView(float frameUnitWidth)
        {
            base.RefreshView(frameUnitWidth);
            m_TrackItemStyle.RefreshView(frameUnitWidth, effectEvent);
            ClearEffectPreviewObj();
            TickView(SkillMasterEditorWindow.instance.curSelectedFrameIndex);
        }

        private void SetTrackName(string name)
        {
            m_SubTrackStyle.SetTrackName(name);
        }

        public void Destroy()
        {
            ClearEffectPreviewObj();
            m_SubTrackStyle.DeleteView();
        }

        /// <summary>
        /// 应用拖拽
        /// </summary>
        private void ApplyDrag()
        {
            if (m_StartDragFrame == frameIndex)
            {
                return;
            }
            SkillMasterInspector.instance.SetTrackItemFrameIndex(frameIndex);
        }

        /// <summary>
        /// 将特效 GO 在场景中的位置应用到特效数据中
        /// </summary>
        public void ApplyEffectModelTransform()
        {
            if (m_EffectPreviewObj == null)
            {
                return;
            }
            var characterTrans = SkillMasterEditorWindow.instance.curPreviewCharacterObj.transform;
            // var characterRootPos = SkillMasterEditorWindow.instance.GetPosFromRootMotion(effectEvent.frameIndex, true);
            // var oldCharacterPos = characterTrans.position;

            // characterTrans.position = characterRootPos;
            effectEvent.positionOffset = characterTrans.InverseTransformPoint(m_EffectPreviewObj.transform.position);
            effectEvent.rotation = m_EffectPreviewObj.transform.eulerAngles - characterTrans.eulerAngles;
            effectEvent.scale = m_EffectPreviewObj.transform.localScale;
            // characterTrans.position = oldCharacterPos;
        }

        public void ClearEffectPreviewObj()
        {
            if (m_EffectPreviewObj == null)
            {
                return;
            }
            Object.DestroyImmediate(m_EffectPreviewObj);
            m_EffectPreviewObj = null;
        }

        #region Callback

        private void OnMouseDown(MouseDownEvent evt)
        {
            m_IsMouseDrag = true;
            m_StartDragPosX = evt.mousePosition.x;
            m_StartDragFrame = frameIndex;
            Select();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (m_IsMouseDrag)
            {
                ApplyDrag();
            }
            m_IsMouseDrag = false;
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            if (m_IsMouseDrag)
            {
                ApplyDrag();
            }
            m_IsMouseDrag = false;
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!m_IsMouseDrag)
            {
                return;
            }
            var offsetPos = evt.mousePosition.x - m_StartDragPosX;
            var offsetFrame = Mathf.RoundToInt(offsetPos / frameUnitWidth);
            var targetFrame = m_StartDragFrame + offsetFrame;
            if (targetFrame < 0 || offsetFrame == 0)
            {
                return;
            }
            frameIndex = targetFrame;
            effectEvent.frameIndex = frameIndex;
            RefreshView(frameUnitWidth);
        }

        /// <summary>
        /// 监听用户音效拖拽
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            var objs = DragAndDrop.objectReferences;
            var prefab = objs[0] as GameObject;
            if (prefab != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        /// <summary>
        /// 监听用户松手 放入音效片段
        /// </summary>
        /// <param name="evt"></param>
        private void OnDragExited(DragExitedEvent evt)
        {
            var objs = DragAndDrop.objectReferences;
            var prefab = objs[0] as GameObject;
            if (prefab == null)
            {
                return;
            }
            // 放置特效资源
            var selectFrameIndex = SkillMasterEditorWindow.instance.GetFrameIndexByPos(evt.localMousePosition.x);
            if (selectFrameIndex < 0)
            {
                return;
            }
            effectEvent.effectPrefab = prefab;
            effectEvent.positionOffset = Vector3.zero;
            effectEvent.rotation = Vector3.zero;
            effectEvent.scale = Vector3.one;
            effectEvent.autoDestroy = true;
            effectEvent.frameIndex = selectFrameIndex;

            var particles = prefab.GetComponentsInChildren<ParticleSystem>();
            var maxDurationTime = -1f;
            foreach (var p in particles)
            {
                if (p.main.duration > maxDurationTime)
                {
                    maxDurationTime = p.main.duration;
                }
            }
            effectEvent.durationFrame = (int) (maxDurationTime * SkillMasterEditorWindow.instance.skillConfig.frameRate);

            frameIndex = selectFrameIndex;
            ForceRefreshView();
            SkillMasterEditorWindow.instance.SaveConfig();
        }

        #endregion
    }
}