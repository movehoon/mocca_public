using Malee.List;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSequence : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public float time;
    }

    public bool                     hide = false;
    public bool                     active = true;

    [Header("Tutorial Comment")]
    public int                      numbering;
    public string                   title;
    public string                   content;

    [Header("Video url")]
    public string                   videoUrl;

    public TextAsset                textTimeline;


    [System.Serializable]
    public class StepList : ReorderableArray<Step>
    {
    }

    [Header("Video Sequence Steps")]
    //[Reorderable]
    StepList         stepList = new StepList();

    public int StepLength => stepList.Length;

    private void Awake()
	{
        Init();
    }


	void Start()
    {

    }

    void Update()
    {

    }

    private void Init()
    {
        stepList.Clear();

        if(textTimeline == null) return;
        string str = System.Text.Encoding.UTF8.GetString(textTimeline.bytes);

        var lines = str.Split('\n');

        for(int i=0;i<lines.Length;i++)
        {
            var data = lines[i].Split(',');

            if(data.Length == 0) continue;
            string value = data[0].Trim();

            if(value.StartsWith("//")) continue;
            if(string.IsNullOrWhiteSpace(value)) continue;

            float time;

            var v = value.Split(':');

            if( v.Length == 1 )
            {
                if(float.TryParse(value, out time))
                {
                    stepList.Add(new Step()
                    {
                        time = time
                    });
                }
            }
            else if( v.Length == 2)
            {
                float minute = 0.0f;
                if(float.TryParse(v[0], out minute))
                {
                    if(float.TryParse(v[1], out time))
                    {
                        stepList.Add(new Step()
                        {
                            time = minute * 60.0f + time
                        });
                    }
                }
            }
        }
    }

	internal int GetCurrentStepIndex(float time)
	{
        if(stepList == null || stepList.Length == 0) return 0;

        for(int f=0;f<stepList.Length-1;f++)
        {
            if(time < stepList[f+1].time )
            {
                return f;
			}
		}

        return stepList.Length - 1;
    }

	internal float GetStepTime(int index)
	{
        if(stepList == null || stepList.Length == 0) return 0;

        index = Mathf.Clamp(index, 0, stepList.Length - 1);

        return stepList[index].time;
    }
}
