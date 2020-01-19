// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|custl-5334-OUT;n:type:ShaderForge.SFN_Fresnel,id:9520,x:32686,y:32884,varname:node_9520,prsc:2|NRM-3385-OUT,EXP-2675-OUT;n:type:ShaderForge.SFN_Color,id:5051,x:32674,y:33119,ptovrint:False,ptlb:node_5051,ptin:_node_5051,varname:node_5051,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3014706,c2:1,c3:0.9691825,c4:1;n:type:ShaderForge.SFN_NormalVector,id:3385,x:32421,y:32822,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:2675,x:32289,y:33037,ptovrint:False,ptlb:node_2675,ptin:_node_2675,varname:node_2675,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.213583,max:10;n:type:ShaderForge.SFN_Multiply,id:5334,x:32876,y:32982,varname:node_5334,prsc:2|A-9520-OUT,B-5051-RGB,C-3534-OUT;n:type:ShaderForge.SFN_Slider,id:3534,x:32569,y:33316,ptovrint:False,ptlb:node_3534,ptin:_node_3534,varname:node_3534,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.9881258,max:1;proporder:5051-2675-3534;pass:END;sub:END;*/

Shader "Shader Forge/Holo_face" {
    Properties {
        _node_5051 ("node_5051", Color) = (0.3014706,1,0.9691825,1)
        _node_2675 ("node_2675", Range(0, 10)) = 4.213583
        _node_3534 ("node_3534", Range(0, 1)) = 0.9881258
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            //#pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _node_5051;
            uniform float _node_2675;
            uniform float _node_3534;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
                float3 node_5334 = (pow(1.0-max(0,dot(i.normalDir, viewDirection)),_node_2675)*_node_5051.rgb*_node_3534);
                float3 finalColor = node_5334;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
