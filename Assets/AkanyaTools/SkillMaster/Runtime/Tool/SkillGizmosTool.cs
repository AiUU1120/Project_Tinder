/*
 * @Author: AiUU
 * @Description: 技能检测绘制工具类
 * @AkanyaTech.SkillMaster
 */

#if UNITY_EDITOR

using AkanyaTools.SkillMaster.Runtime.Component;
using AkanyaTools.SkillMaster.Runtime.Data.Event;
using AkanyaTools.EditorHelper;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Tool
{
    public static class SkillGizmosTool
    {
        public static void DrawDetectionGizmos(SkillDetectionFrameEvent e, SkillPlayer skillPlayer)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            var modelTransform = skillPlayer.modelTransform == null ? skillPlayer.transform : skillPlayer.modelTransform;
            switch (e.detectionType)
            {
                case DetectionType.Weapon:
                    var weaponDetectionData = (WeaponDetectionData) e.detectionData;
                    if (!string.IsNullOrEmpty(weaponDetectionData.weaponName) && skillPlayer.skillWeaponsDic.TryGetValue(weaponDetectionData.weaponName, out var weapon))
                    {
                        var weaponCol = weapon.GetComponent<Collider>();
                        var transform = weaponCol.transform;
                        var weaponRotateAndPositionMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
                        Gizmos.matrix = weaponRotateAndPositionMatrix;
                        if (weaponCol is BoxCollider boxCol)
                        {
                            Gizmos.DrawCube(boxCol.center, boxCol.size);
                        }
                        else if (weaponCol is SphereCollider sphereCol)
                        {
                            Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);
                        }
                    }
                    break;
                case DetectionType.Box:
                    var boxDetectionData = (BoxDetectionData) e.detectionData;
                    var boxPosition = modelTransform.TransformPoint(boxDetectionData.position);
                    var boxRotation = modelTransform.rotation * Quaternion.Euler(boxDetectionData.rotation);
                    var boxRotateAndPositionMatrix = Matrix4x4.TRS(boxPosition, boxRotation, Vector3.one);
                    Gizmos.matrix = boxRotateAndPositionMatrix;
                    Gizmos.DrawCube(Vector3.zero, boxDetectionData.scale);
                    break;
                case DetectionType.Sphere:
                    var sphereDetectionData = (SphereDetectionData) e.detectionData;
                    Gizmos.DrawSphere(modelTransform.TransformPoint(sphereDetectionData.position), sphereDetectionData.radius);
                    break;
                case DetectionType.Sector:
                    var sectorDetectionData = (SectorDetectionData) e.detectionData;
                    var sectorMesh = MeshGenerator.GenerateSectorMesh(sectorDetectionData.outerRadius, sectorDetectionData.innerRadius, sectorDetectionData.height, sectorDetectionData.angle);
                    var sectorPosition = modelTransform.TransformPoint(sectorDetectionData.position);
                    var sectorRotation = modelTransform.rotation * Quaternion.Euler(sectorDetectionData.rotation);
                    Gizmos.DrawMesh(sectorMesh, sectorPosition, sectorRotation);
                    break;
            }
            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
#endif