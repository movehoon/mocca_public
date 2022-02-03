using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Video;

//------------------------------------------------
// 동영상 플레이어 시간 슬라이더바 처리 컴포넌트
// videoPlayer.time 값 변경시 즉시 동영상 위치가
// 즉시 변경이 안되서 코드가 좀 복잡함
//
// 슬라이더 바 클릭으로 즉시 이동처리
// 슬라이더 핸들 드래그로 이동처리
//------------------------------------------------
public class UIMoviePlayerSlider : Slider
{
    [Header("for Movie Player")]
    public static bool IsPointerDown = false;
    //public bool IsDragging = false;

    float seekInterval = 0.2f;
    DateTime lastSeekTime = DateTime.MinValue;

    public VideoPlayer  videoPlayer;

	public static bool seeking = false;
	float _vt = 0.0f;

	public float videoTime
	{
		get
		{
			return _vt;
		}

		set
		{
			if( _vt != value )
			{
				_vt = value;
				videoPlayer.time = _vt;
				seeking = true;
				lastSeekTime = DateTime.Now;
			}
		}
	}

	public bool canSeek
    {
        get 
        {
			return videoPlayer.canSetTime == true && seeking == false &&
					(DateTime.Now - lastSeekTime).TotalSeconds > seekInterval ;
        }
    }

	protected override void Start()
	{
		base.Start();
        SetVideoPlayerEvent();
    }

	private void SetVideoPlayerEvent()
	{
		videoPlayer.started += (s) =>
		{
			minValue = 0.0f;
			maxValue = (float)s.length;
			seeking = false;
			_vt = (float)s.clockTime;
		};

		videoPlayer.prepareCompleted += (s) =>
		{
			minValue = 0.0f;
			maxValue = (float)s.length;
			seeking = false;
			_vt = (float)s.clockTime;
		};

		videoPlayer.seekCompleted += (s) =>
		{
			seeking = false;
			//_vt = (float)s.time;
		};
	}


	protected override void OnEnable()
	{
		base.OnEnable();
		SetValueWithoutNotify(0.0f);
		IsPointerDown = false;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		IsPointerDown = false;
	}

	protected override void Update()
	{
		if(videoPlayer == null) return;

		if(videoPlayer.isPrepared == false) return;
		if(canSeek == false) return;

		if(IsPointerDown) return;

		if( value != videoTime )
		{
			SetValueWithoutNotify(videoTime);
		}

		_vt = (float)videoPlayer.clockTime;
	}


	public override void SetValueWithoutNotify(float input)
	{
		base.SetValueWithoutNotify(input);
	}


	void SyncTimeFrame()
	{
		if(canSeek == true)
		{
			if(videoPlayer.isPrepared == true)
			{
				videoTime = value;
				SetValueWithoutNotify(value);
			}
		}
	}


	public override void OnPointerDown(PointerEventData eventData)
	{
		IsPointerDown = true;
		base.OnPointerDown(eventData);

		SyncTimeFrame();
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		IsPointerDown = false;
		base.OnPointerUp(eventData);

		SyncTimeFrame();
	}


	public override void OnDrag(PointerEventData eventData)
	{
		IsPointerDown = true;
		base.OnDrag(eventData);

		SyncTimeFrame();
	}


}
