using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EaiIntent
{
	public string result;
	public string intent;
}

public class PreProcess : MonoBehaviour {

	bool isWWWGetting = false;

	public string Run (string input) {
		Debug.Log("PreProcess: " + input);
		string url = "http://localhost:5009/intent/" + input;

		WWW www = new WWW(url);

		StartCoroutine(WaitForRequest(www));
//		while (isWWWGetting)
//			;

		return www.text.ToString();
	}

	private IEnumerator WaitForRequest(WWW www)
	{
		isWWWGetting = true;
		yield return www;
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
			EaiIntent intent = JsonUtility.FromJson<EaiIntent> (www.text);
			Debug.Log ("result: " + intent.result);
			Debug.Log ("intent: " + intent.intent);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
		isWWWGetting = false;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private static PreProcess _instance = null;
	public static PreProcess Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(PreProcess)) as PreProcess;
			}
			return _instance;
		}
	}
}
