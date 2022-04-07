using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPowderRoot : MonoBehaviour
{
    public Camera MainCamera;

    public GameObject ColoPowder_Root;  //1

    public GameObject ColoPowder_145_190;  //1
    public GameObject ColoPowder_145_150;  //2
    public GameObject ColoPowder_150_155;  //3
    public GameObject ColoPowder_155_160;  //4
    public GameObject ColoPowder_160_165;  //5
    public GameObject ColoPowder_165_170;  //6
    public GameObject ColoPowder_170_175;  //7
    public GameObject ColoPowder_175_180;  //8
    public GameObject ColoPowder_180_185;  //9
    public GameObject ColoPowder_185_190;  //10

    public SmokeSceneManager.HumanHeight humanheight = SmokeSceneManager.HumanHeight.H_145_190;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
    }

    void Update()
    {
        this.transform.LookAt(MainCamera.transform);

        if (Input.GetKey(KeyCode.Alpha1))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_145_190.SetActive(true);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_145_150.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha3))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_150_155.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha4))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_155_160.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha5))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_160_165.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha6))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_165_170.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha7))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_170_175.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha8))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_175_180.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha9))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_180_185.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha0))
        {
            //���߂ɂ��ׂĔ�\���ɂ���
            All_NoActive();
            ColoPowder_185_190.SetActive(true);
        }

    }

    public void AudioPlay()
    {
        //�����G�t�F�N�g�@�Đ��J�n����
        ColoPowder_Root.GetComponent<AudioSource>().Play();
    }

    public void Explosion()
    {
        //���߂ɂ��ׂĔ�\���ɂ���
        All_NoActive();

        //�A���N���b�N�̏ꍇ�ɁA�O��Invoke Method���L�����Z��
        CancelInvoke();

        //12�b��ɌĂяo��
        Invoke("Destroy_self", SmokeSceneManager.Instance.ANIMATION_TIME);

        switch (humanheight)
        {
            case SmokeSceneManager.HumanHeight.H_145_190:
                {
                    ColoPowder_145_190.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_145_150:
                {
                    ColoPowder_145_150.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_150_155:
                {
                    ColoPowder_150_155.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_155_160:
                {
                    ColoPowder_155_160.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_160_165:
                {
                    ColoPowder_160_165.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_165_170:
                {
                    ColoPowder_165_170.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_170_175:
                {
                    ColoPowder_170_175.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_175_180:
                {
                    ColoPowder_175_180.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_180_185:
                {
                    ColoPowder_180_185.SetActive(true);
                    break;
                }

            case SmokeSceneManager.HumanHeight.H_185_190:
                {
                    ColoPowder_185_190.SetActive(true);
                    break;
                }
        }
    }

    void All_NoActive()
    {
        ColoPowder_145_190.SetActive(false);
        ColoPowder_145_150.SetActive(false);
        ColoPowder_150_155.SetActive(false);
        ColoPowder_155_160.SetActive(false);
        ColoPowder_160_165.SetActive(false);
        ColoPowder_165_170.SetActive(false);
        ColoPowder_170_175.SetActive(false);
        ColoPowder_175_180.SetActive(false);
        ColoPowder_180_185.SetActive(false);
        ColoPowder_185_190.SetActive(false);
    }


    void Destroy_self()
    {
        SmokeSceneManager.Instance.ANIMATION_RUNNING = false;
        Destroy(this.gameObject);
    }

}