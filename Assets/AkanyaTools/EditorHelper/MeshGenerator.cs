/*
* @Author: AiUU
* @Description: 编辑器网格生成器
* @AkanyaTech.EditorHelper
*/

using UnityEngine;

namespace AkanyaTools.EditorHelper
{
    public static class MeshGenerator
    {
        /// <summary>
        /// 生成扇形网格
        /// </summary>
        /// <param name="outerRadius">扇形外半径</param>
        /// <param name="innerRadius">扇形内半径</param>
        /// <param name="height">扇形高度</param>
        /// <param name="angle">扇形角度</param>
        /// <returns></returns>
        public static Mesh GenerateSectorMesh(float outerRadius, float innerRadius, float height, float angle)
        {
            var mesh = new Mesh();
            var centerPos = Vector3.zero;
            var direction = Vector3.forward;
            var rightDir = Quaternion.AngleAxis(angle / 2, Vector3.up) * direction;
            const float deltaAngle = 2.5f;
            var quadCount = (int) (angle / deltaAngle);
            var lineCount = quadCount + 1;
            var vertices = new Vector3[lineCount * 2 * 2];
            var triangles = new int[quadCount * 6 * 4 + 6 + 12];

            // 计算底面
            for (var i = 0; i < lineCount; i++)
            {
                var angleOffset = i * deltaAngle;
                var dir = Quaternion.AngleAxis(-angleOffset, Vector3.up) * rightDir;
                vertices[i * 2] = centerPos + dir * innerRadius;
                vertices[i * 2 + 1] = centerPos + dir * outerRadius;
                // 处理三角形 由于是底面 用逆时针顺序连接顶点
                if (i < lineCount - 1)
                {
                    triangles[i * 6 + 0] = i * 2 + 1;
                    triangles[i * 6 + 1] = i * 2 + 2;
                    triangles[i * 6 + 2] = i * 2 + 0;
                    triangles[i * 6 + 3] = i * 2 + 1;
                    triangles[i * 6 + 4] = i * 2 + 3;
                    triangles[i * 6 + 5] = i * 2 + 2;
                }
            }

            // 计算顶面
            for (var i = lineCount; i < lineCount * 2; i++)
            {
                var angleOffset = (i - lineCount) * deltaAngle;
                var dir = Quaternion.AngleAxis(-angleOffset, Vector3.up) * rightDir;
                var minPos = centerPos + dir * innerRadius;
                minPos.y += height;
                var maxPos = centerPos + dir * outerRadius;
                maxPos.y += height;
                vertices[i * 2] = minPos;
                vertices[i * 2 + 1] = maxPos;
                // 处理三角形 由于是顶面 用顺时针顺序连接顶点
                if (i < lineCount * 2 - 1)
                {
                    triangles[i * 6 + 0] = i * 2 + 0;
                    triangles[i * 6 + 1] = i * 2 + 2;
                    triangles[i * 6 + 2] = i * 2 + 1;
                    triangles[i * 6 + 3] = i * 2 + 2;
                    triangles[i * 6 + 4] = i * 2 + 3;
                    triangles[i * 6 + 5] = i * 2 + 1;
                }
            }

            // 计算右面
            var start = 2 * lineCount - 1;
            triangles[start * 6 + 0] = 0;
            triangles[start * 6 + 1] = lineCount * 2;
            triangles[start * 6 + 2] = 1;
            triangles[start * 6 + 3] = lineCount * 2;
            triangles[start * 6 + 4] = lineCount * 2 + 1;
            triangles[start * 6 + 5] = 1;

            // 计算左面
            triangles[start * 6 + 6] = (lineCount - 1) * 2 + 1;
            triangles[start * 6 + 7] = (lineCount * 2 - 1) * 2;
            triangles[start * 6 + 8] = (lineCount - 1) * 2;
            triangles[start * 6 + 9] = (lineCount - 1) * 2 + 1;
            triangles[start * 6 + 10] = (lineCount * 2 - 1) * 2 + 1;
            triangles[start * 6 + 11] = (lineCount * 2 - 1) * 2;

            // 计算后面
            start += 2;
            for (var i = 0; i < quadCount; i++)
            {
                var index = start + i;
                triangles[index * 6 + 0] = lineCount * 2 + i * 2 + 1;
                triangles[index * 6 + 1] = i * 2 + 1 + 2;
                triangles[index * 6 + 2] = i * 2 + 1;
                triangles[index * 6 + 3] = lineCount * 2 + i * 2 + 1;
                triangles[index * 6 + 4] = lineCount * 2 + i * 2 + 1 + 2;
                triangles[index * 6 + 5] = i * 2 + 1 + 2;
            }

            // 计算前面
            start += quadCount;
            for (var i = 0; i < quadCount; i++)
            {
                var index = start + i;
                triangles[index * 6 + 0] = i * 2 + 0;
                triangles[index * 6 + 1] = i * 2 + 2;
                triangles[index * 6 + 2] = lineCount * 2 + i * 2;
                triangles[index * 6 + 3] = i * 2 + 2;
                triangles[index * 6 + 4] = lineCount * 2 + i * 2 + 2;
                triangles[index * 6 + 5] = lineCount * 2 + i * 2;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}