using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;


public class TutorialMoviePlayer : MonoBehaviour
{
	public VideoPlayer  videoPlayer;

	[Header("Objects")]
	public TMPro.TMP_Text   textTitle;

	[Header("Controls")]
	public GameObject       controlRoot;
	public Button           buttonPlay;
	public Button           buttonVolume;
	public UIMoviePlayerSlider	sliderTimeLine;
	public Slider           sliderVolume;
	public Image            imageMute;
	public TMPro.TMP_Text   textTime;
	public TMPro.TMP_Text   textStep;

	public RawImage         imageVideo;

	public Image            imagePlayButton;
	public Toggle           autoStep;



	[Header("Sprite")]
	public Sprite           spritePlayButton;
	public Sprite           spritePauseButton;
	public Sprite           spriteReplayButton;


	[Header("Debug")]
	public bool             debugLog = false;

	bool seeking = false;
	int lastStepIndex = 0;

	public float Time
	{
		get
		{
			return (float)videoPlayer.time;
		}

		set
		{
			float time = (float)value;

			if( Time != time )
			{
				videoPlayer.time = time;
				seeking = true;
				sliderTimeLine.SetValueWithoutNotify(time);
			}
		}
	}

	public float TimeLength
	{
		get
		{
			return (float)videoPlayer.length;
		}
	}

	public bool NeedTimelineSync { get; set; } = true;

	public bool AutoStepMode{ get{ return autoStep.isOn; } }

	public float Volume
	{
		get
		{
			if(videoPlayer.isPrepared)
			{
				if(videoPlayer.IsAudioTrackEnabled(0))
				{
					return videoPlayer.GetDirectAudioVolume(0);
				}
			}

			return lastVolume;
		}

		set
		{
			sliderVolume.SetValueWithoutNotify(value);
			imageMute.gameObject.SetActive(value == 0.0f);

			if( videoPlayer.isPrepared )
			{
				if( videoPlayer.IsAudioTrackEnabled(0) )
				{
					videoPlayer.SetDirectAudioVolume(0, value);
				}
			}

			lastVolume = value;
		}
	}

	float lastVolume = 1.0f;

	VideoSequence   videoSequence;

	private void Awake()
	{
		sliderTimeLine.videoPlayer = videoPlayer;

		videoPlayer.started += VideoPlayer_started;
		videoPlayer.frameDropped += VideoPlayer_frameDropped;
		videoPlayer.errorReceived += VideoPlayer_errorReceived;
		videoPlayer.clockResyncOccurred += VideoPlayer_clockResyncOccurred;
		videoPlayer.seekCompleted += VideoPlayer_seekCompleted;
		videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
		videoPlayer.frameReady += VideoPlayer_frameReady;
		videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
	}

	public void OnEnable()
	{
		imageVideo.gameObject.SetActive(false);
	}

	private void VideoPlayer_prepareCompleted(VideoPlayer source)
	{
		seeking = false;

		if(debugLog)
		{
			Debug.Log("VideoPlayer_prepareCompleted");
		}

		if(imageVideo.gameObject.activeInHierarchy == false)
		{
			imageVideo.gameObject.SetActive(true);
		}

	}

	private void VideoPlayer_frameReady(VideoPlayer source, long frameIdx)
	{
		if(debugLog)
		{
			Debug.Log("VideoPlayer_frameReady");
		}
	}

	private void VideoPlayer_loopPointReached(VideoPlayer source)
	{
		if(debugLog)
		{
			Debug.Log("VideoPlayer_loopPointReached");
		}
		imagePlayButton.sprite = spriteReplayButton;
	}

	private void VideoPlayer_seekCompleted(VideoPlayer source)
	{
		seeking = false;
		if(debugLog)
		{
			Debug.Log("VideoPlayer_seekCompleted");
		}
	}

	private void VideoPlayer_clockResyncOccurred(VideoPlayer source, double seconds)
	{
		if(debugLog)
		{
			Debug.Log("VideoPlayer_clockResyncOccurred");
		}
	}
	private void VideoPlayer_errorReceived(VideoPlayer source, string message)
	{
		if(debugLog)
		{
			Debug.Log("VideoPlayer_errorReceived");
		}
	}

	private void VideoPlayer_frameDropped(VideoPlayer source)
	{
		if(debugLog)
		{
			Debug.Log("VideoPlayer_frameDropped");
		}
	}

	private void VideoPlayer_started(VideoPlayer source)
	{
		if(debugLog)
		{
			Debug.Log("VideoPlayer_started");
		}

		seeking = false;
		imagePlayButton.sprite = spritePauseButton;
	}

	void Start()
    {
		lastStepIndex = 0;
	}

    void Update()
    {
		UpdateText();

		UpdateTimeStep();
	}

	private void UpdateText()
	{
		if(videoPlayer.isPrepared == false)
		{
			textTime.text = "";
			return;
		}

		if(videoPlayer.isActiveAndEnabled &&
			videoPlayer.isPrepared)
		{
			textTime.text = string.Format("{0:D2}:{1:D2} / {2:D2}:{3:D2}", (int)(videoPlayer.clockTime/ 60.0f), (int)(videoPlayer.clockTime% 60), (int)(videoPlayer.length/60.0f), (int)(videoPlayer.length%60) );
		}
	}


	//public void OnPointerEnter(PointerEventData eventData)
	//{
	//	controlRoot.gameObject.SetActive(true);
	//}

	//public void OnPointerExit(PointerEventData eventData)
	//{
	//	controlRoot.gameObject.SetActive(false);
	//}


	public void OnPlayButtonClicked()
	{
		if(videoPlayer.isPrepared == false) return;

		if(videoPlayer.isPlaying )
		{
			imagePlayButton.sprite = spritePlayButton;
			videoPlayer.Pause();
		}
		else
		{
			videoPlayer.Play();
		}
	}

	public void Pause()
	{
		imagePlayButton.sprite = spritePlayButton;
		videoPlayer.Pause();
	}


	public void OnVolumeButtonClicked()
	{
		if(videoPlayer.isPrepared == false) return;
		
		int track = videoPlayer.audioTrackCount;
		if(track < 1) return;

		if( Volume != 0 )
		{
			Volume = 0.0f;
		}
		else
		{
			Volume = 1.0f;
		}
	}


	public void OnUrlButtonClicked()
	{
		InputBox.Show("video url", videoPlayer.url, "Enter Text...", (res, url) =>
		 {
			if( res == true)
			{
				 videoPlayer.url = url;
			}
		 });
	}

	public void OnCloseButtonClicked()
	{
		videoPlayer.Stop();
		gameObject.SetActive(false);
	}

	internal void Play(VideoSequence seq)
	{
		gameObject.SetActive(true);

		videoSequence = seq;
		textTitle.text = seq.title;

		videoPlayer.url = seq.videoUrl;

		lastStepIndex = 0;
 	}

	private void UpdateTimeStep()
	{
		if(videoSequence == null) return;

		int current = videoSequence.GetCurrentStepIndex( Time );
		int length = videoSequence.StepLength;

		textStep.text = string.Format("Step {0}/{1}", current + 1, length);

		if(seeking == true 
			|| UIMoviePlayerSlider.seeking == true
			|| UIMoviePlayerSlider.IsPointerDown == true
			|| AutoStepMode == true )
		{
			lastStepIndex = current;
			return;
		}

		if( current > lastStepIndex && videoPlayer.isPlaying )
		{
			Pause();
		}

		lastStepIndex = current;
	}


	public void OnNextStepClicked()
	{
		if(videoSequence == null) return;
		int current = videoSequence.GetCurrentStepIndex( Time );

		Time = videoSequence.GetStepTime(current + 1);
	}


	public void OnPrevStepClicked()
	{
		if(videoSequence == null) return;
		int current = videoSequence.GetCurrentStepIndex( Time );

		float curTime = videoSequence.GetStepTime(current);

		//if( isStop)

		if(videoPlayer.isPlaying == false)
		{
			imagePlayButton.sprite = spritePlayButton;
		}

		if(curTime <= Time && Time <= curTime + 1.5f)
		{
			Time = videoSequence.GetStepTime(current - 1);
		}
		else
		{
			Time = videoSequence.GetStepTime(current);
		}
	}

}
