using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
	
	public static event Action  OnLanguageChanged;  // 언어 변경시 호출되는 이벤트

	public TextAsset textFile;

	public bool useTestKey = false;

	public KeyCode testKey = KeyCode.F7;

	public enum Language
	{
		KOR,
		ENG,
	
		DEFAULT = -1
	};

	public class LocalText
	{
		public string	kor ="";
		public string	eng ="";
		public string	id ="";
		public string	group = "common";
	}


	static List<LocalText>		textList = new List<LocalText>();


	static Language _language = Language.DEFAULT;

	public static Language		CurrentLanguage
	{
		get
		{
			if (_language != Language.DEFAULT)	return _language;

			if ( PlayerPrefs.HasKey("language") == false )
			{
				if( Application.systemLanguage == SystemLanguage.Korean )
				{
					_language = Language.KOR;
				}
				else
				{
					_language = Language.ENG;
				}
			}
			else
			{
				_language = (Language)PlayerPrefs.GetInt("language");
			}

			return _language;
		}

		set
		{
			_language = value;
			PlayerPrefs.SetInt("language", (int)_language);
		}
	}


	private void Awake()
	{
		Init();
	}


	static bool CheckID(string text)
	{
		return text.StartsWith("[ID_");
	}


	static string ParseTextID(string text)
	{
		var sp = text.Split(']');
		if (sp.Length == 0) return text;

		string id = sp[0] + "]";

		return id;
	}



	public static LocalText GetLocalText(string text, string groupName = "common", string secondGroup = "common")
    {
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		int index = -1;

		if (CheckID(text)) // ID 사용일 경우
		{
			string id = ParseTextID(text);

			index = textList.FindIndex(x => x.id == id && x.group == groupName);

			if (index == -1)
			{
				index = textList.FindIndex(x => x.group == secondGroup && x.id == id);
			}
		}
		else
		{
			//먼저 같은 그룹 이름으로 찾는다.
			index = textList.FindIndex(x => x.group == groupName && (text == x.kor || text == x.eng || text == x.id));

			if (index == -1)
			{   //같은 그룹이 없으면 그냥 찾는다.
				index = textList.FindIndex(x => x.group == secondGroup && (text == x.kor || text == x.eng || text == x.id));
			}
		}

		if (index != -1)
		{
			return textList[index];
		}

		return null;
	}



	static public string ConvertText(string text,string groupName = "common", string secondGroup = "common")
	{
		if (string.IsNullOrWhiteSpace(text)) return text;

		//text = text.Trim();

		int index = -1;

		if ( CheckID(text) ) // ID 사용일 경우
		{
			string id = ParseTextID(text);

			index = textList.FindIndex(x => x.id == id && x.group == groupName);

			if( index == -1)
			{
				index = textList.FindIndex(x => x.group == secondGroup && x.id == id);
			}
		}
		else 
		{
			//먼저 같은 그룹 이름으로 찾는다.
			index = textList.FindIndex(x => x.group == groupName && (text == x.kor || text == x.eng || text == x.id) );

			if( index == -1 )
			{	//같은 그룹이 없으면 그냥 찾는다.
				index = textList.FindIndex(x => x.group == secondGroup && (text == x.kor || text == x.eng || text == x.id) );
			}
		}

		if ( index != -1 )
		{
			switch(CurrentLanguage)
			{
				case Language.KOR:	
					text = textList[index].kor;
					break;

				case Language.ENG:
					text = textList[index].eng;
					break;
			}
		}

		return text;
	}


	static public void ChangeLanguage( Language language)
	{
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif

		CurrentLanguage = language;

		LocalizeAll();

		OnLanguageChanged?.Invoke();
	}



	static public void GetGroupName(GameObject gameObject, out string group1, out string group2)
	{

		var group = gameObject.GetComponentInParent<LocalizationGroup>();

		if (group == null)
		{
			group1 = "common";
			group2 = "common";
			return;
		}

		if( group.doNotLocalize )
		{
			group1 = "disable";
			group2 = "disable";
			return;
		}

		group1 = group.textGroup;
		group2 = group.secondGroup;
	}


	static public void LocalizeAll()
	{
		var tmpTextList = FindObjectsOfType<TMPro.TMP_Text>();

		foreach (var obj in tmpTextList)
		{
			Localize(obj);
		}

		var textList = FindObjectsOfType<Text>();
		foreach (var obj in textList)
		{
			Localize(obj);
		}

		var dropdownList = FindObjectsOfType<Dropdown>();
		foreach (var obj in dropdownList)
		{
			Localize(obj);
		}

		var tmpdropdownList = FindObjectsOfType<TMPro.TMP_Dropdown>();
		foreach(var obj in tmpdropdownList)
		{
			Localize(obj);
		}


	}


	static public void LocalizeComponentsInChildren(GameObject target)
	{
		var tmpTextList = target.GetComponentsInChildren<TMPro.TMP_Text>();

		foreach (var obj in tmpTextList)
		{
			Localize(obj);
		}

		var textList = target.GetComponentsInChildren<Text>();

		foreach (var obj in textList)
		{
			Localize(obj);
		}

		var dropdownList = target.GetComponentsInChildren<Dropdown>();
		foreach (var obj in dropdownList)
		{
			Localize(obj);
		}

		var tmpdropdownList = FindObjectsOfType<TMPro.TMP_Dropdown>();
		foreach(var obj in tmpdropdownList)
		{
			Localize(obj);
		}

	}




	static public void Localize(TMPro.TMP_Text text)
	{
		string g1 = "";
		string g2 = "";

		GetGroupName(text.gameObject, out g1, out g2);
		string t2 = LocalizationManager.ConvertText(text.text, g1, g2);
		text.text = t2;
	}



	static public void Localize(Text text)
	{
		string g1 = "";
		string g2 = "";

		GetGroupName(text.gameObject, out g1, out g2);
		string t2 = LocalizationManager.ConvertText(text.text, g1, g2);
		text.text = t2;
	}


	static public void Localize(Dropdown dropdown)
	{
		if (dropdown == null) return;

		string g1 = "";
		string g2 = "";

		GetGroupName(dropdown.gameObject, out g1, out g2);

		foreach (var o in dropdown.options)
		{
			string t2 = LocalizationManager.ConvertText( o.text, g1, g2);
			o.text = t2;
		}
	}


	static public void Localize(TMPro.TMP_Dropdown dropdown)
	{
		if(dropdown == null) return;

		string g1 = "";
		string g2 = "";

		GetGroupName(dropdown.gameObject, out g1, out g2);

		foreach(var o in dropdown.options)
		{
			string t2 = LocalizationManager.ConvertText( o.text, g1, g2);
			o.text = t2;
		}
	}



	private void Init()
	{
		textList.Clear();

#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif 

		string str = System.Text.Encoding.UTF8.GetString(textFile.bytes);

		string[] separators = { "\n", "\r" };
		string[] separatorsText = { "=" };
		string[] separatorsText2 = { "==" };
		string[] separatorsGroupText = { "=", "[", "]", "/", "\"", };

		string[] sp = str.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

		string group = "";

		foreach (var text in sp)
		{

			if (string.IsNullOrWhiteSpace(text)) continue;


			string line = text.Trim();

			if (line.StartsWith("//")) continue;

			string id = "";


			//그룹지정인지??
			if (line.StartsWith("[group=", true, null))
			{
				Debug.Log(group);

				var groups = text.Split(separatorsGroupText, StringSplitOptions.RemoveEmptyEntries);

				if (groups.Length >= 2)
				{
					group = groups[1];
				}
			}


			//[ID_ 로 시작 하는지 체크
			if (line.StartsWith("[ID_"))
			{
				var ids = line.Split(']');

				if (ids.Length < 2) continue;
				id = ids[0] + "]";
				line = ids[1];
			}


			string[] words;

			if( line.Contains("=="))
			{
				words = line.Split(separatorsText2, System.StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				words = line.Split(separatorsText, System.StringSplitOptions.RemoveEmptyEntries);
			}


			if (words == null) continue;
			if (words.Length < 2) continue;

			var localText = new LocalText();

			localText.kor = words[1].Trim().Replace("\\n", "\n");		// \n 적용
			localText.eng = words[0].Trim().Replace("\\n", "\n");
			localText.group = group;

			if (string.IsNullOrWhiteSpace(id) == false)
			{
				localText.id = id;
			}

			textList.Add(localText);
		}

		//Debug.Log("OK : " + textList.Count.ToString());
	}


	private void OnEnable()
	{
		ChangeLanguage( CurrentLanguage );
	}




	void Update()
	{

		if ( useTestKey && Input.GetKeyUp(testKey))
		{
			Init();

			if (CurrentLanguage == Language.ENG)
			{
				ChangeLanguage(Language.KOR);
			}
			else
			{
				ChangeLanguage(Language.ENG);
			}

		}
	}


}
