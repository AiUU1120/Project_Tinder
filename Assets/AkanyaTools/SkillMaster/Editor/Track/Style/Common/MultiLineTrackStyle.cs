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

namespace AkanyaTools.SkillMaster.Editor.Track.Style.Common
{
    public sealed class MultiLineTrackStyle : TrackStyleBase
    {
        private const string menu_asset_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/Common/MultiLineTrackMenu.uxml";

        private const string content_asset_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/Common/MultiLineTrackContent.uxml";

        /// <summary>
        /// 头部高度
        /// </summary>
        private const float head_height = 35;

        /// <summary>
        /// 子轨道高度
        /// </summary>
        private const float item_height = 32;

        private Action m_OnAddSubTrack;

        private Func<int, bool> m_OnDeleteSubTrack;

        private Action<int, int> m_OnSwapSubTrack;

        private Action<SubTrackStyle, string> m_OnSubTrackNameChange;

        private VisualElement m_SubTrackMenuParent;

        private readonly List<SubTrackStyle> m_SubTracks = new();

        private int m_SelectedTrackIndex = -1;

        private bool m_IsDragging;

        public void Init(VisualElement menuParent, VisualElement contentParent, string title, Action onAddSubTrack, Func<int, bool> onDeleteSubTrack, Action<int, int> onSwapSubTrack,
            Action<SubTrackStyle, string> onSubTrackNameChange)
        {
            this.menuParent = menuParent;
            this.contentParent = contentParent;
            m_OnAddSubTrack = onAddSubTrack;
            m_OnDeleteSubTrack = onDeleteSubTrack;
            m_OnSwapSubTrack = onSwapSubTrack;
            m_OnSubTrackNameChange = onSubTrackNameChange;

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
        /// 删除子轨道视图
        /// </summary>
        /// <param name="subTrackStyle">待删除子轨道</param>
        private void DeleteSubTrackView(SubTrackStyle subTrackStyle)
        {
            var index = subTrackStyle.GetIndex();
            subTrackStyle.Remove();
            m_SubTracks.RemoveAt(index);
            UpdateAllSubTrackIndex(index);
            UpdateSize();
        }

        /// <summary>
        /// 删除子轨道数据 包括视图
        /// </summary>
        /// <param name="subTrackStyle">待删除子轨道</param>
        private void DeleteSubTrackData(SubTrackStyle subTrackStyle)
        {
            if (m_OnDeleteSubTrack == null)
            {
                return;
            }
            var index = subTrackStyle.GetIndex();
            if (!m_OnDeleteSubTrack.Invoke(index))
            {
                return;
            }
            subTrackStyle.Remove();
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
            m_OnSwapSubTrack?.Invoke(index1, index2);
        }

        /// <summary>
        /// 添加子轨道
        /// </summary>
        public SubTrackStyle AddSubTrack()
        {
            var subTrack = new SubTrackStyle();
            subTrack.Init(m_SubTracks.Count, m_SubTrackMenuParent, contentRoot, DeleteSubTrackData, DeleteSubTrackView, UpdateSubTrackName);
            m_SubTracks.Add(subTrack);
            UpdateSize();
            return subTrack;
        }

        private void UpdateSubTrackName(SubTrackStyle subTrackStyle, string name)
        {
            m_OnSubTrackNameChange?.Invoke(subTrackStyle, name);
        }

        #region Callback

        /// <summary>
        /// 添加子轨道
        /// </summary>
        private void OnAddBtnClick()
        {
            m_OnAddSubTrack?.Invoke();
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

        public sealed class SubTrackStyle
        {
            private const string menu_item_asset_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/Common/MultiLineTrackMenuItem.uxml";

            private const string content_item_asset_path = "Assets/AkanyaTools/SkillMaster/Static Resources/Style/Track/Common/MultiLineTrackContentItem.uxml";

            private VisualElement m_MenuRoot;

            public VisualElement trackRoot;

            private VisualElement m_MenuParent;

            private VisualElement m_TrackParent;

            private VisualElement m_Content;

            private TextField m_TrackNameTxtField;

            private Action<SubTrackStyle> m_OnDeleteData;

            private Action<SubTrackStyle> m_OnDeleteView;

            private Action<SubTrackStyle, string> m_OnTrackNameChange;

            private static readonly Color s_NormalColor = new(0, 0, 0, 0);

            private static readonly Color s_SelectedColor = new(0, 0, 0, 0.5f);

            private int m_Index;

            private string m_OldTrackName;

            public void Init(int index, VisualElement menuParent, VisualElement trackParent, Action<SubTrackStyle> onDeleteData, Action<SubTrackStyle> onDeleteView,
                Action<SubTrackStyle, string> onTrackNameChange)
            {
                m_MenuParent = menuParent;
                m_TrackParent = trackParent;
                m_OnDeleteData = onDeleteData;
                m_OnDeleteView = onDeleteView;
                m_OnTrackNameChange = onTrackNameChange;

                m_MenuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menu_item_asset_path).Instantiate().Query().ToList()[1];
                menuParent.Add(m_MenuRoot);

                var deleteBtn = m_MenuRoot.NiceQ<Button>("DeleteBtn");
                deleteBtn.clicked += () => { m_OnDeleteData?.Invoke(this); };

                trackRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(content_item_asset_path).Instantiate().Query().ToList()[1];
                trackParent.Add(trackRoot);

                m_TrackNameTxtField = m_MenuRoot.NiceQ<TextField>("TrackNameTxtField");
                m_TrackNameTxtField.RegisterCallback<FocusInEvent>(OnTrackNameTxtFieldFocusIn);
                m_TrackNameTxtField.RegisterCallback<FocusOutEvent>(OnTrackNameTxtFieldFocusOut);

                SetIndex(index);
                UnSelect();
            }

            public void AddItem(VisualElement content)
            {
                m_Content = content;
                trackRoot.Add(content);
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

            /// <summary>
            /// 设置轨道名称
            /// </summary>
            /// <param name="name"></param>
            public void SetTrackName(string name)
            {
                m_TrackNameTxtField.value = name;
            }

            public void SetIndex(int index)
            {
                m_Index = index;

                var menuPos = m_MenuRoot.transform.position;
                menuPos.y = index * item_height;
                m_MenuRoot.transform.position = menuPos;

                var trackPos = trackRoot.transform.position;
                trackPos.y = index * item_height + head_height;
                trackRoot.transform.position = trackPos;
            }

            public int GetIndex() => m_Index;

            public void DeleteView()
            {
                m_OnDeleteView?.Invoke(this);
            }

            /// <summary>
            /// 显示层面上移除子轨道
            /// </summary>
            public void Remove()
            {
                m_MenuParent.Remove(m_MenuRoot);
                m_TrackParent.Remove(trackRoot);
            }

            #region Callback

            private void OnTrackNameTxtFieldFocusIn(FocusInEvent evt)
            {
                m_OldTrackName = m_TrackNameTxtField.value;
            }

            private void OnTrackNameTxtFieldFocusOut(FocusOutEvent evt)
            {
                if (m_OldTrackName == m_TrackNameTxtField.value)
                {
                    return;
                }
                m_OnTrackNameChange?.Invoke(this, m_TrackNameTxtField.value);
            }

            #endregion
        }
    }
}