using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillKey_key : MonoBehaviour
{
    public RenderTexture _renderTextureSrc;
    public RenderTexture _renderTextureDst;
    public Material _material;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // カメラのレンダーターゲットを解除  
        //Graphics.SetRenderTarget(null);

        // nullは画面をレンダリング対象とすることを意味する
        // 画面にBlitする場合
        Graphics.Blit(_renderTextureSrc, _renderTextureDst, _material);
    }

    void OnPostRender()
    {    
    }
}
