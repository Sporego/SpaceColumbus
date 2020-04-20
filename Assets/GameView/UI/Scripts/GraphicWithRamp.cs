using UnityEngine;

public class GraphicWithRamp : GraphicShaderControl
{
    public GraphicWithRampModifier Ramp = new GraphicWithRampModifier();
    public GraphicWithDitherModifier Dither = new GraphicWithDitherModifier();
    public GraphicWithBlendModeModifier Blend = new GraphicWithBlendModeModifier();

    override public void Initialize()
    {
        this.AddModifier(new GraphicWithRectModifier());
        this.AddModifier(Blend);
        this.AddModifier(Ramp);
        this.AddModifier(Dither);
        base.Initialize();
    }
}

[System.Serializable]
public class GraphicWithRampModifier : ShaderControlModifier
{
    private static string RampPowerField = "_RampPower";
    private static string RampScaleField = "_RampScale";
    private static string RampDirectionField = "_RampDirection";
    private static string RadialField = "_Radial";
    private static string InvertField = "_Invert";
    private static string AlphaRampField = "_ApplyAlpha";
    private static string Color1Field = "_Color1";
    private static string Color2Field = "_Color2";

    public Vector2 RampDirection = new Vector2(1, 0);

    public Color Color1 = new Color(1, 1, 1, 1);
    public Color Color2 = new Color(1, 1, 1, 1);

    [Range(0.01f, 10)] public float Power = 1;
    [Range(0.01f, 10)] public float Scale = 1;

    public bool Radial = true;
    public bool Invert = false;
    public bool ApplyAlpha = true;

    override public void ApplyModifier(GraphicShaderControl shaderControl)
    {
        shaderControl.SetColor(Color1Field, Color1);
        shaderControl.SetColor(Color2Field, Color2);
        shaderControl.SetFloat(RampPowerField, Power);
        shaderControl.SetFloat(RampScaleField, Scale);
        shaderControl.SetBool(RadialField, Radial);
        shaderControl.SetBool(InvertField, Invert);
        shaderControl.SetBool(AlphaRampField, ApplyAlpha);
        shaderControl.SetVector(RampDirectionField, RampDirection.normalized);
    }
}
