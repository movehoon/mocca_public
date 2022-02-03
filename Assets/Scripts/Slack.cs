using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slack : MonoBehaviour
{
	static public void SendMessage(string message , string channelName = "#exception-report")
	{
		var url = "https://slack.com/api/chat.postMessage";
		var data = new WWWForm();
		data.AddField("token", "xoxb-403931185061-909158273334-82nsxmnWJIWLSFXV5ukGyjCw");
		data.AddField("channel", channelName);

		data.AddField("username", "exception bot");

		data.AddField("text", message);

		new WWW(url, data);
	}

	internal static void SendException(string message , Exception e)
	{
		string txt = "D2E 예외발생 : " + message + "\n\n";

		txt += "Exception 정보------------\n";
		txt += string.Format("Messge = {0} \n StackTrace = {1} \n Source = {2}", e.Message, e.StackTrace, e.Source);

		SendMessage(txt);
	}


	internal static void SendException(string message, ProjectDesc projectDesc , Exception e)
	{
		string txt = "D2E 예외발생 - " + message + "\n\n";

		txt += "[Exception 정보]\n";
		txt += string.Format("메시지 = {0} \n 콜스택 = {1} \n", e.Message, e.StackTrace);

		txt += "\n[Project 정보]\n";

		if( projectDesc == null )
		{
			txt += "null\n";
		}
		else
		{
			string json = JsonUtility.ToJson(projectDesc, true);
			txt += json;

			txt += "\n[Project Logic 링크]\n";
			txt += string.Format("http://3.34.241.132:5870/projectlogic?uuid={0}", projectDesc.uuid);
		}

		SendMessage(txt);
	}

}
