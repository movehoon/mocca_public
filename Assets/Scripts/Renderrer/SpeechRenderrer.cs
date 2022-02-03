using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using REEL.D2E;
using REEL.D2EEditor;
using System.Collections.Generic;

public class SpeechRenderrer : Singleton<SpeechRenderrer>, Renderrer
{
    public enum SpeakerType
    {
        none, jinho, mijin, matt, danna, nara, nhajun, ndain
    }
    // Korean Male: jinho, nminsang, nsinu, njihun
    // Korea Female: nara, mijin, njiyun, nsujin
    // Korean Male Child: nhajun
    // Korean Female Child: ndain
    // English Male: matt
    // English Female: danna
    
    public bool useLipsync = true;

    public bool testTTS = false;
    public string testText = "long long 반갑습니다";

    public string[] ttsTestList = { "안녕하세요? 만나서 반갑습니다. 저는 모카입니다.",
                                    "오늘 날씨 참 좋네요",
                                    "가나다라마바사아자차카타파하",
                                    "아에이오우",
                                    "오빤강남스타일"};


    bool isPlaying = false;
    private AudioSource audioSource;
    private WaitForSeconds waitOneSec = null;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        Init();
    }


	public static string LastTTS = "";

    int testCount = 0;

    public void TestTTS()
    {
        testCount = testCount % ttsTestList.Length;
        Play(ttsTestList[testCount]);

        testCount++;
    }

    void Start()
    {
    }

    void Update()
    {
        if (testTTS == true)
        {
            testTTS = false;
            Play(testText);

        }
    }

    public void Init()
    {
        waitOneSec = new WaitForSeconds(1f);
    }

    public bool IsRunning()
    {
        return (isPlaying || IsSpeaking);
    }

    public bool IsSpeaking
    {
        get
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            return audioSource.isPlaying;
        }
    }

    private bool IsFinished
    {
        get
        {
            return !IsSpeaking;
        }
    }

    public void Play(string speech)
    {
        Play(speech, SpeakerType.none);
    }

    public void Play(string speech, SpeakerType _speaker = SpeakerType.none, bool showInLog = true)
    {
        if (speech == "_STOP_")
        {
            StopCoroutine(Say(speech));
            Stop();
        }
        else
        {
            if (speech.Length > 0)
            {
                isPlaying = true;
            }
            LastTTS = speech;

            SpeakerType speaker = SpeakerType.nara;
            if (_speaker == SpeakerType.none) {
                if (Utils.Speaker == 0)
                {
                    if (Player.Instance.language == LocalizationManager.Language.KOR)
                    {
                        speaker = SpeakerType.nara;
                    }
                    else if (Player.Instance.language == LocalizationManager.Language.ENG)
                    {
                        speaker = SpeakerType.danna;
                    }
                    else
                    {
                        if (LocalizationManager.CurrentLanguage == LocalizationManager.Language.KOR)
                        {
                            speaker = SpeakerType.nara;
                        }
                        else
                        {
                            speaker = SpeakerType.danna;
                        }
                    }
                }
                else if (Utils.Speaker == 1)
                {
                    speaker = SpeakerType.nhajun;
                }
                else if (Utils.Speaker == 2)
                {
                    speaker = SpeakerType.ndain;
                }
            }
            else {
                speaker = _speaker;
            }
            StartCoroutine(Say(speech, speaker));
            if (LogWindow.Instance != null && showInLog)
            {
                LogWindow.Instance.PrintLog("MOCCA", speech);
            }
        }
    }

    void OnDisable()
    {
        Stop();
    }

    public void Stop()
    {
        isPlaying = false;
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }


    public static bool Validator(object sender, X509Certificate certificate, X509Chain chain,
                                  SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    IEnumerator Say(string speech, SpeakerType speaker = SpeakerType.nara)
    {
        //Debug.Log("[SpeechRenderrer::Say]" + speech + " with " + speaker.ToString());
        //#if UNITY_ANDROID
        //        string uriSpeech = Application.persistentDataPath + "/tts.mp3";
        //#else
        //        string uriSpeech = Application.dataPath + "/tts.mp3";
        //#endif
        //        File.Delete(uriSpeech);

        //ServicePointManager.ServerCertificateValidationCallback = Validator;

        string url = "https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts";
        //string url = "https://naveropenapi.apigw.ntruss.com/voice/v1/tts";
        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //request.Headers.Add("X-NCP-APIGW-API-KEY-ID", "4lk8cmcq67");
        //request.Headers.Add("X-NCP-APIGW-API-KEY", "Dnv1bksb2Trwh7DIbahih3QxFR9FOtAEdN1fPZz2");
        //request.Method = "POST";
        //byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=jinho&speed=0&text=" + speech);
        //request.ContentType = "application/x-www-form-urlencoded";
        //request.ContentLength = byteDataParams.Length;
        //Stream st = request.GetRequestStream();
        //st.Write(byteDataParams, 0, byteDataParams.Length);
        //st.Close();
        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //string status = response.StatusCode.ToString();
        ////Console.WriteLine("status=" + status);
        //using (Stream output = File.OpenWrite(uriSpeech))
        //using (Stream input = response.GetResponseStream())
        //{
        //    input.CopyTo(output);
        //}

        //WWW mp3Open = new WWW(uriSpeech);
        //while (mp3Open.isDone)
        //{
        //    yield return null;
        //}

        //byte[] mp3bytes = File.ReadAllBytes(uriSpeech);
        //audioSource.clip = Utils.GetAudioClipFromMP3ByteArray(mp3bytes);
        //audioSource.Play();

        //StopCoroutine("Feedback");
        //StartCoroutine("Feedback");

        WWWForm form = new WWWForm();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("X-NCP-APIGW-API-KEY-ID", "4lk8cmcq67");
        headers.Add("X-NCP-APIGW-API-KEY", "Dnv1bksb2Trwh7DIbahih3QxFR9FOtAEdN1fPZz2");
#if UNITY_ANDROID
        form.AddField("speaker", "nara");
#else
        form.AddField("speaker", speaker.ToString());
#endif
        form.AddField("speed", "0");
        form.AddField("text", speech);

        byte[] rawData = form.data;
        using (WWW ttsRequest = new WWW(url, rawData, headers))
        {
            yield return ttsRequest;

            if (ttsRequest.error != null)
            {
                Debug.Log(ttsRequest.error);
                StartCoroutine("Feedback");
            }
            else
            {
                audioSource.clip = Utils.GetAudioClipFromMP3ByteArray(ttsRequest.bytes);
                audioSource.Play();

                StopCoroutine("Feedback");
                if (Player.Instance.IsPlaying)
                {
                    StartCoroutine("Feedback");
                }
            }
        }

        yield return null;
    }

    IEnumerator Feedback()
    {
        while (audioSource.isPlaying)
        {
            yield return waitOneSec;

            Arbitor.Instance.Insert(Utils.TopicHeader + D2EConstants.TOPIC_TTS_FEEDBACK, "playing");
        }
        Arbitor.Instance.Insert(Utils.TopicHeader + D2EConstants.TOPIC_TTS_FEEDBACK, "done");
        isPlaying = false;
    }
}