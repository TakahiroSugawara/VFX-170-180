using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
//using Klak.Hap;

public class SC_SceneRoot_VFX_Final : MonoBehaviour
{
    #region 変数の定義

    public GameObject Space3DModel;

    public GameObject TextPrefab;
    public GameObject EmojiPrefab;
    public static  List<GameObject> CreatedTexts;
    public PlayableDirector playableDirector;
    public bool ChinemaMoveFlag = true;
    public static bool isFinished = false;

    public GameObject GO_Comment_Layout_A;
    public GameObject GO_Comment_Layout_B;
    public GameObject GO_Comment_Layout_C;
    public GameObject GO_Comment_Layout_D;
    public GameObject GO_Comment_Layout_E;


    public GameObject GO_Emoji_Layout_A;
    public GameObject GO_Emoji_Layout_B;
    public GameObject GO_Emoji_Layout_C;
    public GameObject GO_Emoji_Layout_D;
    public GameObject GO_Emoji_Layout_E;

    public enum SpawnMode
    { 
        LayoutA,
        LayoutB,
        LayoutC,
        LayoutD,
        LayoutE,
        Mode6
    }

    const string COMMAND_LAYOUT_A = "LAYOUT|A";
    const string COMMAND_LAYOUT_B = "LAYOUT|B";
    const string COMMAND_LAYOUT_C = "LAYOUT|C";
    const string COMMAND_LAYOUT_D = "LAYOUT|D";
    const string COMMAND_LAYOUT_E = "LAYOUT|E";

    const string COMMAND_EMOJI = "EMOJI";

    const string COMMAND_COMMENT = "COMMENT|";

    SpawnMode NowSpawnMode = SpawnMode.LayoutA;


    public static int Index_CommentPosition;
    public static int Index_CommentList;
    public static int Index_EmojiPosition;

    public struct CommentFormat
    {
        public string Comment;
        public string Name;
    };

    /// <summary>
    /// JsonのBodyの内容をデータをすべて格納
    /// </summary>
    private List<Dictionary<string, object>> JsonDatas = new List<Dictionary<string, object>>();
    public List<CommentFormat> CommentList = new List<CommentFormat>();

    public Vector3[] vertices;
    public Matrix4x4 thisMatrix;

    public float SpawnRate = 0.0f;

    private bool keyIsBlock = false; //キー入力ブロックフラグ
    private DateTime pressedKeyTime; //前回キー入力された時間
    private TimeSpan elapsedTime; //キー入力されてからの経過時間

    private TimeSpan blockTime = new TimeSpan(0, 0, 1); //ブロックする時間　1s

    public enum Mode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public float emojiScale = 10.0f;

    #endregion 

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Index_CommentPosition = 0;
            CreatedTexts = new List<GameObject>();

            #region UDP Recieve

            //UDP Recieve Action
            CommonManager.subject
                .ObserveOnMainThread()
                .Subscribe(msg =>
                {
                    //Layout Command
                    if (msg.Contains(COMMAND_LAYOUT_A))
                    {
                        DoAction(COMMAND_LAYOUT_A);
                    }
                    else if (msg.Contains(COMMAND_LAYOUT_B))
                    {
                        DoAction(COMMAND_LAYOUT_B);
                    }
                    else if (msg.Contains(COMMAND_LAYOUT_C))
                    {
                        DoAction(COMMAND_LAYOUT_C);
                    }
                    else if (msg.Contains(COMMAND_LAYOUT_D))
                    {
                        DoAction(COMMAND_LAYOUT_D);
                    }
                    else if (msg.Contains(COMMAND_LAYOUT_E))
                    {
                        DoAction(COMMAND_LAYOUT_E);
                    }
                    //Hap Command
                    else if (msg.Contains(COMMAND_EMOJI))
                    {
                        DoAction(COMMAND_EMOJI,msg);
                    }
                    //Comment Command
                    else if (msg.Contains(COMMAND_COMMENT))
                    {
                        DoAction(COMMAND_COMMENT, msg);
                    }

                }).AddTo(this);

            #endregion

            JsonRead();

            //3D Vetex Read
            thisMatrix = Space3DModel.transform.localToWorldMatrix;

            Mesh mesh = Space3DModel.GetComponent<MeshFilter>().mesh;
            vertices = mesh.vertices;

            InvokeRepeating(nameof(SpawnComment), 0, SpawnRate);

        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }

    }

    // Update is called once per frame
    void Update()
    {
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

        #region KeyBoard 入力

        //Layout Command
        if (Input.GetKey(KeyCode.Alpha1))
        {
            DoAction(COMMAND_LAYOUT_A);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            DoAction(COMMAND_LAYOUT_B);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            DoAction(COMMAND_LAYOUT_C);
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            DoAction(COMMAND_LAYOUT_D);
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            DoAction(COMMAND_LAYOUT_E);
        }
        else if (Input.GetKey(KeyCode.Tab))
        {
            keyIsBlock = true;
            pressedKeyTime = DateTime.Now;
            CommonManager.Instance.GUI_SHOW_FLAG = !CommonManager.Instance.GUI_SHOW_FLAG;
            Cursor.visible = CommonManager.Instance.GUI_SHOW_FLAG;
        }

        #endregion

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

    void DoAction(string Command, string msg = "")
    {
        keyIsBlock = true;
        pressedKeyTime = DateTime.Now;

        switch (Command)
        {
            case COMMAND_LAYOUT_A:
                NowSpawnMode = SpawnMode.LayoutA;
                break;
            case COMMAND_LAYOUT_B:
                NowSpawnMode = SpawnMode.LayoutB;
                break;
            case COMMAND_LAYOUT_C:
                NowSpawnMode = SpawnMode.LayoutC;
                break;
            case COMMAND_LAYOUT_D:
                NowSpawnMode = SpawnMode.LayoutD;
                break;
            case COMMAND_LAYOUT_E:
                NowSpawnMode = SpawnMode.LayoutE;
                break;
            case COMMAND_EMOJI:
                var sepa_emoji = msg.Split('|');
                SpawnEmoji(COMMAND_EMOJI, sepa_emoji[1]);
                break;
            case COMMAND_COMMENT:
                //コメントがUDPで送られた場合は、
                //Listに追加して、Indexを0に戻すことで、
                //追加したコメントを一番初めに表示していくことにする。
                Index_CommentList = 0;
                var sepa_come = msg.Split('|');

                CommentList.RemoveAt(CommentList.Count - 1);
                CommentFormat cf = new CommentFormat();
                cf.Name = sepa_come[1];
                cf.Comment = sepa_come[2];
                CommentList.Insert(0, cf);

                break;

            default:
                break;

        }


    }


    void SpawnComment()
    {
        System.Random r2 = new System.Random();

        CreatedTexts.Add(Instantiate(TextPrefab));

        //起動時にチラつきを防止する
        CreatedTexts[CreatedTexts.Count - 1].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        CreatedTexts[CreatedTexts.Count - 1].GetComponent<MeshRenderer>().enabled = false;

        //起動時にチラつきを防止する
        CreatedTexts[CreatedTexts.Count - 1].transform.GetChild(0).GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        CreatedTexts[CreatedTexts.Count - 1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        CreatedTexts[CreatedTexts.Count - 1].SetActive(true);

        //乱数を生成
        var r_j = r2.Next(0, CommentList.Count);

        //json
        #region CommentListの上から順に取っていく

        string comment = string.Empty;
        string name = string.Empty;
        if (Index_CommentList >= CommentList.Count)
        {
            Index_CommentList = 0;
        }
        comment = CommentList[Index_CommentList].Comment;
        name = CommentList[Index_CommentList].Name;
        Index_CommentList++;

        
        TextPrefab.GetComponent<TextMeshPro>().text = comment;
        TextPrefab.transform.GetChild(0).GetComponent<TextMeshPro>().text = name;

        #endregion

        #region Position 子オブジェクトのを上から順に

        GameObject child = null;
        //座標専用のGameObjectから座標を決定する
        switch (NowSpawnMode)
        {
            case SpawnMode.LayoutA:
                
                if (Index_CommentPosition >= GO_Comment_Layout_A.transform.childCount)
                {
                    Index_CommentPosition = 0;
                }
                child = GO_Comment_Layout_A.transform.GetChild(Index_CommentPosition).gameObject;
                Index_CommentPosition++;
                break;
            case SpawnMode.LayoutB:

                if (Index_CommentPosition >= GO_Comment_Layout_B.transform.childCount)
                {
                    Index_CommentPosition = 0;
                }
                child = GO_Comment_Layout_B.transform.GetChild(Index_CommentPosition).gameObject;
                Index_CommentPosition++;

                break;
            case SpawnMode.LayoutC:

                if (Index_CommentPosition >= GO_Comment_Layout_C.transform.childCount)
                {
                    Index_CommentPosition = 0;
                }
                child = GO_Comment_Layout_C.transform.GetChild(Index_CommentPosition).gameObject;
                Index_CommentPosition++;
                break;
            case SpawnMode.LayoutD:

                if (Index_CommentPosition >= GO_Comment_Layout_D.transform.childCount)
                {
                    Index_CommentPosition = 0;
                }
                child = GO_Comment_Layout_D.transform.GetChild(Index_CommentPosition).gameObject;
                Index_CommentPosition++;
                break;
            case SpawnMode.LayoutE:

                if (Index_CommentPosition >= GO_Comment_Layout_E.transform.childCount)
                {
                    Index_CommentPosition = 0;
                }
                child = GO_Comment_Layout_E.transform.GetChild(Index_CommentPosition).gameObject;
                Index_CommentPosition++;
                break;

            default:
                #region ここは通らない
                
                if (Index_CommentPosition >= GO_Comment_Layout_A.transform.childCount)
                {
                    Index_CommentPosition = 0;
                }
                child = GO_Comment_Layout_A.transform.GetChild(Index_CommentPosition).gameObject;
                Index_CommentPosition++;

                #endregion

                break;
        }

        Vector3 pos = child.transform.position;
        //Debug.Log("thisMatrix " + child.transform.position);

        CreatedTexts[CreatedTexts.Count - 1].transform.localPosition = pos;
        CreatedTexts[CreatedTexts.Count - 1].transform.localScale = Vector3.one * 0.5f;

        #endregion

        //3D model ver
        //CreatedTexts[CreatedTexts.Count - 1].transform.SetParent(Space3DModel.transform);
        CreatedTexts[CreatedTexts.Count - 1].transform.SetParent(child.transform);

    }

    //void SpawnHap(string hapKind)
    //{
    //    System.Random r2 = new System.Random();

    //    var hapInstatiated =  Instantiate(HapPrefab);

    //    //乱数を生成
    //    var r_j = r2.Next(0, CommentList.Count);

    //    var HapFilePath = Application.dataPath + "../../Hap_Movies/";
    //    var filename = "";

    //    //Material を分ける
    //    var rt = new RenderTexture(1500, 1500, 16, RenderTextureFormat.ARGB32);
    //    rt.Create();
    //    hapInstatiated.GetComponent<HapPlayer>().targetTexture = rt;

    //    //取得したシェーダーを元に新しいマテリアルを作成
    //    //対象のシェーダー情報を取得
    //    Shader sh = hapInstatiated.GetComponent<MeshRenderer>().material.shader;
    //    Material mat = new Material(sh);
    //    hapInstatiated.GetComponent<MeshRenderer>().material = mat;

    //    Mode mode = Mode.Opaque;
    //    hapInstatiated.GetComponent<MeshRenderer>().material.SetFloat("_Mode", (float)mode);  // <= これが必要
    //    hapInstatiated.GetComponent<MeshRenderer>().material.SetTexture("_RT_Hap_Script_Created", rt);

    //    hapInstatiated.GetComponent<MeshRenderer>().material.SetOverrideTag("RenderType", "Opaque");
    //    hapInstatiated.GetComponent<MeshRenderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    //    hapInstatiated.GetComponent<MeshRenderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    //    hapInstatiated.GetComponent<MeshRenderer>().material.SetInt("_ZWrite", 0);
    //    hapInstatiated.GetComponent<MeshRenderer>().material.DisableKeyword("_ALPHATEST_ON");
    //    hapInstatiated.GetComponent<MeshRenderer>().material.DisableKeyword("_ALPHABLEND_ON");
    //    hapInstatiated.GetComponent<MeshRenderer>().material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
    //    hapInstatiated.GetComponent<MeshRenderer>().material.renderQueue = 3000;


    //    hapInstatiated.GetComponent<MeshRenderer>().material.SetFloat("_Alphathreshold", 0.01f); ;


    //    switch (hapKind)
    //    {
    //        case COMMAND_EMOJI:
    //            filename = "hap_1.mov";
    //            hapInstatiated.GetComponent<HapPlayer>().Open(HapFilePath + filename, HapPlayer.PathMode.LocalFileSystem);
    //            break;

    //        case COMMAND_HAP_2:
    //            filename = "hap_2.mov";
    //            hapInstatiated.GetComponent<HapPlayer>().Open(HapFilePath +  filename, HapPlayer.PathMode.LocalFileSystem);
    //            break;

    //        case COMMAND_HAP_3:
    //            filename = "hap_3.mov";
    //            hapInstatiated.GetComponent<HapPlayer>().Open(HapFilePath +  filename, HapPlayer.PathMode.LocalFileSystem);
    //            break;

    //        case COMMAND_HAP_4:
    //            filename = "hap_4.mov";
    //            hapInstatiated.GetComponent<HapPlayer>().Open(HapFilePath +  filename, HapPlayer.PathMode.LocalFileSystem);
    //            break;

    //        case COMMAND_HAP_5:
    //            filename = "hap_5.mov";
    //            hapInstatiated.GetComponent<HapPlayer>().Open(HapFilePath + filename, HapPlayer.PathMode.LocalFileSystem);
    //            break;

    //    }



    //    #region Position を子オブジェクトの上から順に生成

    //    GameObject child = null;
    //    //座標専用のGameObjectから座標を決定する
    //    switch (NowSpawnMode)
    //    {
    //        case SpawnMode.LayoutA:

    //            if (Index_HapPosition >= GO_Hap_Layout_A.transform.childCount)
    //            {
    //                Index_HapPosition = 0;
    //            }
    //            child = GO_Hap_Layout_A.transform.GetChild(Index_HapPosition).gameObject;
    //            Index_HapPosition++;

    //            break;
    //        case SpawnMode.LayoutB:

    //            if (Index_HapPosition >= GO_Hap_Layout_B.transform.childCount)
    //            {
    //                Index_HapPosition = 0;
    //            }
    //            child = GO_Hap_Layout_B.transform.GetChild(Index_HapPosition).gameObject;
    //            Index_HapPosition++;

    //            break;
    //        case SpawnMode.LayoutC:

    //            if (Index_HapPosition >= GO_Hap_Layout_C.transform.childCount)
    //            {
    //                Index_HapPosition = 0;
    //            }
    //            child = GO_Hap_Layout_C.transform.GetChild(Index_HapPosition).gameObject;
    //            Index_HapPosition++;

    //            break;
    //        case SpawnMode.LayoutD:

    //            if (Index_HapPosition >= GO_Hap_Layout_D.transform.childCount)
    //            {
    //                Index_HapPosition = 0;
    //            }
    //            child = GO_Hap_Layout_D.transform.GetChild(Index_HapPosition).gameObject;
    //            Index_HapPosition++;

    //            break;
    //        case SpawnMode.LayoutE:

    //            if (Index_HapPosition >= GO_Hap_Layout_E.transform.childCount)
    //            {
    //                Index_HapPosition = 0;
    //            }
    //            child = GO_Hap_Layout_E.transform.GetChild(Index_HapPosition).gameObject;
    //            Index_HapPosition++;

    //            break;

    //        default:
    //            #region ここは通らない

    //            if (Index_HapPosition >= GO_Comment_Layout_A.transform.childCount)
    //            {
    //                Index_HapPosition = 0;
    //            }
    //            child = GO_Comment_Layout_A.transform.GetChild(Index_HapPosition).gameObject;
    //            Index_HapPosition++;

    //            #endregion

    //            break;
    //    }

    //    Vector3 pos = child.transform.position;
    //    Debug.Log("thisMatrix " + child.transform.position);

    //    hapInstatiated.transform.localPosition = pos;

    //    hapInstatiated.transform.SetParent(child.transform);
    //    hapInstatiated.transform.localScale = Vector3.one;
    //    #endregion


    //    hapInstatiated.GetComponent<HapPlayer>().SetTime(0);
    //    hapInstatiated.GetComponent<HapPlayer>().loop = false;
    //    hapInstatiated.SetActive(true);
    //}

    void SpawnEmoji(string hapKind,string msg)
    {
        System.Random r2 = new System.Random();

        var emojiInstatiated = Instantiate(EmojiPrefab);

        emojiInstatiated.GetComponent<TextMeshPro>().text = msg;

        #region Position を子オブジェクトの上から順に生成

        GameObject child = null;
        //座標専用のGameObjectから座標を決定する
        switch (NowSpawnMode)
        {
            case SpawnMode.LayoutA:

                if (Index_EmojiPosition >= GO_Emoji_Layout_A.transform.childCount)
                {
                    Index_EmojiPosition = 0;
                }
                child = GO_Emoji_Layout_A.transform.GetChild(Index_EmojiPosition).gameObject;
                Index_EmojiPosition++;

                break;
            case SpawnMode.LayoutB:

                if (Index_EmojiPosition >= GO_Emoji_Layout_B.transform.childCount)
                {
                    Index_EmojiPosition = 0;
                }
                child = GO_Emoji_Layout_B.transform.GetChild(Index_EmojiPosition).gameObject;
                Index_EmojiPosition++;

                break;
            case SpawnMode.LayoutC:

                if (Index_EmojiPosition >= GO_Emoji_Layout_C.transform.childCount)
                {
                    Index_EmojiPosition = 0;
                }
                child = GO_Emoji_Layout_C.transform.GetChild(Index_EmojiPosition).gameObject;
                Index_EmojiPosition++;

                break;
            case SpawnMode.LayoutD:

                if (Index_EmojiPosition >= GO_Emoji_Layout_D.transform.childCount)
                {
                    Index_EmojiPosition = 0;
                }
                child = GO_Emoji_Layout_D.transform.GetChild(Index_EmojiPosition).gameObject;
                Index_EmojiPosition++;

                break;
            case SpawnMode.LayoutE:

                if (Index_EmojiPosition >= GO_Emoji_Layout_E.transform.childCount)
                {
                    Index_EmojiPosition = 0;
                }
                child = GO_Emoji_Layout_E.transform.GetChild(Index_EmojiPosition).gameObject;
                Index_EmojiPosition++;

                break;

            default:
                #region ここは通らない

                if (Index_EmojiPosition >= GO_Comment_Layout_A.transform.childCount)
                {
                    Index_EmojiPosition = 0;
                }
                child = GO_Comment_Layout_A.transform.GetChild(Index_EmojiPosition).gameObject;
                Index_EmojiPosition++;

                #endregion

                break;
        }

        Vector3 pos = child.transform.position;
        Debug.Log("thisMatrix " + child.transform.position);

        emojiInstatiated.transform.localPosition = pos;

        emojiInstatiated.transform.SetParent(child.transform);
        emojiInstatiated.transform.localScale = Vector3.one * emojiScale;
        #endregion

        emojiInstatiated.SetActive(true);
    }

    private void ModeChange(SpawnMode mode )
    {
        switch(mode)
        {
            case SpawnMode.LayoutA:
                break;
            case SpawnMode.LayoutB:
                break;
            case SpawnMode.LayoutC:
                break;
            case SpawnMode.LayoutD:
                break;
            case SpawnMode.LayoutE:
                break;
        }
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

                CommentFormat cf = new CommentFormat();
                cf.Name = b["name"];
                cf.Comment = b["comment"];

                CommentList.Add(cf);
            }
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }



}
