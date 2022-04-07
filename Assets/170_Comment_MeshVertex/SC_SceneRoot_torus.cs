using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using TMPro;

using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;
using System.Linq;
using Battlehub.Dispatcher;

using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Windows;

[Serializable]
public class JsonBody
{
    public string Category = "Manage";
    public bool PriorityFlag = false;
    public string TemplateID = "Random";
    public string Name = "Jsonの名前です！";
    public string Comment = "Jsonのコメントです！";
}

public class SC_SceneRoot_torus : MonoBehaviour
{
    public GameObject Space3DModel;
    public GameObject TextPrefab;
    public static  List<GameObject> CreatedTexts;
    public PlayableDirector playableDirector;
    public bool ChinemaMoveFlag = true;

    /// <summary>
    /// JsonのBodyの内容をデータをすべて格納
    /// </summary>
    private List<Dictionary<string, object>> JsonDatas = new List<Dictionary<string, object>>();
    public List<string> CommentList = new List<string>();


    int LOCA_LPORT = 13333;
    static UdpClient udp;
    Thread thread;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            CreatedTexts = new List<GameObject>();

            #region UDP Init

            udp = new UdpClient(LOCA_LPORT);
            udp.Client.ReceiveTimeout = 1000;
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();

            #endregion

            JsonRead();

            //3D Vetex Read
            Matrix4x4 thisMatrix = Space3DModel.transform.localToWorldMatrix;

            Mesh mesh = Space3DModel.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            int i = 0;
            //json
            System.Random r2 = new System.Random();

            foreach (Vector3 vertex in vertices)
            {
                CreatedTexts.Add(Instantiate(TextPrefab));

                CreatedTexts[CreatedTexts.Count -1].SetActive(true);

                var comment = CommentList[r2.Next(0, CommentList.Count)];
                TextPrefab.GetComponent<TextMeshPro>().text = comment;

                Vector3 pos = thisMatrix.MultiplyPoint3x4(vertex);
                Debug.Log("mesh1 vertex at " + thisMatrix.MultiplyPoint3x4(vertex));

                //y座標のみ乱数で調整
                float adjust = 0.5f;
                pos.x += UnityEngine.Random.Range(-adjust, adjust);
                //vec.y += UnityEngine.Random.Range(-adjust, adjust);
                //vec.z += UnityEngine.Random.Range(-adjust, adjust);
                CreatedTexts[CreatedTexts.Count - 1].transform.localPosition = pos;
                CreatedTexts[CreatedTexts.Count - 1].transform.localScale = Vector3.one * 0.5f;

                CreatedTexts[CreatedTexts.Count - 1].transform.SetParent(Space3DModel.transform);
                i++;
            }
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (!ChinemaMoveFlag)
        {
            if (playableDirector.state == PlayState.Playing)
            {
                playableDirector.Stop();
            }
        }
        else
        {
            if (playableDirector.state == PlayState.Paused)
            {
                playableDirector.Play();
            }
        }


    }
    void OnDestroy()
    {
        udp.Close();
        udp.Dispose();
        udp = null;

        thread.Abort();

    }

    private void JsonRead()
    {
        try
        {

            //Json Read
            var JsonFilePath = Application.dataPath + "/Json/";
            string stringJson_Case = File.ReadAllText(JsonFilePath + "init.json");
            dynamic json_Case = Utf8JsonExtension.ParseJsonText<dynamic>(stringJson_Case);


            //body
            foreach (var b in json_Case["body"])
            {
                var test = b;
                JsonDatas.Add(b);

                JsonBody j = new JsonBody();
                j.Comment = b["comment"];

                CommentList.Add(j.Comment);
            }
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private static void ThreadMethod()
    {
        while (true)
        {
            try
            {

                IPEndPoint remoteEP = null;
                byte[] data = udp.Receive(ref remoteEP);
                string text = Encoding.UTF8.GetString(data);

                List<float> distansList = new List<float>();

                Dispatcher.Current.BeginInvoke(() =>
                {
                    foreach (GameObject t in CreatedTexts)
                    {
                        distansList.Add(t.GetComponent<TextToCameraDistance>().Distance);
                    }
                    int pos = distansList.IndexOf(distansList.Min());
                    CreatedTexts[pos].GetComponent<TextMeshPro>().text = text;

                    Debug.Log("id = " + pos + "Text = " + text);
                });
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

}
