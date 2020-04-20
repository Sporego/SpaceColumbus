using UnityEngine;

[System.Serializable]
public class GraphicWithMaskModifier : ShaderControlModifier
{
    private static string MaskField = "_MaskTex";
    private static string MaskWeightField = "_MaskWeight";

    public Texture2D Mask;
    [Range(0, 1)] public float MaskWeight = 0.1f;

    override public void ApplyModifier(GraphicShaderControl shaderControl)
    {
        shaderControl.SetTexture(MaskField, Mask);
        shaderControl.SetFloat(MaskWeightField, MaskWeight);
    }
}
