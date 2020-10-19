using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * We hard code the possible textures here because it makes them easier to animate.
 * This script assumes it is attached to the Face object of a VRoid character.
 */
public class FaceTextureBlend : MonoBehaviour
{
    public Material faceSkinMaterial; // E.g. "F00_000_00_Face_00_SKIN"
    public Texture blushTexture;
    [Range(0.0f, 1.0f)] public float blushLerp = 0.0f;
    public Texture sweatTexture;
    [Range(0.0f, 1.0f)] public float sweatLerp = 0.0f;

    private Texture lastTexture = null;
    private float lastBlushLerp = 0.0f;
    private float lastSweatLerp = 0.0f;

    void OnValidate()
    {
        // Cannot look up materials in validate function without causing "material leak" messages.
        UpdateTexture();
    }

    private void Update()
    {
        if (faceSkinMaterial == null)
        {
            // VRoid characters have skin at index 7
            faceSkinMaterial = GetComponent<SkinnedMeshRenderer>().materials[7]; 
        }
        UpdateTexture();
    }

    // Update is called once per frame
    private void UpdateTexture()
    {
        if (faceSkinMaterial == null)
        {
            return;
        }
        if (blushTexture != null && blushLerp != lastBlushLerp)
        {
            SetTexture(blushTexture, blushLerp);
            lastBlushLerp = blushLerp;
            lastSweatLerp = sweatLerp = 0.0f;
        }
        else if (sweatTexture != null && sweatLerp != lastSweatLerp)
        {
            SetTexture(sweatTexture, sweatLerp);
            lastSweatLerp = sweatLerp;
            lastBlushLerp = blushLerp = 0.0f;
        }
    }

    private void SetTexture(Texture texture, float lerp)
    {
        // Change the texture efficently
        if (texture != lastTexture)
        {
            lastTexture = texture;
            faceSkinMaterial.SetTexture("_SecondaryTex", texture);
        }
        faceSkinMaterial.SetFloat("_TextureLerp", lerp);
    }
}
