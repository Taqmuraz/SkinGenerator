using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class SkinNodeDataStream : ISkinNodeDataStream
{
    List<List<SkinNodeData>> buffers = new List<List<SkinNodeData>>()
    {
        new List<SkinNodeData>()
    };

    public void Write(SkinNodeData skinNodeData)
    {
        buffers[buffers.Count - 1].Add(skinNodeData);
    }

    public void PushBuffer()
    {
        buffers.Add(new List<SkinNodeData>());
    }

    public void BuildSkin(IMeshDataStream stream, ISkinJoint[] joints)
    {
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
        int expectedVerticesCount = 4;
        int globalIndex = 0;

        foreach (var subSkin in buffers)
        {
            if (subSkin.Count == 0) continue;

            stream.PushIndexBuffer();

            int nodeNumber = 0;

            var vertexData = subSkin.SelectMany(s =>
            {
                if (s.Vertices.Length != expectedVerticesCount)
                {
                    throw new System.InvalidOperationException($"Unexpected vertices count : {expectedVerticesCount}");
                }

                if (nodeNumber < subSkin.Count - 1) stream.WriteIndices(localIndices.Select(l => l + globalIndex).ToArray());
                nodeNumber++;
                globalIndex += expectedVerticesCount;

                return s.Vertices.Select(v => new VertexData(v, v.normalized, Vector2.zero, new Vector3Int(s.Index, -1, -1), new Vector3(1f, 0f, 0f)));
            }).ToArray();

            stream.Write(vertexData);
        }
        stream.PushJoints(joints);
    }
}
