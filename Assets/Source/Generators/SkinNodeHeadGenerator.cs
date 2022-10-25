using System.Linq;
using UnityEngine;

public sealed class SkinNodeHeadGenerator : ISkinNodeGenerator
{
    Vector3 size;
    Vector3 offset;
    Quaternion rotation;

    public SkinNodeHeadGenerator(Vector3 offset, Vector3 size, Quaternion rotation)
    {
        this.offset = offset;
        this.size = size;
        this.rotation = rotation;
    }

    public void Generate(IMeshDataStream meshDataStream, ISkinJoint joint)
    {
        meshDataStream.PushIndexBuffer(joint.MaterialIndex);

        Vector3[] vertices =
        {
            new Vector3(-0.5f, 0f, -3f),
            new Vector3(0.5f, 0f, -3f),
            new Vector3(1.5f, 1f, 0f),
            new Vector3(2.5f, 5f, -3f),
            new Vector3(1.5f, 6f, 0f),
            new Vector3(-1.5f, 6f, 0f),
            new Vector3(-2.5f, 5f, -3f),
            new Vector3(-1.5f, 1f, 0f),
            new Vector3(-0.5f, 2f, 3f),
            new Vector3(0.5f, 2f, 3f),
            new Vector3(0.5f, 4f, 3f),
            new Vector3(-0.5f, 5f, 3f)
        };

        int[] indices =
        {
            0,6,3,
            3,1,0,

            1,3,2,
            0,7,6,

            6,5,4,
            4,3,6,

            5,6,11,
            3,4,10,

            4,5,11,
            11,10,4,

            6,7,8,
            8,11,6,
            3,10,9,
            9,2,3,

            10,11,8,
            8,9,10,

            8,7,0,
            1,2,9,
            9,8,0,
            0,1,9
        };
        var data = vertices.Select(v => new VertexData(joint.Matrix.MultiplyPoint3x4(rotation * (offset + new Vector3(v.x * size.x, v.y * size.y, v.z * size.z))), Vector3.zero, Vector2.zero, new Vector3Int(joint.Index, -1, -1), new Vector3(1f, 0f, 0f))).ToArray();
        meshDataStream.WriteVertexData(data);
        meshDataStream.WriteIndices(indices);
    }
}