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
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_145_190.SetActive(true);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_145_150.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha3))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_150_155.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha4))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_155_160.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha5))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_160_165.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha6))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_165_170.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha7))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_170_175.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha8))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_175_180.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha9))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_180_185.SetActive(true);
        }
        else if(Input.GetKey(KeyCode.Alpha0))
        {
            //初めにすべて非表示にする
            All_NoActive();
            ColoPowder_185_190.SetActive(true);
        }

    }

    public void AudioPlay()
    {
        //爆発エフェクト　再生開始処理
        ColoPowder_Root.GetComponent<AudioSource>().Play();
    }

    public void Explosion()
    {
        //初めにすべて非表示にする
        All_NoActive();

        //連続クリックの場合に、前のInvoke Methodをキャンセル
        CancelInvoke();

        //12秒後に呼び出す
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