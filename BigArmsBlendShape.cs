using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigArmsBlendShape : MonoBehaviour
{
    public bool computeNow;
    public Mesh mesh;
    public Vector3 leftShoulder;
    public Vector3 rightShoulder;
    public float leftShoulderX;
    public float leftWristX;
    public float rightShoulderX;
    public float rightWristX;
    public float leftArmLength;
    public float rightArmLength;
    public float armY;
    public float armZ;
    public float minX;
    public float maxX;

    public bool addNow;
    public string targetShapeName = "ArmMuscles1";
    public float radius = 0.1f;

    // Called when the user changes something in the inpector panel.
    // This script estimates where things are for the character.
    void OnValidate()
    {
        // "Compute Now" is a checkbox. If clicked, the following is executed.
        // The idea is to find the important bones in the character, then work out the size of the arm.
        if (computeNow)
        {
            computeNow = false;
            mesh = transform.Find("Body").GetComponent<SkinnedMeshRenderer>().sharedMesh;
            leftShoulder = transform.Find("Root/J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm").position;
            rightShoulder = transform.Find("Root/J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm").position;
            leftShoulderX = leftShoulder.x;
            leftWristX = leftShoulder.x - 0.6f;
            rightShoulderX = rightShoulder.x;
            rightWristX = rightShoulderX + 0.6f;
            armY = leftShoulder.y;
            armZ = leftShoulder.z;

            ComputeAverageYandZ();
            float handLength = (leftShoulderX - minX) / 4.0f;
            leftWristX = minX + handLength;
            rightWristX = maxX - handLength;
            leftArmLength = leftShoulderX - leftWristX;
            rightArmLength = rightWristX - rightShoulderX;
        }

        // After reviewing the computed values above, the user can click "Add Now",
        // or they can manually tweak the results of the compute phase before clicking
        // "Add Now".
        if (addNow)
        {
            addNow = false;
            AddMissingBlendShapes();
        }
    }

    // Computer the average Y and Z values of the arm. Any vertex to the right of the right shoulder is considered a part of the arm.
    private void ComputeAverageYandZ()
    {
        Vector3[] verteces = mesh.vertices;
        minX = 0.0f;
        maxX = 0.0f;
        float totalY = 0.0f;
        float totalZ = 0.0f;
        int count = 0;
        foreach (Vector3 v in verteces)
        {
            if (v.x >= rightShoulderX + 0.1f)
            {
                totalY += v.y;
                totalZ += v.z;
                count++;
            }
            if (v.x > maxX)
            {
                maxX = v.x;
            }
            if (v.x < minX)
            {
                minX = v.x;
            }
        }
        armY = totalY / (float)count;
        armZ = totalZ / (float)count;
    }

    // Add the default set of extra blend shapes.
    void AddMissingBlendShapes()
    {
        mesh = transform.Find("Body").GetComponent<SkinnedMeshRenderer>().sharedMesh;
        AddArmBlendshape();
    }

    // Create a new blendshape which uses the centroid of the arm (it assumes the arm 
    // is pointing horizontally sideways) to push everything outwards a little.
    private bool AddArmBlendshape()
    {
        if (mesh.GetBlendShapeIndex(targetShapeName) >= 0)
        {
            // Blend shape already exists. Don't try to add it again.
            return false;
        }

        Vector3[] deltaVertices = new Vector3[mesh.vertexCount];
        Vector3[] deltaNormals = new Vector3[mesh.vertexCount];
        Vector3[] deltaTangents = new Vector3[mesh.vertexCount];

        Vector3[] verteces = mesh.vertices;

        // Find the source shape index.
        float frameWeight = 1.0f;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = verteces[i];

            if (Between(v.x, leftWristX, leftShoulderX)
                && Between(v.y, armY - radius, armY + radius)
                && Between(v.z, armZ - radius, armZ + radius))
            {
                deltaVertices[i] = Expand(v, leftShoulderX - v.x, leftArmLength);
            }
            else if (Between(v.x, rightShoulderX, rightWristX)
                && Between(v.y, armY - radius, armY + radius)
                && Between(v.z, armZ - radius, armZ + radius))
            {
                deltaVertices[i] = Expand(v, v.x - rightShoulderX, rightArmLength);
            }
            else
            {
                deltaVertices[i] = Vector3.zero;
            }

            deltaNormals[i] = Vector3.zero;
            deltaTangents[i] = Vector3.zero;
        }

        mesh.AddBlendShapeFrame(targetShapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);

        return true;
    }

    private bool Between(float value, float min, float max)
    {
        return value >= min && value <= max;
    }

    // Take a vertex and work out a blendshape to extend it, nothing at the shoulder and wrist,
    // but increasing in the middle of the arm.
    private Vector3 Expand(Vector3 vertex, float howFarDownArm, float armLength)
    {
        //Debug.Log(" armY=" + armY + "  armZ= " + armZ + "  howFarDownArm=" + howFarDownArm + " len=" + armLength + "  v = " + V3ToString(vertex));

        float scale = 1.0f;
        float slopeLength = armLength / 4.0f;
        if (howFarDownArm < slopeLength)
        {
            scale = howFarDownArm / slopeLength;
        }
        else if (howFarDownArm > armLength - slopeLength)
        {
            scale = (armLength - howFarDownArm) / slopeLength;
        }

        float dy = vertex.y - armY;
        float dz = vertex.z - armZ;
        
        float newDy = (1.0f - Mathf.Pow(1.0f - (Mathf.Abs(dy) / radius), 2.0f)) * radius;
        if (newDy < 0.0f) newDy = 0.0f;
        if (dy < 0.0f) newDy = -newDy;
        float newDz = (1.0f - Mathf.Pow(1.0f - (Mathf.Abs(dz) / radius), 2.0f)) * radius;
        if (newDz < 0.0f) newDz = 0.0f;
        if (dz < 0.0f) newDz = -newDz;

        Vector3 resp = new Vector3(0.0f, (newDy - dy) * scale, (newDz - dz) * scale);
        //Debug.Log("EXPAND " + V3ToString(vertex) + " -> " + V3ToString(resp) + "  dy=" + dy + " 1-dy/r=" + (1.0f - Mathf.Abs(dy) / radius) + " cubed=" + Mathf.Pow(1.0f - Mathf.Abs(dy) / radius, 3.0f));
        return resp;
    }

    // Debugging subpport.
    private static string V3ToString(Vector3 v)
    {
        return "(" + v.x + "," + v.y + "," + v.z + ")";
    }
}
