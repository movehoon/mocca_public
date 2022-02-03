using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using REEL.D2EEditor;

public class Translator : Singleton<Translator>
{
    [Serializable]
    public class Request
    {
        public string[] text;
        public string model_id = "ko-en";
    }
    [Serializable]
    public class Translation
    {
        public string translation;
    }
    public class Response
    {
        public Translation[] translations;
        public int word_count;
        public int character_count;
    }

    string authenticate(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }
    public IEnumerator Translate(string text, System.Action<string> callback)
    {
        Debug.Log("[Translator:Translate] " + text);

        WWWForm form = new WWWForm();
        Request req = new Request();
        req.text = new string[1];
        req.text[0] = text;
        string jsonString = JsonUtility.ToJson(req);
        //Debug.Log("Translate json: " + jsonString);
        form.AddField("data-raw", jsonString);
        string auth = authenticate("apikey", REEL.D2E.D2EConstants.TRANSLATOR_KEY);
        //Debug.Log("auth: " + auth);

        using (UnityWebRequest www = UnityWebRequest.Post(REEL.D2E.D2EConstants.TRANSLATOR_URI, jsonString))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", auth);

            byte[] jsonBytes = new System.Text.UTF8Encoding().GetBytes(jsonString);
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                callback("_ERROR_");
            }
            else
            {
                string result = www.downloadHandler.text;
                //Debug.Log(result);
                try
                {
                    Response response = JsonUtility.FromJson<Response>(result);
                    result = response.translations[0].translation;
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.ToString());
                    result = "_ERROR_";
                }
                callback(result);
                yield return null;
            }
        }
        yield return null;
    }
}
