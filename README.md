# unity-vroid-scripts

* CopyBlendShapes.cs - Simple code showing how to read and write blend shape definitions (does a verbatim copy).
* CleveryBlendCopier.cs - Experimental Unity C# script to copy blend shapes from one VRoid characte Face to another, even if the meshes have been modified. (It does some expressions okay, but fails around the mouth.) See also [this blog](https://extra-ordinary.tv/2020/09/07/copying-blendshapes-in-unity-with-a-script/)
* AddIFacialMocapBlendShapes.cs - Experimental Unity C# script to generate blend shapes needed by ARKit (and ifacialmocap.com) from the default blend shapes from VRoid Studio (no use of Blender or other tools is required). Takes 5 to 10 minutes to run. See also [this blog](https://extra-ordinary.tv/2020/10/04/generating-ifacialmocap-blend-shapes-in-unity-for-vroid-characters/).
* FaceTextureBlend.cs - Simple example of a script to update a modified VRM/MToon shader that has a "MainTex" and "SecondaryTex" texture, with a "TextureLerp" float where 0 = main texture, and 1 = secondary texture (so can animate blending between the two textures for blushing etc).
* ElectricScooterLocomotion.cs - First cut at a script for a VRoid character to ride an electric scooter (free asset from Unity store). ([blog](https://extra-ordinary.tv/2020/10/30/riding-a-scooter-in-unity-with-vroid-characters/))
