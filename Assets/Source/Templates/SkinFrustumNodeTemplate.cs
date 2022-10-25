using System;
using UnityEngine;

public sealed class SkinFrustumNodeTemplate : SkinNodeTemplate
{
    [SerializeField] float nodeWidth = 0.05f;
    [SerializeField] float nodeDepth = 0.05f;
    [SerializeField] bool differentEnd;
    [SerializeField] float endWidth = 0.05f;
    [SerializeField] float endDepth = 0.05f;

    protected override ISkinNodeGenerator CreateGenerator()
    {
        return new SkinNodeFrustumGenerator(new Vector2(nodeWidth, nodeDepth), differentEnd ? new Vector2(endWidth, endDepth) : new Vector2(nodeWidth, nodeDepth));
    }
}
