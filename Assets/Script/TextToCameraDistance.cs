using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;

public class TextToCameraDistance : MonoBehaviour
{
    public float Distance;

    public float Do_GlitchOFF_Time;
    public float Do_Gravity_Time;
    public float Do_Destroy_Time;

    public float Do_VIsibleTrueTime;

    public VisualEffect _VFX;

    public bool UseGravityFlag = false;
    public bool AutoDestroyFlag = false;
    public bool UseGlitchOFF = false;
    public bool UseVFX = false;

    // Start is called before the first frame update
    void Start()
    {
        //fade in �Ń`���������邽�߁A�f�B���C�������āARenderer��ON�ɂ���
        Invoke("Do_VIsibleTrue", Do_VIsibleTrueTime);

        if (UseGlitchOFF)
        {
            //DelayMethod��3.5�b��ɌĂяo��
            Invoke("GlitchOFF", Do_GlitchOFF_Time);
        }

        if (UseGravityFlag)
        {
            //DelayMethod��3.5�b��ɌĂяo��
            Invoke("GravityMethod", Do_Gravity_Time);
        }

        if (AutoDestroyFlag)
        {
            //DelayMethod��10�b��ɌĂяo��
            Invoke("DoDestroy", Do_Destroy_Time);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //world�@position�ł̋����̔�r
        //Distance = Vector3.Distance(this.transform.position, Camera.transform.position);

        //var position = new Vector3();
        //var rotation = new Vector3(0, 180, 0);
        //position.x = Camera.transform.position.x;
        //position.y = Camera.transform.position.y;
        //position.z = Camera.transform.position.z;
        //this.transform.LookAt(position);
        //this.transform.Rotate(rotation);


    }

    private void GravityMethod()
    {
        this.GetComponent<Rigidbody>().useGravity = true;
        
        //Collider �͂���Ȃ��B�������̂�
        //this.GetComponent<BoxCollider>().enabled = true;
    }
    private void GlitchOFF()
    {
        if (this.GetComponent<Material>() != null)
        {
            this.GetComponent<Material>().SetFloat("_My_GlitchSpeed", 0.0f);
        }

    }

    private void Do_VIsibleTrue()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
    }

    private void DoDestroy()
    {
        Destroy(this.gameObject);
    }
}
