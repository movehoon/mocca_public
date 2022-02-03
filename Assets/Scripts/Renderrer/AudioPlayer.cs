using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AudioPlayer : Singleton<AudioPlayer>, Renderrer
{

    [Serializable]
    public class AudioCmd
    {
        public string cmd;
        public string name;
        public string url;
    }


    private AudioSource audioSource;

    public void Init()
    {

    }

    public void Command(string command)
    {
        AudioCmd audioCmd = JsonUtility.FromJson<AudioCmd>(command);
        if (audioCmd.cmd == "PLAY")
        {
            Arbitor.Instance.PlayAutoGenEmotion(audioCmd.name);
            Play(audioCmd.url);
        }
        else if (audioCmd.cmd == "PAUSE")
        {
            Pause();
        }
        else if (audioCmd.cmd == "STOP")
        {
            Stop();
        }
    }

    public void Play(string url)
    {
        PlayAudio(url);
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
    public bool IsRunning()
    {
        return false;
    }


    void PlayAudio(string url)
    {
        Debug.Log("PlayAudio: " + url);
        StartCoroutine(GetAudioClip(url));
    }

    IEnumerator GetAudioClip(string url)
    {
        Debug.Log("conv_url: " + url);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("ERR: " + www.error);
            }
            else
            {
                //Debug.Log("PlayAudio" + www.downloadHandler.text);
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                myClip.name = "HI";
                Debug.Log("GetAudioClip: " + myClip.name + "(" + myClip.length.ToString() + ")");
                audioSource.Stop();
                audioSource.clip = myClip;
                audioSource.Play();
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
