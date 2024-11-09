/*
* @Author: AiUU
* @Description: SkillMaster SceneGUI 相关
* @AkanyaTech.SkillMaster
*/

using AkanyaTools.SkillMaster.Runtime.Component;
using UnityEditor;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public sealed partial class SkillMasterEditorWindow
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NotInSelectionHierarchy)]
        private static void DrawGizmos(SkillPlayer skillPlayer, GizmoType gizmoType)
        {
            if (instance == null || instance.curPreviewCharacterObj == null)
            {
                return;
            }
            if (instance.curPreviewCharacterObj.GetComponent<SkillPlayer>() != skillPlayer)
            {
                Debug.LogWarning("SkillMaster: 请给演示角色添加 SkillPlayer 组件\n否则 Gizmos 无法绘制");
                return;
            }
            foreach (var track in instance.m_TrackList)
            {
                track.DrawGizmos();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (curPreviewCharacterObj == null)
            {
                return;
            }
            foreach (var track in instance.m_TrackList)
            {
                track.DrawSceneGUI();
            }
        }
    }
}