using UnityEngine;

public class GraphicWithLine : GraphicShaderControl
{
    public GraphicWithMaskModifier Mask = new GraphicWithMaskModifier();
    public GraphicWithLineModifier Line = new GraphicWithLineModifier();
    public GraphicWithDitherModifier Dither = new GraphicWithDitherModifier();
    public GraphicWithBlendModeModifier Blend = new GraphicWithBlendModeModifier();

    override public void Initialize()
    {
        this.AddModifier(new GraphicWithRectModifier());
        this.AddModifier(Blend);
        this.AddModifier(Mask);
        this.AddModifier(Line);
        this.AddModifier(Dither);
        base.Initialize();
    }
}

[System.Serializable]
public class GraphicWithLineModifier : ShaderControlModifier
{
    private static string LineDirectionField = "_LineDirection";
    private static string LineSizeField = "_LineSize";
    private static string LineThicknessField = "_LineThickness";
    private static string LineColorField = "_LineColor";
    private static string ApplyRepeatField = "_ApplyRepeat";
    private static string RepeatFrequencyField = "_RepeatFrequency";

    [Range(0, 360)] public float LineAngle = 0;
    [Range(0, 1000)] public int LineSize = 5;
    [Range(0, 1000)] public int LineThickness = 0;

    public Color LineColor = new Color(1, 1, 1, 1);

    public bool ApplyRepeat = false;
    [Range(0, 1000)] public int RepeatDistance = 1;

    override public void ApplyModifier(GraphicShaderControl shaderControl)
    {
        float angle = Mathf.Deg2Rad * LineAngle;
        Vector2 LineDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // equation: a*x + b*y + c = 0, where b = -1
        float a = LineDirection.x == 0 ? 100000f: LineDirection.y / LineDirection.x;
        float c = 0.5f - 0.5f * a;
        float d = Mathf.Sqrt(a * a + c * c);
        Vector3 lineDirection = new Vector3(a, c, d);

        shaderControl.SetVector(LineDirectionField, lineDirection);
        shaderControl.SetFloat(LineSizeField, LineSize);
        shaderControl.SetFloat(LineThicknessField, LineThickness);
        shaderControl.SetColor(LineColorField, LineColor);

        shaderControl.SetBool(ApplyRepeatField, ApplyRepeat);
        shaderControl.SetFloat(RepeatFrequencyField, RepeatDistance + 1);
    }
}

