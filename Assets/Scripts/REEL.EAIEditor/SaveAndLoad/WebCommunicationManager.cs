using REEL.D2E;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class WebCommunicationManager : Singleton<WebCommunicationManager>
    {
        [SerializeField] private string serverAdress = "http://localhost:3000";
        [SerializeField] private TabManager tabManager;
        [SerializeField] private GameObject robotView;
        [SerializeField] private GameObject fullscreenView;

        private bool isFullscreen = false;

        private void OnEnable()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                MessageBox.Show("[ID_MSG_NETWORK_NOT_CONNECTED]네트워크가 연결되어 있지 않습니다."); // local 추가 완료
                return;
            }

            if (Mqtt.Instance != null)
            {
                string[] topics = new string[]
                {
                    Utils.TopicHeader + D2EConstants.TOPIC_TTS,
                    Utils.TopicHeader + D2EConstants.TOPIC_TTS_FEEDBACK,
                    Utils.TopicHeader + D2EConstants.TOPIC_MOTION,
                    Utils.TopicHeader + D2EConstants.TOPIC_FACIAL,
                    Utils.TopicHeader + D2EConstants.TOPIC_STATUS,
                    Utils.TopicHeader + D2EConstants.TOPIC_INPUT,
                    Utils.TopicHeader + D2EConstants.TOPIC_AUDIO,
                };

                Mqtt.Instance.Connect(D2EConstants.MQTT_IP, topics);
            }
        }

        private void Update()
        {
            if (Mqtt.Instance == null || Application.internetReachability == NetworkReachability.NotReachable)
            {
                return;
            }

            if (Mqtt.Instance.HasGotMessage)
            {
                Mqtt.MQTTMessage message = Mqtt.Instance.GetCurrentMessage();
                if (message != null) Arbitor.Instance.Insert(message);
            }
        }

        public void ToggleFullscreenMode()
        {
            isFullscreen = !isFullscreen;
            UpdateFullscreenMode();
        }

        private void UpdateFullscreenMode()
        {
            fullscreenView.SetActive(isFullscreen);
            robotView.SetActive(!isFullscreen);
        }

        public void Login(string id, string pw, Action<string> getResult)
        {
            string uri = serverAdress + "/api/v1/login/" + id;
            WWWForm formData = new WWWForm();
            formData.AddField("password", pw);
            StartCoroutine(Login(uri, formData, getResult));
        }

        public void Logout()
        {
            string id = PlayerPrefs.GetString(D2EConstants.currentUserIDKey);
            string uri = serverAdress + "/api/v1/login/" + id + "/logout";

            StartCoroutine(Logout(uri, GetLogoutResult));
        }

        private IEnumerator Login(string url, WWWForm formData, Action<string> getResult)
        {
            WWW www = new WWW(url, formData);
            yield return www;

            getResult(www.text);
        }

        private IEnumerator Login(WWW www)
        {
            yield return www;
            Debug.Log(www.text);
        }

        private IEnumerator Logout(string url, Action<string> getResult)
        {
            WWW www = new WWW(url);
            yield return www;

            getResult(www.text);
        }

        private void GetLogoutResult(string message)
        {
            LogoutResult result = JsonUtility.FromJson<LogoutResult>(message);
            if (result.success)
                print(result.message);
        }

        public void SendSTT(string message)
        {
            SendSTT2Server(message);
        }

        private void SendSTT2Server(string speech)
        {
			Arbitor.Instance.Insert(Utils.TopicHeader + D2EConstants.TOPIC_INPUT, speech);
        }
    }
}