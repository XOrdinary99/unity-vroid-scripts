using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CleveryBlendCopier : MonoBehaviour
{
    public bool doCopy = false;
    public string statusMessage = "";
    public Mesh sourceMesh;
    public GameObject target;
    public bool eraseClearBlendShapes = false;
    public bool eraseLogFile = false;

    private Mesh targetMesh;

    private struct SubMeshInfo
    {
        public string name; // For debugging
        public int startIndex;
        public int endIndex;
        public Bounds bounds;

        public string ToString()
        {
            return "sm(" + name + ", range=" + startIndex + "," + endIndex + " center=" + V3ToStr(bounds.center) + " extent=" + V3ToStr(bounds.extents) + ")";
        }
    }

    private SubMeshInfo[] m_sourceSubMeshes;
    private SubMeshInfo[] m_targetSubMeshes;

    // TODO: Needs confirming
    private const int SM_MOUTH = 0;
    private const int SM_EYEWHITES = 1;
    private const int SM_IRIS = 2;
    private const int SM_EYELINE = 3;
    private const int SM_BROW = 4;
    private const int SM_HIGHLIGHTS = 5;
    private const int SM_EYELASHES = 6;
    private const int SM_FACE = 7;
    private const int SM_EXTRA = 8;

    private static string[] m_subMeshNames =
    {
        "Mouth",
        "EyeWhite",
        "Iris",
        "Eyeline",
        "Brow",
        "Highlights",
        "EyeLashes",
        "Face",
        "Extra"
    };

    private struct KeyPoints
    {
        public Vector3 centerOfLeftEye;
        public Vector3 centerOfRightEye;
        public Vector3 nose;
        public Vector3 leftEdgeOfMouth;
        public Vector3 rightEdgeOfMouth;

        public string ToString()
        {
            return "kp(rightEdgeOfMouth=" + V3ToStr(rightEdgeOfMouth) 
                + ", leftEdgeOfMouth=" + V3ToStr(leftEdgeOfMouth) 
                + ", centerOfRightEye=" + V3ToStr(centerOfRightEye)
                + ", centerOfLeftEye=" + V3ToStr(centerOfLeftEye)
                + ", nose=" + V3ToStr(nose)
                + ")";
        }
    }

    private KeyPoints m_sourceKeyPoints;
    private KeyPoints m_targetKeyPoints;

    private System.Func<Vector3,Vector3>[] m_targetToSourceWarps;

    private StreamWriter m_writer;

    void OnValidate()
    {
        // If boolean field set to true, invoke the code and set field back to false.
        // This is like a button but no need to build a full Editor class.
        if (eraseLogFile)
        {
            eraseLogFile = false;
            statusMessage = "Restarting log file";
            if (m_writer != null)
            {
                m_writer.Close();
                DebugLog("New log file started");
            }
            statusMessage = "Log file restarted.";
        }

        if (eraseClearBlendShapes)
        {
            statusMessage = "Erasing blend shapes.";
            eraseClearBlendShapes = false;
            target.GetComponent<SkinnedMeshRenderer>().sharedMesh.ClearBlendShapes();
            statusMessage = "Blend shapes cleared";
        }

        if (doCopy)
        {
            statusMessage = "Starting copy of blend shapes";
            doCopy = false;
            CopyBlendShapes();
            statusMessage = "Blend shapes copied";
            DebugLog(this.ToString());
        }
    }

    void CopyBlendShapes()
    {
        //sourceMesh = source.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        targetMesh = target.GetComponent<SkinnedMeshRenderer>().sharedMesh;

        SetStatusMessage("Locating sub meshes");
        m_sourceSubMeshes = LocateSubMeshes(sourceMesh);
        m_targetSubMeshes = LocateSubMeshes(targetMesh);

        SetStatusMessage("Finding key points in meshes");
        m_sourceKeyPoints = FindKeyPoints(sourceMesh);
        m_targetKeyPoints = FindKeyPoints(targetMesh);

        DebugLog("SOURCE KEY POINTS: " + m_sourceKeyPoints.ToString());
        DebugLog("SOURCE BOUNDS: center=" + V3ToStr(m_sourceSubMeshes[SM_FACE].bounds.center) + ", extents=" + V3ToStr(m_sourceSubMeshes[SM_FACE].bounds.extents));
        DebugLog("TARGET KEY POINTS: " + m_targetKeyPoints.ToString());
        DebugLog("TARGET BOUNDS: center=" + V3ToStr(m_targetSubMeshes[SM_FACE].bounds.center) + ", extents=" + V3ToStr(m_targetSubMeshes[SM_FACE].bounds.extents));

        SetStatusMessage("Calculating face warps");
        CalculateFaceWarps();

        SetStatusMessage("Looping through blend shapes");
        for (int shapeIndex = 0; shapeIndex < sourceMesh.blendShapeCount; shapeIndex++)
        {
            string shapeName = sourceMesh.GetBlendShapeName(shapeIndex);
            DebugLog("======== BLEND SHAPE " + shapeName);

            // Don't copy if target already has a blend shape with this name. (It throws an exception.)
            if (targetMesh.GetBlendShapeIndex(shapeName) < 0)
            {
                // Copy across the keyframes in the blendshape (most have 1 keyframe).
                int frameCount = sourceMesh.GetBlendShapeFrameCount(shapeIndex);
                for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                {
                    CopyBlendShapesOfFrame(shapeName, shapeIndex, frameIndex);
                }
                //break; // TODO: FOR NOW JUST DO ONE!
            }
            else
            {
                SetStatusMessage("Blend Shape " + shapeName + " already there");
            }
        }
    }

    private static SubMeshInfo[] LocateSubMeshes(Mesh mesh)
    {
        var subMeshes = new SubMeshInfo[9];

        // For now this is a bit hacky - can improve over time if needed.
        // If VRoid character has 9 meshes, then the ears have been merged into the face already.
        // If 10 meshes, index 7 and 8 are face and ears - lets merge them together.
        // Some tools merge them, so let's work with those tools as well.
        int j = 0;
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            UnityEngine.Rendering.SubMeshDescriptor smd = mesh.GetSubMesh(i);

            SubMeshInfo smi = new SubMeshInfo();
            smi.name = m_subMeshNames[j];
            smi.startIndex = smd.firstVertex; // Or is it indexStart?
            smi.endIndex = smd.firstVertex + smd.vertexCount;
            smi.bounds = smd.bounds;

            // HACK: If size 10, do some magic at index 7 (face) to include following submesh (ears)
            // They share the same material and some tools merge them.
            // (If VRoid changes the order of subMeshes this will stuff things up big time!)
            if (i == 7 && mesh.subMeshCount == 10)
            {
                i++;
                smd = mesh.GetSubMesh(i);
                smi.bounds.Encapsulate(smd.bounds);
                smi.endIndex = smd.firstVertex + smd.vertexCount;
            }

            subMeshes[j++] = smi;
        }

        return subMeshes;
    }

    // Find some key points on the face (center of eyes etc)
    // These don't have to be exact - they just help normalize the source mesh to the target mesh a bit as eyes, nose, mouth can be different sizes.
    private KeyPoints FindKeyPoints(Mesh mesh)
    {
        // TODO: This assumes sub meshes are at certain places - if VRoid changes, these assumptions may become incorrect.

        KeyPoints kp = new KeyPoints();

        // Just pick any point in mesh as a relative value for Z so Z does not dominate below because the mouth is further forward in the head.
        float baseZ = mesh.vertices[0].z;

        // Assume subMesh 0 is the mouth. Find forward left and forward right most points as edges of mouth.
        UnityEngine.Rendering.SubMeshDescriptor mouthSmd = mesh.GetSubMesh(SM_MOUTH);
        for (int i = mouthSmd.firstVertex; i < mouthSmd.firstVertex + mouthSmd.vertexCount; i++)
        {
            Vector3 v = mesh.vertices[i];

            // Left is +ve X. Forward is +ve Z. We want front left, so add the -X and Z offsets.
            if ((v.x + (v.z - baseZ) / 2.0f) > (kp.leftEdgeOfMouth.x + (kp.leftEdgeOfMouth.z - baseZ) / 2.0f))
            {
                kp.leftEdgeOfMouth = v;
            }

            // Right is -ve X. Forward is +ve Z. We want front right, so add the X and Z offsets.
            if ((-v.x + (v.z - baseZ) / 2.0f) > (-kp.rightEdgeOfMouth.x + (kp.rightEdgeOfMouth.z - baseZ) / 2.0f))
            {
                kp.rightEdgeOfMouth = v;
            }
        }

        // Just approximate the eye positions. 
        // TODO: Could do a more accurate estimation by perhaps taking average of all vertices with +ve and -ve X values.
        UnityEngine.Rendering.SubMeshDescriptor eyeSmd = mesh.GetSubMesh(SM_EYEWHITES);
        kp.centerOfLeftEye = eyeSmd.bounds.center + new Vector3(eyeSmd.bounds.extents.x * 0.75f, 0, 0);
        kp.centerOfRightEye = eyeSmd.bounds.center - new Vector3(eyeSmd.bounds.extents.x * 0.75f, 0, 0);

        return kp;
    }

    // For each point on the target mesh, we need to find the closest vertex in the source mesh and copy
    private void CalculateFaceWarps()
    {
        m_targetToSourceWarps = new System.Func<Vector3,Vector3>[m_targetSubMeshes.Length];

        for (int i = 0; i < m_targetToSourceWarps.Length; i++)
        {
            if (i == SM_MOUTH)
            {
                m_targetToSourceWarps[i] = (Vector3 targetVertex) => (
                    new Vector3(
                        targetVertex.x / (m_targetKeyPoints.leftEdgeOfMouth.x - m_targetKeyPoints.rightEdgeOfMouth.x) * (m_sourceKeyPoints.leftEdgeOfMouth.x - m_sourceKeyPoints.rightEdgeOfMouth.x),
                        targetVertex.y / m_targetKeyPoints.leftEdgeOfMouth.y * m_sourceKeyPoints.leftEdgeOfMouth.y,
                        targetVertex.z / m_targetKeyPoints.leftEdgeOfMouth.z * m_sourceKeyPoints.leftEdgeOfMouth.z
                    )
                );
            }
            else
            {
                m_targetToSourceWarps[i] = (Vector3 targetVertex) => (
                    NineSquareTargetToSourceWarp(
                        targetVertex,
                        m_targetSubMeshes[SM_FACE].bounds, m_targetKeyPoints.centerOfRightEye, m_targetKeyPoints.centerOfLeftEye, m_targetKeyPoints.rightEdgeOfMouth, m_targetKeyPoints.leftEdgeOfMouth,
                        m_sourceSubMeshes[SM_FACE].bounds, m_sourceKeyPoints.centerOfRightEye, m_sourceKeyPoints.centerOfLeftEye, m_sourceKeyPoints.rightEdgeOfMouth, m_sourceKeyPoints.leftEdgeOfMouth
                    )
                );
            }
        }
    }

    // Imagine a 3x3 array of almost squares, with a bounding box and the eyes and mouth corners 4 points inside the array.
    // We assume the height of the eyes and mouths are the same, but the X values for eyes and mouths are different.
    private Vector3 NineSquareTargetToSourceWarp(
        Vector3 targetVertex,
        Bounds targetBounds, Vector3 targetNW, Vector3 targetNE, Vector3 targetSW, Vector3 targetSE,
        Bounds sourceBounds, Vector3 sourceNW, Vector3 sourceNE, Vector3 sourceSW, Vector3 sourceSE)
    {
        if (targetVertex.y > targetNW.y)
        {
            // Upper third of face.
            if (targetVertex.x < targetNW.x)
            {
                // NW
                return new Vector3(
                    sourceNW.x + (targetVertex.x - targetNW.x) * ((sourceNW.x - sourceBounds.center.x) / (targetNW.x - targetBounds.center.x)),
                    sourceNW.y + (targetVertex.y - targetNW.y) * ((sourceNW.y - sourceBounds.center.y) / (targetNW.y - targetBounds.center.y)),
                    targetVertex.z * (sourceBounds.center.z / targetBounds.center.z)
                );
            }
            else if (targetVertex.x > targetNE.x)
            {
                // NE
                return new Vector3(
                    sourceNE.x + (targetVertex.x - targetNE.x) * ((sourceNE.x - sourceBounds.center.x) / (targetNE.x - targetBounds.center.x)),
                    sourceNE.y + (targetVertex.y - targetNE.y) * ((sourceNE.y - sourceBounds.center.y) / (targetNE.y - targetBounds.center.y)),
                    targetVertex.z * (sourceBounds.center.z / targetBounds.center.z)
                );
            }
            else
            {
                // N
                return new Vector3(
                    sourceNW.x + (targetVertex.x - targetNW.x) * ((sourceNE.x - sourceNW.x) / (targetNE.x - targetNW.x)),
                    sourceNW.y + (targetVertex.y - targetNW.y) * ((sourceNE.y - sourceBounds.center.y) / (targetNE.y - targetBounds.center.y)),
                    targetVertex.z * (sourceBounds.center.z / targetBounds.center.z)
                );
            }
        }
        else if (targetVertex.y < targetSW.y)
        {
            // Lower third of face.
            if (targetVertex.x < targetSW.x)
            {
                // SW
                return new Vector3(
                    sourceSW.x + (targetVertex.x - targetSW.x) * ((sourceSW.x - sourceBounds.center.x) / (targetSW.x - targetBounds.center.x)),
                    sourceSW.y + (targetVertex.y - targetSW.y) * ((sourceSW.y - sourceBounds.center.y) / (targetSW.y - targetBounds.center.y)),
                    targetVertex.z * (sourceBounds.center.z / targetBounds.center.z)
                );
            }
            else if (targetVertex.x > targetSE.x)
            {
                // SE
                return new Vector3(
                    sourceSE.x + (targetVertex.x - targetSE.x) * ((sourceSE.x - sourceBounds.center.x) / (targetSE.x - targetBounds.center.x)),
                    sourceSE.y + (targetVertex.y - targetSE.y) * ((sourceSE.y - sourceBounds.center.y) / (targetSE.y - targetBounds.center.y)),
                    targetVertex.z * (sourceBounds.center.z / targetBounds.center.z)
                );
            }
            else
            {
                // S
                return new Vector3(
                    sourceSW.x + (targetVertex.x - targetSW.x) * ((sourceSE.x - sourceSW.x) / (targetSE.x - targetSW.x)),
                    sourceSW.y + (targetVertex.y - targetSW.y) * ((sourceSE.y - sourceBounds.center.y) / (targetSE.y - targetBounds.center.y)),
                    targetVertex.z * (sourceBounds.center.z / targetBounds.center.z)
                );
            }
        }
        else
        {
            // Middle third of face

            Vector3 targetRightEdge = new Vector3(targetBounds.center.x - targetBounds.extents.x, targetVertex.y, targetBounds.center.z - targetBounds.extents.z);
            Vector3 targetRightBorder = targetSW + (targetVertex - targetSW) * ((targetVertex.y - targetSW.y) / (targetNW.y - targetSW.y));
            Vector3 targetLeftBorder = targetSE + (targetVertex - targetSE) * ((targetVertex.y - targetSE.y) / (targetNE.y - targetSE.y));
            Vector3 targetLeftEdge = new Vector3(targetBounds.center.x + targetBounds.extents.x, targetVertex.y, targetBounds.center.z - targetBounds.extents.z);

            float sourceY = sourceSW.y + (targetVertex.y - targetSW.y) * ((sourceSE.y - sourceBounds.center.y) / (targetSE.y - targetBounds.center.y));

            Vector3 sourceRightEdge = new Vector3(sourceBounds.center.x - sourceBounds.extents.x, sourceY, sourceBounds.center.z - sourceBounds.extents.z);
            Vector3 sourceRightBorder = sourceSW + (targetVertex - targetSW) * ((sourceY - sourceSW.y) / (targetNW.y - targetSW.y));
            Vector3 sourceLeftBorder = sourceSE + (targetVertex - targetSE) * ((sourceY - sourceSE.y) / (targetNE.y - targetSE.y));
            Vector3 sourceLeftEdge = new Vector3(sourceBounds.center.x + sourceBounds.extents.x, sourceY, sourceBounds.center.z - sourceBounds.extents.z);

            if (targetVertex.x < targetRightBorder.x)
            {
                // W
                // TODO: This should be scaling x, y, and z separately as the scale factors are different. Try this to start with.
                return sourceRightEdge + (targetVertex - targetRightEdge) * ((sourceRightEdge.x - sourceRightBorder.x) / (targetRightEdge.x - targetRightBorder.x));
            }
            else if (targetVertex.x > targetLeftBorder.x)
            {
                // E
                return sourceLeftEdge + (targetVertex - targetLeftEdge) * ((sourceLeftEdge.x - sourceLeftBorder.x) / (targetLeftEdge.x - targetLeftBorder.x));
            }
            else
            {
                // Middle
                return sourceRightBorder + (targetVertex - targetRightBorder) * ((sourceRightBorder.x - sourceLeftBorder.x) / (targetRightBorder.x - targetLeftBorder.x));
            }
        }
    }

    private void CopyBlendShapesOfFrame(string shapeName, int shapeIndex, int frameIndex)
    {
        Vector3[] sourceDeltaVertices = new Vector3[sourceMesh.vertexCount];
        Vector3[] sourceDeltaNormals = new Vector3[sourceMesh.vertexCount];
        Vector3[] sourceDeltaTangents = new Vector3[sourceMesh.vertexCount];

        Vector3[] targetDeltaVertices = new Vector3[targetMesh.vertexCount];
        Vector3[] targetDeltaNormals = new Vector3[targetMesh.vertexCount];
        Vector3[] targetDeltaTangents = new Vector3[targetMesh.vertexCount];

        float frameWeight = sourceMesh.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
        sourceMesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, sourceDeltaVertices, sourceDeltaNormals, sourceDeltaTangents);

        string debugOut = "";
        for (int sm = 0; sm < m_targetSubMeshes.Length; sm++)
        {
            SubMeshInfo targetSm = m_targetSubMeshes[sm];
            SetStatusMessage("Copying sub mesh " + targetSm.name);

            // Convert Target vertex to an approximate location in the source mesh.
            for (int i = targetSm.startIndex; i < targetSm.endIndex; i++)
            {
                SetStatusMessage("Copying mesh " + targetSm.name + ", vertext " + i);

                Vector3 targetVertex = targetMesh.vertices[i];
                Vector3 sourceVertex = m_targetToSourceWarps[sm](targetVertex);
                //Vector3 sourceVertex = targetVertex;
                debugOut += "WARP: " + V3ToStr(targetVertex) + " -> " + V3ToStr(sourceVertex) + "\n";

                // Find index of nearest vertex in source mesh.
                int nearest = FindNearestSourceVertex(sm, sourceVertex);

                // Copy details from that source to target blend shape.
                targetDeltaVertices[i] = sourceDeltaVertices[nearest];
                targetDeltaNormals[i] = sourceDeltaNormals[nearest];
                targetDeltaTangents[i] = sourceDeltaTangents[nearest];
            }
        }
        SetStatusMessage("Finished copying " + shapeName);
        DebugLog(debugOut);

        targetMesh.AddBlendShapeFrame(shapeName, frameWeight, targetDeltaVertices, targetDeltaNormals, targetDeltaTangents);
    }

    private int FindNearestSourceVertex(int subMeshIndex, Vector3 vertex)
    {
        DebugLog("FindNearestSourceVertex(" + subMeshIndex + ", " + V3ToStr(vertex));
        SubMeshInfo smi = m_sourceSubMeshes[subMeshIndex];

        int bestIndex = smi.startIndex;
        Vector3 bestVertex = sourceMesh.vertices[bestIndex];
        float bestMagnitude = (bestVertex - vertex).magnitude;

        for (int i = smi.startIndex; i < smi.endIndex; i++)
        {
            //DebugLog("i=" + i);
            Vector3 nextVertex = sourceMesh.vertices[i];
            float nextMagnitude = (nextVertex - vertex).magnitude;
            if (nextMagnitude < bestMagnitude)
            {
                //DebugLog("better! i=" + i + " v=" + V3ToStr(nextVertex));
                bestIndex = i;
                bestVertex = nextVertex;
                bestMagnitude = nextMagnitude;
            }
        }
        DebugLog("Returning " + bestIndex);

        return bestIndex;
    }

    public void SetStatusMessage(string newMessage)
    {
        DebugLog(newMessage);
        //statusMessage = newMessage;
    }

    private void DebugLog(string message)
    {
        if (m_writer == null)
        {
            m_writer = new StreamWriter("MyLogFile.txt", false);
        }
        m_writer.WriteLine(message);
    }

    string ToString()
    {
        string s = "";

        if (m_sourceSubMeshes != null)
        {
            s += "Source SubMeshes\n";
            for (int i = 0; i < m_sourceSubMeshes.Length; i++)
            {
                s += "[" + i + "] " + m_sourceSubMeshes[i].ToString() + "\n";
            }
        }

        if (m_targetSubMeshes != null)
        {
            s += "\nTarget SubMeshes\n";
            for (int i = 0; i < m_targetSubMeshes.Length; i++)
            {
                s += "[" + i + "] " + m_targetSubMeshes[i].ToString() + "\n";
            }
        }

        /*
        if (m_sourceKeyPoints != null)
        {
            s += "\nSource Key Points\n";
            s += m_sourceKeyPoints.ToString() + "\n";
        }
        if (m_targetKeyPoints != null)
        {
            s += "\nTarget Key Points\n";
            s += m_targetKeyPoints.ToString() + "\n";
        }
        */

        return s;
    }

    private static string V3ToStr(Vector3 v)
    {
        return (v == null) ? "null" : ("(" + v.x + "," + v.y + "," + v.z + ")");
    }


    private void stuff()
    { 
        // https://docs.unity3d.com/ScriptReference/Mesh.html

        //Mesh sourceMesh = source.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        Mesh targetMesh = target.GetComponent<SkinnedMeshRenderer>().sharedMesh;

        if (sourceMesh.vertexCount != targetMesh.vertexCount)
        {
            // TODO: Add some more contet in here.
            DebugLog("Cannoy copy blend shapes as meshes differ");
            return;
        }

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
