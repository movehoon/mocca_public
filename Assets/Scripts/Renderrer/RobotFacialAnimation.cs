using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFacialAnimation : MonoBehaviour
{
    public enum EmotionType
    {
        normal,     //기본
        happy,      //행복
        angry,      //화남
        fear,       //두려움
        sad,        //슬픔
        smile,      //미소
        surprised,  //놀람
        speak,      //말하기

        winkleft,   //윙크(좌)
        winkright,  //윙크(우)

        gazeup,     //쳐다보기(상)
        gazedown,   //쳐다보기(하)
        gazeleft,   //쳐다보기(좌)
        gazeright   //쳐다보기(우)

    };


    public Animator moccaFaceAnimator;
    public float transitionDuration = 0.15f;

    public EmotionType currentEmotion;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    internal void PlayFacialAnim(string faceName, float animPeriod)
    {
        if( Enum.TryParse<EmotionType>(faceName, true , out currentEmotion) == true )
        {
            currentEmotion = EmotionType.normal;

            if( moccaFaceAnimator != null )
            {
                moccaFaceAnimator.CrossFade(faceName, transitionDuration);
            }
        }
    }
}
