using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class GaussianBlurImage : MonoBehaviour
{
    //public ComputeShader shader;

    public Material mat;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, mat);
    }
}
