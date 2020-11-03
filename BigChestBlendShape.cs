using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigChestBlendShape : MonoBehaviour
{
    public bool addNow;
    public Mesh mesh;
    public string targetShapeName;
    public float waistY;
    public float shoulderX;
    public float shoulderY;
    public float neckY;

    private void Initialize()
    {
        mesh = transform.Find("Body").GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }

    void OnValidate()
    {
        if (addNow)
        {
            addNow = false;
            Initialize();
            AddChestExpanderBlendShape();
        }
    }

    private bool AddChestExpanderBlendShape()
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

            float offset = shoulderX / 2f;
            float slopeDown = shoulderX * 2.5f;
            float outerStartScale = offset * (shoulderX + slopeDown) / slopeDown;

            Vector3 blend;
            if (InRegion("li", v, 0f, shoulderX, waistY, shoulderY, offset, 1f, 1f, 0f, 1f, out blend) // Lower inner
                || InRegion("ui", v, 0f, shoulderX, shoulderY, neckY, offset, 1f, 1f, 1f, 0f, out blend) // Upper inner
                || InRegion("lo", v, shoulderX, shoulderX * 1.5f, waistY, shoulderY, offset, 1f, 0f, 0f, 1f, out blend) // Lower outer
                || InRegion("uo", v, shoulderX, shoulderX * 1.5f, shoulderY, neckY, offset, 1f, 0f, 1f, 0f, out blend)) // Upper outer
            {
                deltaVertices[i] = blend;
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


    /*
     * Check if the vertex is within the specified Y range (e.g. from waist up to shoulder, or shoulder up to neck).
     * If so, work out how far from the center (X=0,Z=0) then scale
     */
    private bool InRegion(string which, Vector3 v, float innerRadius, float outerRadius, float lowerY, float upperY, float offset, float innerScale, float outerScale, float lowerScale, float upperScale, out Vector3 blend)
    {
        blend = Vector3.zero;

        // If the Y value is out of range, skip it.
        if (v.y < lowerY || v.y > upperY)
        {
            //Debug.Log(which + " " + V3ToString(v) + " Y-SKIP");
            return false;
        }

        // Work out distanced from center (X=0,Z=0) to vertex.
        float radiusOfV = new Vector2(v.x, v.z).magnitude;
        if (radiusOfV <= innerRadius || radiusOfV > outerRadius)
        {
            //Debug.Log(which + " " + V3ToString(v) + " R-SKIP");
            return false;
        }

        //Debug.Log(which + " " + V3ToString(v) + " USE");

        // For vertex Y value, work out how far to the edge of the area we cover.
        float scaleForR = innerScale + ((radiusOfV - innerRadius) / (outerRadius - innerRadius)) * (outerScale - innerScale);
        if (scaleForR > 1.0f)
        {
            Debug.Log("!!! " + scaleForR + " is=" + innerScale + " os=" + outerScale + " rOfV=" + radiusOfV + " ir=" + innerRadius + " v=" + V3ToString(v));
        }

        // For vertex Y value, work out much we are to offset the blend by
        float scaleForY = lowerScale + ((v.y - lowerY) / (upperY - lowerY)) * (upperScale - lowerScale);

        float delta = offset * scaleForR * scaleForY;

        // Use original vertex (X,Z) to computed blend size
        blend = new Vector3(delta * (v.x / radiusOfV), 0f, delta * (v.z / radiusOfV));

        return true;
    }

    private static string V3ToString(Vector3 v)
    {
        return "(" + v.x + "," + v.y + "," + v.z + ")";
    }
}
