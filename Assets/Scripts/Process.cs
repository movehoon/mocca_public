//#define FORCE_CALCULATE_TO_INT

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.Networking;
using Newtonsoft.Json;
using REEL.PROJECT;
using REEL.D2EEditor;
using RiveScript;
//using Unity.Barracuda;

public class Process : MonoBehaviour
{
    REEL.PROJECT.Process process;

    Dictionary<string, string> local_variables_number = null;
    Stack loopStack = new Stack();
    Stack functionStack = new Stack();

    //public NNModel modelFile_person;
    //public NNModel modelFile_pose;
    //public kmu_readback kmu_readback;
    //public ComputeShader posenetShader;
    WebCamTexture webcamtexture;

    string tts_feedback = "";
    string express_cmd = "";
    int expression_index;
    bool express_line_changed = false;
    bool node_changed = false;
    string classify_feedback = "";
    string nuance_feedback = "";
    float nuance_score = 0;
    int ekman_emotion = 0;
    static List<Rect> person_detect_result = null;
    String pose;
    bool neuance_playing = false;
    string chatbot_feedback = "";
    string userapi_feedback = "";
    string user_api_feedback = "";
    string teachable_machine_feedback = "";
    bool node_change_after_tts = false;
    string wiki_qna_feedback = "";

    const int NODE_TIMEOUT_LONG_LONG = 25;  // For AI Server such as Gender, DetectPerson...
    const int NODE_TIMEOUT_LONG = 20;        // SpeechRecognition, Say, Express
    const int NODE_TIMEOUT_GENERAL = 10;     // Say, Express
    const int NODE_TIMEOUT_SHORT = 5;        // For AI Server such as Gender, Emotion, DetectPerson...

    const string CLASSIFY_NOTFOUND = "_CLASSIFY_NOTFOUND_";

    const bool USE_SAY_AUTO_GENERATED_EXPRESSION = true;

    bool _pause = false;
    int debug_test;

    string poseRecUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/pose_rec";

    RiveScript.RiveScriptEngine rs;

    public int GetPID()
    {
        return process.id;
    }

    public void Start()
    {

    }

    public string PickRandom(List<string> input)
    {
        int len = input.Count;
        if (len > 0)
        {
            System.Random rnd = new System.Random();
            return input[rnd.Next(1, len)];
        }
        else
        {
            return "";
        }
    }

    public void SetTtsFeedback(string feedback)
    {
        tts_feedback = feedback;
    }
    public void SetExpressCmd(string cmd)
    {
        express_cmd = cmd;
    }

    public void GetInputEvent(string input)
    {
//        Debug.Log("[Process::GetInputEvent]Get: " + input);
    }

    private string GetLoopIndexName(int node_id) { return "__loop" + node_id + "_index"; }
    private string GetLoopLastName(int node_id) { return "__loop" + node_id + "_last"; }
    private string GetWhileName(int node_id) { return "__while" + node_id; }
    private string GetFunctionInputName(int node_id, int index) { return "__FUNCTION" + node_id + "_INPUT" + index; }
    private string GetFunctionOutputName(int node_id, int index) { return "__FUNCTION" + node_id + "_OUTPUT" + index; }
    public string LocalVariableName(int func_id, string name) { return "__FUNCTION" + func_id.ToString() + "_LV_" + name; }

    public void Play(REEL.PROJECT.Process _process, int nodeId = -1)
    {
        gameObject.SetActive(true);
        local_variables_number = new Dictionary<string, string>();

        process = _process;
        _pause = false;

        // 작성자: 장세윤.
        // 추가된 새 프로세스를 MCWorkSpaceManager에 등록.
        if (Player.Instance.runOnSimulator)
        {
            //if (MCWorkspaceManager.Instance != null)
            //{
            //    MCWorkspaceManager.Instance.AddNewProcess(this);
            //}
            MCPlayStateManager.Instance?.AddNewProcess(this);
        }

        // Pre-process for TeachableMachine
        //foreach (REEL.PROJECT.Node node in _process.nodes)
        //{
        //    if (node.type == NodeType.TEACHABLE_MACHINE_SERVER)
        //    {
        //        string input = FetchNodeInput(node.inputs[0]);
        //        Debug.Log("[Process:Play] TM input is " + input);
        //        string server_url = "http://" + REEL.D2E.D2EConstants.TEACHABLEMACHINE_IP + ":3000/load";
        //        StartCoroutine(RestApiUtil.Instance.PostLoadModel("server_url", input, result =>
        //        {
        //        }));

        //    }
        //}


        StopCoroutine("PlayProcess");
        StartCoroutine("PlayProcess", nodeId);
    }
    public void Stop()
    {
        _pause = true;
        StopCoroutine("PlayProcess");
    }

    public void Pause()
    {
        _pause = true;
    }

    public void Resume()
    {
        _pause = false;
    }

    public bool IsTrue(string value)
    {
        value = value.ToLower();
        if (value == "false" || value == "0")
            return false;
        return true;
    }

    REEL.PROJECT.Node FindNode(int node_id)
    {
        foreach (REEL.PROJECT.Node node in process.nodes)
        {
            if (node.id == node_id)
                return node;
        }
        return null;
    }

    [Serializable]
    class NuanceResult
    {
        public string nuance;
        public float score;

    }

    //IEnumerator GetNuance(string speech)
    //{
    //    using (UnityWebRequest request = UnityWebRequest.Get("http://" + REEL.D2E.D2EConstants.NUANCE_IP + ":5000/nuance?speech=" + speech))
    //    {
    //        //Debug.Log("NeuanceURI: " + request.url);
    //        yield return request.SendWebRequest();

    //        if (request.isNetworkError)
    //        {
    //            nuance_feedback = NUANCE_NOTFOUND;
    //            nuance_score = 0;
    //            Debug.Log("[GetNuance] NetworkError");
    //        }
    //        else
    //        {
    //            //Debug.Log("[GetNuance] Response: " + request.downloadHandler.text);
    //            try
    //            {
    //                NuanceResult result = JsonUtility.FromJson<NuanceResult>(request.downloadHandler.text);
    //                //Debug.Log("[GetNuance] Result: " + result.nuance + ", Score: " + result.score.ToString());

    //                if (result.nuance == "1")
    //                {
    //                    nuance_feedback = NUANCE_POSITIVE;
    //                }
    //                else if (result.nuance == "2")
    //                {
    //                    nuance_feedback = NUANCE_NEGATIVE;
    //                }
    //                else
    //                {
    //                    nuance_feedback = NUANCE_NOTFOUND;
    //                }
    //                nuance_score = result.score;
    //                //Debug.Log("[GetNuance] done with " + request.downloadHandler.text);
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.Log(ex.ToString());
    //            }
    //        }
    //    }
    //}

    IEnumerator Classify(string method, string input)
    {
        classify_feedback = "";
        WWWForm form = new WWWForm();
        form.AddField("method", method);
        form.AddField("input", input);
        using (UnityWebRequest request = UnityWebRequest.Post("http://" + REEL.D2E.D2EConstants.CLASSIFIER_IP + ":8080/classifier", form))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                classify_feedback = CLASSIFY_NOTFOUND;
                Debug.Log("[Classify] NetworkError");
            }
            else
            {
                classify_feedback = request.downloadHandler.text;
                Debug.Log("[Classify] done with " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator GetChatbot(string input)
    {
        using (UnityWebRequest request = UnityWebRequest.Post("https://wsapi.simsimi.com/190410/talk", UnityWebRequest.kHttpVerbPOST))
        {
            // "PEhljhEkxnDb2EfviwMUZdew4Bi54/7aOfWt0tix
            request.SetRequestHeader("x-api-key", "syIn93sbeGSXqfjGhgl+Oc1e7FtqJY+zVmkEzTC+");

            byte[] byteArray = Encoding.UTF8.GetBytes("{\"lang\": \"ko\", \"utext\":\"" + input + "\", \"atext_bad_prob_max\": 0.7}");

            UploadHandlerRaw uH = new UploadHandlerRaw(byteArray);
            uH.contentType = "application/json"; //this is ignored?
            request.uploadHandler = uH;

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                chatbot_feedback = "NOT_FOUND";
                Debug.Log("[GetChatbot] NetworkError");
            }
            else
            {
                //Debug.Log("[GetChatbot] " + request.downloadHandler.text);
                //                dynamic json = JsonConvert.DeserializeObject(request.downloadHandler.text);
                Simsim result = JsonConvert.DeserializeObject<Simsim>(request.downloadHandler.text);
                if (request.responseCode == 200)
                {
                    chatbot_feedback = result.atext;
                }
                else
                {
                    chatbot_feedback = "ERROR";
                }
            }
        }
    }

    IEnumerator GetUserApi(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                userapi_feedback = "NOT_FOUND";
                Debug.Log("[GetUserApi] NetworkError");
            }
            else
            {
                //Debug.Log("[GetUserApi] " + request.downloadHandler.text);
                if (request.responseCode == 200)
                {
                    userapi_feedback = request.downloadHandler.text;
                }
                else
                {
                    userapi_feedback = "ERROR";
                }
            }
        }
    }



    IEnumerator GetWikiQnA(string input_question)
    {
        using (UnityWebRequest request = UnityWebRequest.Post("http://aiopen.etri.re.kr:8000/WikiQA", UnityWebRequest.kHttpVerbPOST))
        {
            var reqBody = new
            {
                request_id = "mocca",
                access_key = "3a8aac5a-da19-47fd-a108-112397c8e789",
                argument = new
                {
                    type = "hybridqa",
                    question = input_question
                }
            };
            string jsonString = JsonConvert.SerializeObject(reqBody);
            //Debug.Log("[GetWikiQnA]" + jsonString);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            UploadHandlerRaw uH = new UploadHandlerRaw(byteArray);
            uH.contentType = "application/json"; //this is ignored?
            request.uploadHandler = uH;

            yield return request.SendWebRequest();

            //Debug.Log("[GetWikiQnA]responseCode: " + request.responseCode);
            if (request.isNetworkError)
            {
                wiki_qna_feedback = "NOT_FOUND";
                //Debug.Log("[GetWikiQnA] NetworkError");
            }
            else
            {
                Debug.Log("[GetWikiQnA] " + request.downloadHandler.text);
                dynamic json = JsonConvert.DeserializeObject(request.downloadHandler.text);
                //Debug.Log("[GetWikiQnA]result: " + json["request_id"]);
                if (request.responseCode == 200)
                {
                    try
                    {
                        string answer = json["return_object"]["WiKiInfo"]["AnswerInfo"][0]["answer"];
                        Debug.Log("[GetWikiQnA]answer: " + answer);
                        wiki_qna_feedback = answer;
                    }
                    catch (Exception ex)
                    {
                        wiki_qna_feedback = "그건 잘 모르겠어요";
                    }
                }
                else
                {
                    wiki_qna_feedback = "ERROR";
                }
            }
        }
    }

    public class Simsim
    {
        public string status;
        public string statusMessage;
        public Dictionary<string, string> request;
        public string atext;
        public string lang;
    };

    public string FetchNodeInput(REEL.PROJECT.Input input)
    {
//        Debug.Log("FetchNodeInput " + input.source);
        if (input.source > 0)
        {
            REEL.PROJECT.Node node = FindNode(input.source);
            if (node != null)
            {
                //                Debug.Log("[FetchNodeInput]type:" + node.type + ", id: " + node.id);
                switch (node.type)
                {
                    case NodeType.GET_ELEM:
                        {
                            string value = "";
                            try
                            {
                                string name = FetchNodeInput(node.inputs[0]);
                                Debug.Log("node_index: " + node.inputs[1]);
                                Debug.Log("node_value: " + FetchNodeInput(node.inputs[1]));
                                int index = Convert.ToInt32(FetchNodeInput(node.inputs[1]));
                                if (index >= 0)
                                {
                                    value = Player.Instance.GetElement(name, index);
                                    Debug.Log("[Process]GET_ELEM: " + value);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex.ToString());
                            }
                            return value;
                        }
                    case NodeType.GET_INDEX:
                        {
                            string name = FetchNodeInput(node.inputs[0]);
                            string value = FetchNodeInput(node.inputs[1]);
                            string index = Player.Instance.GetIndex(name, value);
                            //Debug.Log("[Process]GET_INDEX: " + index);
                            return index;
                        }
                    case NodeType.COUNT:
                        {
                            string liat_name = FetchNodeInput(node.inputs[0]);
                            //Debug.Log("[Process]COUNT name: " + liat_name);
                            int count = Player.Instance.GetCount(liat_name);
                            return Convert.ToString(count);
                        }
                    case NodeType.EXIST_ELEM:
                        {
                            string name = FetchNodeInput(node.inputs[0]);
                            string value = FetchNodeInput(node.inputs[1]);
                            bool exist = Player.Instance.ExistElement(name, value);
                            //Debug.Log("[Process]EXIST_ELEM: " + exist);
                            return Convert.ToString(exist);
                        }

                    case NodeType.GET:
                        {
                            if (node.body.type == REEL.PROJECT.DataType.LIST || node.body.type == REEL.PROJECT.DataType.EXPRESSION)
                            {
                                if (node.body.isLocalVariable)
                                {
                                    return LocalVariableName((int)functionStack.Peek(), node.body.name);
                                }
                                else
                                {
                                    return node.body.name;
                                }
                            }
                            else
                            {
                                string name = node.body.name;
                                if (node.body.isLocalVariable)
                                {
                                    name = LocalVariableName((int)functionStack.Peek(), node.body.name);
                                }
                                string value = Player.Instance.GetVariable(name);
                                return value;
                            }
                        }
                    case NodeType.SET:
                        {
                            return FetchNodeInput(node.inputs[0]);
                        }

                    // Operation
                    case NodeType.ADD:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            // 작성자: 이동훈 (2021.11.15) 정수 연산으로 재변경
#if FORCE_CALCULATE_TO_INT
                            int number1 = 0;
                            int number2 = 0;
                            try
                            {
                                number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex.ToString());
                            }
                            try
                            {
                                number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex.ToString());
                            }
                            return Convert.ToString(number1 + number2);
#else
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex.ToString());
                            }

                            try
                            {
                                number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex.ToString());
                            }
                            return Convert.ToString(number1 + number2);
#endif
                        }

                    case NodeType.SUB:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            // 작성자: 이동훈 (2021.11.15) 정수 연산으로 재변경
#if FORCE_CALCULATE_TO_INT
                            int number1 = 0;
                            int number2 = 0;
                            try
                            {
                                number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
#else
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
#endif
                            return Convert.ToString(number1 - number2);
                        }

                    case NodeType.MUL:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            // 작성자: 이동훈 (2021.11.15) 정수 연산으로 재변경
#if FORCE_CALCULATE_TO_INT
                            int number1 = 0;
                            int number2 = 0;
                            try
                            {
                                number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
#else
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
#endif
                            return Convert.ToString(number1 * number2);
                        }

                    case NodeType.DIV:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            // 작성자: 이동훈 (2021.11.15) 정수 연산으로 재변경
#if FORCE_CALCULATE_TO_INT
                            int number1 = 0;
                            int number2 = 0;
                            try
                            {
                                number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
#else
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }

                            try
                            {
                                number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
#endif
                            if (number2 != 0)
                                return Convert.ToString(number1 / number2);
                            else
                                return "";
                        }

                    case NodeType.RANDOM:
                        {
                            int generated_number = 0;
                            try
                            {
                                int number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                                int number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                                if (number2 < number1)
                                {
                                    int swap = number1;
                                    number1 = number2;
                                    number2 = swap;
                                }
                                System.Random random = new System.Random();
                                generated_number = random.Next(number1, number2 + 1);
                            }
                            catch (Exception ex)
                            {

                            }
                            //Debug.Log("Random number is " + generated_number);
                            return Convert.ToString(generated_number);
                        }

                    case NodeType.SIN:
                        {
                            double number1 = 0;
                            try
                            {
                                number1 = double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            double radian = number1 * Math.PI / 180.0;
                            return Convert.ToString(Math.Sin(radian));
                        }
                    case NodeType.COS:
                        {
                            double number1 = 0;
                            try
                            {
                                number1 = double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            double radian = number1 * Math.PI / 180.0;
                            return Convert.ToString(Math.Cos(radian));
                        }
                    case NodeType.TAN:
                        {
                            double number1 = 0;
                            try
                            {
                                number1 = double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            double radian = number1 * Math.PI / 180.0;
                            return Convert.ToString(Math.Tan(radian));
                        }

                    case NodeType.ABS:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            //int number1 = 0;
                            double number1 = 0;
                            try
                            {
                                //number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            return Convert.ToString(Math.Abs(number1));
                        }
                    case NodeType.SQRT:
                        {
                            double number1 = 0;
                            try
                            {
                                number1 = double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            return Convert.ToString(Math.Sqrt(number1));
                        }
                    case NodeType.ROUND:
                        {
                            double number1 = 0;
                            try
                            {
                                number1 = double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            return Convert.ToString(Math.Round(number1));
                        }
                    case NodeType.ROUND_UP:
                        {
                            double number1 = 0;
                            try
                            {
                                number1 = double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            return Convert.ToString(Math.Ceiling(number1));
                        }
                    case NodeType.ROUND_DOWN:
                        {
                            double number1 = 0;
                            try
                            {
                                number1 = double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            return Convert.ToString(Math.Floor(number1));
                        }
                    case NodeType.MOD:
                        {
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
                            return Convert.ToString(number1 % number2);
                        }
                    case NodeType.POWER:
                        {
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                            }
                            catch (Exception ex)
                            {

                            }
                            try
                            {
                                number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                            }
                            catch (Exception ex)
                            {

                            }
                            return Convert.ToString(Math.Pow(number1, number2));
                        }

                    // Bit Operator
                    case NodeType.AND:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            string value2 = FetchNodeInput(node.inputs[1]);
                            if (IsTrue(value1) && IsTrue(value2))
                            {
                                return "true";
                            }
                            return "false";
                        }
                    case NodeType.OR:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            string value2 = FetchNodeInput(node.inputs[1]);
                            if (IsTrue(value1) || IsTrue(value2))
                            {
                                return "true";
                            }
                            return "false";
                        }
                    case NodeType.NOT:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            if (IsTrue(value1))
                            {
                                return "false";
                            }
                            return "true";
                        }
                    case NodeType.XOR:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            string value2 = FetchNodeInput(node.inputs[1]);
                            if (IsTrue(value1) ^ IsTrue(value2))
                            {
                                return "true";
                            }
                            return "false";
                        }

                    // Compare
                    case NodeType.EQUAL:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            string value2 = FetchNodeInput(node.inputs[1]);
                            if (value1 == value2)
                            {
                                return "true";
                            }
                            else
                            {
                                return "false";
                            }
                        }
                    case NodeType.NOT_EQUAL:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            string value2 = FetchNodeInput(node.inputs[1]);
                            if (value1 != value2)
                            {
                                return "true";
                            }
                            return "false";
                        }
                    case NodeType.LESS:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            //int number1 = 0;
                            //int number2 = 0;
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                //number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                                try
                                {
                                    //number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                                    number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                                    if (number1 < number2)
                                    {
                                        return "true";
                                    }
                                }
                                catch (Exception ex) { }
                            }
                            catch (Exception ex) { }
                            return "false";
                        }
                    case NodeType.LESS_EQUAL:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            //int number1 = 0;
                            //int number2 = 0;
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                //number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                                try
                                {
                                    //number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                                    number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                                    if (number1 <= number2)
                                    {
                                        return "true";
                                    }
                                }
                                catch (Exception ex) { }
                            }
                            catch (Exception ex) { }
                            return "false";
                        }
                    case NodeType.GREATER:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            //int number1 = 0;
                            //int number2 = 0;
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                //number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                                try
                                {
                                    //number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                                    number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                                    if (number1 > number2)
                                    {
                                        return "true";
                                    }
                                }
                                catch (Exception ex) { }
                            }
                            catch (Exception ex) { }
                            return "false";
                        }
                    case NodeType.GREATER_EQUAL:
                        {
                            // 작성자: 장세윤 (2021.3.31).
                            // 소수점도 지원하기 위해 숫자 타입을 double로 변경함.
                            //int number1 = 0;
                            //int number2 = 0;
                            double number1 = 0;
                            double number2 = 0;
                            try
                            {
                                //number1 = Int32.Parse(FetchNodeInput(node.inputs[0]));
                                number1 = Double.Parse(FetchNodeInput(node.inputs[0]));
                                try
                                {
                                    //number2 = Int32.Parse(FetchNodeInput(node.inputs[1]));
                                    number2 = Double.Parse(FetchNodeInput(node.inputs[1]));
                                    if (number1 >= number2)
                                    {
                                        return "true";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.Log("Exception " + ex.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.Log("Exception " + ex.ToString());
                            }
                            return "false";
                        }

                    case NodeType.STRCAT:
                        {
                            //string input1 = FetchNodeInput(node.inputs[0]);
                            //string input2 = FetchNodeInput(node.inputs[1]);
                            //return input1 + input2;
                         
                            // 작성자: 장세윤.
                            // Concatenate Texts 노드에서 여러개 입력 처리할 수 있도록 기능 확장.
                            StringBuilder sb = new StringBuilder();
                            for (int ix = 0; ix  < node.inputs.Length; ++ix)
                            {
                                sb.Append(FetchNodeInput(node.inputs[ix]));
                            }

                            return sb.ToString();
                        }
                    case NodeType.STRLEN:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            return value1.Length.ToString();
                        }
                    case NodeType.CONTAINS:
                        {
                            string value1 = FetchNodeInput(node.inputs[0]);
                            string value2 = FetchNodeInput(node.inputs[1]);
                            return value1.Contains(value2).ToString();
                        }

                    case NodeType.LOOP:
                        {
                            return local_variables_number[GetLoopIndexName(node.id)];
                        }

                    case NodeType.SPEECH_REC:
                        {
                            return Player.Instance.GetRecognizedSpeech();
                        }

                    case NodeType.DETECT_PERSON:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_NUM_PERSON_");
                            }
                            else if (input.subid == 1)
                            {
                                return "_DETECTED_PERSON_";
                            }
                            return "";
                        }
                    case NodeType.DETECT_OBJECT:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_NUM_OBJECT_");
                            }
                            else if (input.subid == 1)
                            {
                                return Player.Instance.GetVariable("_OBJ_X_");
                            }
                            return "";
                        }
                    case NodeType.RECOG_FACE:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_PERSON_NAME_");
                            }
                            return "";
                        }
                    case NodeType.REGISTER_NAME:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_PERSON_NAME_");
                            }
                            return "";
                        }
                    case NodeType.DELETE_FACE:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_PERSON_NAME_");
                            }
                            return "";
                        }
                    case NodeType.AGE_GENDER:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_RECOGNIZED_AGE_");
                            }
                            else if (input.subid == 1)
                            {
                                return Player.Instance.GetVariable("_RECOGNIZED_GENDER_");
                            }
                            return "";
                        }
                    case NodeType.HANDS_UP:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_HANDSUP_X_");
                            }
                            else if (input.subid == 1)
                            {
                                return Player.Instance.GetVariable("_HANDSUP_PERSON_");
                            }
                            return "";
                        }
                    case NodeType.POSE_REC:
                        {
                            if (input.subid == 0)
                            {
                                return Player.Instance.GetVariable("_RECOGNIZED_POSE_");
                            }
                            return "";
                        }
                    case NodeType.COACHING:
                        {
                            return Player.Instance.GetVariable("_COACHING_RESULT_");
                        }
                    case NodeType.PICKNUM:
                        {
                            //Debug.Log("[Fetch]" + Player.Instance.GetVariable("_PICKNUM_RESULT_"));
                            return Player.Instance.GetVariable("_PICKNUM_RESULT_");
                        }
                    case NodeType.CHOICE:
                        {
                            return Player.Instance.GetVariable("_CHOICE_RESULT_");
                        }
                    case NodeType.CHAT:
                        {
                            //Debug.Log("[Fetch::CHAT]" + Player.Instance.GetVariable("_CHATBOT_OUTPUT_"));
                            return Player.Instance.GetVariable("_CHATBOT_OUTPUT_");
                        }
                    case NodeType.WIKI_QNA:
                        {
                            //Debug.Log("[Fetch::CHAT]" + Player.Instance.GetVariable("_WIKI_QNA_OUTPUT_"));
                            return Player.Instance.GetVariable("_WIKI_QNA_OUTPUT_");
                        }
                    case NodeType.USER_API:
                        {
                            //Debug.Log("[Fetch::USER_API]" + Player.Instance.GetVariable("_USERAPI_OUTPUT_"));
                            return Player.Instance.GetVariable("_USERAPI_OUTPUT_");
                        }
                    case NodeType.USER_API_CAMERA:
                        {
                            //Debug.Log("[Fetch::USER_API_CAMERA]" + Player.Instance.GetVariable("_USERAPICAMERA_OUTPUT_"));
                            return Player.Instance.GetVariable("_USERAPICAMERA_OUTPUT_");
                        }
                    case NodeType.TEACHABLE_MACHINE_SERVER:
                        {
                            //Debug.Log("[Fetch::TEACHABLE_MACHINE_SERVER]" + Player.Instance.GetVariable("_TEACHABLE_MACHINE_OUTPUT_"));
                            return Player.Instance.GetVariable("_TEACHABLE_MACHINE_OUTPUT_");
                        }
                    case NodeType.DIALOGUE:
                        {
                            string variable_name = Player.Instance.DialogOutputName(node.id);
                            return Player.Instance.GetVariable(variable_name);
                        }

                    case NodeType.BYPASS:
                        {
                            //Debug.Log("[BYPASS] type: " + node.inputs[0].type);
                            if (node.inputs[0].type == DataType.FUNCTION)
                            {
                                // return function's input
                                //Debug.Log("functionStack.Count: " + functionStack.Count);
                                int function_id = (int)functionStack.Peek();
                                string result = FetchNodeInput(FindNode(function_id).inputs[node.inputs[0].subid]);
                                //Debug.Log("[BYPASS]function's input is " + result);
                                return result;
                            }
                            else
                            {
                                string result = FetchNodeInput(node.inputs[0]);
                                //Debug.Log("[BYPASS]node.inputs[0].subid: " + node.inputs[0].subid + ", result: " + result);
                                return result;
                            }
                        }

                    case NodeType.FUNCTION:
                        {
                            //Debug.Log("[FUNCTION] function name: " + node.body.name);

                            string localVariableName = LocalVariableName(input.source, input.subid.ToString());
                            string result = Player.Instance.GetVariable(localVariableName);
                            Debug.Log("Get " + result + " from " + localVariableName);
                            return result;

                            //REEL.PROJECT.Input input0 = new REEL.PROJECT.Input();
                            //input0.id = 0;
                            //input0.type = node.outputs[0].type;
                            //input0.source = Int32.Parse(functionData.outputs[input.subid].value);
                            //input0.subid = 0;
                            //return FetchNodeInput(input0);
                        }
                }
            }
        }
        else if (input.source == 0 || input.source == -1)
        {
            return input.default_value;
        }
        else
        {
            REEL.PROJECT.Node node = FindNode(input.source);
            if (!node.body.isLocalVariable)
            {
                return Player.Instance.GetVariableName(input.source);
            }
            else
            {
                Debug.Log("LocalVar: " + node.body.name);
                return LocalVariableName((int)functionStack.Peek(), node.body.name);
            }
        }
        return "";
    }

    public Node CurrentNode { get; set; }
    private Node GetNodeFromID(int node_id)
    {
        foreach (REEL.PROJECT.Node node in process.nodes)
        {
            // Find current node using nodeID
            if (node.id == node_id)
            {
                return node;
            }
        }
        return null;
    }

    private WaitForSeconds waitPointOneSecond = new WaitForSeconds(0.1f);
    private IEnumerator PlayProcess(int nodeId = -1)
    {
        //Debug.Log("PlayProcess");

        int node_id = 0;
        int node_id_prev = -1;

        if (nodeId != -1)
        {
            node_id = nodeId;
            node_id_prev = nodeId;
            node_changed = true;
        }
        else
        {
            // Find Start Node
            foreach (REEL.PROJECT.Node node in process.nodes)
            {
                if (node.type == NodeType.START)
                {
                    node_id = node.id;
                    node_id_prev = node_id;
                    node_changed = true;
                    break;
                }
            }
        }

        bool tts_playing = false;

        DateTime startTime = new DateTime();
        DateTime startTime_Sub = new DateTime();
        int delay_second = 0;

        Debug.Log("node_id: " + node_id.ToString());

        while (true)
        {
            if (_pause)
            {
                //yield return new WaitForSeconds(0.1f);
                yield return waitPointOneSecond;
                continue;
            }

            REEL.PROJECT.Node curNode = null;
            if (curNode == null)
            {
                foreach (REEL.PROJECT.Node node in process.nodes)
                {
                    // Find current node using nodeID
                    if (node.id == node_id)
                    {
                        curNode = node;
                        // 현재 실행 중인 노드 저장.
                        // 작성자 : 장세윤.
                        CurrentNode = curNode;
                        break;
                    }
                }
            }

            if (curNode != null)
            {
                // 작성자: 장세윤
                // 중단점(Break Point).
                if (curNode != null && curNode.hasBreakPoint == true && curNode.hasProcessed == false)
                {
                    //Player.Instance.Pause();
                    if (MCWorkspaceManager.Instance != null)
                    {
                        //MCWorkspaceManager.Instance.OnPauseOrResumeButtonClicked();
                        MCPlayStateManager.Instance.OnPauseOrResumeButtonClicked();
                    }

                    curNode.hasProcessed = true;
                    continue;
                }

                //// 모카헤드에서 플레이시 MCWorkspaceManager 없을 경우 체크 - kjh
                //if (MCWorkspaceManager.Instance != null)
                //{
                //    // 실행 중인 노드를 표시하기 위해 호출
                //    // 작성자 : 장세윤.
                //    //MCWorkspaceManager.Instance.HighlightNode(GetPID(), curNode.id);
                //}

                //Debug.Log("[Player]Node: " + node_id + " type: " + curNode.type);

                switch (curNode.type)
                {
                    case NodeType.START:
                        node_id = curNode.nexts[0].next;
                        //Debug.Log("[Player]START GoTo: " + node_id);
                        break;
                    case NodeType.STOP:
                        Player.Instance.Stop();
                        if (Player.Instance.runOnSimulator)
                        {
                            if (MCWorkspaceManager.Instance != null)
                            {
                                MCWorkspaceManager.Instance.OnPlayOrStopButtonClicked();
                            }
                        }
                        break;
                    case NodeType.PAUSE:
                        {
                            int pid = Int32.Parse(FetchNodeInput(curNode.inputs[0]));
                            Player.Instance.Pause(pid);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.RESUME:
                        {
                            int pid = Int32.Parse(FetchNodeInput(curNode.inputs[0]));
                            Player.Instance.Resume(pid);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.DELAY:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;
                                delay_second = Int32.Parse(FetchNodeInput(curNode.inputs[0]));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > delay_second)
                                {
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.LOG:
                        {
                            Debug.Log("Process:LOG");
                            string log = FetchNodeInput(curNode.inputs[0]);

                            // 작성자:장세윤.
                            // 20210701.
                            // Log 블록에서 BOOL 변수 출력하도록 처리.
                            // BOOL 변수 값이 0이면 false 또는 거짓 출력
                            // BOOL 변수 값이 1이면 true 또는 참 출력
                            if (curNode.inputs[0].source != -1)
                            {
                                Node node = FindNode(curNode.inputs[0].source);
                                if (node != null && node.type == NodeType.GET)
                                {
                                    string name = node.body.name;
                                    if (node.body.isLocalVariable)
                                    {
                                        name = LocalVariableName((int)functionStack.Peek(), node.body.name);
                                    }

                                    if (Player.Instance.Variables_bool.TryGetValue(name, out string value) == true)
                                    {
                                        log = value == "0" ? "False" : "True";
                                        if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR)
                                        {
                                            log = log == "False" ? "거짓" : "참";
                                        }
                                    }
                                }
                            }

                            // 작성자:장세윤.
                            // 로그 레벨 (Log/Warning/Error) 추가.
                            string logLevel = FetchNodeInput(curNode.inputs[1]);
                            if (Player.Instance.runOnSimulator)
                            {
                                switch (logLevel)
                                {
                                    case "0": LogWindow.Instance.PrintLog("LOG", log); break;
                                    case "1": LogWindow.Instance.PrintWarning("WARN", log); break;
                                    case "2": LogWindow.Instance.PrintError("ERROR", log); break;
                                }
                            }
                            node_id = curNode.nexts[0].next;
                            break;
                        }

                    case NodeType.SET:
                        {
                            string value = "";
                            if (curNode.inputs[0].source > 0 || curNode.inputs[0].source < -1)
                            {
                                value = FetchNodeInput(curNode.inputs[0]);
                            }
                            else
                            {
                                value = curNode.inputs[0].default_value;
                            }
                            if (Player.Instance.GetVariableType(curNode.body.name) == DataType.LIST || Player.Instance.GetVariableType(curNode.body.name) == DataType.EXPRESSION)
                            {
                                string list_value = Player.Instance.GetVariable(value);
                                //Debug.Log("[Process]list_value: " + list_value);
                                string name = curNode.body.name;
                                if (curNode.body.isLocalVariable) {
                                    name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                                }
                                Player.Instance.SetVariable(name, list_value);
                            }
                            else
                            {
                                string name = curNode.body.name;
                                if (curNode.body.isLocalVariable)
                                {
                                    name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                                }
                                Player.Instance.SetVariable(name, value);
                            }
                            //Debug.Log("[Player] Set " + curNode.body.name + " to " + value);

                            node_id = curNode.nexts[0].next;
                            break;
                        }

                    case NodeType.INSERT:
                        {
                            string name = FetchNodeInput(curNode.inputs[0]);
                            if (curNode.body.isLocalVariable)
                            {
                                name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                            }
                            int index = Int32.Parse(FetchNodeInput(curNode.inputs[1]));
                            string value = FetchNodeInput(curNode.inputs[2]);
                            Player.Instance.InsertElement(name, index, value);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.REMOVE:
                        {
                            string name = FetchNodeInput(curNode.inputs[0]);
                            if (curNode.body.isLocalVariable)
                            {
                                name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                            }
                            int index = Int32.Parse(FetchNodeInput(curNode.inputs[1]));
                            Player.Instance.RemoveIndex(name, index);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.REMOVEALL:
                        {
                            string name = FetchNodeInput(curNode.inputs[0]);
                            if (curNode.body.isLocalVariable)
                            {
                                name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                            }
                            Player.Instance.RemoveAll(name);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.SET_ELEM:
                        {
                            string name = FetchNodeInput(curNode.inputs[0]);
                            if (curNode.body.isLocalVariable)
                            {
                                name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                            }
                            int index = Int32.Parse(FetchNodeInput(curNode.inputs[1]));
                            string value = FetchNodeInput(curNode.inputs[2]);
                            Player.Instance.SetElement(name, index, value);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.PUSH_ELEM:
                        {
                            string name = FetchNodeInput(curNode.inputs[0]);
                            if (curNode.body.isLocalVariable)
                            {
                                name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                            }
                            string value = FetchNodeInput(curNode.inputs[1]);
                            Player.Instance.PushElement(name, value);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.REMOVE_ELEM:
                        {
                            string name = FetchNodeInput(curNode.inputs[0]);
                            if (curNode.body.isLocalVariable)
                            {
                                name = LocalVariableName((int)functionStack.Peek(), curNode.body.name);
                            }
                            string value = FetchNodeInput(curNode.inputs[1]);
                            Player.Instance.RemoveElement(name, value);
                            node_id = curNode.nexts[0].next;
                            break;
                        }

                    case NodeType.SAY:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                tts_playing = true;
                                string speech = FetchNodeInput(curNode.inputs[0]);
                                Debug.Log("[Player]Say: " + speech);
                                if (speech.Length > 0)
                                {
                                    tts_feedback = "";
                                }
                                else
                                {
                                    tts_feedback = "done";
                                }
                                Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_TTS, speech);

                                if (USE_SAY_AUTO_GENERATED_EXPRESSION)
                                {
                                    startTime_Sub = DateTime.Now;
                                    ekman_emotion = -1;
                                    StartCoroutine(Translator.Instance.Translate(speech, result =>
                                    {
                                        Debug.Log("Translation result " + result.ToString());
                                        StartCoroutine(SpeechEmotion.Instance.FigureOut(result, emotion_code =>
                                        {
                                            ekman_emotion = emotion_code;
                                            Debug.Log("Emotion is " + emotion_code);
                                        }));
                                    }));
                                    neuance_playing = false;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    if (Player.Instance.runOnSimulator)
                                    {
                                        LogWindow.Instance.PrintError("SAY", "Timeout");
                                    }
                                }

                                if (USE_SAY_AUTO_GENERATED_EXPRESSION)
                                {
                                    if (DateTime.Now.Subtract(startTime_Sub).Seconds > NODE_TIMEOUT_SHORT)
                                    {
                                        ekman_emotion = -1;
                                    }
                                    if (ekman_emotion >= 0)
                                    {
                                        if (!neuance_playing)
                                        {
                                            Arbitor.Instance.RunAutoGeneratedFacial(ekman_emotion);
                                            Arbitor.Instance.RunAutoGeneratedMotion(ekman_emotion);
                                            neuance_playing = true;
                                        }
                                        ekman_emotion = -1;
                                    }
                                }

                                if (tts_feedback == "done")
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    //Debug.Log("[Player]Say Finished & GoTo: " + node_id);
                                }
                                // Next on timeout(10s)
                            }
                            break;
                        }

                    case NodeType.SAY_STOP:
                        {
                            Player.Instance.StopTts();
                            node_id = curNode.nexts[0].next;
                            break;
                        }

                    case NodeType.FACIAL:
                        {
                            string facial = FetchNodeInput(curNode.inputs[0]);
                            //Debug.Log("[Player]Facial: " + facial);
                            Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_FACIAL, facial);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.MOTION:
                        {
                            string motion = FetchNodeInput(curNode.inputs[0]);
                            //Debug.Log("[Player]Motion: " + motion);
                            Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_MOTION, motion);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.MOBILITY:
                        {
                            string mobility = FetchNodeInput(curNode.inputs[0]);
                            //Debug.Log("[Player]Mobility: " + mobility);
                            Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_MOBILITY, mobility);
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.EXPRESS:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                expression_index = 0;
                                express_cmd = "";
                                express_line_changed = true;
                            }
                            else
                            {
                                try
                                {
                                    if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                                    {
                                        tts_playing = false;
                                        node_id = curNode.nexts[0].next;
                                        LogWindow.Instance.PrintError("EXPRESS", "Timeout");
                                    }

                                    string expression_name = FetchNodeInput(curNode.inputs[0]);
                                    string exp = Player.Instance.GetVariable(expression_name);

                                    // 작성자: 장세윤 (2021.3.24)
                                    // Express 노드의 드롭다운이 지역 변수일 때 한번 더 검색해서 변수 값을 읽어오는 로직 추가.
                                    if (string.IsNullOrWhiteSpace(exp) == true)
                                    {
                                        expression_name = LocalVariableName((int)functionStack.Peek(), curNode.inputs[0].default_value);
                                        exp = Player.Instance.GetVariable(expression_name);
                                    }

                                    List<REEL.PROJECT.Expression> expression = JsonConvert.DeserializeObject<List<REEL.PROJECT.Expression>>(exp);
                                    if (!tts_playing)
                                    {
                                        if (express_line_changed)
                                        {
                                            startTime = DateTime.Now;
                                            startTime_Sub = DateTime.Now;

                                            // 작성자: 장세윤 (2021.03.25)
                                            // tts 대사 문자열이 null이 아닌 경우에만 tts 피드백 받도록 처리.
                                            tts_feedback = "done";
                                            if (Utils.IsNullOrEmptyOrWhiteSpace(expression[expression_index].tts) == false)
                                            {
                                                if (expression[expression_index].tts.Length > 0)
                                                {
                                                    tts_playing = true;
                                                    tts_feedback = "";
                                                    Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_TTS, expression[expression_index].tts);
                                                    express_line_changed = false;
                                                }
                                            }

                                            if (!expression[expression_index].facial.Contains("auto"))
                                            {
                                                Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_FACIAL, expression[expression_index].facial);
                                            }
                                            if (!expression[expression_index].motion.Contains("auto"))
                                            {
                                                Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_MOTION, expression[expression_index].motion);
                                            }
                                            if (expression[expression_index].facial.Contains("auto") || expression[expression_index].motion.Contains("auto"))
                                            {
                                                // 작성자: 장세윤 (2021.03.25)
                                                // tts 대사 문자열이 null이 아닌 경우에만 뉘앙스 피드백 받도록 처리.
                                                if (Utils.IsNullOrEmptyOrWhiteSpace(expression[expression_index].tts) == false)
                                                {
                                                    ekman_emotion = -1;
                                                    StartCoroutine(Translator.Instance.Translate(expression[expression_index].tts, result =>
                                                    {
                                                        Debug.Log("Translation result " + result.ToString());
                                                        StartCoroutine(SpeechEmotion.Instance.FigureOut(result, emotion_code =>
                                                        {
                                                            ekman_emotion = emotion_code;
                                                            Debug.Log("Emotion is " + emotion_code);
                                                        }));
                                                    }));
                                                    neuance_playing = false;
                                                }
                                            }
                                            else
                                            {
                                                ekman_emotion = -2;
                                            }

                                            // 작성자: 장세윤 (2021.03.25)
                                            // tts 대사 문자열이 null인 경우, 피드백을 기다리지 않고 다음 expression 으로 넘어가거나
                                            // 마지막 expression 항목인 경우 종료하도록 처리.
                                            if (tts_feedback == "done")
                                            {
                                                expression_index++;
                                                tts_playing = false;
                                                if (expression_index == expression.Count)
                                                {
                                                    node_id = curNode.nexts[0].next;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DateTime.Now.Subtract(startTime_Sub).Seconds > NODE_TIMEOUT_SHORT)
                                        {
                                            Debug.Log("[EXPRESS] Nuance feedback timeout");
                                            //node_id = curNode.nexts[0].next;
                                            ekman_emotion = -1;
                                        }

                                        if (ekman_emotion >= 0)
                                        {
                                            //Debug.Log("nuance_feedback: " + nuance_feedback);
                                            if (expression[expression_index].facial.Contains("auto"))
                                            {
                                                Arbitor.Instance.RunAutoGeneratedFacial(ekman_emotion);
                                            }
                                            if (expression[expression_index].motion.Contains("auto"))
                                            {
                                                Arbitor.Instance.RunAutoGeneratedMotion(ekman_emotion);
                                            }
                                            ekman_emotion = -1;
                                        }

                                        express_line_changed = true;
                                        if (express_cmd == "_STOP_")
                                        {
                                            Player.Instance.StopTts();
                                            tts_playing = false;
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else if (tts_feedback == "done")
                                        {
                                            expression_index++;
                                            tts_playing = false;
                                            if (expression_index == expression.Count)
                                            {
                                                node_id = curNode.nexts[0].next;
                                                //Debug.Log("[Player]EXPRESSION Finished & GoTo: " + node_id);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.EXPRESS_STOP:
                        {
                            Player.Instance.SetExpressCmd("_STOP_");
                            //expression_index = expression.Count;
                            node_id = curNode.nexts[0].next;
                            break;
                        }

                    case NodeType.LOOP:
                        {
                            string index_name = GetLoopIndexName(node_id);
                            string last_name = GetLoopLastName(node_id);
                            int index = 0;
                            int last = 0;
                            int step = 1;
                            if (curNode.inputs.Length > 1)
                            {
                                // [Input] Initial, Condition, Step
                                if (curNode.inputs[1].source > 0)
                                {
                                    int.TryParse(FetchNodeInput(curNode.inputs[1]), out last);
                                }
                                else
                                {
                                    int.TryParse(curNode.inputs[1].default_value, out last);
                                }
                                if (curNode.inputs[2].source > 0)
                                {
                                    int.TryParse(FetchNodeInput(curNode.inputs[2]), out step);
                                }
                                else
                                {
                                    int.TryParse(curNode.inputs[2].default_value, out step);
                                }
                            }
                            else
                            {
                                // [Input] Condition only
                                if (curNode.inputs[0].source > 0)
                                {
                                    int.TryParse(FetchNodeInput(curNode.inputs[0]), out last);
                                }
                                else
                                {
                                    int.TryParse(curNode.inputs[0].default_value, out last);
                                }
                            }

                            if (!local_variables_number.ContainsKey(index_name))
                            {
                                loopStack.Push(node_id);
                                if (curNode.inputs.Length > 1)
                                {
                                    if (curNode.inputs[0].source > 0)
                                    {
                                        index = int.Parse(FetchNodeInput(curNode.inputs[0]));
                                    }
                                    else
                                    {
                                        index = int.Parse(curNode.inputs[0].default_value);
                                    }
                                }
                                local_variables_number.Add(index_name, index.ToString());
                                //local_variables_number.Add(last_name, last.ToString());
                            }
                            else
                            {
                                Debug.Log("OnStep");
                                // OnStep
                                index = Int32.Parse(local_variables_number[index_name]);
                                index += step;
                            }

                            // Check exit
                            //last = Int32.Parse(local_variables_number[last_name]);
                            Debug.Log("LOOP index: " + index + ", last: " + last);

                            if (index < last)
                            {
                                // Next
                                node_id = curNode.nexts[0].next;
                                local_variables_number[index_name] = Convert.ToString(index);
                                //Debug.Log("[Player]Loop Next to: " + node_id);
                            }
                            else
                            {
                                // Exit
                                node_id = curNode.nexts[1].next;
                                //Debug.Log("[Player]Loop Exit to: " + node_id);
                                local_variables_number.Remove(index_name);
                                local_variables_number.Remove(last_name);

                                loopStack.Pop();
                            }
                            break;
                        }

                    case NodeType.WHILE:
                        {
                            string while_name = GetWhileName(node_id);
                            if (!local_variables_number.ContainsKey(while_name))
                            {
                                loopStack.Push(node_id);
                                local_variables_number.Add(while_name, "0");
                            }

                            string state = FetchNodeInput(curNode.inputs[0]);
                            if (IsTrue(state))
                            {
                                // Next
                                node_id = curNode.nexts[0].next;
                                //Debug.Log("[Player]WHILE Next to: " + node_id);
                            }
                            else
                            {
                                // Exit
                                node_id = curNode.nexts[1].next;
                                //Debug.Log("[Player]WHILE Exit to: " + node_id);
                                local_variables_number.Remove(while_name);
                                loopStack.Pop();
                            }
                        }
                        break;

                    case NodeType.SPEECH_REC:
                        if (node_changed)
                        {
                            startTime = DateTime.Now;

                            Player.Instance.ResetRecognizedSpeech();
                        }
                        else
                        {
                            if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                            {
                                tts_playing = false;
                                node_id = curNode.nexts[0].next;
                                if (Player.Instance.runOnSimulator)
                                {
                                    LogWindow.Instance.PrintWarning("SpeechRec", "Timeout");
                                }
                            }

                            if (Player.Instance.GetRecognizedSpeech().Length > 0)
                            {
                                node_id = curNode.nexts[0].next;
                            }
                        }
                        break;

                    case NodeType.YESNO:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                string input = FetchNodeInput(curNode.inputs[0]);
                                AI_MESSAGE request = new AI_MESSAGE();
                                request.method = "yesno";
                                request.input = input;
                                string jsonString = JsonUtility.ToJson(request, prettyPrint: false);
                                Debug.Log("[YesNo]" + jsonString);

                                StartCoroutine(Classify("yesno", jsonString));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[2].next;
                                    LogWindow.Instance.PrintError("YesNo", "Timeout");
                                }

                                //string result = YesNoClient.Instance.Read();
                                if (classify_feedback.Length > 0)
                                {
                                    //YesNoClient.Instance.Close();
                                    //Debug.Log("[Player] Got YesNo result " + result);
                                    try
                                    {
                                        AI_MESSAGE response = JsonUtility.FromJson<AI_MESSAGE>(classify_feedback);
                                        if (response.result.Contains("긍정"))
                                        {
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else if (response.result.Contains("부정"))
                                        {
                                            node_id = curNode.nexts[1].next;
                                        }
                                        else
                                        {
                                            node_id = curNode.nexts[2].next;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log(ex.ToString());
                                        LogWindow.Instance.PrintError("YesNo", "Can't connect to yes/no server");
                                        node_id = curNode.nexts[2].next;
                                    }
                                }
                            }
                            break;
                        }
                    case NodeType.COACHING:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                string input = FetchNodeInput(curNode.inputs[0]);
                                AI_MESSAGE request = new AI_MESSAGE();
                                request.method = "movements";
                                request.input = input;
                                string jsonString = JsonUtility.ToJson(request, prettyPrint: false);
                                //Debug.Log("[Coaching]" + jsonString);

                                StartCoroutine(Classify("movements", jsonString));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("Coaching", "Timeout");
                                }

                                if (classify_feedback.Length > 0)
                                {
                                    try
                                    {
                                        AI_MESSAGE response = JsonUtility.FromJson<AI_MESSAGE>(classify_feedback);
                                        Player.Instance.NewVariableString("_COACHING_RESULT_", response.result);
                                        //Debug.Log("[Coaching] Result: " + response.result);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log(ex.ToString());
                                        LogWindow.Instance.PrintError("Coaching", "Can't connect to coaching server");
                                        Player.Instance.NewVariableString("_COACHING_RESULT_", "");
                                    }
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.PICKNUM:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                string input = FetchNodeInput(curNode.inputs[0]);
                                AI_MESSAGE request = new AI_MESSAGE();
                                request.method = "picknum";
                                request.input = input;
                                string jsonString = JsonUtility.ToJson(request, prettyPrint: false);
                                //Debug.Log("[PickNum]" + jsonString);

                                StartCoroutine(Classify("picknum", jsonString));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("PickNum", "Timeout");
                                }

                                if (classify_feedback.Length > 0)
                                {
                                    try
                                    {
                                        AI_MESSAGE response = JsonUtility.FromJson<AI_MESSAGE>(classify_feedback);
                                        Player.Instance.NewVariableNumber("_PICKNUM_RESULT_", int.Parse(response.result).ToString());
                                        //Debug.Log("[PickNum] Result: " + response.result);
                                    }
                                    catch (FormatException ex)
                                    {
                                        Debug.Log(ex.ToString());
                                        LogWindow.Instance.PrintError("PICKNUM", "Can't recognize number");
                                        Player.Instance.NewVariableNumber("_PICKNUM_RESULT_", "-1");
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log(ex.ToString());
                                        LogWindow.Instance.PrintError("PICKNUM", "Can't connect to number server");
                                        Player.Instance.NewVariableNumber("_PICKNUM_RESULT_", "-1");
                                    }
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.CHOICE:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                string list_name = FetchNodeInput(curNode.inputs[0]);
                                string input = FetchNodeInput(curNode.inputs[1]);

                                CHOICE_INPUT choice_input = new CHOICE_INPUT();
                                choice_input.choice = new string[Player.Instance.GetCount(list_name)];
                                for (int i = 0; i < Player.Instance.GetCount(list_name); i++)
                                {
                                    choice_input.choice[i] = Player.Instance.GetElement(list_name, i);
                                }
                                choice_input.answer = input;

                                CHOICE_MESSAGE request = new CHOICE_MESSAGE();
                                request.method = "choice";
                                request.input = choice_input;
                                string jsonString = JsonUtility.ToJson(request, prettyPrint: false);
                                //Debug.Log("[Choice]" + jsonString);

                                StartCoroutine(Classify("choice", jsonString));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("Choice", "Timeout");
                                }

                                if (classify_feedback.Length > 0)
                                {
                                    YesNoClient.Instance.Close();
                                    //Debug.Log("[Choice] response: " + classify_feedback);
                                    try
                                    {
                                        AI_MESSAGE response = JsonUtility.FromJson<AI_MESSAGE>(classify_feedback);
                                        Player.Instance.NewVariableNumber("_CHOICE_RESULT_", response.result);
                                        //Debug.Log("[Choice] Result: " + response.result);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log(ex.ToString());
                                        LogWindow.Instance.PrintError("Choice", "Can't connect to choice server");
                                        Player.Instance.NewVariableNumber("_CHOICE_RESULT_", "-1");
                                    }
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.CHAT:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                chatbot_feedback = "";
                                string input = FetchNodeInput(curNode.inputs[0]);
                                StartCoroutine(GetChatbot(input));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                                {
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("CHAT", "Timeout");
                                }

                                if (chatbot_feedback.Length > 0)
                                {
                                    //Debug.Log("[CHATBOT]result: " + chatbot_feedback);
                                    Player.Instance.NewVariableString("_CHATBOT_OUTPUT_", chatbot_feedback);
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.WIKI_QNA:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                wiki_qna_feedback = "";
                                string input = FetchNodeInput(curNode.inputs[0]);
                                StartCoroutine(GetWikiQnA(input));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                                {
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("BRAINY", "Timeout");
                                }

                                if (wiki_qna_feedback.Length > 0)
                                {
                                    //Debug.Log("[USER_API]result: " + wiki_qna_feedback);
                                    Player.Instance.NewVariableString("_WIKI_QNA_OUTPUT_", wiki_qna_feedback);
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.CHATBOT_PIZZA_ORDER:
                        {
                            if (node_changed)
                            {
                                StartCoroutine(ChatbotApi.Instance.Open());
                                node_change_after_tts = false;
                            }
                            else
                            {
                                if (Player.Instance.GetRecognizedSpeech().Length > 0)
                                {
                                    string user_input = Player.Instance.GetRecognizedSpeech();
                                    //Debug.Log("User Input: " + user_input);
                                    string input = FetchNodeInput(curNode.inputs[0]);
                                    string reply = "";
                                    if (user_input.Contains(input))
                                    {
                                        node_change_after_tts = true;
                                    }
                                    else
                                    {
                                        StartCoroutine(ChatbotApi.Instance.Request(user_input));
                                    }
                                    Player.Instance.ResetRecognizedSpeech();
                                }

                                string response = ChatbotApi.Instance.Feedback;
                                if (response.Length > 0)
                                {
                                    if (response.Trim().Length > 0)
                                    {
                                        Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_TTS, response);
                                    }
                                }

                                if (node_change_after_tts)
                                {
                                    //Debug.Log("name: " + name + ", result: " + result);
                                    //Player.Instance.NewVariableString(variable_name, result);
                                    if (!SpeechRenderrer.Instance.IsRunning())
                                    {
                                        node_id = curNode.nexts[0].next;
                                    }
                                }
                                
                                if (ChatbotApi.Instance.State == "end")
                                {
                                    node_change_after_tts = true;
                                    //node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.USER_API:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                userapi_feedback = "";
                                string input = FetchNodeInput(curNode.inputs[0]);
                                StartCoroutine(GetUserApi(input));
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                                {
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("USER_API", "Timeout");
                                }

                                if (userapi_feedback.Length > 0)
                                {
                                    //Debug.Log("[USER_API]result: " + userapi_feedback);
                                    Player.Instance.NewVariableString("_USERAPI_OUTPUT_", userapi_feedback);
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            break;
                        }
                    case NodeType.USER_API_CAMERA:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                if (Webcam.Instance.IsAvailable)
                                {
                                    user_api_feedback = "";
                                    string input = FetchNodeInput(curNode.inputs[0]);
                                    //Webcam.Instance.PostPicture(input);
                                    StartCoroutine(Webcam.Instance.GetTexture(224, texture =>
                                    {
                                        if (RestApiUtil.Instance != null)
                                        {
                                            StartCoroutine(RestApiUtil.Instance.PostWithImage(input, texture, result =>
                                            {
                                                user_api_feedback = result;
                                            }));
                                        }
                                        else
                                        {
                                            user_api_feedback = "_ERROR_";
                                        }
                                    }));
                                    //yield return StartCoroutine(Webcam.Instance.PostPicture(input));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("USER_API_CAMERA", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                                {
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("USER_API_CAMERA", "Timeout");
                                }

                                if (user_api_feedback.Length > 0)
                                {
                                    // Find Gender
                                    try
                                    {
                                        if (user_api_feedback == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("USER_API_CAMERA", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            //Debug.Log("[USER_API_CAMERA]result: " + Webcam.Instance.Result);
                                            Player.Instance.NewVariableString("_USERAPICAMERA_OUTPUT_", user_api_feedback);
                                            node_id = curNode.nexts[0].next;
                                        }
                                    }
                                    catch
                                    {
                                        node_id = curNode.nexts[0].next;
                                    }
                                }
                            }
                            break;
                        }

                    case NodeType.TEACHABLE_MACHINE_SERVER:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                if (Webcam.Instance.IsAvailable)
                                {
                                    teachable_machine_feedback = "";
                                    string server_url = "http://" + REEL.D2E.D2EConstants.TEACHABLEMACHINE_IP + ":3000/predict";
                                    Debug.Log("TM url: " + server_url);
                                    string input = FetchNodeInput(curNode.inputs[0]);
                                    StartCoroutine(Webcam.Instance.GetTexture(224, texture =>
                                    {
                                        if (RestApiUtil.Instance != null)
                                        {
                                            StartCoroutine(RestApiUtil.Instance.PostWithImageModel(server_url, input, texture, result =>
                                            {
                                                teachable_machine_feedback = result;
                                            }));
                                        }
                                        else
                                        {
                                            teachable_machine_feedback = "_ERROR_";
                                        }
                                    }));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("TEACHABLE_MACHINE_SERVER", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG)
                                {
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("TEACHABLE_MACHINE_SERVER", "Timeout");
                                }

                                if (teachable_machine_feedback.Length > 0)
                                {
                                    Debug.Log("TMS result " + teachable_machine_feedback);
                                    try
                                    {
                                        if (teachable_machine_feedback == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("TEACHABLE_MACHINE_SERVER", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            Player.Instance.NewVariableString("_TEACHABLE_MACHINE_OUTPUT_", teachable_machine_feedback);
                                            node_id = curNode.nexts[0].next;
                                        }
                                    }
                                    catch
                                    {
                                        node_id = curNode.nexts[0].next;
                                    }
                                }
                            }
                            break;
                        }

                    case NodeType.DIALOGUE:
                        {
#if false
                            if (node_changed)
                            {
                                StartCoroutine(ChatbotApi.Instance.Open());
                                debug_test = 0;
                            }
                            else
                            {
                                string feedback = ChatbotApi.Instance.Feedback;
                                if (feedback.Length > 0)
                                {
                                    debug_test++;
                                    if (debug_test == 1)
                                    {
                                        StartCoroutine(ChatbotApi.Instance.Request("치즈피자 라지 사이즈요"));
                                    }
                                    else if (debug_test == 2)
                                    {
                                        StartCoroutine(ChatbotApi.Instance.Request("아니"));
                                    }
                                    else if (debug_test == 3)
                                    {
                                        StartCoroutine(ChatbotApi.Instance.Request("치즈피자 라지 사이즈요"));
                                    }
                                    else if (debug_test == 4)
                                    {
                                        StartCoroutine(ChatbotApi.Instance.Request("그래"));
                                    }
                                }
                                if (ChatbotApi.Instance.State == "end")
                                {
                                    node_id = curNode.nexts[0].next;
                                }
                                yield return new WaitForSeconds(5);
                            }
#else
                            if (node_changed)
                            {
                                startTime = DateTime.Now;
                                //Debug.Log("value: " + curNode.body.value);
                                string script = curNode.body.value;
                                script += "\n\n+ request rivescript result\n- <get RESULT>\n";
                                //Debug.Log("script: " + script);
                                // Initialize rivescript
                                rs = new RiveScript.RiveScriptEngine(Config.UTF8);
                                //string value = "+ hello\n - hi\n + hi\n - yo\n + 여름\n - 여름에 역시 팥빙수죠\n";
                                rs.stream(script);
                                rs.sortReplies();

                                Player.Instance.ResetRecognizedSpeech();

                                node_change_after_tts = false;
                            }
                            else
                            {
                                if (Player.Instance.GetRecognizedSpeech().Length > 0)
                                {
                                    string user_input = Player.Instance.GetRecognizedSpeech();
                                    //Debug.Log("User Input: " + user_input);
                                    string input = FetchNodeInput(curNode.inputs[0]);
                                    string reply = "";
                                    if (user_input.Contains(input))
                                    {
                                        node_change_after_tts = true;
                                    }
                                    else
                                    {
                                        string speech = rs.reply("default", user_input);
                                        if (speech.Contains("[QUIT]"))
                                        {
                                            speech = speech.Replace("[QUIT]", "");
                                            node_change_after_tts = true;
                                        }
                                        else if (speech.Contains("ERR: No Reply"))
                                        {
                                            speech = "";
                                            if (Player.Instance.runOnSimulator)
                                            {
                                                string block_name = LocalizationManager.ConvertText("Dialogue");
                                                string log = LocalizationManager.ConvertText("[ID_MSG_NO_REPLY_MATCHED]");
                                                LogWindow.Instance.PrintWarning(block_name, log);
                                            }
                                        }
                                        if (speech.Trim().Length > 0)
                                        {
                                            Arbitor.Instance.Insert(Utils.TopicHeader + REEL.D2E.D2EConstants.TOPIC_TTS, speech);
                                        }
                                    }
                                }
                                Player.Instance.ResetRecognizedSpeech();

                                if (node_change_after_tts)
                                {
                                    string result = rs.reply("default", "request rivescript result");
                                    string variable_name = Player.Instance.DialogOutputName(curNode.id);
                                    //Debug.Log("name: " + name + ", result: " + result);
                                    Player.Instance.NewVariableString(variable_name, result);
                                    if (!SpeechRenderrer.Instance.IsRunning())
                                    {
                                        node_id = curNode.nexts[0].next;
                                    }
                                }
                            }
#endif
                        }
                        break;

                    case NodeType.IF:
                        {
                            string if_input = FetchNodeInput(curNode.inputs[0]);
                            //Debug.Log("[Player] if input: " + if_input);
                            try
                            {
                                int number = Int32.Parse(if_input);
                                if (number != 0)
                                {
                                    node_id = curNode.nexts[0].next;
                                }
                                else
                                {
                                    node_id = curNode.nexts[1].next;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (IsTrue(if_input))
                                {
                                    node_id = curNode.nexts[0].next;
                                }
                                else
                                {
                                    node_id = curNode.nexts[1].next;
                                }
                            }
                            break;
                        }

                    case NodeType.SWITCH:
                        {
                            string switch_input = FetchNodeInput(curNode.inputs[0]);
                            //Debug.Log("[Player] switch input: " + switch_input);
                            bool found = false;
                            int default_id = -1;
                            foreach (REEL.PROJECT.Next next in curNode.nexts)
                            {
                                if (switch_input == next.value)
                                {
                                    node_id = next.next;
                                    found = true;
                                }
                                if (next.value == "DEFAULT")
                                {
                                    default_id = next.next;
                                }
                            }
                            if (!found)
                            {
                                node_id = default_id;
                            }
                            break;
                        }

                    case NodeType.MATCH:
                        {
                            string input = FetchNodeInput(curNode.inputs[0]);
                            //Debug.Log("[Process] MATCH input: " + input);
                            bool matched = false;
                            int default_id = -1;
                            foreach (REEL.PROJECT.Next next in curNode.nexts)
                            {
                                if (next.value.Trim().Equals(string.Empty))
                                    continue;
//                                Regex regex = new Regex(next.value);
                                Match match = Regex.Match(input, next.value, RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    node_id = next.next;
                                    matched = true;
                                    break;
                                }
                                if (next.value == "DEFAULT")
                                {
                                    default_id = next.next;
                                }
                            }
                            if (!matched)
                            {
                                node_id = default_id;
                            }
                            break;
                        }
                    case NodeType.BREAK:
                        {
                            Debug.Log("BREAK");
                            if (loopStack.Count > 0)
                            {
                                Node node = GetNodeFromID((int)loopStack.Pop());
                                node_id = node.nexts[1].next;
                            }
                            else
                            {
                                Debug.Log("BREAK:ERR");
                                node_id = -1;
                            }
                            break;
                        }
                    case NodeType.CONTINUE:
                        {
                            Debug.Log("CONTINUE");
                            if (loopStack.Count > 0)
                            {
                                //try
                                //{
                                //    string index_name = GetLoopIndexName((int)loopStack.Peek());
                                //    int index = Int32.Parse(local_variables_number[index_name]);
                                //    index++;
                                //    local_variables_number[index_name] = index.ToString();
                                //}
                                //catch (Exception ex)
                                //{
                                //    Debug.Log(ex.ToString());
                                //}

                                Node node = GetNodeFromID((int)loopStack.Peek());
                                node_id = (int)loopStack.Peek();
                            }
                            else
                            {
                                Debug.Log("CONTINUE:ERR");
                                node_id = -1;
                            }
                            break;
                        }


                    case NodeType.GENDER:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                if (Webcam.Instance.IsAvailable)
                                {
                                    //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.SERVER_IP + ":5000/analysis");
                                    string analysisUri = "http://" + REEL.D2E.D2EConstants.SERVER_IP + ":5000/analysis";
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(analysisUri));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("Gender", "Webcam is not available");
                                    node_id = curNode.nexts[2].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    node_id = curNode.nexts[2].next;
                                    LogWindow.Instance.PrintError("Gender", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    // Find Gender
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("Gender", "Error occur on processing");
                                            node_id = curNode.nexts[2].next;
                                        }
                                        else
                                        {
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string gender = jsonObject[0]["Gender"]["Value"].ToString().Replace("\"", string.Empty);
                                            //Debug.Log("Gender: " + gender);
                                            if (gender.Equals("Male"))
                                            {
                                                node_id = curNode.nexts[0].next;
                                            }
                                            else if (gender.Equals("Female"))
                                            {
                                                node_id = curNode.nexts[1].next;
                                            }
                                            else
                                            {
                                                node_id = curNode.nexts[2].next;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        node_id = curNode.nexts[2].next;
                                    }
                                }
                            }
                            break;
                        }

                    case NodeType.EMOTION:
                        if (node_changed)
                        {
                            startTime = DateTime.Now;

                            if (Webcam.Instance.IsAvailable)
                            {
                                //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.SERVER_IP + ":5000/analysis");
                                string analysisUri = "http://" + REEL.D2E.D2EConstants.SERVER_IP + ":5000/analysis";
                                yield return StartCoroutine(Webcam.Instance.PostPicture(analysisUri));
                            }
                            else
                            {
                                LogWindow.Instance.PrintError("Emotion", "Webcam is not available");
                                node_id = curNode.nexts[7].next;
                            }
                        }
                        else
                        {
                            if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                            {
                                node_id = curNode.nexts[7].next;
                                LogWindow.Instance.PrintError("Emotion", "Timeout");
                            }

                            if (Webcam.Instance.Result.Length > 0)
                            {
                                try
                                {
                                    if (Webcam.Instance.Result == "_ERROR_")
                                    {
                                        LogWindow.Instance.PrintError("Emotion", "Error occur on processing");
                                        node_id = curNode.nexts[7].next;
                                    }
                                    else
                                    {
                                        // Find Emotion
                                        JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                        JSONObject emotions = jsonObject[0]["Emotions"];
                                        string emotion = "neuteral";
                                        float confidence = 0f;
                                        int length = emotions.Count;
                                        for (int i = 0; i < length; i++)
                                        {
                                            if (emotions[i]["Confidence"] != null)
                                            {
                                                float confi = float.Parse(emotions[i]["Confidence"].ToString());
                                                if (confidence < confi)
                                                {
                                                    confidence = confi;
                                                    emotion = emotions[i]["Type"].ToString().Replace("\"", string.Empty);
                                                }
                                            }
                                        }
                                        //Debug.Log("Emotion: " + emotion);
                                        // 수정자: 장세윤 (20211104)
                                        // 기존에 작업했던 프리팹의 순서와 실제 표정이 맞지 않아 순서 일치하도록 변경함.
                                        if (emotion.Equals("CALM"))
                                        {
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else if (emotion.Equals("SURPRISED"))
                                        {
                                            node_id = curNode.nexts[1].next;
                                        }
                                        else if (emotion.Equals("HAPPY"))
                                        {
                                            node_id = curNode.nexts[2].next;
                                        }
                                        else if (emotion.Equals("ANGRY"))
                                        {
                                            node_id = curNode.nexts[3].next;
                                        }
                                        else if (emotion.Equals("DISGUSTED"))
                                        {
                                            node_id = curNode.nexts[4].next;
                                        }
                                        else if (emotion.Equals("CONFUSED"))
                                        {
                                            node_id = curNode.nexts[5].next;
                                        }
                                        else if (emotion.Equals("SAD"))
                                        {
                                            node_id = curNode.nexts[6].next;
                                        }
                                        else
                                        {
                                            node_id = curNode.nexts[7].next;
                                        }
                                        
#region 기존 코드 백업
                                        //if (emotion.Equals("CALM"))
                                        //{
                                        //    node_id = curNode.nexts[0].next;
                                        //}
                                        //else if (emotion.Equals("SAD"))
                                        //{
                                        //    node_id = curNode.nexts[1].next;
                                        //}
                                        //else if (emotion.Equals("DISGUSTED"))
                                        //{
                                        //    node_id = curNode.nexts[2].next;
                                        //}
                                        //else if (emotion.Equals("HAPPY"))
                                        //{
                                        //    node_id = curNode.nexts[3].next;
                                        //}
                                        //else if (emotion.Equals("CONFUSED"))
                                        //{
                                        //    node_id = curNode.nexts[4].next;
                                        //}
                                        //else if (emotion.Equals("ANGRY"))
                                        //{
                                        //    node_id = curNode.nexts[5].next;
                                        //}
                                        //else if (emotion.Equals("SURPRISED"))
                                        //{
                                        //    node_id = curNode.nexts[6].next;
                                        //}
                                        //else
                                        //{
                                        //    node_id = curNode.nexts[7].next;
                                        //}
#endregion
                                    }
                                }
                                catch
                                {
                                    node_id = curNode.nexts[7].next;
                                }
                            }
                        }
                        break;

                    case NodeType.DETECT_PERSON:
                        {

                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                if (Webcam.Instance.IsAvailable)
                                {
                                    //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/detect_person");
                                    string detectPersonUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/detect_person";
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(detectPersonUri));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("DetectPerson", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG_LONG)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("DetectPerson", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("DetectPerson", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            // Find Emotion
                                            Debug.Log("get _DETECT_PERSON_");
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string num_person = jsonObject["N_Person"].ToString().Replace("\"", string.Empty);
                                            List<string> crop_person = JsonConvert.DeserializeObject<List<string>>(jsonObject["crop_img"].ToString());
                                            Debug.Log("set variable");
                                            if (Player.Instance.IsList("_DETECTED_PERSON_"))
                                            {
                                                Debug.Log("_DETECTED_PERSON_ is exist");
                                                Player.Instance.RemoveAll("_DETECTED_PERSON_");
                                                Debug.Log("_DETECTED_PERSON_ is deleted");
                                            }
                                            Debug.Log("defore _DETECTED_PERSON_ is create");
                                            Player.Instance.NewListString("_DETECTED_PERSON_", crop_person);
                                            Debug.Log("_DETECTED_PERSON_ is create");

                                            if (!Player.Instance.SetVariable("_NUM_PERSON_", num_person))
                                            {
                                                Player.Instance.NewVariableString("_NUM_PERSON_", num_person);
                                                Debug.Log("_NUM_PERSON_ is create");
                                            }
                                            Debug.Log("num_person:" + num_person);
                                            Debug.Log("crop_person : " + crop_person.ToString());
                                            node_id = curNode.nexts[0].next;
                                            Debug.Log("node_id:" + node_id);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log(ex.ToString());
                                    }
                                }
                            }
                            break;
                        }

                    case NodeType.DETECT_OBJECT:
                        {
                            string name;

                            if (node_changed && curNode.inputs[0].source == -1)
                            {
                                startTime = DateTime.Now;
                                name = FetchNodeInput(curNode.inputs[0]);

                                if (Webcam.Instance.IsAvailable)
                                {
                                    string detectObjectUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/detect_object";
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(detectObjectUri, name));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("detectObject", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else if (node_changed && curNode.inputs[0].source != -1)
                            {
                                startTime = DateTime.Now;

                                name = FetchNodeInput(curNode.inputs[0]);
                                if (Webcam.Instance.IsAvailable)
                                {
                                    string detectObjectUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/detect_object";
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(detectObjectUri, name));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("detectObject", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("detectObject", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("detectObject", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            // Find Emotion
                                            Debug.Log("get _NUM_OBJECT_");
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string num_obj = jsonObject["N_Object"].ToString().Replace("\"", string.Empty);
                                            string x = jsonObject["coords"].ToString().Replace("\"", string.Empty); Debug.Log("set variable");
                                            Debug.Log("Detect object result: " + num_obj + "coords" + x);

                                            if (!Player.Instance.SetVariable("_NUM_OBJECT_", num_obj))
                                            {
                                                Player.Instance.NewVariableString("_NUM_OBJECT_", num_obj);
                                            }
                                            if (!Player.Instance.SetVariable("_OBJ_X_", x))
                                            {
                                                Player.Instance.NewVariableString("_OBJ_X_", x);
                                            }
                                            node_id = curNode.nexts[0].next;

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Log(ex.ToString());
                                    }
                                }
                            }
                            break;
                        }

                    case NodeType.RECOG_FACE:
                        {
                            string image;
                            string user_id = FirebaseManager.CurrentUserID;

                            if (node_changed && curNode.inputs[0].source == -1)
                            {
                                startTime = DateTime.Now;                                

                                if (Webcam.Instance.IsAvailable)
                                {
                                    //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/recognition_face");
                                    string reqFaceUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/recognition_face/" + user_id;
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(reqFaceUri));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("RecogFace", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else if (node_changed && curNode.inputs[0].source != -1)
                            {
                                startTime = DateTime.Now;

                                image = FetchNodeInput(curNode.inputs[0]);
                                if (Webcam.Instance.IsAvailable)
                                {
                                    Webcam.Instance.sendPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/recognition_face/" + user_id, image);
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("RecogFace", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("RecogFace", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("RecogFace", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            // Find Emotion
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string name = jsonObject["Name"].ToString().Replace("\"", string.Empty);
                                            if (name == "Fail")
                                            {
                                                node_id = curNode.nexts[1].next;
                                            }
                                            else
                                            {
                                                if (!Player.Instance.SetVariable("_PERSON_NAME_", name))
                                                {
                                                    Player.Instance.NewVariableString("_PERSON_NAME_", name);
                                                }
                                                node_id = curNode.nexts[0].next;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            break;
                        }
                    case NodeType.REGISTER_NAME:
                        {
                            string user_id = FirebaseManager.CurrentUserID;
                            string image;
                            string name;

                            if (node_changed && curNode.inputs[1].source == -1)
                            {
                                startTime = DateTime.Now;

                                name = FetchNodeInput(curNode.inputs[0]);
                                if (Webcam.Instance.IsAvailable)
                                {
                                    //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/regist_name", name);
                                    string registNameUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/regist_name";
                                    // yield return StartCoroutine(Webcam.Instance.PostPicture(registNameUri, name));
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(registNameUri, user_id + '+' + name));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("RegisterName", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else if (node_changed && curNode.inputs[1].source != -1)
                            {
                                startTime = DateTime.Now;

                                name = FetchNodeInput(curNode.inputs[0]);
                                image = FetchNodeInput(curNode.inputs[1]);
                                Debug.Log("name : " + name);
                                if (Webcam.Instance.IsAvailable)
                                {
                                    // Check again
                                    Webcam.Instance.sendPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/regist_name", image, user_id + '+' + name);
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("RegisterName", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("RegisterName", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("RegisterName", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            name = FetchNodeInput(curNode.inputs[0]);
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string isregist = jsonObject["Result"].ToString().Replace("\"", string.Empty);
                                            Debug.Log("regist name result: " + isregist);
                                            if (isregist.Equals("Fail"))
                                            {
                                                Debug.Log("regist name result: " + isregist);
                                                node_id = curNode.nexts[0].next;
                                            }
                                            else
                                            {
                                                Debug.Log("regist name result: " + isregist);
                                                if (!Player.Instance.SetVariable("_PERSON_NAME_", name))
                                                {
                                                    Debug.Log("add name : " + name);
                                                    Player.Instance.NewVariableString("_PERSON_NAME_", name);
                                                }

                                                node_id = curNode.nexts[0].next;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        Debug.Log("exception REGISTER_NAME");
                                    }
                                }
                            }
                            break;
                        }
                    case NodeType.DELETE_FACE:
                        {
                            string user_id = FirebaseManager.CurrentUserID;
                            string name = FetchNodeInput(curNode.inputs[0]);

                            string uri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/delete_face/" + user_id + '/' + name;
                            UnityWebRequest www = UnityWebRequest.Get(uri);
                            www.SendWebRequest();
                            if (www.isNetworkError || www.isHttpError)
                            {
                                Debug.Log("[DELETE_FACE]" + www.error);
                            }
                            else
                            {
                                Debug.Log("[DELETE_FACE]" + www.downloadHandler.text);
                            }
                            node_id = curNode.nexts[0].next;
                            break;
                        }
                    case NodeType.AGE_GENDER:
                        {
                            string image;
                            if (node_changed && curNode.inputs[0].source == -1)
                            {
                                startTime = DateTime.Now;

                                if (Webcam.Instance.IsAvailable)
                                {
                                    //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/age_gender");
                                    string ageGenderUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/age_gender";
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(ageGenderUri));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("AgeGender", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else if (node_changed && curNode.inputs[0].source != -1)
                            {
                                startTime = DateTime.Now;

                                image = FetchNodeInput(curNode.inputs[0]);
                                if (Webcam.Instance.IsAvailable)
                                {
                                    Webcam.Instance.sendPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/age_gender", image);
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("AgeGender", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_LONG_LONG)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("AgeGender", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("AgeGender", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string recognized_age = jsonObject[0]["Age"].ToString().Replace("\"", string.Empty);
                                            string recognzied_gender = jsonObject[0]["Gender"].ToString().Replace("\"", string.Empty);
                                            //Debug.Log("jsonObject[0] : " + jsonObject[0].ToString());
                                            //Debug.Log("recognized_age : " + recognized_age);
                                            //Debug.Log("recognzied_gender : " + recognzied_gender);
                                            if (recognized_age == "Fail" || recognzied_gender == "Fail")
                                            {
                                                node_id = curNode.nexts[1].next;
                                            }
                                            else
                                            {
                                                if (!Player.Instance.SetVariable("_RECOGNIZED_AGE_", recognized_age))
                                                {
                                                    Player.Instance.NewVariableString("_RECOGNIZED_AGE_", recognized_age);
                                                }
                                                if (!Player.Instance.SetVariable("_RECOGNIZED_GENDER_", recognzied_gender))
                                                {
                                                    Player.Instance.NewVariableString("_RECOGNIZED_GENDER_", recognzied_gender);
                                                }
                                                node_id = curNode.nexts[0].next;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        node_id = curNode.nexts[1].next;
                                    }
                                }
                            }
                            break;
                        }
                    case NodeType.HANDS_UP:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                if (Webcam.Instance.IsAvailable)
                                {
                                    //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/hands_up");
                                    string handsUpUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/hands_up";
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(handsUpUri));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("HandsUp", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("HandsUp", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("HandsUp", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string handsup = jsonObject["HandsUp"].ToString().Replace("\"", string.Empty);
                                            string x = jsonObject["x"].ToString().Replace("\"", string.Empty);
                                            Debug.Log("handsup result : " + handsup);
                                            if (handsup.Equals("Fail"))
                                            {
                                                Debug.Log("retake picture");
                                                string handsUpUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/hands_up";
                                                yield return StartCoroutine(Webcam.Instance.PostPicture(handsUpUri));
                                            }
                                            else
                                            {
                                                if (!Player.Instance.SetVariable("_HANDSUP_PERSON_", handsup))
                                                {
                                                    Player.Instance.NewVariableString("_HANDSUP_PERSON_", handsup);
                                                }
                                                if (!Player.Instance.SetVariable("_HANDSUP_X_", x))
                                                {
                                                    Player.Instance.NewVariableString("_HANDSUP_X_", x);
                                                }
                                                node_id = curNode.nexts[0].next;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                    }
                                    //catch
                                    //{
                                    //}
                                }
                            }
                            break;
                        }
                    case NodeType.POSE_REC:
                        {
                            if (node_changed)
                            {
                                startTime = DateTime.Now;

                                if (Webcam.Instance.IsAvailable)
                                {
                                    //Webcam.Instance.PostPicture("http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/hands_up");
                                    string poserectestUri = "http://" + REEL.D2E.D2EConstants.KMU_IP + ":5000/pose_rec";
                                    yield return StartCoroutine(Webcam.Instance.PostPicture(poserectestUri));
                                }
                                else
                                {
                                    LogWindow.Instance.PrintError("HandsUp", "Webcam is not available");
                                    node_id = curNode.nexts[0].next;
                                }
                            }
                            else
                            {
                                if (DateTime.Now.Subtract(startTime).Seconds > NODE_TIMEOUT_SHORT)
                                {
                                    tts_playing = false;
                                    node_id = curNode.nexts[0].next;
                                    LogWindow.Instance.PrintError("PoseRec", "Timeout");
                                }

                                if (Webcam.Instance.Result.Length > 0)
                                {
                                    try
                                    {
                                        if (Webcam.Instance.Result == "_ERROR_")
                                        {
                                            LogWindow.Instance.PrintError("PoseRec", "Error occur on processing");
                                            node_id = curNode.nexts[0].next;
                                        }
                                        else
                                        {
                                            JSONObject jsonObject = new JSONObject(Webcam.Instance.Result);
                                            string pose = jsonObject["activity"].ToString().Replace("\"", string.Empty);
                                            Debug.Log("pose result : " + pose);
                                            if (pose.Equals("Adjust the Camera Angle"))
                                            {
                                                Debug.Log("retake picture");
                                                if (!Player.Instance.SetVariable("_RECOGNIZED_POSE_", pose))
                                                {
                                                    Player.Instance.NewVariableString("_RECOGNIZED_POSE_", pose);
                                                }
                                                node_id = curNode.nexts[0].next;
                                            }
                                            else
                                            {
                                                if (!Player.Instance.SetVariable("_RECOGNIZED_POSE_", pose))
                                                {
                                                    Player.Instance.NewVariableString("_RECOGNIZED_POSE_", pose);
                                                }
                                                node_id = curNode.nexts[0].next;
                                            }
                                        }
                                    }
                                    finally
                                    {
                                    }
                                    //catch
                                    //{
                                    //}
                                }
                            }
                            break;
                        }
                    case NodeType.FUNCTION:
                        {
                            JSONObject jsonObject = new JSONObject(curNode.body.value);
                            //Debug.Log("FUNCTION: " + jsonObject.ToString());
                            node_id = Int32.Parse(jsonObject["start_id"].ToString());
                            functionStack.Push(curNode.id);
                            Player.Instance.CreateLocalVariables(curNode.id, node_id);
                        }
                        break;
                    case NodeType.FUNCTION_OUTPUT:
                        {
                            Debug.Log("FUNCTION_OUTPUT");
                            foreach (REEL.PROJECT.Input input in curNode.inputs)
                            {
                                Debug.Log("ID: " + input.id + ", TYPE: " + input.type.ToString());
                                string value = FetchNodeInput(input);
                                string localVariableName = LocalVariableName((int)functionStack.Peek(), input.id.ToString());
                                if (input.type == DataType.BOOL) {
                                    Player.Instance.NewVariableBool(localVariableName, value);
                                    Debug.Log("Save Bool");
                                }
                                else if (input.type == DataType.NUMBER)
                                {
                                    Player.Instance.NewVariableNumber(localVariableName, value);
                                    Debug.Log("Save Number");
                                }
                                else if (input.type == DataType.STRING)
                                {
                                    Player.Instance.NewVariableString(localVariableName, value);
                                    Debug.Log("Save String");
                                }
                                else
                                {
                                    Player.Instance.NewVariableString(localVariableName, value);
                                }
                                Debug.Log("Save " + value + " to " + localVariableName);
                                value = Player.Instance.GetVariable(localVariableName);
                                Debug.Log("Get " + value + " from " + localVariableName);
                            }
                            node_id = -2;
                        }
                        break;
                    default:
                        Debug.Log("[ProcessProject] Unknown node type " + curNode.type + " with id " + curNode.id);
                        break;
                }
            }
            else
            {
                break;
            }


            //Debug.Log(node_id + " : " + node_id_prev);
            // PreProcess for new Node
            if (node_id != node_id_prev)
            {
                // 작성자 : 장세윤
                // 중단점 처리.
                Node node = FindNode(node_id_prev);
                if (node != null && node.hasBreakPoint)
                {
                    node.hasProcessed = false;
                }

                node_changed = true;
                //Debug.Log("[" + process.id + "]Switch node to " + node_id);
                // Check Loop Stack
                if (node_id == -1)
                {
                    if (loopStack.Count > 0)
                    {
                        // Jump to LOOP
                        node_id = (int)loopStack.Peek();
                    }
                    else
                    {
                        if (Player.Instance.runOnSimulator)
                        {
                            //MCWorkspaceManager.Instance.IsSimulation = false;
                            //if (MCWorkspaceManager.Instance != null)
                            //{
                            //    MCWorkspaceManager.Instance.OnProcessEnd(GetPID());
                            //}
                            MCPlayStateManager.Instance?.OnProcessEnd(GetPID());
                        }
                        yield return null;
                    }
                }
                else if (node_id == -2)
                {
                    int function_id = (int)functionStack.Pop();
                    if (functionStack.Count == 0)
                    {
                        functionStack.Push(function_id);
                    }
                    node_id = FindNode(function_id).nexts[0].next;

                    Debug.Log(function_id + " function end. move to " + node_id);

                    if (node_id == -1)
                    {
                        if (loopStack.Count > 0)
                        {
                            // Jump to LOOP
                            node_id = (int)loopStack.Pop();
                            loopStack.Push(node_id);
                        }
                        else
                        {
                            if (Player.Instance.runOnSimulator)
                            {
                                //MCWorkspaceManager.Instance.IsSimulation = false;
                                //if (MCWorkspaceManager.Instance != null)
                                //{
                                //    MCWorkspaceManager.Instance.OnProcessEnd(GetPID());
                                //}
                                MCPlayStateManager.Instance?.OnProcessEnd(GetPID());
                            }
                            yield return null;
                        }
                    }
                }
            }
            else
            {
                node_changed = false;

                // 작성자: 장세윤.
                // StartNode만 배치했을 때 프로그램 실행이 종료되지 않는 문제 예외처리(에디터에서).
                if (node_id == -1)
                {
                    //if (MCWorkspaceManager.Instance != null)
                    //{
                    //    MCWorkspaceManager.Instance.OnProcessEnd(GetPID());
                    //}
                    MCPlayStateManager.Instance?.OnProcessEnd(GetPID());
                }
            }

            //Debug.LogWarning($"node_id_prev: {node_id_prev} / node_id: {node_id} / GetPID(): {GetPID()}");
            node_id_prev = node_id;

            //yield return new WaitForSeconds(0.1f);
            yield return waitPointOneSecond;
        }

        //----------------------------------------
        // 프로젝트 중단 튜토리얼 이벤트 - kjh
        //----------------------------------------
        TutorialManager.SendEvent(Tutorial.CustomEvent.ProjectStopped);

        yield return null;
    }

    //public static void OnCompleteReadBack(AsyncGPUReadbackRequest request)
    //{
    //    if (request.hasError)
    //    {
    //        Debug.Log("GPU readback error detected.");
    //    }
    //    person_detect_result = Classification.Update(request.GetData<byte>().ToArray());
    //}
}
