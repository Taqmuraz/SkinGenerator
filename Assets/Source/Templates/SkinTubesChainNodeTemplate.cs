using UnityEngine;

public sealed class SkinTubesChainNodeTemplate : SkinNodeTemplate, ISkinEditorObject
{
    [SerializeField] float nodeWidth = 0.05f;
    [SerializeField] float nodeDepth = 0.05f;
    [SerializeField] Vector3[] points;

    protected override ISkinNodeGenerator CreateGenerator()
    {
        return new SkinNodeTubesChainGenerator(points, new Vector2(nodeWidth, nodeDepth));
    }

    public void DrawObject(ISkinEditorGUI editor)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = editor.DrawPositionHandle(points[i], Matrix, Color.yellow);
        }
    }
}