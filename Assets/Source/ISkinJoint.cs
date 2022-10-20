using UnityEngine;

public interface ISkinJoint
{
    Matrix4x4 Matrix { get; }
    int Index { get; }
}
