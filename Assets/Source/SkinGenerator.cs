using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkinGenerator : MonoBehaviour
{
    [SerializeField] SkinNodeTemplate rootNode;
    [SerializeField] GameObject targetObject;
    [SerializeField] Material material;

    Mesh lastGeneratedMesh;

    [ContextMenu("Generate skin")]
    void GenerateSkin()
    {
        List<SkinNodeTemplate> joints = new List<SkinNodeTemplate>();
        rootNode.TraceIndices(joints);
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
    }

    private void OnDrawGizmos()
    {
        if (lastGeneratedMesh != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireMesh(lastGeneratedMesh);
        }
    }
}