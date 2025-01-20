/*
 * @Author: AiUU
 * @Description: 技能检测工具类
 * @AkanyaTech.SkillMaster
 */

using AkanyaTools.SkillMaster.Runtime.Data.Event;
using UnityEngine;

namespace AkanyaTools.SkillMaster.Runtime.Tool
{
    public static class SkillDetectionTool
    {
        private static readonly Collider[] s_DetectionResCols = new Collider[99];

        /// <summary>
        /// 形状类型检测
        /// </summary>
        public static Collider[] ShapeDetection(Transform sourceTransform, DetectionDataBase e, DetectionType type, LayerMask layerMask)
        {
            switch (type)
            {
                case DetectionType.Box:
                    return BoxDetection(sourceTransform, (BoxDetectionData) e, layerMask);
                case DetectionType.Sphere:
                    return SphereDetection(sourceTransform, (SphereDetectionData) e, layerMask);
                case DetectionType.Sector:
                    return SectorDetection(sourceTransform, (SectorDetectionData) e, layerMask);
            }
            return null;
        }

        public static Collider[] BoxDetection(Transform sourceTransform, BoxDetectionData e, LayerMask layerMask)
        {
            ClearResCols();
            Physics.OverlapBoxNonAlloc(sourceTransform.TransformPoint(e.position), e.scale * 0.5f, s_DetectionResCols, sourceTransform.rotation * Quaternion.Euler(e.rotation), layerMask);
            return s_DetectionResCols;
        }

        public static Collider[] SphereDetection(Transform sourceTransform, SphereDetectionData e, LayerMask layerMask)
        {
            ClearResCols();
            Physics.OverlapSphereNonAlloc(sourceTransform.TransformPoint(e.position), e.radius, s_DetectionResCols, layerMask);
            return s_DetectionResCols;
        }

        public static Collider[] SectorDetection(Transform sourceTransform, SectorDetectionData e, LayerMask layerMask)
        {
            ClearResCols();
            var scale = new Vector3() { x = e.outerRadius * 2, y = e.height, z = e.outerRadius * 2 };
            var sourceRotation = sourceTransform.rotation;
            var sectorForward = sourceRotation * Quaternion.Euler(e.rotation) * Vector3.forward;
            var sourcePosition = sourceTransform.position;
            // 先构建Box检测
            Physics.OverlapBoxNonAlloc(sourceTransform.TransformPoint(e.position), scale * 0.5f, s_DetectionResCols, sourceRotation * Quaternion.Euler(e.rotation), layerMask);
            // 过滤无效检测
            for (var i = 0; i < s_DetectionResCols.Length; i++)
            {
                var col = s_DetectionResCols[i];
                if (col == null)
                {
                    continue;
                }
                // 过滤内半径内的与外半径外的
                var point = col.ClosestPoint(sourcePosition);
                var distance = Vector3.Distance(point, sourcePosition);
                var isRemove = distance < e.innerRadius || distance > e.outerRadius;
                // 过滤不在角度内的
                if (!isRemove)
                {
                    var angle = Vector3.Angle(sectorForward, point - sourcePosition);
                    isRemove = angle > e.angle * 0.5f;
                }
                if (isRemove)
                {
                    s_DetectionResCols[i] = null;
                }
            }
            return s_DetectionResCols;
        }

        private static void ClearResCols()
        {
            for (var i = 0; i < s_DetectionResCols.Length; i++)
            {
                s_DetectionResCols[i] = null;
            }
        }
    }
}