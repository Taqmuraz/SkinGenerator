using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkinNodeTemplate), true)]
public class SkinNodeTemplateEditor : Editor, ISkinEditorGUI
{
    public void OnSceneGUI()
    {
        var targetObject = target as ISkinEditorObject;

        if (targetObject != null)
        {
            Undo.RecordObject(target, $"Editor changes for {target.name}");
            targetObject.DrawObject(this);
        }
    }

    public Vector3 DrawPositionHandle(Vector3 position, Matrix4x4 matrix, Color32 color)
    {
        Handles.color = color;
        Handles.matrix = matrix;
        return Handles.FreeMoveHandle(position, Quaternion.identity, 0.1f, Vector3.zero, Handles.SphereHandleCap);
    }
}
