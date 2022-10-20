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
        int expectedVerticesCount = 4;
        int globalIndex = 0;

        foreach (var subSkin in buffers)
        {
            if (subSkin.Count == 0) continue;

            stream.PushIndexBuffer();
            stream.WriteIndices(beginLocalIndices.Select(l => l + globalIndex).ToArray());

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

                int parentIndex = s.ParentIndex;
                int index = s.Index;
                float ownWeight;
                float parentWeight;
                if (parentIndex == -1)
                {
                    ownWeight = 1f;
                    parentWeight = 0f;
                }
                else
                {
                    ownWeight = 0.5f;
                    parentWeight = 0.5f;
                }

                return s.Vertices.Select(v => new VertexData(v, v.normalized, Vector2.zero, new Vector3Int(s.Index, parentIndex, -1), new Vector3(ownWeight, parentWeight, 0f)));
            }).ToArray();

            stream.WriteIndices(endLocalIndices.Select(l => l + globalIndex - expectedVerticesCount * 2).ToArray());

            stream.Write(vertexData);
        }
        stream.PushJoints(joints);
    }
}
