using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using REEL.D2EEditor;
using REEL.PROJECT;

public class Utils
{
    const float HIGHLIGHT_SPEED = 10.0f;            //곡선 highlight 강조 애니메이션 속도 

    static public float EDITOR_AREA_LEFT = 0;
    static public float EDITOR_AREA_TOP = 0;
    static public float EDITOR_AREA_RIGHT = 10000;       //임시
    //static public float EDITOR_AREA_BOTTOM = -1500;     //임시
    static public float EDITOR_AREA_BOTTOM = -10000;     //임시

    public static int FIND_ERROR_CODE = -1;

    public enum ParameterType
    {
        None, Number, String, Face, Motion, Mobility, Boolean, ArrayOrList
    }

    public static Color GetParameterColor(REEL.PROJECT.DataType type, REEL.PROJECT.NodeType nodeType = REEL.PROJECT.NodeType.VARIABLE)
    {
        if(nodeType == REEL.PROJECT.NodeType.LIST)
        {
            return new Color(0.3607843f, 0.4509804f, 0.09411766f, 1f);
        }

        //switch (type)
        //{
        //    case REEL.PROJECT.DataType.BOOL: return Color.red;
        //    case REEL.PROJECT.DataType.NUMBER: return new Color(0.2f, 0.7f, 0f, 1f);
        //    case REEL.PROJECT.DataType.STRING: return Color.magenta;
        //    //case REEL.PROJECT.DataType.VARIABLE: return new Color(0f, 0.6f, 1f, 1f);
        //    case REEL.PROJECT.DataType.LIST: return new Color(0.2f, 0.2f, 0.8f, 1f);
        //    case REEL.PROJECT.DataType.EXPRESSION: return new Color(0.5f, 0.5f, 1f, 1f);

        //    default: return new Color(0.7f, 0.7f, 0.7f, 1f);
        //}

        Color color = Color.white;
        switch(type)
        {
            //case REEL.PROJECT.DataType.BOOL: return new Color(0.9450981f, 0.2039216f, 0.1882353f, 1f);
            //case REEL.PROJECT.DataType.NUMBER: return new Color(0.9490197f, 0.5058824f, 0.03529412f, 1f);
            ////case REEL.PROJECT.DataType.STRING: return new Color(0.9333334f, 0.8000001f, 0.3686275f, 1f);
            ////case REEL.PROJECT.DataType.STRING: return new Color(0.72f, 0.56f, 0.43f);
            //case REEL.PROJECT.DataType.LIST: return new Color(0.3607843f, 0.4509804f, 0.09411766f, 1f);
            //case REEL.PROJECT.DataType.EXPRESSION: return new Color(0.2352941f, 0.9607844f, 0.7882354f, 1f);
            //case REEL.PROJECT.DataType.FACIAL: return new Color(0.4196079f, 0.7529413f, 1f, 1f);
            //case REEL.PROJECT.DataType.MOTION: return new Color(0.5137255f, 0.5333334f, 0.9960785f, 1f);
            //case REEL.PROJECT.DataType.MOBILITY: return new Color(0.7490196f, 0.5137255f, 1f, 1f);

            //default: return new Color(0.9647059f, 0.509804f, 1f, 1f);

            case REEL.PROJECT.DataType.BOOL:
            {
                ColorUtility.TryParseHtmlString("#c7403a", out color);
                return color;
            }
            case REEL.PROJECT.DataType.NUMBER:
            {
                ColorUtility.TryParseHtmlString("#b96b21", out color);
                return color;
            }
            case REEL.PROJECT.DataType.STRING:
            {
                ColorUtility.TryParseHtmlString("#b88f0b", out color);
                return color;
            }
            case REEL.PROJECT.DataType.LIST:
            {
                ColorUtility.TryParseHtmlString("#7a871b", out color);
                return color;
            }
            case REEL.PROJECT.DataType.EXPRESSION:
            {
                ColorUtility.TryParseHtmlString("#0f8e77", out color);
                return color;
            }
            case REEL.PROJECT.DataType.FACIAL:
            {
                ColorUtility.TryParseHtmlString("#1b88b3", out color);
                return color;
            }
            case REEL.PROJECT.DataType.MOTION:
            {
                ColorUtility.TryParseHtmlString("#5959c3", out color);
                return color;
            }
            case REEL.PROJECT.DataType.MOBILITY:
            {
                ColorUtility.TryParseHtmlString("#9a4db5", out color);
                return color;
            }

            default: return new Color(0.9647059f, 0.509804f, 1f, 1f);
        }
    }

    static int idCount = 1;

    public static int NewGUID
    {
        get
        {
            DateTime now = DateTime.Now;
            DateTime zeroDate = DateTime.MinValue
                .AddHours(now.Hour)
                .AddMinutes(now.Minute)
                .AddSeconds(now.Second)
                .AddMilliseconds(now.Millisecond);

            int id = (int)(zeroDate.Ticks % int.MaxValue) + idCount;
            idCount++;

            return id;
        }
    }

    // 노드 추가할 때 타입별로 이름 변경하는데
    // 이때 노드 이름 뒤에 순번 붙여줄 때 사용.
    static int nodeNumber = 1;
    public static int NewNodeNumber
    {
        get
        {
            return nodeNumber++;
        }
    }

    public static void ResetNodeNumber()
    {
        nodeNumber = 1;
    }
    

    public static void LogRed(object log)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=red> {log} </color>");
#elif UNITY_STANDALONE_WIN && DEBUG
        LogWindow.Instance.PrintLogDebug($"<color=red> {log} </color>");
#endif
    }

    public static void LogBlue(object log)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=blue> {log} </color>");
#elif UNITY_STANDALONE_WIN && DEBUG
        LogWindow.Instance.PrintLogDebug($"<color=blue> {log} </color>");
#endif
    }

    public static void LogGreen(object log)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=green> {log} </color>");
#elif UNITY_STANDALONE_WIN && DEBUG
        LogWindow.Instance.PrintLogDebug($"<color=green> {log} </color>");
#endif
    }

    public static void Log(object log)
    {
#if UNITY_EDITOR
        Debug.Log($"{log}");
#elif UNITY_STANDALONE_WIN && DEBUG
        LogWindow.Instance.PrintLogDebug($"{log}");
#endif
    }


    private static GraphPane graphPane = null;
    public static GraphPane GetGraphPane()
    {
        if(graphPane is null)
        {
            graphPane = MCEditorManager.Instance
                .GetPane(MCEditorManager.PaneType.Graph_Pane)
                .GetComponent<GraphPane>();
        }

        return graphPane;
    }

    private static TabManager tabManager = null;
    public static TabManager GetTabManager()
    {
        if (tabManager == null)
        {
            tabManager = GameObject.FindObjectOfType<TabManager>();
        }

        return tabManager;
    }

    private static MCTables tables = null;
    public static MCTables GetTables()
    {
        if(tables is null)
        {
            tables = GameObject.FindObjectOfType<MCTables>();
        }

        return tables;
    }

    private static MCUserMenu userMenu = null;
    public static MCUserMenu GetUserMenu()
    {
        if (userMenu == null)
        {
            userMenu = GameObject.FindObjectOfType<MCUserMenu>();
        }

        return userMenu;
    }

    public static string CompileToJson(ProjectData data, bool prettyPrint = false)
    {
        return JsonUtility.ToJson(data, prettyPrint);
    }

    public static T LoadFromJson<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    public static bool IsProjectNullOrOnSimulation
    {
        get
        {
            return MCPlayStateManager.IsProjectNull || MCPlayStateManager.Instance.IsSimulation;
        }
    }

    public static string CurrentTabName { get { return GetTabManager().CurrentTab.TabName; } }

    public static bool IsCameraUsingType(NodeType nodeType)
    {
        if (nodeType == NodeType.AGE_GENDER
            || nodeType == NodeType.DELETE_FACE
            || nodeType == NodeType.DETECT_OBJECT
            || nodeType == NodeType.DETECT_PERSON
            || nodeType == NodeType.EMOTION
            || nodeType == NodeType.GENDER
            || nodeType == NodeType.HANDS_UP
            || nodeType == NodeType.POSE_REC
            || nodeType == NodeType.RECOG_FACE
            || nodeType == NodeType.TEACHABLE_MACHINE_SERVER
            || nodeType == NodeType.USER_API_CAMERA)
        {
            return true;
        }

        return false;
    }

    public static GameObject CreateNewLineGO()
    {
        return MCEditorManager.Instance.CreateNewLineGO();
    }

    public static MCBezierLine CreateNewLine()
    {
        return CreateNewLineGO().GetComponent<MCBezierLine>();
    }

    public static bool IsNullOrEmptyOrWhiteSpace(string value)
    {
        return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
    }

    public static bool IsTheSameFunction(int id1, int id2, string name1, string name2)
    {
        return id1.Equals(id2) || name1.Equals(name2);
    }

    public static bool IsLeftButtonClicked(PointerEventData eventData)
    {
        return eventData.button == PointerEventData.InputButton.Left;
    }

    public static AudioClip GetAudioClipFromMP3ByteArray(byte[] in_aMP3Data)
    {
        AudioClip l_oAudioClip = null;
        Stream l_oByteStream = new MemoryStream(in_aMP3Data);
        MP3Sharp.MP3Stream l_oMP3Stream = new MP3Sharp.MP3Stream(l_oByteStream);

        try
        {
            //Get the converted stream data
            MemoryStream l_oConvertedAudioData = new MemoryStream();
            byte[] l_aBuffer = new byte[2048];
            //byte[] l_aBuffer = new byte[4096];
            int l_nBytesReturned = -1;
            int l_nTotalBytesReturned = 0;

            while(l_nBytesReturned != 0)
            {
                l_nBytesReturned = l_oMP3Stream.Read(l_aBuffer, 0, l_aBuffer.Length);
                l_oConvertedAudioData.Write(l_aBuffer, 0, l_nBytesReturned);
                l_nTotalBytesReturned += l_nBytesReturned;
            }

            //Debug.Log("MP3 file has " + l_oMP3Stream.ChannelCount + " channels with a frequency of " + l_oMP3Stream.Frequency);

            byte[] l_aConvertedAudioData = l_oConvertedAudioData.ToArray();
            //Debug.Log("Converted Data has " + l_aConvertedAudioData.Length + " bytes of data");

            ////Convert the byte converted byte data into float form in the range of 0.0-1.0
            //float[] l_aFloatArray = new float[l_aConvertedAudioData.Length / 2];

            //for(int i = 0; i < l_aFloatArray.Length; i++)
            //{
            //    if(BitConverter.IsLittleEndian)
            //    {
            //        //Evaluate earlier when pulling from server and/or local filesystem - not needed here
            //        //Array.Reverse( l_aConvertedAudioData, i * 2, 2 );
            //    }

            //    //Yikes, remember that it is SIGNED Int16, not unsigned (spent a bit of time before realizing I screwed this up...)
            //    l_aFloatArray[i] = (float)(BitConverter.ToInt16(l_aConvertedAudioData, i * 2) / 32768.0f);
            //}
            float[] l_aFloatArray = ConvertByteToFloat16(l_aConvertedAudioData);

            //For some reason the MP3 header is readin as single channel despite it containing 2 channels of data (investigate later)
            if (l_oMP3Stream.ChannelCount == 1)
            {
                l_oAudioClip = AudioClip.Create("MySound", l_aFloatArray.Length, 1, l_oMP3Stream.Frequency, false);
            }

            else
            {
                l_oAudioClip = AudioClip.Create("MySound", l_aFloatArray.Length, 2, l_oMP3Stream.Frequency, false);
            }

            l_oAudioClip.SetData(l_aFloatArray, 0);

            //Utils.LogRed($"[MP3 to AudioClip] AudioClip.channels: {l_oAudioClip.channels} / l_oMP3Stream.ChannelCount: {l_oMP3Stream.ChannelCount}");
        }
        catch(Exception ex)
        {
            Debug.Log(ex.ToString());
        }
        return l_oAudioClip;
    }

    public static float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 4];
        for (int i = 0; i < floatArr.Length; i++)
        {
            //if (BitConverter.IsLittleEndian)
                //Array.Reverse(array, i * 4, 4);
            floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
        }
        return floatArr;
    }
    public static float[] ConvertByteToFloat16(byte[] array)
    {
        float[] floatArr = new float[array.Length / 4];
        for (int i = 0; i < floatArr.Length; i++)
        {
            //if (BitConverter.IsLittleEndian)
            //Array.Reverse(array, i * 2, 2);
            floatArr[i] = (float)(BitConverter.ToInt16(array, i * 4) / 32767.0f);
        }
        return floatArr;
    }

    // GC 오버헤드때문에 밖으로 뺌 - kjh
    static UIVertex[] verts =
    {
        new UIVertex()  {   uv0 = new Vector2(0.5f, 0)  },
        new UIVertex()  {   uv0 = new Vector2(0.5f, 1)  },
        new UIVertex()  {   uv0 = new Vector2(0.5f, 1)  },
        new UIVertex()  {   uv0 = new Vector2(0.5f, 0)  }
    };

    static public void AddLine(VertexHelper vh, Vector3 p0, Vector3 p1, float width, Color color)
    {
        Vector3 crossVector = p1 - p0;
        Vector3 normal = Vector3.Cross(crossVector, new Vector3(0f, 0f, 1f));
        normal.Normalize();

        Vector3 n = normal * width * 0.5f;

        verts[0].color = color;
        verts[1].color = color;
        verts[2].color = color;
        verts[3].color = color;

        verts[0].position = p0 + n;
        verts[1].position = p0 - n;

        verts[2].position = p1 - n;
        verts[3].position = p1 + n;

        vh.AddUIVertexQuad(verts);
    }

    static public void AddLine(VertexHelper vh, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float width, Color color)
    {
        verts[0].color = color;
        verts[1].color = color;
        verts[2].color = color;
        verts[3].color = color;

        verts[0].position = p0;
        verts[1].position = p1;

        verts[2].position = p2;
        verts[3].position = p3;

        vh.AddUIVertexQuad(verts);
    }


    public static List<Vector2> AddLineList(VertexHelper vertexHelper, List<Vector2> pointList, float width, Color color, bool animation = false)
    {
        float weight = 1.0f;

        Vector3 start;
        Vector3 end;
        Vector3 normal;

        Vector3 p0 = Vector3.zero;
        Vector3 p1 = Vector3.zero;
        Vector3 p2,p3;

        for(int ix = 0; ix < pointList.Count - 1; ++ix)
        {
            if (animation == true)
            {
                weight = (Mathf.Sin((pointList.Count - ix) / (float)pointList.Count * (Mathf.PI * 2.0f) + (Time.time * HIGHLIGHT_SPEED)) + 1.0f) * 0.5f;
                weight = Mathf.Min(weight * weight, 1.0f);
                color.a = weight;
            }

            start = pointList[ix];
            end = pointList[ix+1];

            normal = Vector3.Cross(end - start, new Vector3(0f, 0f, 1f));
            normal.Normalize();
            Vector3 n = normal * width * 0.5f;

            if( ix == 0 )
            {
                p0 = start + n;
                p1 = start - n;
                p2 = end - n;
                p3 = end + n;

                AddLine(vertexHelper, p0, p1, p2, p3, width * weight, color);
            }
            else
            {
                p2 = end - n;
                p3 = end + n;

                AddLine(vertexHelper, p0, p1, p2, p3, width * weight, color);
            }

            p0 = p3;
            p1 = p2;
        }

        return pointList;
    }

    public static List<Vector2> AddBezier(VertexHelper vertexHelper, Vector2[] points, int numPoints, float width, Color color, bool animation = false)
    {
        if (points.Length < 4)
        {
            return new List<Vector2>();
        }

        var pointList = new List<Vector2>(numPoints + 1);

        for (int ix = 0; ix <= numPoints; ++ix)
        {
            pointList.Add(CalculateCubicBezierPoint(1f / numPoints * ix, points[0], points[1], points[2], points[3]));
        }

        return AddLineList(vertexHelper, pointList, width, color, animation);
    }


    public static Vector3 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    public static bool SetMousePositionToLocalPointInRectangle(PointerEventData data, out Vector2 globalMousePos)
    {
        RectTransform graphPane = MCEditorManager.Instance.LineParentTransform;

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            graphPane,
            data.position,
            data.pressEventCamera,
            out globalMousePos);
    }

    public static string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }

    public static string TopicHeader
    {
        get
        {
            return PlayerPrefs.GetString("TopicHeader", "/" + LoggedId);
        }
        set
        {
            PlayerPrefs.SetString("TopicHeader", value);
        }
    }

    public static string RobotTopicHeader
    {
        get
        {
            return PlayerPrefs.GetString("RobotTopicHeader", "/" + RobotId);
        }
    }

    public static int TtsSource
    {
        get
        {
            return PlayerPrefs.GetInt("TtsSource");
        }
        set
        {
            PlayerPrefs.SetInt("TtsSource", value);
        }
    }

    public static string LoggedId
    {
        get
        {
            string id = PlayerPrefs.GetString("LoggedId", "");
            if (id.Length == 0)
            {
                id = System.Guid.NewGuid().ToString();
            }
            return id;
        }
        set
        {
            PlayerPrefs.SetString("LoggedId", value);
        }
    }

    public static string RobotId
    {
        get
        {
            string id = PlayerPrefs.GetString("RobotId", "");
            if (id.Length == 0)
            {
                id = System.Guid.NewGuid().ToString();
            }
            return id;
        }
        set
        {
            PlayerPrefs.SetString("RobotId", value);
        }
    }

    public static string RosId
    {
        get
        {
            string id = PlayerPrefs.GetString("RosId", "");
            if (id.Length == 0)
            {
                id = "localhost";
            }
            return id;
        }
        set
        {
            PlayerPrefs.SetString("RosId", value);
        }
    }

    public static int UseMQTTSend
    {
        get
        {
            return PlayerPrefs.GetInt("UseMQTTSend", 0);
        }
        set
        {
            PlayerPrefs.SetInt("UseMQTTSend", value);
        }
    }

    public static int UseMQTTFeedback
    {
        get
        {
            return PlayerPrefs.GetInt("UseMQTTFeedback", 0);
        }
        set
        {
            PlayerPrefs.SetInt("UseMQTTFeedback", value);
        }
    }

    public static string WebcamName
    {
        get
        {
            return PlayerPrefs.GetString("WebcamName", "");
        }
        set
        {
            PlayerPrefs.SetString("WebcamName", value);
        }
    }

    public static int Speaker
    {
        get
        {
            return PlayerPrefs.GetInt("SPEAKER", 0);
        }
        set
        {
            PlayerPrefs.SetInt("SPEAKER", value);
        }
    }
}