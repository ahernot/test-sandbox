# test-sandbox

## To do
* Add texture mapping based on slope, height, vertex painting
* Issues: noticeable seams in Perlin noise (visible when high vertex count)


Shaders & materials: https://forum.unity.com/threads/shaders-vs-materials.628672/

Texture help:
* https://forum.unity.com/threads/shaders-vs-materials.628672/
* https://answers.unity.com/questions/12835/how-to-automatically-apply-different-textures-on-t.html
* https://www.red-gate.com/simple-talk/dotnet/c-programming/creating-a-shader-in-unity/
* Blending (example): https://forum.unity.com/threads/blend-between-textures-based-on-height.210221/
* Get angle: https://answers.unity.com/questions/1682486/how-do-i-texture-a-procedural-mesh-based-on-its-an.html

Lighting seams: https://forum.unity.com/threads/lighting-seam-on-tileable-mesh.533407/
Free sand texture: https://3dtextures.me/2020/02/14/sand-005/


Make a HeightMap class
* with chunk management
* with multiple Noise layers for seafloor, hills, cliffs
* with constant roughness regardless of mesh size (not more granularity of closer-spaced vertices)
* Create chunk noise manager which normalises noise value

<br><br>

# Packages
* Shader Graph 7.5.3
* ProBuilder 4.4.0

<br><br>

# Assets
For more information on materials & shaders: <a href="https://docs.unity3d.com/Manual/StandardShaderMaterialParameters.html" target="_blank">link</a>
* Sand 005 SD (`Assets/Resources/Objects/Sand_005_SD` — 1024px)
  * albedo map (`Sand_005_baseColor.jpg`)
  * normal map (`Sand_005_normal.jpg`)
  * height map (`Sand_005_height`)
  * occlusion map (`Sand_005_ambientOcclusion.jpg`)
  * smoothness map (`Sand_005_roughness.jpg`)

* Cliff Rock Two (`Assets/Resources/Objects/Cliff Rock Two` – 4096px) – from <a href="https://www.cgtrader.com/free-3d-models/exterior/other/cliff-rock-two" target="_blank">link</a>
  * albedo map (`Cliff_Rock_Two_BaseColor.png`)
  * normal map (`Cliff_Rock_Two_Normal.png`)
  * height map (`Cliff_Rock_Two_Height.png`)
  * occlusion map (`Cliff_Rock_Two_AO.png`)
  * smoothness map (`Cliff_Rock_Two_Roughness.png`)
  * metallic map (`Cliff_Rock_Two_Metallic.png`)
