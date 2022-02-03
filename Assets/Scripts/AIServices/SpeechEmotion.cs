using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpeechEmotion : Singleton<SpeechEmotion>
{
    [Serializable]
    public class ReqKeywords
    {
        public bool emotion;
    }
    [Serializable]
    public class Sentiment
    {
        public string targets;
    }
    [Serializable]
    public class Features
    {
        //public Sentiment sentiment;
        public ReqKeywords keywords;
    }
    [Serializable]
    public class Request
    {
        public string text;
        public Features features;
    }


    [Serializable]
    public class Usage
    {
        public int text_units;
        public int text_characters;
        public int features;
    }
    [Serializable]
    public class Emotion
    {
        public double anger;
        public double disgust;
        public double fear;
        public double joy;
        public double sadness;
    }
    [Serializable]
    public class Keywords
    {
        public string text;
        public float relevance;
        public Emotion emotion;
        public int count;
    }
    [Serializable]
    public class Response
    {
        public Usage usage;
        public string language;
        public Keywords[] keywords;
    }

    string authenticate(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }
    public IEnumerator FigureOut(string text, System.Action<int> callback)
    {
        Debug.Log("[IEnumerator:FigureOut] " + text);

        WWWForm form = new WWWForm();
        Request req = new Request();
        req.text = text;
        req.features = new Features();
        req.features.keywords = new ReqKeywords();
        req.features.keywords.emotion = true;
        string jsonString = JsonUtility.ToJson(req);
        Debug.Log("Translate json: " + jsonString);
        form.AddField("data-raw", jsonString);
        string auth = authenticate("apikey", REEL.D2E.D2EConstants.EMOTION_KEY);
        //Debug.Log("auth: " + auth);

        if (text.ToLower().Contains("surprise"))
        {
            callback(6);
            yield return null;
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequest.Post(REEL.D2E.D2EConstants.EMOTION_URI, jsonString))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", auth);

                byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(jsonString);
                www.uploadHandler = new UploadHandlerRaw(jsonBytes);

                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    callback(0);
                }
                else
                {
                    int ret = 0;
                    string result = www.downloadHandler.text;
                    Debug.Log(result);
                    try
                    {
                        Response response = JsonUtility.FromJson<Response>(result);
                        //Debug.Log(response.ToString());
                        //Debug.Log(response.language);
                        //Debug.Log(response.keywords.ToString());
                        //Debug.Log(response.keywords.Length);
                        //Debug.Log(response.keywords[0].text);
                        //result = response.keywords[0].emotion.joy.ToString();
                        if (response.keywords.Length > 0)
                        {
                            Emotion emo = response.keywords[0].emotion;
                            int choose = -1;
                            double score = 0;
                            if (score < emo.anger)
                            {
                                score = emo.anger;
                                choose = 1;
                            }
                            if (score < emo.disgust)
                            {
                                score = emo.disgust;
                                choose = 2;
                            }
                            if (score < emo.fear)
                            {
                                score = emo.fear;
                                choose = 3;
                            }
                            if (score < emo.joy)
                            {
                                score = emo.joy;
                                choose = 4;
                            }
                            if (score < emo.sadness)
                            {
                                score = emo.sadness;
                                choose = 5;
                            }
                            if (score > 0.3)
                            {
                                ret = choose;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.ToString());
                    }
                    callback(ret);
                    yield return null;
                }
            }
            yield return null;
        }
    }
}
