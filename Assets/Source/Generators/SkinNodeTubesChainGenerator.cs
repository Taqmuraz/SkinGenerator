using System.Linq;
using UnityEngine;

public sealed class SkinNodeTubesChainGenerator : SkinNodeTubeGenerator
{
    Vector3[] points;
    Vector2 size;

    public SkinNodeTubesChainGenerator(Vector3[] points, Vector2 size)
    {
        this.points = points;
        this.size = size;
    }

    public override void Generate(IMeshDataStream meshDataStream, ISkinJoint joint)
    {
        if (points.Length > 1)
        {
            meshDataStream.PushIndexBuffer(joint.MaterialIndex);

            for (int i = 1; i < points.Length; i++)
            {
                Vector3 start = joint.Matrix.MultiplyPoint3x4(points[i - 1]);
                Vector3 end = joint.Matrix.MultiplyPoint3x4(points[i]);

                meshDataStream.WriteVertexData(CreateVertices(start, Quaternion.FromToRotation(Vector3.up, (end - start).normalized), size, new Vector2(0f, i - 1), joint.Index));
                meshDataStream.WriteVertexData(CreateVertices(end, Quaternion.FromToRotation(Vector3.up, (end - start).normalized), size, new Vector2(0f, i), joint.Index));
                meshDataStream.WriteIndices(LocalIndices.Select(l => l + (i - 1) * 8).ToArray());
                meshDataStream.WriteIndices(LocalStartIndices.Select(l => l + (i - 1) * 8).ToArray());
                meshDataStream.WriteIndices(LocalEndIndices.Select(l => l + (i - 1) * 8).ToArray());
            }
        }
    }
}
