Shader "Beautify/Beautify" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OverlayTex ("Overlay (RGB)", 2D) = "black" {}
		_ScreenLum ("Screen Luminance", 2D) = "white" {}
		_BloomTex ("Bloom Luminance", 2D) = "black" {}
		_DoFTex ("Blur (RGB)", 2D) = "black" {}
		_Sharpen ("Sharpen Data", Vector) = (2.5, 0.035, 0.5)
		_ColorBoost ("Color Boost Data", Vector) = (1.1, 1.1, 0.08, 0)
		_Dither ("Dither Data", Vector) = (5, 0, 0, 1.0)
		_FXColor ("FXColor Color", Color) = (1,1,1,0)
		_Vignetting ("Vignetting", Color) = (0.3,0.3,0.3,0.05)
		_VignettingAspectRatio ("Vignetting Aspect Ratio", Float) = 1.0
		_Frame("Frame Data", Vector) = (50,50,50,0)
		_Outline("Outline", Color) = (0,0,0,0.8)
		_Dirt("Dirt Data", Vector) = (0.5,0.5,0.5,0.5)
		_Bloom("Bloom Data", Vector) = (0.5,0,0)
		_BloomWeights("Bloom Weights", Vector) = (0.35,0.55,0.7,0.8)
		_BloomWeights2("Bloom Weights 2", Vector) = (0.35,0.55,0.7,0.8)
		_CompareParams("Compare Params", Vector) = (0.785398175, 0.001, 0, 0)
		_AFTint ("Anamorphic Flares Tint", Color) = (1,1,1,0.5)
		_BokehData("Bokeh Data", Vector) = (10,1,0,1)
		_BokehData2("Bokeh Data 2", Vector) = (1000.0, 4, 0, 0)
	}

Subshader {	

  Pass { // 0
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCompare
      #pragma target 3.0
      #pragma multi_compile __ DALTONIZE SEPIA TINT NIGHT_VISION THERMAL_VISION
	  #pragma multi_compile __ DEPTH_OF_FIELD
	  #pragma multi_compile __ OUTLINE
	  #pragma multi_compile __ DIRT
	  #pragma multi_compile __ BLOOM
	  #pragma multi_compile __ UNITY_COLORSPACE_GAMMA	  
      #include "Beautify.cginc"
      ENDCG
  }
 
  Pass { // 1
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBeautify
      #pragma target 3.0
      #pragma multi_compile __ DALTONIZE SEPIA TINT NIGHT_VISION THERMAL_VISION
	  #pragma multi_compile __ DEPTH_OF_FIELD
	  #pragma multi_compile __ OUTLINE
	  #pragma multi_compile __ DIRT
	  #pragma multi_compile __ BLOOM
	  #pragma multi_compile __ UNITY_COLORSPACE_GAMMA	  
      #include "Beautify.cginc"
      ENDCG
  }
  
  Pass { // 2
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragLum
      #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma multi_compile __ UNITY_COLORSPACE_GAMMA	  
      #include "BeautifyLum.cginc"
      ENDCG
  }  
  
  
  Pass { // 3
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragDebugBloom
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  }   
  
  Pass { // 4
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertBlurH
      #pragma fragment fragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  }    
  
      
  Pass { // 5
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertBlurV
      #pragma fragment fragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  }    

            
  Pass { // 6
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBloomCompose
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  }   
  
  Pass { // 7
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResample
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  }   
    
  Pass { // 8
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  Blend One One
	  
      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResample
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  } 


  Pass { // 9
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  
      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragLumAntiflicker
      #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma multi_compile __ UNITY_COLORSPACE_GAMMA	  
      #include "BeautifyLum.cginc"
      ENDCG
  }  

  Pass { // 10
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  Blend One One

      CGPROGRAM
      #pragma vertex vertCross
      #pragma fragment fragResampleAF
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  } 

  Pass { // 11
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }
	  Blend One One

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCopy
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyLum.cginc"
      ENDCG
  } 

  Pass { // 12 DoF CoC
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCoC
      #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma multi_compile __ UNITY_COLORSPACE_GAMMA	  
      #include "BeautifyDoF.cginc"
      ENDCG
  } 
 
  Pass { // 13 DoF CoC Debug
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragCoCDebug
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyDoF.cginc"
      ENDCG
  } 
 
  Pass { // 14 DoF Blur
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode Off }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragBlur
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "BeautifyDoF.cginc"
      ENDCG
  }    

}
FallBack Off
}
