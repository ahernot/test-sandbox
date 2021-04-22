# test-sandbox
Unity 2019.4.18f1

<a rel="license" href="http://creativecommons.org/licenses/by-nc-nd/4.0/"><img alt="Creative Commons Licence" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-nd/4.0/80x15.png" /></a><br />This work is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-nc-nd/4.0/">Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License</a>.

<br><br>

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

* Refine noise layers in editor, with support for a mountain layer and a general terrain layer

<br><br>

## Packages
* Shader Graph 7.5.3
* ProBuilder 4.4.0

<br><br>

## Assets
For more information on materials & shaders: <a href="https://docs.unity3d.com/Manual/StandardShaderMaterialParameters.html" target="_blank">link</a>
* Sand 005 SD (`Assets/Resources/Objects/Sand_005_SD` – 1024px)
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

* Tree coral – from <a href="https://free3d.com/3d-model/tree-coral-v2--625204.html" target="_blank">link</a>
