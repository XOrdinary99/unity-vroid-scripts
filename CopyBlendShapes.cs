public class CopyBlendShapes : MonoBehaviour
{
    public GameObject source;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        // https://docs.unity3d.com/ScriptReference/Mesh.html

        Mesh sourceMesh = source.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        Mesh targetMesh = target.GetComponent<SkinnedMeshRenderer>().sharedMesh;

        Vector3[] deltaVertices = new Vector3[sourceMesh.vertexCount];
        Vector3[] deltaNormals = new Vector3[sourceMesh.vertexCount];
        Vector3[] deltaTangents = new Vector3[sourceMesh.vertexCount];

        for (int shapeIndex = 0; shapeIndex < sourceMesh.blendShapeCount; shapeIndex++)
        {
            string shapeName = sourceMesh.GetBlendShapeName(shapeIndex);

            // Don't copy if target already has a blend shape with this name. (It throws an exception.)
            if (targetMesh.GetBlendShapeIndex(shapeName) < 0)
            {
                // Copy across the keyframes in the blendshape (most have 1 keyframe).
                int frameCount = sourceMesh.GetBlendShapeFrameCount(shapeIndex);
                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    float frameWeight = sourceMesh.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
                    sourceMesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                    targetMesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
                }
            }
        }
    }
}
