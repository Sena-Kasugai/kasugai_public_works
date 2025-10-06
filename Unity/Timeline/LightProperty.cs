using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VLB;

[ExecuteInEditMode]
public class LightProperty : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Light[] lightParam;
    [Space(20)]

    [SerializeField] private Color color;
    [SerializeField] private float intensity;

    public Color Color
    {
        get => color;
        set => color = value;
    }

    public float Intensity
    {
        get => intensity;
        set => intensity = value;
    }

    private Color prevColor;
    private float prevIntensity;

    private MaterialPropertyBlock block;
    public MaterialPropertyBlock Block
    { 
        get
        {
            if(block.IsUnityNull()) block = new MaterialPropertyBlock();
            return block;
        }
        set
        {
            block = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        InitLightProperty();
    }

    private void InitLightProperty()
    {
        block = new MaterialPropertyBlock();
    }
    
    // Update is called once per frame
    private void LateUpdate() {
        LightUpdate();
        
    }

    private void LightUpdate()
    {
        if (!color.Equals(prevColor) || !intensity.Equals(prevIntensity))
        {
            var setColor = color * intensity;
            foreach (Renderer renderer in renderers)
            {
                if(renderer == null) continue;
                renderer.GetPropertyBlock(Block);
                Block.SetColor("_Color", setColor);
                Block.SetColor("_UnlitColor", setColor);
                Block.SetColor("_EmissionColor", setColor);
                Block.SetColor("_BaseColor", setColor);
                renderer.SetPropertyBlock(Block);
            }
            if(lightParam != null)
            {
                foreach(Light light in lightParam)
                {
                    if(light == null) continue;
                    light.color = setColor;
                }
            }
            prevColor = setColor;
            prevIntensity = intensity;
        }
    }
}
