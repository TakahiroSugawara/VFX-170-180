using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapManager : MonoBehaviour
{
    public float HapLifeTime = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DestroyMyself), HapLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetBlendMode(this.gameObject.GetComponent<MeshRenderer>().material, Mode.Opaque);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            SetBlendMode(this.gameObject.GetComponent<MeshRenderer>().material, Mode.Cutout);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            SetBlendMode(this.gameObject.GetComponent<MeshRenderer>().material, Mode.Fade);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            SetBlendMode(this.gameObject.GetComponent<MeshRenderer>().material, Mode.Transparent);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            this.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_Alphathreshold",0.02f);
        }
    }

    void DestroyMyself()
    {
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        Destroy(this.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
        Destroy(this.gameObject.GetComponent<MeshRenderer>().material);
    }

    public enum Mode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public static void SetBlendMode(Material material, Mode blendMode)
    {
        material.SetFloat("_Mode", (float)blendMode);  // <= ‚±‚ê‚ª•K—v

        switch (blendMode)
        {
            case Mode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case Mode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case Mode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case Mode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}
