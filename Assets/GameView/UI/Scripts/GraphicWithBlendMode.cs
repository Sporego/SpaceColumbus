using UnityEngine;

[System.Serializable]
public class GraphicWithBlendModeModifier : ShaderControlModifier
{
    private static string SrcBlendField = "_SrcBlend";
    private static string DstBlendField = "_DstBlend";

    public UnityEngine.Rendering.BlendMode SrcBlend = UnityEngine.Rendering.BlendMode.SrcAlpha;
    public UnityEngine.Rendering.BlendMode DstBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;

    override public void ApplyModifier(GraphicShaderControl shaderControl)
    {
        shaderControl.SetInt(SrcBlendField, (int)SrcBlend);
        shaderControl.SetInt(DstBlendField, (int)DstBlend);
    }
}
