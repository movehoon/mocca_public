using REEL.D2E;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Ping : MonoBehaviour
{
	static public int MS = 0;

	public int interval = 30;

	public Image targetImege = null;


	public Sprite textureLv0 = null;
	public Sprite textureLv1 = null;
	public Sprite textureLv2 = null;
	public Sprite textureLv3 = null;
	public Sprite textureLv4 = null;
	public Sprite textureLv5 = null;
	public Sprite textureLvError = null;

	public Text textMs = null;
	public TMPro.TextMeshProUGUI tmp_textMs = null;

	public int msLv0 = 5000;
	public int msLv1 = 1000;
	public int msLv2 = 750;
	public int msLv3 = 500;
	public int msLv4 = 250;
	public int msLv5 = 100;

	DateTime sendTime = DateTime.MinValue;

	private string pingIP = "";

	public string TextMS
    {
		get
		{
			if (tmp_textMs != null)
            {
				return tmp_textMs.text;
			}

			else if (textMs != null)
            {
				return textMs.text;
            }

			return "";
		}
        set
        {
			if (tmp_textMs != null)
			{
				tmp_textMs.text = value;
			}

			else if (textMs != null)
            {
				textMs.text = value;
			}
		}
	}


	// Use this for initialization
	void Start ()
	{
		pingIP = "http://" + D2EConstants.SERVER_IP + ":5000/echo?echo=ping";

		StartCoroutine(Test());
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	void SetText(string text)
	{
		//if (textMs == null) return;

		//textMs.text = text;
		TextMS = text;
    }

	IEnumerator Test()
	{
		while(true)
		//for(int f=0;f<10;f++)
		{
			//Debug.Log("SEND PING!");

			sendTime = DateTime.Now;
			//using (UnityWebRequest request = UnityWebRequest.Get("http://52.78.62.151:5000/echo?echo=ping"))
			using (UnityWebRequest request = UnityWebRequest.Get(pingIP))
			{
				yield return request.SendWebRequest();

				MS = (int)(DateTime.Now - sendTime).TotalMilliseconds;

				string msg = string.Format("Ping : {0:N0}MS", MS);

				if (request.isNetworkError) // Error
				{
					SetText("Error!");
					//Debug.Log("ERROR!" + msg);
					targetImege.sprite = textureLvError;
				}
				else // Success
				{
					SetText( MS.ToString() + "ms" );
					//Debug.Log(msg);

					if (MS <= msLv5) SetLvTexture(5);
					else if(MS <= msLv4) SetLvTexture(4);
					else if (MS <= msLv3) SetLvTexture(3);
					else if (MS <= msLv2) SetLvTexture(2);
					else if (MS <= msLv1) SetLvTexture(1);
					else SetLvTexture(0);
				}

				yield return new WaitForSecondsRealtime(interval);
			}
		}
	}

	private void SetLvTexture(int v)
	{
		if (targetImege == null) return;
		switch( v )
		{
			case 0: targetImege.sprite = textureLv0; break;
			case 1: targetImege.sprite = textureLv1; break;
			case 2: targetImege.sprite = textureLv2; break;
			case 3: targetImege.sprite = textureLv3; break;
			case 4: targetImege.sprite = textureLv4; break;
			case 5: targetImege.sprite = textureLv5; break;
		}

	}
}
