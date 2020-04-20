using UnityEngine;

[System.Serializable]
public class GraphicWithDitherModifier : ShaderControlModifier
{
    private static string ApplyDitherField = "_ApplyDither";
    private static string DitherStrengthField = "_DitherStrength";

    public static bool GlobalAllowDither = true;

    public bool ApplyDither = false;
    [Range(1, 1024)] public float DitherStrengthInverse = 128;

    override public void ApplyModifier(GraphicShaderControl shaderControl)
    {
        shaderControl.SetBool(ApplyDitherField, ApplyDither && GlobalAllowDither);
        shaderControl.SetFloat(DitherStrengthField, 1f / DitherStrengthInverse);
    }
}