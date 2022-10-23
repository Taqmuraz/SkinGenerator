using System.Linq;
using UnityEngine;

public sealed class SkinNodeFrustumGenerator : ISkinNodeGenerator
{
    Vector2 startSize;
    Vector2 endSize;

    public SkinNodeFrustumGenerator(Vector2 startSize, Vector2 endSize)
    {
        this.startSize = startSize;
        this.endSize = endSize;
    }

    public void Generate(IMeshDataStream meshDataStream, ISkinJoint joint)
    {
        var child = joint.Children.FirstOrDefault();
        if (child != null)
        {
            Quaternion rotation = joint.Matrix.rotation;
            Vector3 position = joint.Matrix.GetPosition();
            Vector3 endPosition = child.Matrix.GetPosition();

            meshDataStream.PushIndexBuffer(joint.MaterialIndex);

            int[] localIndices =
            {
               0, 1, 5,
               4, 0, 5,
               0, 4, 7,
               7, 3, 0,
               6, 2, 3,
               3, 7, 6,
               1, 2, 6,
               6, 5, 1
            };
            int[] beginLocalIndices =
            {
                0, 3, 2,
                2, 1, 0
            };
            int[] endLocalIndices =
            {
                7, 4, 5,
                5, 6, 7
            };

            meshDataStream.WriteVertexData(CreateVertices(position, rotation, startSize, Vector2.zero));
            meshDataStream.WriteVertexData(CreateVertices(endPosition, rotation, endSize, Vector2.up));
            meshDataStream.WriteIndices(beginLocalIndices);
            meshDataStream.WriteIndices(localIndices);
            meshDataStream.WriteIndices(endLocalIndices);
        }

        VertexData[] CreateVertices(Vector3 position, Quaternion rotation, Vector2 size, Vector2 uvOffset)
        {
            float halfWidth = size.x * 0.5f;
            float halfDepth = size.y * 0.5f;

            Vector3[] localVertices = new Vector3[]
            {
                new Vector3(-halfWidth, 0f, -halfDepth),
                new Vector3(-halfWidth, 0f, halfDepth),
                new Vector3(halfWidth, 0f, halfDepth),
                new Vector3(halfWidth, 0f, -halfDepth),
            };
            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(2f/3f, 0),
                new Vector2(1f/3f, 0),
            };
            return Enumerable.Range(0, localVertices.Length)
                .Select(i => new VertexData(Matrix4x4.TRS(position, rotation, Vector3.one).MultiplyPoint3x4(localVertices[i]), Vector3.zero, uvs[i] + uvOffset, new Vector3Int(joint.Index, -1, -1), new Vector3(1f, 0f, 0f))).ToArray();
        }
    }
}
