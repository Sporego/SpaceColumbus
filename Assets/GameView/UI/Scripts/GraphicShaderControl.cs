using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public abstract class GraphicShaderControl : MonoBehaviour
{
    public Material Material;

    protected Image graphic;

    List<ShaderControlModifier> modifiers;

    protected virtual void Awake()
    {
        this.modifiers = new List<ShaderControlModifier>();
    }

    public void AddModifier(ShaderControlModifier modifier)
    {
        this.modifiers.Add(modifier);
    }

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void OnValidate()
    {
        Awake();
        Start();
    }

    public virtual void Initialize()
    {
        if (Material == null)
            Debug.Log(this.name + " has no Material assigned.");

        graphic = this.GetComponent<Image>();
        graphic.material = Instantiate(Material);

        //Debug.Log("Shader control has " + modifiers.Count + " modifiers.");
        foreach (var modifier in modifiers)
            modifier.ApplyModifier(this);
    }

    public void SetInt(string name, int value) { graphic.material.SetInt(name, value); }
    public void SetFloat(string name, float value) { graphic.material.SetFloat(name, value); }
    public void SetTexture(string name, Texture value) { graphic.material.SetTexture(name, value); }
    public void SetColor(string name, Color value) { graphic.material.SetColor(name, value); }
    public void SetBool(string name, bool value) { SetFloat(name, value ? 1 : 0); }
    public void SetVector(string name, Vector4 value) { graphic.material.SetVector(name, value); }
    public void SetVector(string name, Vector3 value) { SetVector(name, new Vector4(value.x, value.y, value.z, 0)); }
    public void SetVector(string name, Vector2 value) { SetVector(name, new Vector4(value.x, value.y, 0, 0)); }
}

// decorator
[System.Serializable]
public abstract class ShaderControlModifier
{
    public abstract void ApplyModifier(GraphicShaderControl shaderControl);
}
