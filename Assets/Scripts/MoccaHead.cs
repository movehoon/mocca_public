#define USE_ROS_BRIDGE

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using REEL.D2E;


#if (USE_ROS_BRIDGE)
using RosSharp.RosBridgeClient;
#endif

public class MoccaHead : Singleton<MoccaHead> {

    public InputField IF_rosIp;
    public InputField IF_rmtID;
    public Button ButtonSpeechRec;
    public Toggle toggleRemoteSend;
    public Toggle toggleRemoteReceive;
    public GameObject RosConnector;
    public AudioSource audioSource;
    public Toggle toggleSpeaker1;
    public Toggle toggleSpeaker2;
    public Toggle toggleSpeaker3;

    string motion_test = "{\"DoubleArrays\":[{\"time\":0.9,\"array\":[-20.0,45.0,45.0,-20.0,-45.0,-45.0,0.0,-25.0]},{\"time\":0.4,\"array\":[-20.0,75.0,55.0,-20.0,-75.0,-55.0,0.0,-25.0]}]}";
    string PROJ_Story3 = "{\"version\":0,\"title\":\"평가story3\",\"owner\":\"이동훈\",\"variables\":[{\"type\":102,\"id\":-101,\"inputs\":[],\"body\":{\"name\":\"paragraph1\",\"type\":6,\"isLocalVariable\":false,\"value\":\"[{\\\"tts\\\":\\\"옛날 옛날에 흥부와 놀부라는 형제가 살았습니다.\\\",\\\"facial\\\":\\\"smile\\\",\\\"motion\\\":\\\"hi\\\"},{\\\"tts\\\":\\\"동생 흥부는 마음이 착했지만, 형 놀부는 욕심쟁이였습니다.\\\",\\\"facial\\\":\\\"gazeright\\\",\\\"motion\\\":\\\"nodRight\\\"},{\\\"tts\\\":\\\"어느 추운 겨울날, 갑자기 형제의 아버지가 돌아가시자 놀부는 흥부의 식구를 쫓아내기로 했습니다.\\\",\\\"facial\\\":\\\"sad\\\",\\\"motion\\\":\\\"nodLeft\\\"},{\\\"tts\\\":\\\"그렇게 쫓겨난 흥부는 가족들과 비가 오면 비가 새고,\\\",\\\"facial\\\":\\\"sad\\\",\\\"motion\\\":\\\"sad\\\"},{\\\"tts\\\":\\\"창문 사이로 바람도 쌩쌩 들어오는 허름한 집에서 살게 되었습니다.\\\",\\\"facial\\\":\\\"angry\\\",\\\"motion\\\":\\\"angry\\\"}]\"},\"outputs\":[],\"nexts\":[],\"nodePosition\":{\"x\":0.0,\"y\":0.0},\"name\":\"\",\"description\":\"\",\"hasBreakPoint\":false,\"hasProcessed\":false}],\"functions\":[],\"processes\":[{\"id\":1536705841,\"priority\":5,\"nodes\":[{\"type\":200,\"id\":0,\"inputs\":[],\"body\":{\"name\":\"\",\"type\":0,\"isLocalVariable\":false,\"value\":\"\"},\"outputs\":[],\"nexts\":[{\"value\":\"NEXT\",\"next\":126772769}],\"nodePosition\":{\"x\":109.30375671386719,\"y\":-79.75125122070313},\"name\":\"\",\"description\":\"\",\"hasBreakPoint\":false,\"hasProcessed\":false},{\"type\":300,\"id\":1860291254,\"inputs\":[],\"body\":{\"name\":\"paragraph1\",\"type\":6,\"isLocalVariable\":false,\"value\":\"[{\\\"tts\\\":\\\"옛날 옛날에 흥부와 놀부라는 형제가 살았습니다.\\\",\\\"facial\\\":\\\"smile\\\",\\\"motion\\\":\\\"hi\\\"},{\\\"tts\\\":\\\"동생 흥부는 마음이 착했지만, 형 놀부는 욕심쟁이였습니다.\\\",\\\"facial\\\":\\\"gazeright\\\",\\\"motion\\\":\\\"nodRight\\\"},{\\\"tts\\\":\\\"어느 추운 겨울날, 갑자기 형제의 아버지가 돌아가시자 놀부는 흥부의 식구를 쫓아내기로 했습니다.\\\",\\\"facial\\\":\\\"sad\\\",\\\"motion\\\":\\\"nodLeft\\\"},{\\\"tts\\\":\\\"그렇게 쫓겨난 흥부는 가족들과 비가 오면 비가 새고,\\\",\\\"facial\\\":\\\"sad\\\",\\\"motion\\\":\\\"sad\\\"},{\\\"tts\\\":\\\"창문 사이로 바람도 쌩쌩 들어오는 허름한 집에서 살게 되었습니다.\\\",\\\"facial\\\":\\\"angry\\\",\\\"motion\\\":\\\"angry\\\"}]\"},\"outputs\":[{\"id\":0,\"type\":6,\"value\":\"paragraph1\",\"name\":\"\"}],\"nexts\":[],\"nodePosition\":{\"x\":342.21368408203127,\"y\":-269.43072509765627},\"name\":\"\",\"description\":\"\",\"hasBreakPoint\":false,\"hasProcessed\":false},{\"type\":2001,\"id\":1918801255,\"inputs\":[{\"id\":0,\"type\":6,\"source\":1860291254,\"subid\":0,\"default_value\":\"\",\"name\":\"\"}],\"body\":{\"name\":\"\",\"type\":0,\"isLocalVariable\":false,\"value\":\"\"},\"outputs\":[],\"nexts\":[{\"value\":\"NEXT\",\"next\":1860256413}],\"nodePosition\":{\"x\":608.1758422851563,\"y\":-113.75119018554688},\"name\":\"\",\"description\":\"\",\"hasBreakPoint\":false,\"hasProcessed\":false},{\"type\":2000,\"id\":1860256413,\"inputs\":[{\"id\":0,\"type\":3,\"source\":-1,\"subid\":-1,\"default_value\":\"이야기를 하는 로봇의 동작이나 표정이 적절했는지 평가해 주세요\",\"name\":\"\"}],\"body\":{\"name\":\"\",\"type\":0,\"isLocalVariable\":false,\"value\":\"\"},\"outputs\":[],\"nexts\":[{\"value\":\"NEXT\",\"next\":-1}],\"nodePosition\":{\"x\":887.2664184570313,\"y\":-103.75119018554688},\"name\":\"\",\"description\":\"\",\"hasBreakPoint\":false,\"hasProcessed\":false},{\"type\":2000,\"id\":126772769,\"inputs\":[{\"id\":0,\"type\":3,\"source\":-1,\"subid\":-1,\"default_value\":\"흥부와 놀부 이야기입니다\",\"name\":\"\"}],\"body\":{\"name\":\"\",\"type\":0,\"isLocalVariable\":false,\"value\":\"\"},\"outputs\":[],\"nexts\":[{\"value\":\"NEXT\",\"next\":1918801255}],\"nodePosition\":{\"x\":348.43121337890627,\"y\":-103.75119018554688},\"name\":\"\",\"description\":\"\",\"hasBreakPoint\":false,\"hasProcessed\":false}],\"name\":\"Process_1\",\"description\":\"\"}],\"language\":0,\"comments\":[]}";

    class RobotMessage
    {
        private string messageType = string.Empty;      // facial / motion.
        private string message = string.Empty;          //  표정 or 모션 이름.

        public RobotMessage() { }
        public RobotMessage(string[] robotMessage)
        {
            SetMessage(robotMessage);
        }

        public string GetMessageType { get { return messageType; } }
        public string GetMessage { get { return message; } }

        public void SetMessage(string[] robotMessage)
        {
            messageType = robotMessage[0];
            message = robotMessage[1];
        }

        public override string ToString()
        {
            return "messageType: " + messageType + ", message: " + message;
        }
    }
    
	public REEL.Animation.RobotFacialRenderer robotFacialRenderer;

	Dictionary<string, Action<string>> messageProcessors = new Dictionary<string, Action<string>>();

	public void ConnectBT(string bt_addr) {
        // ConnectBT Function
        Debug.Log("[MoccaHead]ConnectBT:" + bt_addr);
		//BluetoothManager.Instance.Connect(bt_addr);
	}

	public void SaveIpAddress() {
		//Utils.RobotId = IF_rosI.text;
        MqttRefresh();
    }

    public void ShowSpeaker(int id) {
        if (id==0) {
            toggleSpeaker1.isOn = true;
        }
        else if (id==1) {
            toggleSpeaker2.isOn = true;
        }
        else if (id==2) {
            toggleSpeaker3.isOn = true;
        }
    }
    public void SelectSpeaker(int id)
    {
        if (id==0) {
            Utils.Speaker = 0;
        }
        else if (id==1) {
            Utils.Speaker = 1;
        }
        else if (id==2) {
            Utils.Speaker = 2;
        }
    }

    public void ToggleRemoteSend()
    {
        Debug.Log("[ToggleRemoteSend]" + toggleRemoteSend.isOn);
        Arbitor.Instance.OnUseMqttSendChanged(toggleRemoteSend.isOn);
    }
    
    public void ToggleRemoteReceive()
    {
        Debug.Log("[ToggleRemoteReceive]" + toggleRemoteReceive.isOn);
        Arbitor.Instance.OnUseMqttFeedBackChanged(toggleRemoteReceive.isOn);
    }

    public void MoveMobile(string direction)
    {
        Debug.Log("[MoccaHead:MoveMobile] " + direction);
#if (USE_ROS_BRIDGE)
        CmdVelPublisher cmdVel = RosConnector.GetComponentInChildren<CmdVelPublisher>();
        if (direction.Contains("forward"))
        {
            cmdVel.SendMobility(0.5f, 0.0f);
        }
        else if (direction.Contains("backward"))
        {
            cmdVel.SendMobility(-0.5f, 0.0f);
        }
        else if (direction.Contains("left"))
        {
            cmdVel.SendMobility(0.0f, 0.5f);
        }
        else if (direction.Contains("right"))
        {
            cmdVel.SendMobility(0.0f, -0.5f);
        }
        else if (direction.Contains("stop"))
        {
            cmdVel.SendMobility(0.0f, 0.0f);
        }
#endif
    }


    public void PlayMotionName(string motion_name)
    {
        Debug.Log("[MoccaHead:PlayMotionName] " + motion_name);
#if (USE_ROS_BRIDGE)
        //MotionPublisher mot = RosConnector.GetComponentInChildren<MotionPublisher>();
        //mot.PlayMotionName(motion_name);
        MotionFileManager motionFileManager = FindObjectOfType<MotionFileManager>();
        if (motionFileManager != null)
        {
            if (motionFileManager.GetMotionData(motion_name, out MotionData motionData) == true)
            {
                string motionJson = JsonUtility.ToJson(motionData);
                MotionPublisher mot = RosConnector.GetComponentInChildren<MotionPublisher>();
                mot.PlayMotionRaw(motionJson);
                Debug.Log("[MoccaHead:PlayMotionName] PlayMotionRaw: " + motionJson);
            }
        }
        else
        {
            Debug.Log("[MoccaHead:PlayMotionName] Can't find file");
        }
#endif
    }

    public void PlayMotionData(string motion_data)
    {
        Debug.Log("PlayMotionData: " + motion_data);
#if (USE_ROS_BRIDGE)
        MotionPublisher mot = RosConnector.GetComponentInChildren<MotionPublisher>();
        mot.PlayMotionRaw(motion_data);
#endif
    }

    public void RosReconnect()
    {
        Debug.Log("MoccaHead::RosReconnect");
        Utils.RosId = IF_rosIp.text;
#if (USE_ROS_BRIDGE)
        //RosConnector.GetComponentInChildren<RosConnector>().Reconnect();
#endif
    }

    public void SetRmtID()
    {
        Debug.Log("MoccaHead::RmtID");
        Utils.LoggedId = IF_rmtID.text;
    }

    public void OnButtonRecognize()
    {
        SpeechRecognition.Instance.OnButtonRecognize();
        ButtonSpeechRec.GetComponentInChildren<Text>().color = Color.red;
        ButtonSpeechRec.GetComponentInChildren<Text>().color = SpeechRecognition.Instance.IsRecognizing ? Color.red : Color.black;
    }

    public void PlayScenario()
    {
        Player.Instance.Play();
    }

    public void OnPlayProject()
    {
        Player.Instance.Play(PROJ_Story3);
    }

    void MqttRefresh()
    {
        string[] subscribe_topics = new string[]
        {
            Utils.TopicHeader + D2EConstants.TOPIC_TTS,
            Utils.TopicHeader + D2EConstants.TOPIC_TTS_FEEDBACK,
            Utils.TopicHeader + D2EConstants.TOPIC_MOTION,
            Utils.TopicHeader + D2EConstants.TOPIC_MOBILITY,
            Utils.TopicHeader + D2EConstants.TOPIC_FACIAL,
            Utils.TopicHeader + D2EConstants.TOPIC_AUDIO,
            Utils.TopicHeader + D2EConstants.TOPIC_STATUS,
            Utils.TopicHeader + D2EConstants.TOPIC_INPUT,
        };
        Mqtt.Instance.Connect(D2EConstants.MQTT_IP, subscribe_topics);
    }

    // Use this for initialization
    void Start () {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ShowSpeaker(Utils.Speaker);

        IF_rosIp.text = Utils.RosId;
        IF_rmtID.text = Utils.LoggedId;

        MqttRefresh();

        //InitMessageProcessors();

        SpeechRenderrer.Instance.Init();

        toggleRemoteSend.isOn = (Utils.UseMQTTSend > 0) ? true : false;
        toggleRemoteReceive.isOn = (Utils.UseMQTTFeedback > 0) ? true : false;

    }

	private void Update()
    {
        if (Mqtt.Instance == null) return;

        if (Mqtt.Instance.HasGotMessage)
        {
            Mqtt.MQTTMessage message = Mqtt.Instance.GetCurrentMessage();
            Debug.Log("topic: " + message.topic + ", msg: " + message.message);
            if (message != null)
            {
                Arbitor.Instance.Insert(message);
            }
        }

        try
        {
            string speech = SpeechRecognition.Instance.Result;
            if (speech.Length > 0)
            {
                SpeechRecognition.Instance.Clear();
                Debug.Log("[MoccaHead]Speech: " + speech);
                Arbitor.Instance.Insert(Utils.RobotTopicHeader + REEL.D2E.D2EConstants.TOPIC_INPUT, speech);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

//	void InitMessageProcessors()
//    {
//        messageProcessors.Add(Utils.RobotTopicHeader + D2EConstants.TOPIC_TTS, SpeechRenderrer.Instance.Play);
//        messageProcessors.Add(Utils.RobotTopicHeader + D2EConstants.TOPIC_FACIAL, robotFacialRenderer.Play);
//#if UNITY_ANDROID||UNITY_IOS
//        messageProcessors.Add(Utils.RobotTopicHeader + D2EConstants.TOPIC_MOTION, PlayMotion);
//        messageProcessors.Add(Utils.RobotTopicHeader + D2EConstants.TOPIC_MOBILITY, PlayMobility);
//#endif
//    }

    public void PlayMotion(string motion)
    {
        Debug.Log("PlayMotion: " + motion);
        PlayMotionName(motion);
		//BluetoothManager.Instance.Send(" \nmot:" + motion + "\n\n");
    }

    public void PlayMobility(string mobility)
    {
        Debug.Log("PlayMobility: " + mobility);
        //BluetoothManager.Instance.Send("\nmob:" + mobility + "\n\n");
    }

  //  public void Insert(Mqtt.MQTTMessage mqttMessage)
  //  {
  //      if (mqttMessage == null)
  //          Debug.LogError("mqttMessage null !!!!!!!!!!!!");
  //      ProcessCommand(mqttMessage.topic, mqttMessage.message);
  //  }

  //  void ProcessCommand(string topic, string command)
  //  {
  //      if (topic == (Utils.RobotTopicHeader + D2EConstants.TOPIC_TTS_FEEDBACK))
  //      {
		//	if(Player.Instance != null )
		//	{
		//		Player.Instance.GetTtsFeedback(command);
		//	}
		//}
  //      else if (topic == (Utils.RobotTopicHeader + D2EConstants.TOPIC_INPUT))
  //      {
  //          if (Player.Instance != null)
  //          {
  //              Player.Instance.GetInputEvent(command);
  //          }
  //      }
  //      else
  //      {
  //          Action<string> processor;
  //          Debug.Log("[MoccaHead::ProcessCommand]Topic: " + topic + ", Message: " + command);
  //          if (messageProcessors.TryGetValue(topic, out processor))
  //          {
  //              processor(command);
  //          }
  //      }
  //  }
}
