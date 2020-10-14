using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddIFacialMocapBendShapes : MonoBehaviour
{
    public Mesh mesh;
    public bool addNow;

    // Start is called before the first frame update
    void Start()
    {
        AddMissingBlendShapes();
    }

    void OnValidate()
    {
        if (addNow)
        {
            addNow = false;
            AddMissingBlendShapes();
        }
    }

    // Add the default set of extra blend shapes.
    void AddMissingBlendShapes()
    {
        mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;

        // TODO: Should look for submesh using F00_000_00_Eye Iris_00_EYE material.
        int irisSubmesh = 5;

        // TODO: Should look for submesh using F00_000_00_Face Brow_00_FACE material.
        int browSubmesh = 4;

        AddBlendShape("browDownLeft", "Face.M_F00_000_00_Fcl_BRW_Angry", 1.0f, 1.0f, true, false, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("browDownRight", "Face.M_F00_000_00_Fcl_BRW_Angry", 1.0f, 1.0f, false, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("browInnerUp", "Face.M_F00_000_00_Fcl_BRW_Surprised", 1.0f, 1.0f, true, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddOuterUpBlendShape("browOuterUpLeft", 0.05f, true, false, browSubmesh);
        AddOuterUpBlendShape("browOuterUpRight", 0.05f, false, true, browSubmesh);
        AddBlendShape("cheekSquintLeft", "Face.M_F00_000_00_Fcl_EYE_Joy", 0.5f, 1.0f, true, false, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("cheekSquintRight", "Face.M_F00_000_00_Fcl_EYE_Joy", 0.5f, 1.0f, false, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeBlinkLeft", "Face.M_F00_000_00_Fcl_EYE_Close", 1.0f, 1.0f, true, false, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeBlinkRight", "Face.M_F00_000_00_Fcl_EYE_Close", 1.0f, 1.0f, false, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeSquintLeft", "Face.M_F00_000_00_Fcl_EYE_Joy", 0.5f, 1.0f, true, false, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeSquintRight", "Face.M_F00_000_00_Fcl_EYE_Joy", 0.5f, 1.0f, false, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeWideLeft", "Face.M_F00_000_00_Fcl_EYE_Surprised", 1.0f, 2.0f, true, false, true, true, ScaleNormalization.DefaultScale, -1, irisSubmesh);
        AddBlendShape("eyeWideRight", "Face.M_F00_000_00_Fcl_EYE_Surprised", 1.0f, 2.0f, false, true, true, true, ScaleNormalization.DefaultScale, -1, irisSubmesh);
        AddBlendShape("irisShrinkLeft", "Face.M_F00_000_00_Fcl_EYE_Surprised", 1.0f, 1.0f, true, false, true, true, ScaleNormalization.DefaultScale, irisSubmesh, -1);
        AddBlendShape("irisShrinkRight", "Face.M_F00_000_00_Fcl_EYE_Surprised", 1.0f, 1.0f, false, true, true, true, ScaleNormalization.DefaultScale, irisSubmesh, -1);
        AddJawOpenBlendShape("jawOpen", "Face.M_F00_000_00_Fcl_MTH_O", 0.15f, 0.05f);
        AddBlendShape("mouthDimpleLeft", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.8f, 1.0f, true, false, true, true, ScaleNormalization.ZeroAtCenter, -1, -1); 
        AddBlendShape("mouthDimpleRight", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.8f, 1.0f, false, true, true, true, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthFrownLeft", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.8f, -1.0f, true, false, true, true, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthFrownRight", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.8f, -1.0f, false, true, true, true, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthLeft", "Face.M_F00_000_00_Fcl_MTH_Fun", 1.0f, 1.0f, true, false, true, true, ScaleNormalization.LeftToRight, -1, -1);
        AddBlendShape("mouthRight", "Face.M_F00_000_00_Fcl_MTH_Fun", 1.0f, 1.0f, false, true, true, true, ScaleNormalization.RightToLeft, -1, -1);
        AddBlendShape("mouthPressLeft", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.7f, 1.0f, true, false, true, true, ScaleNormalization.LeftToRight, -1, -1);
        AddBlendShape("mouthPressRight", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.7f, 1.0f, false, true, true, true, ScaleNormalization.RightToLeft, -1, -1);
        AddBlendShape("mouthSmileLeft", "Face.M_F00_000_00_Fcl_MTH_Fun", 1.0f, 1.0f, true, false, true, true, ScaleNormalization.LeftToRight, -1, -1); 
        AddBlendShape("mouthSmileRight", "Face.M_F00_000_00_Fcl_MTH_Fun", 1.0f, 1.0f, false, true, true, true, ScaleNormalization.RightToLeft, -1, -1);
        AddBlendShape("mouthStretchLeft", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.8f, -1.0f, true, false, true, true, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthStretchRight", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.8f, -1.0f, false, true, true, true, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthUpperUpLeft", "Face.M_F00_000_00_Fcl_MTH_O", 1.5f, 1.0f, true, false, true, false, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthUpperUpRight", "Face.M_F00_000_00_Fcl_MTH_O", 1.5f, 1.0f, false, true, true, false, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthLowerDownLeft", "Face.M_F00_000_00_Fcl_MTH_O", 1.0f, 1.0f, true, false, false, true, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthLowerDownRight", "Face.M_F00_000_00_Fcl_MTH_O", 1.0f, 1.0f, false, true, false, true, ScaleNormalization.ZeroAtCenter, -1, -1);
        AddBlendShape("mouthFunnel", "Face.M_F00_000_00_Fcl_MTH_Sorrow", 1.0f, 1.0f, true, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("mouthPucker", "Face.M_F00_000_00_Fcl_MTH_U", 0.5f, 1.0f, true, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("mouthShrugLower", "Face.M_F00_000_00_Fcl_MTH_Fun", 0.25f, -1.0f, true, true, true, true, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("mouthShrugUpper", "Face.M_F00_000_00_Fcl_MTH_O", 0.25f, 1.0f, true, true, true, false, ScaleNormalization.DefaultScale, -1, -1);

        // The following are all zeroed, but included to stop warnings about undefined blend shapes.
        AddBlendShape("cheekPuff", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookDownLeft", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookDownRight", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookInLeft", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookInRight", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookOutLeft", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookOutRight", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookUpLeft", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("eyeLookUpRight", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("jawForward", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("jawLeft", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("jawRight", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("mouthClose", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("noseSneerLeft", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("noseSneerRight", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("mouthRollLower", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
        AddBlendShape("mouthRollUpper", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);

        // Not ARKit, but generated by iFacialMocap. Not sure how to generate this one.
        AddBlendShape("tongueOut", "Face.M_F00_000_00_Fcl_MTH_O", 0.0f, 0.0f, false, false, false, false, ScaleNormalization.DefaultScale, -1, -1);
    }

    public enum ScaleNormalization { DefaultScale, LeftToRight, RightToLeft, ZeroAtCenter };

    private bool AddBlendShape(
        string targetShapeName,
        string sourceShapeName,
        float magnitudeScale,
        float yScale,
        bool keepLeft,
        bool keepRight,
        bool keepUpward,
        bool keepDownward,
        ScaleNormalization scaleNormalization, 
        int onlyIncludeThisSubmesh, 
        int onlyExcludeThisSubmesh)
    {
        if (mesh.GetBlendShapeIndex(targetShapeName) >= 0)
        {
            // Blend shape already exists. Don't try to add it again.
            return false;
        }

        Vector3[] deltaVertices = new Vector3[mesh.vertexCount];
        Vector3[] deltaNormals = new Vector3[mesh.vertexCount];
        Vector3[] deltaTangents = new Vector3[mesh.vertexCount];

        // Find the source shape index.
        int shapeIndex = FindShapeIndex(sourceShapeName);
        if (shapeIndex < 0)
        {
            // Failed to find the source shape.
            return false;
        }

        // Copy across the keyframes in the blendshape (most have 1 keyframe).
        int frameCount = mesh.GetBlendShapeFrameCount(shapeIndex);
        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
        {
            float frameWeight = mesh.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
            mesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);
            AdjustBlendShape(deltaVertices, deltaNormals, deltaTangents, magnitudeScale, yScale, keepLeft, keepRight, keepUpward, keepDownward, scaleNormalization, onlyIncludeThisSubmesh, onlyExcludeThisSubmesh);
            mesh.AddBlendShapeFrame(targetShapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
        }

        return true;
    }

    private int FindShapeIndex(string shapeName)
    {
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            if (shapeName == mesh.GetBlendShapeName(i))
            {
                return i;
            }
        }
        return -1;
    }

    private void AdjustBlendShape(
        Vector3[] deltaVertices,
        Vector3[] deltaNormals,
        Vector3[] deltaTangents,
        float magnitudeScale,
        float yScale,
        bool keepLeft,
        bool keepRight,
        bool keepUpward,
        bool keepDownward,
        ScaleNormalization scaleNormalization,
        int onlyIncludeThisSubmesh,
        int onlyExcludeThisSubmesh)
    {
        Vector3[] vertices = mesh.vertices;
        Bounds bounds = mesh.bounds;
        float minX = bounds.center.x - bounds.extents.x;
        float maxX = bounds.center.x + bounds.extents.x;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = vertices[i];
            Debug.Log("v=" + V3ToString(v) + "  dv=" + V3ToString(deltaVertices[i]));

            bool zeroIt = false;

            if (v.x > 0.0f && !keepRight)
            {
                zeroIt = true;
                Debug.Log("ZA");
            }

            if (v.x < 0.0f && !keepLeft)
            {
                zeroIt = true;
                Debug.Log("ZB");
            }

            if (deltaVertices[i].y > 0.0f && !keepUpward)
            {
                zeroIt = true;
                Debug.Log("ZC");
            }

            if (deltaVertices[i].y < 0.0f && !keepDownward)
            {
                zeroIt = true;
                Debug.Log("ZD");
            }

            if (zeroIt)
            {
                deltaVertices[i] = Vector3.zero;
                deltaNormals[i] = Vector3.zero;
                deltaTangents[i] = Vector3.zero;
            }
            else
            {
                Debug.Log("KEEP");

                // Adjust the scaling factor based on position on face.
                float scaleFactor = magnitudeScale;
                switch (scaleNormalization)
                {
                    case ScaleNormalization.LeftToRight:
                        {
                            float localScale = (v.x - minX) / (maxX - minX);
                            Debug.Log("1.v.x=" + v.x + ", minX=" + minX + ", maxX=" + maxX + ", lscale=" + localScale);
                            if (localScale < 0.0f)
                            {
                                localScale = 0.0f;
                            }
                            if (localScale > 1.0f)
                            {
                                localScale = 1.0f;
                            }
                            scaleFactor *= localScale;
                            break;
                        }
                    case ScaleNormalization.RightToLeft:
                        {
                            float localScale = (v.x - minX) / (maxX - minX);
                            Debug.Log("2.v.x=" + v.x + ", minX=" + minX + ", maxX=" + maxX + ", lscale=" + localScale);
                            if (localScale < 0.0f)
                            {
                                localScale = 0.0f;
                            }
                            if (localScale > 1.0f)
                            {
                                localScale = 1.0f;
                            }
                            scaleFactor *= (1.0f - localScale);
                            break;
                        }
                    case ScaleNormalization.ZeroAtCenter:
                        {
                            float localScale = v.x / (maxX / 4.0f);
                            Debug.Log("localScale = " + localScale);
                            Debug.Log("2.v.x=" + v.x + ", minX=" + minX + ", maxX=" + maxX + ", lscale=" + localScale);
                            if (localScale < 0.0f)
                            {
                                localScale = -localScale;
                            }
                            if (localScale > 1.0f)
                            {
                                localScale = 1.0f;
                            }
                            Debug.Log("final localScale = " + localScale);
                            scaleFactor *= localScale;
                            Debug.Log("scaleFactor= " + scaleFactor);
                            break;
                        }
                }

                // Multiply in magnitude scale.
                deltaVertices[i] *= scaleFactor;
                deltaNormals[i] *= scaleFactor;
                deltaTangents[i] *= scaleFactor;

                // Multiple in Y scale (e.g. scale = -1 to invert)
                deltaVertices[i].y *= yScale;
                deltaNormals[i].y *= yScale;
                deltaTangents[i].y *= yScale;
            }
        }

        if (onlyIncludeThisSubmesh >= 0)
        {
            // Set everything to zero not in requested submesh
            UnityEngine.Rendering.SubMeshDescriptor smd = mesh.GetSubMesh(onlyIncludeThisSubmesh);
            for (int i = 0; i < smd.firstVertex; i++)
            {
                deltaVertices[i] = Vector3.zero;
                deltaNormals[i] = Vector3.zero;
                deltaTangents[i] = Vector3.zero;
            }
            for (int i = smd.firstVertex + smd.vertexCount; i < mesh.vertexCount; i++)
            {
                deltaVertices[i] = Vector3.zero;
                deltaNormals[i] = Vector3.zero;
                deltaTangents[i] = Vector3.zero;
            }
        }

        if (onlyExcludeThisSubmesh >= 0)
        {
            // Set everything to zero in requested submesh
            UnityEngine.Rendering.SubMeshDescriptor smd = mesh.GetSubMesh(onlyExcludeThisSubmesh);
            for (int i = smd.firstVertex; i < smd.firstVertex + smd.vertexCount;  i++)
            {
                deltaVertices[i] = Vector3.zero;
                deltaNormals[i] = Vector3.zero;
                deltaTangents[i] = Vector3.zero;
            }
        }
    }

    private bool AddJawOpenBlendShape(string targetShapeName, string sourceShapeName, float howMuchOfFace, float scaleAsFractionOfFace)
    {
        if (mesh.GetBlendShapeIndex(targetShapeName) >= 0)
        {
            // Blend shape already exists. Don't try to add it again.
            return false;
        }

        // The mesh extent is from center of face to edge, so multiply by two for total face size.
        float scale = scaleAsFractionOfFace * 2.0f * mesh.bounds.extents.magnitude;

        Vector3[] deltaVertices = new Vector3[mesh.vertexCount];
        Vector3[] deltaNormals = new Vector3[mesh.vertexCount];
        Vector3[] deltaTangents = new Vector3[mesh.vertexCount];

        float bottomOfFace = (mesh.bounds.center - mesh.bounds.extents).y;
        float topOfFace = (mesh.bounds.center + mesh.bounds.extents).y;
        float cutoffY = bottomOfFace + howMuchOfFace * (topOfFace - bottomOfFace);

        Vector3[] verteces = mesh.vertices;

        // Find the source shape index.
        int shapeIndex = FindShapeIndex(sourceShapeName);
        if (shapeIndex < 0)
        {
            // Failed to find the source shape.
            return false;
        }

        // Copy across the keyframes in the blendshape (most have 1 keyframe).
        int frameCount = mesh.GetBlendShapeFrameCount(shapeIndex);
        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
        {
            float frameWeight = mesh.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
            mesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, deltaVertices, deltaNormals, deltaTangents);

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 v = verteces[i];
                float yOffset = (v.y > cutoffY) ? 0.0f : -(scale * ((cutoffY - v.y) / (cutoffY - bottomOfFace)));
                deltaVertices[i].y += yOffset;
            }

            mesh.AddBlendShapeFrame(targetShapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
        }

        return true;
    }

    private bool AddOuterUpBlendShape(string targetShapeName, float scaleAsFractionOfFace, bool keepLeft, bool keepRight, int onlyIncludeThisSubmesh)
    {
        if (mesh.GetBlendShapeIndex(targetShapeName) >= 0)
        {
            // Blend shape already exists. Don't try to add it again.
            return false;
        }

        // The mesh extent is from center of face to edge, so multiply by two for total face size.
        float maxYOffset = scaleAsFractionOfFace * 2.0f * mesh.bounds.extents.magnitude;

        Vector3[] deltaVertices = new Vector3[mesh.vertexCount];
        Vector3[] deltaNormals = new Vector3[mesh.vertexCount];
        Vector3[] deltaTangents = new Vector3[mesh.vertexCount];

        Vector3[] verteces = mesh.vertices;
        Bounds bounds = mesh.bounds;
        float minX = bounds.center.x - bounds.extents.x;
        float maxX = bounds.center.x + bounds.extents.x;

        UnityEngine.Rendering.SubMeshDescriptor smd = mesh.GetSubMesh(onlyIncludeThisSubmesh);

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = verteces[i];
            float yOffset = 0.0f;

            if (i >= smd.firstVertex && i < smd.firstVertex + smd.vertexCount)
            {
                if ((v.x <= 0.0f && keepLeft) || (v.x >= 0.0f && keepRight))
                {
                    float localScale = v.x / (maxX / 2.0f);
                    if (localScale < 0.0f)
                    {
                        localScale = -localScale;
                    }
                    if (localScale > 1.0f)
                    {
                        localScale = 1.0f;
                    }

                    yOffset = localScale * maxYOffset;
                }
            }

            deltaVertices[i] = new Vector3(0, yOffset, 0);
            deltaNormals[i] = Vector3.zero;
            deltaTangents[i] = Vector3.zero;
        }

        float frameWeight = 100.0f;
        mesh.AddBlendShapeFrame(targetShapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);

        return true;
    }

    private static string V3ToString(Vector3 v)
    {
        return "(" + v.x + "," + v.y + "," + v.z + ")";
    }
}
