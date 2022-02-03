using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RestApiUtil : Singleton<RestApiUtil>
{

    public IEnumerator PostWithImage(string uri, Texture2D texture, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", texture.EncodeToPNG());

        Debug.Log("[RestApiUtil:PostWithImage]Upload to " + uri);
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                callback("_ERROR_");
            }
            else
            {
                string result = www.downloadHandler.text;
                Debug.Log(result);
                callback(result);
                yield return null;
            }
        }
        yield return null;
    }

    public IEnumerator PostWithImageModel(string uri, string model_url, Texture2D texture, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("url", model_url);
        form.AddBinaryData("image", texture.EncodeToPNG());

        Debug.Log("[RestApiUtil:PostWithImageModel]Upload to " + uri + " " + model_url);
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                callback("_ERROR_");
            }
            else
            {
                string result = www.downloadHandler.text;
                Debug.Log(result);
                callback(result);
                yield return null;
            }
        }
        yield return null;
    }

    public IEnumerator PostLoadModel(string uri, string model_url, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("url", model_url);

        Debug.Log("[RestApiUtil:PostWithImageModel]Upload to " + uri + " " + model_url);
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                callback("_ERROR_");
            }
            else
            {
                string result = www.downloadHandler.text;
                Debug.Log(result);
                callback(result);
                yield return null;
            }
        }
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
