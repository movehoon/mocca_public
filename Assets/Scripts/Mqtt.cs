using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using System.Security.Policy;

public class Mqtt : Singleton<Mqtt>
{
    public class MQTTMessage
    {
        public string topic;
        public string message;
    }

    private MqttClient mqttClient;
    private Dictionary<string, string> dic = new Dictionary<string, string>();

    Queue<MQTTMessage> messageQueue = new Queue<MQTTMessage>();

	DateTime sendTime = DateTime.Now;
	bool sendWait = false;

    //string _uri = "";
    //string[] _topics;
    
    public void Connect(string uri, string[] topics)
    {
        //_uri = uri;
        //_topics = topics;
        if (mqttClient != null)
        {
            if (mqttClient.IsConnected)
            {
                mqttClient.Disconnect();
            }
        }

        // 작성자 : 장세윤.
        // Mqtt 연결할 때 약간 멈추는 현상이 있어서, 비동기 방식으로 연결하도록 변경.
        //mqttClient = new MqttClient(uri, 1883, false, null, topics, ConnectToMqttAndSubscribeMessage);
        StartCoroutine(MqttConnect(uri, topics));

        // 작성자 : 장세윤.
        // 예전 코드 백업.
        //mqttClient = new MqttClient(uri, 1883, false, null);
        //mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        //string clientId = Guid.NewGuid().ToString();
        //mqttClient.Connect(clientId);
        //Debug.Log("MQTT IsConnected: " + mqttClient.IsConnected);
        //foreach (string topic in topics)
        //{
        //    ushort result = mqttClient.Subscribe(
        //        new string[] { topic },
        //        new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        //    Debug.Log("[MQTT] Subbscribe topic " + topic + ", with: " + result);
        //}
    }

    private IEnumerator MqttConnect(string uri, string[] topics)
    {
        mqttClient = new MqttClient(uri, 1883, false, null, topics, ConnectToMqttAndSubscribeMessage);
        yield return new WaitForSeconds(0.1f);
    }

    private void ConnectToMqttAndSubscribeMessage(string[] topics)
    {
        mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        mqttClient.Connect(clientId);
        Debug.Log("MQTT IsConnected: " + mqttClient.IsConnected);
        foreach (string topic in topics)
        {
            ushort result = mqttClient.Subscribe(
                new string[] { topic },
                new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            Debug.Log("[MQTT] Subbscribe topic " + topic + ", with: " + result);
        }
    }

    public bool HasGotMessage
    {
        get { return messageQueue.Count > 0; }
    }

    public MQTTMessage GetCurrentMessage()
    {
        return messageQueue.Dequeue();
    }

    public void Send(string topic, string message)
    {
        //Debug.Log("[Send]Topic: " + topic + ", Message: " + message);
        if (mqttClient == null)
        {
            return;
        }

        try
        {
            mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }

		//test
		sendTime = DateTime.Now;
		sendWait = true;
	}

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string topic = e.Topic;
        string message = System.Text.Encoding.UTF8.GetString(e.Message);
        //Debug.Log("[MQTT]Topic: " + topic + ", Message: " + message);
        messageQueue.Enqueue(new MQTTMessage() { topic = topic, message = message });
		//MQTTMessage newMessage = new MQTTMessage() { topic = topic, message = message };

		if(sendWait )
		{
			//test
			//Debug.Log(string.Format("{0:N0}MS", (DateTime.Now - sendTime).TotalMilliseconds));
			sendTime = DateTime.Now;
			sendWait = false;
		}
	}
}