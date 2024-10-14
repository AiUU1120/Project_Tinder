/*
* @Author: AiUU
* @Description: SkillMaster 多行轨道样式
* @AkanyaTech.SkillMaster
*/

using System;
using System.Collections.Generic;
using FrameTools.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AkanyaTools.SkillMaster.Editor.Track.Style
{
    public sealed class MultiLineTrackStyle : TrackStyleBase
    {
        private const string menu_asset_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/MultiLineTrackMenu.uxml";

        private const string content_asset_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/MultiLineTrackContent.uxml";

        /// <summary>
        /// 头部高度
        /// </summary>
        private const float head_height = 35;

        /// <summary>
        /// 子轨道高度
        /// </summary>
        private const float item_height = 32;

        private Func<bool> m_OnAddSubTrack;

        private Func<int, bool> m_OnDeleteSubTrack;

        private VisualElement m_SubTrackMenuParent;

        private readonly List<SubTrack> m_SubTracks = new();

        private int m_SelectedTrackIndex = -1;

        private bool m_IsDragging;

        public void Init(VisualElement menuParent, VisualElement contentParent, string title, Func<bool> onAddSubTrack, Func<int, bool> onDeleteSubTrack)
        {
            this.menuParent = menuParent;
            this.contentParent = contentParent;
            m_OnAddSubTrack = onAddSubTrack;
            m_OnDeleteSubTrack = onDeleteSubTrack;

            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menu_asset_path).Instantiate().Query().ToList()[1];
            menuParent.Add(menuRoot);

            m_SubTrackMenuParent = menuRoot.NiceQ<VisualElement>("TrackMenuList");
            m_SubTrackMenuParent.RegisterCallback<MouseMoveEvent>(OnSubTrackMenuParentMouseMove);
            m_SubTrackMenuParent.RegisterCallback<MouseDownEvent>(OnSubTrackMenuParentMouseDown);
            m_SubTrackMenuParent.RegisterCallback<MouseUpEvent>(OnSubTrackMenuParentMouseUp);
            m_SubTrackMenuParent.RegisterCallback<MouseOutEvent>(OnSubTrackMenuParentMouseOut);

            contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(content_asset_path).Instantiate().Query().ToList()[1];
            contentParent.Add(contentRoot);

            titleLabel = menuRoot.NiceQ<Label>("Title");
            titleLabel.text = title;

            var addBtn = menuRoot.NiceQ<Button>("AddBtn");
            addBtn.clicked += OnAddBtnClick;
        }

        /// <summary>
        /// 删除子轨道
        /// </summary>
        /// <param name="subTrack">待删除子轨道</param>
        private void DeleteSubTrack(SubTrack subTrack)
        {
            if (m_OnDeleteSubTrack == null)
            {
                return;
            }
            var index = subTrack.GetIndex();
            if (!m_OnDeleteSubTrack(index))
            {
                return;
            }
            subTrack.Destroy();
            m_SubTracks.RemoveAt(index);
            UpdateAllSubTrackIndex(index);
            UpdateSize();
        }

        /// <summary>
        /// 更新大小
        /// </summary>
        private void UpdateSize()
        {
            var height = head_height + m_SubTracks.Count * item_height;
            menuRoot.style.height = height;
            contentRoot.style.height = height;
            m_SubTrackMenuParent.style.height = m_SubTracks.Count * item_height;
        }

        /// <summary>
        /// 更新所有子轨道的索引
        /// </summary>
        private void UpdateAllSubTrackIndex(int startIndex = 0)
        {
            for (var i = startIndex; i < m_SubTracks.Count; i++)
            {
                m_SubTracks[i].SetIndex(i);
            }
        }

        private int GetSubTrackIndexByPos(float mousePosY)
        {
            var index = Mathf.RoundToInt(mousePosY / item_height);
            index = Mathf.Clamp(index, 0, m_SubTracks.Count - 1);
            return index;
        }

        private void SwapSubTrack(int index1, int index2)
        {
            if (index1 == index2)
            {
                return;
            }
            var subTrack1 = m_SubTracks[index1];
            var subTrack2 = m_SubTracks[index2];
            m_SubTracks[index1] = subTrack2;
            m_SubTracks[index2] = subTrack1;
            UpdateAllSubTrackIndex();
        }

        #region Callback

        /// <summary>
        /// 添加子轨道
        /// </summary>
        private void OnAddBtnClick()
        {
            if (m_OnAddSubTrack == null)
            {
                return;
            }
            var subTrack = new SubTrack();
            subTrack.Init(m_SubTracks.Count, m_SubTrackMenuParent, contentRoot, DeleteSubTrack);
            m_SubTracks.Add(subTrack);
            UpdateSize();
        }

        private void OnSubTrackMenuParentMouseMove(MouseMoveEvent evt)
        {
            if (m_SelectedTrackIndex == -1 || !m_IsDragging)
            {
                return;
            }
            var posY = evt.localMousePosition.y - item_height * 0.5f;
            var targetIndex = GetSubTrackIndexByPos(posY);
            if (targetIndex == m_SelectedTrackIndex)
            {
                return;
            }
            SwapSubTrack(m_SelectedTrackIndex, targetIndex);
            m_SelectedTrackIndex = targetIndex;
        }

        private void OnSubTrackMenuParentMouseDown(MouseDownEvent evt)
        {
            if (m_SelectedTrackIndex != -1)
            {
                m_SubTracks[m_SelectedTrackIndex].UnSelect();
            }
            var posY = evt.localMousePosition.y - item_height * 0.5f;
            m_SelectedTrackIndex = GetSubTrackIndexByPos(posY);
            m_SubTracks[m_SelectedTrackIndex].Select();
            m_IsDragging = true;
        }

        private void OnSubTrackMenuParentMouseUp(MouseUpEvent evt)
        {
            m_IsDragging = false;
        }

        private void OnSubTrackMenuParentMouseOut(MouseOutEvent evt)
        {
            if (m_SubTrackMenuParent.contentRect.Contains(evt.localMousePosition))
            {
                return;
            }
            m_IsDragging = false;
        }

        #endregion

        private sealed class SubTrack
        {
            private const string menu_item_asset_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/MultiLineTrackMenuItem.uxml";

            private const string content_item_asset_path = "Assets/AkanyaTools/SkillMaster/Editor/Track/MultiLineTrackContentItem.uxml";

            private VisualElement m_MenuRoot;

            private VisualElement m_TrackRoot;

            private VisualElement m_MenuParent;

            private VisualElement m_TrackParent;

            private Action<SubTrack> m_OnDelete;

            private static readonly Color s_NormalColor = new(0, 0, 0, 0);

            private static readonly Color s_SelectedColor = new(0, 0, 0, 0.5f);

            private int m_Index;

            public void Init(int index, VisualElement menuParent, VisualElement trackParent, Action<SubTrack> onDelete)
            {
                this.m_MenuParent = menuParent;
                this.m_TrackParent = trackParent;
                m_OnDelete = onDelete;

                m_MenuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menu_item_asset_path).Instantiate().Query().ToList()[1];
                menuParent.Add(m_MenuRoot);

                var deleteBtn = m_MenuRoot.NiceQ<Button>("DeleteBtn");
                deleteBtn.clicked += () => { m_OnDelete?.Invoke(this); };

                m_TrackRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(content_item_asset_path).Instantiate().Query().ToList()[1];
                trackParent.Add(m_TrackRoot);

                SetIndex(index);
                UnSelect();
            }

            /// <summary>
            /// 选中
            /// </summary>
            public void Select()
            {
                m_MenuRoot.style.backgroundColor = s_SelectedColor;
            }

            /// <summary>
            /// 取消选中
            /// </summary>
            public void UnSelect()
            {
                m_MenuRoot.style.backgroundColor = s_NormalColor;
            }

            public void SetIndex(int index)
            {
                m_Index = index;

                var menuPos = m_MenuRoot.transform.position;
                menuPos.y = index * item_height;
                m_MenuRoot.transform.position = menuPos;

                var trackPos = m_TrackRoot.transform.position;
                trackPos.y = index * item_height + head_height;
                m_TrackRoot.transform.position = trackPos;
            }

            public int GetIndex() => m_Index;

            public void Destroy()
            {
                m_MenuParent.Remove(m_MenuRoot);
                m_TrackParent.Remove(m_TrackRoot);
            }
        }
    }
}