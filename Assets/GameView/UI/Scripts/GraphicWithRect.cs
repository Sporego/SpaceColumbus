using UnityEngine;

public class GraphicWithRectModifier : ShaderControlModifier
{
    private static string SizeXField = "_SizeX";
    private static string SizeYField = "_SizeY";

    protected Rect rect;

    override public void ApplyModifier(GraphicShaderControl shaderControl)
    {
        rect = shaderControl.GetComponent<RectTransform>().rect;
        shaderControl.SetFloat(SizeXField, rect.width);
        shaderControl.SetFloat(SizeYField, rect.height);
    }
}
