Shader "Invertex/Environment/BlendLayersExample"
{
    Properties
    {
        _Metalness ("Metalness", Range(0, 1)) = 0.1
        _Smoothness ("Smoothness", Range(0, 1)) = 0.1
        _MainTex ("Base Texture", 2D) = "white" {}
        _BaseHeight ("Base Height", Range(-50, 50)) = 0
        _BlendSharpness ("Blend Sharpness", Range(0.06, 4)) = 0.75
 
        [Space(30)]
            _Layer1 ("Layer 1", 2D) = "white" {}
            _Layer1Height ("Layer 1 Height", Range(0, 20)) = 1
        [Space(20)]
            _Layer2 ("Layer 2", 2D) = "white" {}
            _Layer2Height ("Layer 2 Height", Range(0, 20)) = 1
        [Space(20)]
            _Layer3 ("Layer 3", 2D) = "white" {}
            _Layer3Height("Layer 3 Height", Range(0, 20)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0
 
        float _Metalness;
        float _Smoothness;
 
        UNITY_DECLARE_TEX2D(_MainTex);
        float _BaseHeight;
        float _BlendSharpness;
 
        UNITY_DECLARE_TEX2D(_Layer1);
        float _Layer1Height;
 
        UNITY_DECLARE_TEX2D(_Layer2);
        float _Layer2Height;
 
        UNITY_DECLARE_TEX2D(_Layer3);
        float _Layer3Height;
 
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Layer1;
            float2 uv_Layer2;
            float2 uv_Layer3;
            float3 worldPos; //Unity will automatically fill this with vertex world position when using Surface shaders
        };
        //Custom struct so we can return multiple values from a method, allowing us to make cleaner code
        struct blendingData
        {
            float height;
            float4 result;
        };
 
        blendingData BlendLayer(float4 layer, float layerHeight, blendingData bd)
        {
            //Remove this layer's height from the total height so our next layer buids from it, but don't let it go below 0
            bd.height = max(0, bd.height - layerHeight);
            //Clamp our height to 1 or less and drive the lerp by it. If the height was already below 1, then our contribution will  be less
            //giving a smoother blend the closer it is to 0.
            float t = min(1, bd.height * _BlendSharpness);
            //Blend (lerp) from our previous layer result using the previous value
            bd.result = lerp(bd.result, layer, t);
            return bd; //Return our blendingData struct that contains the result and newly adjusted height, so we can use it again
        }
 
        void surf (Input i, inout SurfaceOutputStandard o)
        {
            blendingData bdata;
                bdata.height = i.worldPos.y - _BaseHeight;
                bdata.result = UNITY_SAMPLE_TEX2D(_MainTex, i.uv_MainTex);
                float4 layer1 = UNITY_SAMPLE_TEX2D(_Layer1, i.uv_Layer1);
                float4 layer2 = UNITY_SAMPLE_TEX2D(_Layer2, i.uv_Layer2);
                float4 layer3 = UNITY_SAMPLE_TEX2D(_Layer3, i.uv_Layer3);
 
                bdata = BlendLayer(layer1, _Layer1Height, bdata);
                bdata = BlendLayer(layer2, _Layer2Height, bdata);
                bdata = BlendLayer(layer3, _Layer3Height, bdata);
                //And can keep adding layers as long as you have enough samplers... Or you could use a texture array and a loop for some real craziness :)
 
            o.Albedo = bdata.result;
            o.Metallic = _Metalness;
            o.Smoothness = _Smoothness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
