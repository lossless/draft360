using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picture : Snapshot
{
    [SerializeField] Renderer quadRenderer;

    public void SetQuadTexture(Texture2D _texture)
    {
        quadRenderer.material.mainTexture = _texture;
    }
}
