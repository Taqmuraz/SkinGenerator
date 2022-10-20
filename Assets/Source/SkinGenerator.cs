using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinGenerator : MonoBehaviour
{
    [SerializeField] SkinNodeTemplate rootNode;
    [SerializeField] GameObject targetObject;
    [SerializeField] Material material;
    [SerializeField] bool generateOnStart = true;

    Mesh lastGeneratedMesh;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateSkin();
        }
    }

    [ContextMenu("Generate skin")]
    void GenerateSkin()
    {
        List<SkinNodeTemplate> nodes = new List<SkinNodeTemplate>();
        rootNode.TraceIndices(nodes, null);
        var meshStream = new MeshDataStream();
        meshStream.PushJoints(nodes.ToArray());
        foreach (var node in nodes)
        {
            node.AppendMeshData(meshStream);
        }
        lastGeneratedMesh = meshStream.BuildMesh();

        var renderer = targetObject.GetComponent<SkinnedMeshRenderer>();
        if(renderer == null) renderer = targetObject.AddComponent<SkinnedMeshRenderer>();

        renderer.bones = nodes.Select(j => j.transform).ToArray();
        renderer.sharedMesh = lastGeneratedMesh;
        renderer.rootBone = rootNode.transform;
        renderer.materials = Enumerable.Repeat(material, lastGeneratedMesh.subMeshCount).ToArray();
        renderer.localBounds = lastGeneratedMesh.bounds;
    }

    sealed class GizmosDrawSkinStream : IMeshDataStream
    {
        List<VertexData> vertices = new List<VertexData>();
        List<List<int>> indices = new List<List<int>>();

        public void PushJoints(ISkinJoint[] joints)
        {
        }

        public void Write(VertexData[] vertexData)
        {
            vertices.AddRange(vertexData);
        }

        public void WriteIndices(int[] indices)
        {
            this.indices.Last().AddRange(indices);
        }

        public void PushIndexBuffer(out int lastIndex)
        {
            lastIndex = vertices.Count;
            indices.Add(new List<int>());
        }

        public void Draw()
        {
            foreach (var submesh in indices)
            {
                for (int i = 0; i < submesh.Count; i+=3)
                {
                    Vector3 p0 = vertices[submesh[i]].Position;
                    Vector3 p1 = vertices[submesh[i + 1]].Position;
                    Vector3 p2 = vertices[submesh[i + 2]].Position;

                    Gizmos.DrawLine(p0, p1);
                    Gizmos.DrawLine(p0, p2);
                    Gizmos.DrawLine(p2, p1);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        if (rootNode != null)
        {
            var nodes = new List<SkinNodeTemplate>();
            rootNode.TraceIndices(nodes, null);
            var stream = new GizmosDrawSkinStream();
            Gizmos.color = Color.yellow;

            foreach (var node in nodes) node.AppendMeshData(stream);

            stream.Draw();
        }
        if (lastGeneratedMesh != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(lastGeneratedMesh);
        }
    }
}