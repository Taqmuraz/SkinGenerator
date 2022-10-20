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
        List<SkinNodeTemplate> joints = new List<SkinNodeTemplate>();
        rootNode.TraceIndices(joints, null);
        var skinStream = new SkinNodeDataStream();
        rootNode.AppendMeshData(skinStream);
        var meshStream = new MeshDataStream();
        skinStream.BuildSkin(meshStream, joints.ToArray());
        lastGeneratedMesh = meshStream.BuildMesh();

        var renderer = targetObject.GetComponent<SkinnedMeshRenderer>();
        if(renderer == null) renderer = targetObject.AddComponent<SkinnedMeshRenderer>();

        renderer.bones = joints.Select(j => j.transform).ToArray();
        renderer.sharedMesh = lastGeneratedMesh;
        renderer.rootBone = rootNode.transform;
        renderer.materials = Enumerable.Repeat(material, lastGeneratedMesh.subMeshCount).ToArray();
        renderer.localBounds = lastGeneratedMesh.bounds;
    }

    sealed class GizmosDrawSkinStream : ISkinNodeDataStream
    {
        public void Write(SkinNodeData skinNodeData)
        {
            var vertices = skinNodeData.Vertices;
            Gizmos.DrawLine(vertices[0], vertices[1]);
            Gizmos.DrawLine(vertices[1], vertices[2]);
            Gizmos.DrawLine(vertices[2], vertices[3]);
            Gizmos.DrawLine(vertices[3], vertices[0]);
        }

        public void PushBuffer()
        {
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (rootNode != null)
        {
            var stream = new GizmosDrawSkinStream();
            Gizmos.color = Color.yellow;
            rootNode.AppendMeshData(stream);
        }
        if (lastGeneratedMesh != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireMesh(lastGeneratedMesh);
        }
    }
}