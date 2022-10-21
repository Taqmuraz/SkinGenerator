public interface IMeshDataStream
{
    void PushJoints(ISkinJoint[] joints);
    void WriteVertexData(VertexData[] vertexData);
    void WriteIndices(int[] indices);
    void PushIndexBuffer(int submeshIndex);
}
