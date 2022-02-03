using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;
using REEL.D2E;

public class WebSurvey : Singleton<WebSurvey>
{
    const string FILENAME = "/survey.txt";

    public InputField speechMessage;
    public AudioClip audioClip;
    public Button buttonSpeechRec;

    public REEL.Animation.RobotFacialRenderer robotFacialRenderer;

    string requested = "";

    public Text TTS;

    private Hashtable session_ident = new Hashtable();

    //RiveScript.RiveScript _rs = new RiveScript.RiveScript(utf8: true, debug: true);

    private GCSpeechRecognition _speechRecognition;

    public void SendSTT()
    {
        SendSTT2Server(speechMessage.text);
    }

    public void SendSTT(string message)
    {
        SendSTT2Server(message);
    }

    public void SendSTT2Server(string speech)
    {
        Mqtt.Instance.Send("/input", speech);
        //mqttClient.Publish("/input", Encoding.UTF8.GetBytes(speech));
    }

    public void WebLogin()
    {
        string uri = "http://localhost:3000/api/v1/login/reel";
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");
        var formData = System.Text.Encoding.UTF8.GetBytes("{\"password\": \"1234\"}");
        WWW www = new WWW(uri, formData, postHeader);
        StartCoroutine(Login(www));
    }

    IEnumerator Login(WWW www)
    {
        yield return www;
        Debug.Log(www.text);
    }

    public void WebUpload()
    {
        StartCoroutine("Upload");
    }

    IEnumerator Upload()
    {
        //string filepath = Application.dataPath + "/movie_dummy.zip";
        string filepath = Application.dataPath + "/TestProject.zip";
        if (File.Exists(filepath))
        {
            byte[] data = File.ReadAllBytes(filepath);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("file", data, "movie_dummy.zip", "application/zip"));

            //UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/api/v1/users/reel/projects/movie_dummy", formData);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/api/v1/users/reel/projects/TestProject", formData);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
        else
        {
            Debug.Log("File not exist");
        }
    }

    public void WebPlay()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        //string uri = "http://localhost:3000/api/v1/users/reel/projects/movie_dummy/play";
        string uri = "http://localhost:3000/api/v1/users/reel/projects/TestProject/play";
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
        }
    }

    private static bool _speechRecStart;
    public void OnClickedSpeechRec()
    {
        if (!_speechRecStart)
        {
            _speechRecognition.StartRecord(false);

            buttonSpeechRec.GetComponentInChildren<Text>().text = "Recognizing...";
            _speechRecStart = true;

        }
        else
        {
            //ApplySpeechContextPhrases();
            _speechRecognition.StopRecord();

            buttonSpeechRec.GetComponentInChildren<Text>().text = "Speech Recognition";
            _speechRecStart = false;
        }
    }

    //private void ApplySpeechContextPhrases()
    //{
    //    string[] phrases = _contextPhrases.text.Trim().Split(","[0]);

    //    if (phrases.Length > 0)
    //        _speechRecognition.SetContext(new List<string[]>() { phrases });
    //}


    private void Awake()
    {
        if (!PlayerPrefs.HasKey("UUID"))
        {
            PlayerPrefs.SetString("UUID", System.Guid.NewGuid().ToString());
        }
    }

    // Use this for initialization
    void Start()
    {
        SpeechRenderrer.Instance.Init();

        // Google Cloud Speech Recognition
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.SetLanguage(Enumerators.LanguageCode.ko_KR);
        _speechRecognition.RecognitionSuccessEvent += RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent += SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent += LongRecognitionSuccessEventHandler;

        //string[] topics = new string[]
        //{
        //    D2EConstants.TOPIC_TTS,
        //    D2EConstants.TOPIC_MOTION,
        //    D2EConstants.TOPIC_FACIAL,
        //    D2EConstants.TOPIC_STATUS
        //};

        //Mqtt.Instance.Connect("localhost", topics);

        // rivescript
//#if UNITY_EDITOR || UNITY_WEBGL
//        Debug.Log("UNITY_EDITOR");
//        string filepath = Application.dataPath + FILENAME;
//#elif UNITY_ANDROID
//        Debug.Log("UNITY_ANDROID");
//        string filepath = Application.persistentDataPath + FILENAME;
//#elif UNITY_IOS
//        Debug.Log("UNITY_IOS");
//        string filepath = Application.persistentDataPath + FILENAME;
//#endif
//        Debug.Log("filepath: " + filepath);
//        if (_rs.loadFile(filepath))
//        {
//            _rs.sortReplies();
//            Debug.Log("Successfully load file");

//            try
//            {
//                var r1 = _rs.reply("default", "init");
//                Debug.Log(string.Format("{0}", r1));
//            }
//            catch (System.Exception ex)
//            {
//                Debug.Log(string.Format("{0}", ex));
//            }
//        }
//        else
//        {
//            Debug.Log("Fail to load " + Application.persistentDataPath + FILENAME + " file");
//        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Mqtt.Instance.HasGotMessage)
        //{
        //    Arbitor.Instance.Insert(Mqtt.Instance.GetCurrentMessage());
        //}

        //        if (Mqtt.Instance.Available(TOPIC_TTS))
        //        {
        //            string received_tts = Mqtt.Instance.Get(TOPIC_TTS);
        //#if UNITY_EDITOR || UNITY_STANDALONE

        //            //StartCoroutine(Say(received_tts));
        //#elif UNITY_ANDROID
        //            AndroidJNI.AttachCurrentThread();
        //            SpeechRenderrer.Instance.Play(received_tts);
        //            AndroidJNI.DetachCurrentThread();
        //#endif
        //            Arbitor.Instance.Insert(received_tts);
        //            TTS.text = received_tts;
        //        }
        //        if (Mqtt.Instance.Available(TOPIC_FACIAL))
        //        {
        //            string received_facial = Mqtt.Instance.Get(TOPIC_FACIAL);
        //            robotFacialRenderer.Play(received_facial);
        //            received_facial = "";
        //        }
        //        if (Mqtt.Instance.Available(TOPIC_MOTION))
        //        {
        //            string received_motion = Mqtt.Instance.Get(TOPIC_MOTION);
        //            REEL.PoseAnimation.RobotTransformController.Instance.PlayMotion(received_motion);
        //            received_motion = "";
        //        }
    }

    private void OnDestroy()
    {
        _speechRecognition.RecognitionSuccessEvent -= RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent -= SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent -= LongRecognitionSuccessEventHandler;
    }

    private void SpeechRecognizedFailedEventHandler(string obj, long requestIndex)
    {
        Debug.Log("SpeechRecognizedFailedEventHandler: " + obj);
    }

    private void RecognitionSuccessEventHandler(RecognitionResponse obj, long requestIndex)
    {
        Debug.Log("RecognitionSuccessEventHandler: " + obj);
        if (obj != null && obj.results.Length > 0)
        {
            Debug.Log("Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript);

            SendSTT2Server(obj.results[0].alternatives[0].transcript);
        }
        else
        {
            Debug.Log("Speech Recognition succeeded! Words are no detected.");
        }
    }

    private void LongRecognitionSuccessEventHandler(OperationResponse operation, long index)
    {
        Debug.Log("LongRecognitionSuccessEventHandler: " + operation);
        if (operation != null && operation.response.results.Length > 0)
        {
            Debug.Log("Long Speech Recognition succeeded! Detected Most useful: " + operation.response.results[0].alternatives[0].transcript);

            string other = "\nDetected alternative: ";

            foreach (var result in operation.response.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    if (operation.response.results[0].alternatives[0] != alternative)
                        other += alternative.transcript + ", ";
                }
            }

            Debug.Log(other);
        }
        else
        {
            Debug.Log("Speech Recognition succeeded! Words are no detected.");
        }
    }
}