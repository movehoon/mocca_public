using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ChatbotApi : Singleton<ChatbotApi>
{
    string feedback;
    string uuid;
    string state;

    public string Feedback
    {
        get
        {
            string _feedback = feedback;
            feedback = "";
            return _feedback;
        }
    }
    public string State
    {
        get { return state; }
    }

    public IEnumerator Open()
    {
        feedback = "";
        using (UnityWebRequest request = UnityWebRequest.Post("http://aiopen.etri.re.kr:8000/Dialog", UnityWebRequest.kHttpVerbPOST))
        {
            var reqBody = new
            {
                request_id = "access_key",
                access_key = "3a8aac5a-da19-47fd-a108-112397c8e789",
                argument = new
                {
                    method = "open_dialog",
                    name = "Mocca_Pizza",
                    access_method = "internal_data"
                }
            };
            string jsonString = JsonConvert.SerializeObject(reqBody);
            //Debug.Log("[ChatbotApi:Open]" + jsonString);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            UploadHandlerRaw uH = new UploadHandlerRaw(byteArray);
            uH.contentType = "application/json"; //this is ignored?
            request.uploadHandler = uH;

            yield return request.SendWebRequest();

            Debug.Log("[ChatbotApi:Open]responseCode: " + request.responseCode);
            if (request.isNetworkError)
            {
                feedback = "NOT_FOUND";
                Debug.Log("[ChatbotApi:Open] NetworkError");
            }
            else
            {
                Debug.Log("[GetWikiQnA] " + request.downloadHandler.text);
                dynamic json = JsonConvert.DeserializeObject(request.downloadHandler.text);
                //Debug.Log("[ChatbotApi:Open]result: " + json["request_id"]);
                if (request.responseCode == 200)
                {
                    try
                    {
                        uuid = json["return_object"]["uuid"];
                        feedback = json["return_object"]["result"]["system_text"];
                        state = json["return_object"]["result"]["state"];
                        Debug.Log("[GetWikiQnA]uuid: " + uuid);
                        Debug.Log("[GetWikiQnA]feedback: " + feedback);
                        Debug.Log("[GetWikiQnA]state: " + state);
                    }
                    catch (Exception ex)
                    {
                        state = "NOT_FOUND";
                        feedback = "NOT_FOUND";
                    }
                }
                else
                {
                    state = "ERROR";
                    feedback = "ERROR";
                }
            }
        }
    }

    public IEnumerator Request(string input)
    {
        using (UnityWebRequest request = UnityWebRequest.Post("http://aiopen.etri.re.kr:8000/Dialog", UnityWebRequest.kHttpVerbPOST))
        {
            var reqBody = new
            {
                access_key = "3a8aac5a-da19-47fd-a108-112397c8e789",
                argument = new
                {
                    method = "dialog",
                    uuid = uuid,
                    text = input
                }
            };
            string jsonString = JsonConvert.SerializeObject(reqBody);
            Debug.Log("[ChatbotApi:Request]" + jsonString);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            UploadHandlerRaw uH = new UploadHandlerRaw(byteArray);
            uH.contentType = "application/json"; //this is ignored?
            request.uploadHandler = uH;

            yield return request.SendWebRequest();

            Debug.Log("[ChatbotApi:Request]responseCode: " + request.responseCode);
            if (request.isNetworkError)
            {
                feedback = "NOT_FOUND";
                Debug.Log("[ChatbotApi:Request] NetworkError");
            }
            else
            {
                Debug.Log("[GetWikiQnA] " + request.downloadHandler.text);
                dynamic json = JsonConvert.DeserializeObject(request.downloadHandler.text);
                Debug.Log("[ChatbotApi:Request]result: " + json["request_id"]);
                if (request.responseCode == 200)
                {
                    try
                    {
                        feedback = json["return_object"]["result"]["system_text"];
                        state = json["return_object"]["result"]["state"];
                        Debug.Log("[GetWikiQnA]answer: " + feedback);
                    }
                    catch (Exception ex)
                    {
                        feedback = "그건 잘 모르겠어요";
                    }
                }
                else
                {
                    feedback = "ERROR";
                }
            }
        }
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
