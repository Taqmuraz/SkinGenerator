using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public sealed class SkinNodeTemplate : MonoBehaviour, ISkinNode, ISkinJoint
{
    [SerializeField] float nodeWidth = 0.05f;
    [SerializeField] bool connectWithChildren;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, nodeWidth * 0.5f);
        foreach (var child in childTemplates)
        {
            Gizmos.DrawLine(transform.position, child.transform.position);
        }
    }

    public void AppendMeshData(ISkinNodeDataStream skinStream)
    {
        if (connectWithChildren || childTemplates.Length <= 1) GenerateNode(skinStream);

        switch (childTemplates.Length)
        {
            case 0:
                return;
            case 1:
                var singleChild = childTemplates[0];
                singleChild.AppendMeshData(skinStream);
                break;
            default:
                foreach (var child in childTemplates)
                {
                    skinStream.PushBuffer();
                    if (connectWithChildren) GenerateNode(skinStream);
                    child.AppendMeshData(skinStream);
                }
                break;
        }
    }
    void GenerateNode(ISkinNodeDataStream stream)
    {
        float halfWidth = nodeWidth * 0.5f;

        Vector3[] localVertices = new Vector3[]
        {
            new Vector3(-halfWidth, 0f, -halfWidth),
            new Vector3(-halfWidth, 0f, halfWidth),
            new Vector3(halfWidth, 0f, halfWidth),
            new Vector3(halfWidth, 0f, -halfWidth),
        };

        stream.Write(new SkinNodeData(index, ParentIndex, localVertices.Select(l => Matrix.MultiplyPoint3x4(l)).ToArray()));
    }

    public Matrix4x4 Matrix => transform.localToWorldMatrix;
    public int Index => index;

    public int ParentIndex => parent == null ? -1 : parent.index;
}
