using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2E
{
    public class D2EConstants
    {
        public static readonly string TOPIC_TTS = "/tts";
        public static readonly string TOPIC_TTS_FEEDBACK = "/tts/feedback";
        public static readonly string TOPIC_MOTION = "/motion";
        public static readonly string TOPIC_MOTION_FEEDBACK = "/motion/feedback";
	public static readonly string TOPIC_MOBILITY = "/mobility";
        public static readonly string TOPIC_FACIAL = "/facial";
        public static readonly string TOPIC_FACIAL_FEEDBACK = "/facial/feedback";
        public static readonly string TOPIC_AUDIO = "/audio";
        public static readonly string TOPIC_STATUS = "/status";
        public static readonly string TOPIC_INPUT = "/input";

        public static readonly string currentUserIDKey = "LoginID";
        public static readonly string currentProjectKey = "CurrentProject";
        public static string BaseProjectPath
        {
            get { return Application.dataPath + "/Data/"; }
        }

        public static readonly string SERVER_IP_PRIMARY = "";
        public static readonly string SERVER_IP_SECONDARY = "";
        public static readonly string SERVER_IP = SERVER_IP_PRIMARY;

        public static readonly string MQTT_IP = "";
        public static readonly string CLASSIFIER_IP = SERVER_IP;
        public static readonly string NUANCE_IP = SERVER_IP;
        public static readonly string AMAZON_IP = "";
        public static readonly string TEACHABLEMACHINE_IP = SERVER_IP;
        //public static readonly string KMU_IP = "";
        //public static readonly string KMU_IP = "";
        public static readonly string KMU_IP = "";

        public static readonly string TRANSLATOR_URI = "";
        public static readonly string TRANSLATOR_KEY = "";
        public static readonly string EMOTION_URI = "";
        public static readonly string EMOTION_KEY = "";
    }

    [System.Serializable]
    public class LoginResult
    {
        public bool success;
        public int code;
        public string message;
        public string id;

        public override string ToString()
        {
            return success.ToString() + " : " + code.ToString() + " : " + message + " : " + id;
        }
    }

    [System.Serializable]
    public class LogoutResult
    {
        public bool success;
        public int code;
        public string message;

        public override string ToString()
        {
            return success.ToString() + " : " + code.ToString() + " : " + message;
        }
    }
}