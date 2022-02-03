using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using Newtonsoft.Json;

public class SimsimiResult
{
    public string id;
    public int result;
    public string msg;
    public string response;
}

public class Program : MonoBehaviour
{
    // Script Editing
    public InputField mIfScript;

    public Text text;

    // Dialog
    public InputField mIfDialog;
    public InputField mIfSay;

    // Setting Panel
    public InputField btAddress;
    public InputField speechRecognition;
    public InputField mRecognizedWord;
    public InputField mSimsimiReply;

    const string SIMSIMI_URI = "http://sandbox.api.simsimi.com/request.p?key=418cf7fc-ea4f-4ed4-88d7-f4a900f13cba&lc=ko&ft=1.0&text=";
    // static HttpClient _simsimi = new HttpClient();


    public void OnStartSpeechRecognition()
    {
        Debug.Log("OnStartSpeechRecognition");
        SpeechRecognition.Instance.OnButtonRecognize();
    }

    public void AskSimsimi()
    {
        StartCoroutine(GetSimsimi(mRecognizedWord.text));
    }

    public void SimsimiSpeak()
    {
        if (mSimsimiReply.text.Length > 0)
        {
            Arbitor.Instance.Insert(mSimsimiReply.text);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    IEnumerator GetSimsimi(string input)
    {
        Debug.Log("Request: " + SIMSIMI_URI + input);
        UnityWebRequest request = UnityWebRequest.Get(SIMSIMI_URI + input);
        yield return request.Send();
        Debug.Log(request.downloadHandler.text);

        SimsimiResult simsimiResult = JsonUtility.FromJson<SimsimiResult>(request.downloadHandler.text);
        if (simsimiResult.result == 100)
        {
            mSimsimiReply.text = simsimiResult.response;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private static Program _instance = null;
    public static Program Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(Program)) as Program;
            }
            return _instance;
        }
    }
}
