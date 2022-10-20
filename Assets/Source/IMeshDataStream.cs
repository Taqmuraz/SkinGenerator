public interface IMeshDataStream
{
    void PushJoints(ISkinJoint[] joints);
    void Write(VertexData[] vertexData);
    void WriteIndices(int[] indices);
    void PushIndexBuffer(out int lastIndex);
}
