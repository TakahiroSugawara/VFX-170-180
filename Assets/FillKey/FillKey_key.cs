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
        // �J�����̃����_�[�^�[�Q�b�g������  
        //Graphics.SetRenderTarget(null);

        // null�͉�ʂ������_�����O�ΏۂƂ��邱�Ƃ��Ӗ�����
        // ��ʂ�Blit����ꍇ
        Graphics.Blit(_renderTextureSrc, _renderTextureDst, _material);
    }

    void OnPostRender()
    {    
    }
}
