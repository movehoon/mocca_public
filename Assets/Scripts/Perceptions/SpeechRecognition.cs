using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

public class SpeechRecognition : Singleton<SpeechRecognition> {

    private GCSpeechRecognition _speechRecognition;

    public void OnButtonRecognize()
    {
        if (!_isRecognizing)
        {
            StartRecord();
        }
        else
        {
            StopRecord();
        }
        _isRecognizing = !_isRecognizing;
    }

    void StartRecord() {
        Debug.Log("[SpeechRec] StartRecord");
        _result = "";
        _speechRecognition.StartRecord(false);
    }

    void StopRecord() {
        Debug.Log("[SpeechRec] StopRecord");
        _speechRecognition.StopRecord();
    }

    private bool _isRecognizing = false;
    public bool IsRecognizing
    {
        get
        {
            return _isRecognizing;
        }
    }

    private string _result = "";
    public string Result {
        get
        {
            return _result;
        }
	}

    public void Clear()
    {
        _result = "";
    }

	public void OnRecognized (string words) {

	}

    // Use this for initialization
    void Start () {
        // Google Cloud Speech Recognition
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.SetLanguage(Enumerators.LanguageCode.ko_KR);
        _speechRecognition.RecognitionSuccessEvent += RecognitionSuccessEventHandler;
        _speechRecognition.NetworkRequestFailedEvent += SpeechRecognizedFailedEventHandler;
        _speechRecognition.LongRecognitionSuccessEvent += LongRecognitionSuccessEventHandler;

    }

    // Update is called once per frame
    void Update () {
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
            _result = obj.results[0].alternatives[0].transcript;
            //SendSTT2Server(obj.results[0].alternatives[0].transcript);
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
