using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public sealed class SkinNodeTemplate : MonoBehaviour, ISkinNode, ISkinJoint
{
    [SerializeField] float nodeWidth = 0.05f;
    [SerializeField] float nodeDepth = 0.05f;
    [SerializeField] bool nonDrawable;
    [SerializeField] int siblingIndex;
    int index;
    SkinNodeTemplate[] childTemplates;
    int transformChildren;
    SkinNodeTemplate parent;

    public void TraceIndices(List<SkinNodeTemplate> jointsList, SkinNodeTemplate parent)
    {
        this.parent = parent;
        index = jointsList.Count;
        jointsList.Add(this);
        foreach (var child in childTemplates) child.TraceIndices(jointsList, this);
    }
    void TraceTransform(Transform transform, List<SkinNodeTemplate> templates)
    {
        var template = transform.GetComponent<SkinNodeTemplate>();
        if (template != null) templates.Add(template);
        else foreach (Transform child in transform) TraceTransform(child, templates);
    }

    [ContextMenu("Reset template")]
    void ResetTemplate()
    {
        RebindChildren();
        foreach (var child in childTemplates) child.RebindChildren();
    }
    void Awake()
    {
        RebindChildren();
    }
    void RebindChildren()
    {
        var traceList = new List<SkinNodeTemplate>();
        foreach (Transform child in transform) TraceTransform(child, traceList);
        childTemplates = traceList.ToArray();
    }

    void Update()
    {
        if (childTemplates.Any(c => c == null)) RebindChildren();
        if (transformChildren != transform.childCount)
        {
            RebindChildren();
            transformChildren = transform.childCount;
        }
    }
    public void AppendMeshData(ISkinNodeDataStream skinStream)
    {
        foreach (var child in childTemplates)
        {
            if (!nonDrawable)
            {
                Quaternion rotation = transform.rotation;
                GenerateNode(skinStream, index, transform.position, rotation);
                child.GenerateNode(skinStream, index, child.transform.position, rotation);
                skinStream.PushBuffer();
            }

            child.AppendMeshData(skinStream);
        }
    }
    void GenerateNode(ISkinNodeDataStream stream, int ownerIndex, Vector3 position, Quaternion rotation)
    {
        float halfWidth = nodeWidth * 0.5f;
        float halfDepth = nodeDepth * 0.5f;

        Vector3[] localVertices = new Vector3[]
        {
            new Vector3(-halfWidth, 0f, -halfDepth),
            new Vector3(-halfWidth, 0f, halfDepth),
            new Vector3(halfWidth, 0f, halfDepth),
            new Vector3(halfWidth, 0f, -halfDepth),
        };
        stream.Write(new SkinNodeData(ownerIndex, localVertices.Select(l => Matrix4x4.TRS(position, rotation, Vector3.one).MultiplyPoint3x4(l)).ToArray()));
    }

    public Matrix4x4 Matrix => transform.localToWorldMatrix;
    public int Index => index;
}
