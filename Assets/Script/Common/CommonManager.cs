using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UniRx;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Text;

using Utf8Json;
using UnityEngine.SceneManagement; // コレ重要

public class ConfigJsonFormat
{
    public string remote_ip = "127.0.0.1";
    public int remote_port = 12345;
    public int local_port = 23456;
    //ここは、あれかサーバーサイドからデータが欲しいのか
    public string mode = "ghana"; //"ghana" or "pie" or ""(未使用)

    public string data_path = "data_path"; //"ghana" or "pie" or ""(未使用)
    public string pie_Idle_filename = "";
    public string pie_video_filename_A = "";
    public string pie_video_filename_B = "";
    public string pie_video_filename_C = "";
    public string pie_video_filename_D = "";
    public string pie_video_filename_E = "";

    public string ghana_Idle_filename = "";
    public string ghana_video_filename_A = "";
}

public class UdpJsonFormat
{
    public string from = "127.0.0.1";
    public string to = "127.0.0.1";
    public string command = "127.0.0.1";
}


public class CommonManager : SingletonMonoBehaviour<CommonManager>
{
    public enum Mode
    {
        ghana,
        pie,
        none
    }

    public static Subject<string> subject = new Subject<string>();
    public static ConfigJsonFormat JSON_DATA = new ConfigJsonFormat();
    public static Mode NowMode = Mode.ghana;
    public static Mode NextMode = Mode.ghana;
    public bool GUI_SHOW_FLAG = false;

    //UDP
    private static UdpClient UDP_RECEIVER;
    private static bool QuitFlag = false;
    private static UdpClient UDP_SENDER;

    private UdpClient client;

    int frameCount = 0;
    float prevTime = 0.0f;
    float fps;

    private bool keyIsBlock = false; //キー入力ブロックフラグ
    private DateTime pressedKeyTime; //前回キー入力された時間
    private TimeSpan elapsedTime; //キー入力されてからの経過時間

    private TimeSpan blockTime = new TimeSpan(0, 0, 1); //ブロックする時間　1s

    public RenderTexture RT_Fill;
    public RenderTexture RT_Key;
    public Material MT_Key;

    public Rect rect_fill;
    public Rect rect_key;

    // Start is called before the first frame update
    override protected void Awake()
    {
        // 子クラスでAwakeを使う場合は
        // 必ず親クラスのAwakeをCallして
        // 複数のGameObjectにアタッチされないようにします.
        base.Awake();

        ReadeConfig();

        //UDP RECIEVER
        UDP_RECEIVER = new UdpClient(JSON_DATA.local_port);
        UDP_RECEIVER.BeginReceive(OnReceived, UDP_RECEIVER);

        //UDP SENDER
        UDP_SENDER = new UdpClient();
        UDP_SENDER.Connect(JSON_DATA.remote_ip, JSON_DATA.remote_port);

        //Application.targetFrameRate = 30;

        GUI_SHOW_FLAG = false;
        Cursor.visible = GUI_SHOW_FLAG;
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            fps = frameCount / time;
            Debug.Log(fps);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }


        // nullは画面をレンダリング対象とすることを意味する
        // 画面にBlitする場合
        //Graphics.Blit(RT_Fill, RT_Key, MT_Key);

    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            Graphics.DrawTexture(rect_fill, RT_Fill);
            Graphics.DrawTexture(rect_key, RT_Fill, MT_Key);
        }

        if (GUI_SHOW_FLAG)
        {
            GUI.skin.label.fontSize = 50;
            GUILayout.Label(fps.ToString());
            GUILayout.Label("Ver:1.8");

        }

    }

    void OnDestroy()
    {
        QuitFlag = true;
        UDP_RECEIVER.Close();
        UDP_RECEIVER.Dispose();
        UDP_RECEIVER = null;
    }

    public string GetFilePath(string name)
    {
        return JSON_DATA.data_path + "\\" + name;
    }

    private void OnReceived(System.IAsyncResult result)
    {
        UdpClient getUdp = (UdpClient)result.AsyncState;
        IPEndPoint ipEnd = null;

        if (!QuitFlag)
        {
            getUdp.BeginReceive(OnReceived, getUdp);
            try
            {
                byte[] getByte = getUdp.EndReceive(result, ref ipEnd);

                var message = Encoding.UTF8.GetString(getByte);
                subject.OnNext(message);

                Debug.Log("UDP_RECIEVE : " + message);
            }
            catch (Exception ex)
            {
                Debug.Log("UDP_RECIEVE_ERROR" + ex);
            }
        }
        else
        {
            Debug.Log("UDP_RECIEVE：" + "Quit");
            return;
        }
    }

    public void SendMessage(string message)
    {
        var m = Encoding.UTF8.GetBytes(message);
        client.Send(m, m.Length);
    }


    public void ReadeConfig()
    {
        try
        {
            var JsonFilePath = Application.dataPath + "../../Config/";

            string stringJson = File.ReadAllText(JsonFilePath + "config_" + SceneManager.GetActiveScene().name + ".json");
            dynamic json_string = Utf8JsonExtension.ParseJsonText<dynamic>(stringJson);

            JSON_DATA.remote_ip = json_string["remote_ip"];

            double d_remote_port = json_string["remote_port"];
            JSON_DATA.remote_port = (int)d_remote_port;

            double d_local_port = json_string["local_port"];
            JSON_DATA.local_port = (int)d_local_port;

            JSON_DATA.mode = json_string["mode"];

            JSON_DATA.data_path = json_string["data_path"];

            string sceneName = SceneManager.GetActiveScene().name;

        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    public string ParseJson(string json)
    {
        return Utf8JsonExtension.ParseJsonText<dynamic>(json);
    }
}

#region 　json関連

public static class Utf8JsonExtension
{
    /// <summary>
    /// JSONファイルを読んで dynamic オブジェクトを返す。尚、配列は全て List<dynamic> で返る
    /// </summary>
    public static T ReadJsonFile<T>(string fileName)
    {
        string jsonText = ReadFile(fileName);
        return ParseJsonText<T>(jsonText);
    }

    public static T ParseJsonText<T>(string jsonText)
    {
        T tmpJson = JsonSerializer.Deserialize<T>(jsonText);

        if (tmpJson == null /*|| tmpJson.Count == 0*/)
        {
            Debug.LogErrorFormat("Utf8JsonExtension.ParseJsonText：json形式として問題があります");
        }
        else
        {
            Debug.LogFormat("Utf8JsonExtension.ParseJsonText：jsonとしてパースしました");
        }
        return tmpJson;
    }

    /// <summary>
    /// 指定したファイルが存在したら、全て読み込んで文字列で返す
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="Enc">指定しないと Encoding.UTF8 として読み込む</param>
    /// <returns></returns>
    public static string ReadFile(string fileName, Encoding Enc = null)
    {
        string resultText = "";
        FileInfo fi = new FileInfo(fileName);
        if (fi.Exists)
        {
            if (Enc == null) Enc = Encoding.UTF8;
            resultText = File.ReadAllText(fileName, Enc);
        }
        else
        {
            Debug.LogWarningFormat("{0}( {1} ) file not found !", "Utf8JsonExtension", fileName);
        }
        return resultText;
    }
}

#endregion
