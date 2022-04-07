using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SmokeSceneManager : SingletonMonoBehaviour<SmokeSceneManager>
{
    //Bodyの配列
    public struct body
    {
        public float height;
        public Vector2 heart_position_xy;
    }

    public Text Body1_Height;
    public Text Body2_Height;
    public Text FPS;
    public Text FOV_TEXT;

    public bool DebugModeFlag = true;

    public bool ANIMATION_RUNNING = false;
    public float ANIMATION_TIME = 12;

    public List<double> BodyHeight_mm = new List<double>();

    public GameObject debug_obj_1;
    public GameObject debug_obj_2;
    public GameObject debug_obj_3;
    public GameObject debug_obj_4;
    public GameObject debug_obj_5;
    public GameObject debug_obj_6;
    public GameObject debug_obj_7;

    public Camera MainProjectorCamera;
    public float FOV = 22.6f;

    private bool keyIsBlock = false; //キー入力ブロックフラグ
    private DateTime pressedKeyTime; //前回キー入力された時間
    private TimeSpan elapsedTime; //キー入力されてからの経過時間

    private TimeSpan blockTime = new TimeSpan(0, 0, 1); //ブロックする時間　1s

    public enum HumanHeight
    {
        H_145_190,  //1
        H_145_150,  //2
        H_150_155,  //3
        H_155_160,  //4
        H_160_165,  //5
        H_165_170,  //6
        H_170_175,  //7
        H_175_180,  //8
        H_180_185,  //9
        H_185_190,  //10
    }

    public float Length = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        double Body1 = 0;
        BodyHeight_mm.Add(Body1);
        double Body2 = 0;
        BodyHeight_mm.Add(Body2);
        double Bodydebug = 0;
        BodyHeight_mm.Add(Bodydebug);

        Application.targetFrameRate = 30; //30FPSに設定

        Cursor.visible = false;

        FOV_TEXT.text = "FOV : " + FOV.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = Length;

        Body1_Height.text = (BodyHeight_mm[0] / 10).ToString();
        Body2_Height.text = (BodyHeight_mm[1] / 10).ToString();
        float fps = 1f / Time.deltaTime;
        FPS.text = fps.ToString("00.0");

        if (keyIsBlock)
        {
            elapsedTime = DateTime.Now - pressedKeyTime;
            if (elapsedTime > blockTime)
            {
                keyIsBlock = false;
            }
            else
            {
                return;
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            keyIsBlock = true;
            pressedKeyTime = DateTime.Now;

            DebugModeFlag = !DebugModeFlag;

            debug_obj_1.SetActive(DebugModeFlag);
            debug_obj_2.SetActive(DebugModeFlag);
            debug_obj_3.SetActive(DebugModeFlag);
            debug_obj_4.SetActive(DebugModeFlag);
            debug_obj_5.SetActive(DebugModeFlag);
            debug_obj_6.SetActive(DebugModeFlag);
            debug_obj_7.SetActive(DebugModeFlag);

            Cursor.visible = DebugModeFlag;

        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            FOV += 0.1f;
            MainProjectorCamera.fieldOfView = FOV;
            FOV_TEXT.text = "FOV : " + FOV.ToString();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            FOV -= 0.1f;
            MainProjectorCamera.fieldOfView = FOV;
            FOV_TEXT.text = "FOV : " + FOV.ToString();
        }
    }
}
