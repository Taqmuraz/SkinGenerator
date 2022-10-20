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

            meshDataStream.PushIndexBuffer(out int startIndex);

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

            MoveIndices(beginLocalIndices, startIndex);
            MoveIndices(localIndices, startIndex);
            MoveIndices(endLocalIndices, startIndex);

            meshDataStream.Write(CreateVertices(position, rotation, startSize));
            meshDataStream.Write(CreateVertices(endPosition, rotation, endSize));
            meshDataStream.WriteIndices(beginLocalIndices);
            meshDataStream.WriteIndices(localIndices);
            meshDataStream.WriteIndices(endLocalIndices);
        }

        void MoveIndices(int[] local, int offset)
        {
            for (int i = 0; i < local.Length; i++) local[i] += offset;
        }

        VertexData[] CreateVertices(Vector3 position, Quaternion rotation, Vector2 size)
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

            return localVertices.Select(l => Matrix4x4.TRS(position, rotation, Vector3.one).MultiplyPoint3x4(l))
                .Select(v => new VertexData(v, Vector3.zero, Vector2.zero, new Vector3Int(joint.Index, -1, -1), new Vector3(1f, 0f, 0f))).ToArray();
        }
    }
}
