using UnityEngine;

public class GraphicWithBorder : GraphicShaderControl
{
    public GraphicWithMaskModifier Mask = new GraphicWithMaskModifier();
    public GraphicWithBorderModifier Border = new GraphicWithBorderModifier();
    public GraphicWithDitherModifier Dither = new GraphicWithDitherModifier();
    public GraphicWithBlendModeModifier Blend = new GraphicWithBlendModeModifier();

    override public void Initialize()
    {
        this.AddModifier(new GraphicWithRectModifier());
        this.AddModifier(Blend);
        this.AddModifier(Mask);
        this.AddModifier(Border);
        this.AddModifier(Dither);
        base.Initialize();
    }
}

[System.Serializable]
public class GraphicWithBorderModifier : ShaderControlModifier
{
    private static string BorderSizeField = "_BorderSize";
    private static string BorderThicknessField = "_BorderThickness";
    private static string BorderColorField = "_BorderColor";
    private static string RenderBorderOnlyField = "_BorderOnly";

    [Range(0, 1000)] public int BorderSize = 5;
    [Range(0, 1000)] public int BorderThickness = 0;

    public Color BorderColor = new Color(1, 1, 1, 1);

    public bool RenderBorderOnly = true;

    override public void ApplyModifier(GraphicShaderControl shaderControl)
    {
        shaderControl.SetFloat(BorderSizeField, BorderSize);
        shaderControl.SetFloat(BorderThicknessField, BorderThickness);
        shaderControl.SetColor(BorderColorField, BorderColor);
        shaderControl.SetBool(RenderBorderOnlyField, RenderBorderOnly);
    }
}

