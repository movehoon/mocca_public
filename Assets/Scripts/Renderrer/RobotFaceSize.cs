using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RobotFaceSize : MonoBehaviour {


    public float minSize = 3.0f;
    public float sizeStep = 1.0f;



    public UnityEngine.UI.Slider sliderFaceSize;
    public UnityEngine.UI.Slider sliderFaceHeight;
    public UnityEngine.UI.Slider sliderEyeDist;
    public UnityEngine.UI.Slider sliderMouthDist;



	public float valueFaceSize = 8.0f;
	public float valueFaceHeight = -1.5f;
	public float valueEyeDist = 0.45f;
	public float valueMouthDist = -0.3f;

	public float scaleFaceSize = 0.5f;
	public float scaleFaceHeight = 0.5f;
	public float scaleEyeDist = 0.03f;
	public float scaleMouthDist = 0.05f;




	public Transform leftEye;
    public Transform rightEye;
    public Transform mouth;
    public Transform mouthSay;


    public float FaceSize
    {
        get
        {
            return sliderFaceSize ? sliderFaceSize.value : 0.0f;
        }

        set
        {
            sliderFaceSize.value = value;
            PlayerPrefs.SetFloat("FaceSize", sliderFaceSize.value);
            transform.localScale = Vector3.one * (valueFaceSize + (value * scaleFaceSize));
        }
    }


    public float FaceHeight
    {
        get
        {
            return sliderFaceHeight? sliderFaceHeight.value : 0.0f;
        }

        set
        {
            sliderFaceHeight.value = value;
            PlayerPrefs.SetFloat("FaceHeight", sliderFaceHeight.value);
            transform.localPosition = new Vector3(0.0f, valueFaceHeight + (value * scaleFaceHeight));

        }


    }



    public float EyeDist
    {
        get
        {
            return sliderEyeDist ? sliderEyeDist.value : 0.0f;
        }
        set
        {
            sliderEyeDist.value = value;
            PlayerPrefs.SetFloat("EyeDist", sliderEyeDist.value);

            leftEye.localPosition = new Vector3( -valueEyeDist + (- value* scaleEyeDist), 0.0f);
            rightEye.localPosition = new Vector3( valueEyeDist + (value * scaleEyeDist), 0.0f);
        }
    }



    public float MouthDist
    {
        get
        {
            return sliderMouthDist ? sliderMouthDist.value : 0.0f;
        }
        set
        {
            sliderMouthDist.value = value;
            PlayerPrefs.SetFloat("MouthDist", sliderMouthDist.value);


            Vector3 pos = mouth.localPosition;
            pos.z = valueMouthDist + value * scaleMouthDist;
            mouth.localPosition = pos;

            pos = mouthSay.localPosition;
            pos.z = valueMouthDist + value * scaleMouthDist;
            mouthSay.localPosition = pos;
        }
    }


	public void SetAndStretchToParentSize(RectTransform _mRect, RectTransform _parent)
	{
		_mRect.anchoredPosition = _parent.position;
		_mRect.anchorMin = new Vector2(0, 0);
		_mRect.anchorMax = new Vector2(1, 1);
		_mRect.pivot = new Vector2(0.5f, 0.5f);
		_mRect.sizeDelta = _parent.rect.size;
		_mRect.transform.SetParent(_parent);
	}


	void InitSlider(UnityEngine.UI.Slider slider , UnityAction<float> call )
	{
		GameObject handle = slider.transform.Find("Handle Slide Area/Handle").gameObject;

		slider.minValue = -10;
		slider.maxValue = 10;
		slider.wholeNumbers = true;

		GameObject gameObject = new GameObject("value text");
		//gameObject.transform.SetParent(handle.transform);
		UnityEngine.UI.Text textUi = gameObject.AddComponent<UnityEngine.UI.Text>(); ;

		textUi.alignment = TextAnchor.MiddleCenter;
		textUi.color = Color.black;
		textUi.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		textUi.text = "0";

		SetAndStretchToParentSize(textUi.rectTransform, handle.GetComponent<RectTransform>());
		textUi.resizeTextForBestFit = true;

		slider.onValueChanged.AddListener( call );
		slider.onValueChanged.AddListener( (v) => { textUi.text = v.ToString(); });

	}


	// Use this for initialization
	void Start ()
    {
		InitSlider(sliderFaceSize, (v) => { FaceSize = v; } );
		InitSlider(sliderFaceHeight, (v) => { FaceHeight = v; });
		InitSlider(sliderEyeDist, (v) => { EyeDist = v; });
		InitSlider(sliderMouthDist, (v) => { MouthDist = v; });

		FaceSize = PlayerPrefs.GetFloat("FaceSize", 0.0f);
		FaceHeight = PlayerPrefs.GetFloat("FaceHeight", 0.0f);
		EyeDist = PlayerPrefs.GetFloat("EyeDist", 0.0f);
		MouthDist = PlayerPrefs.GetFloat("MouthDist", 0.0f);

	}

	// Update is called once per frame
	void Update () {
		
	}


	public void ResetDefaultSize()
	{
		FaceSize = 0;
		FaceHeight = 0;
		EyeDist = 0;
		MouthDist = 0;
	}

}
