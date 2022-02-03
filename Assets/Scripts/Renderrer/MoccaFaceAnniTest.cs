using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoccaFaceAnniTest : MonoBehaviour
{
    public bool normal;     //기본
    public bool happy;      //행복
    public bool angry;      //화남
    public bool fear;       //두려움
    public bool sad;        //슬픔
    public bool smile;      //미소
    public bool surprised;  //놀람
    public bool speak;      //말하기

    public bool winkleft;   //윙크(좌)
    public bool winkright;  //윙크(우)

    public bool gazeup;     //쳐다보기(상)
    public bool gazedown;   //쳐다보기(하)
    public bool gazeleft;   //쳐다보기(좌)
    public bool gazeright;  //쳐다보기(우)



    Animator animator;


    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        Clear();

    }

    private void Clear()
    {
        normal = false;
        happy = false;
        angry = false;
        fear = false;
        sad = false;
        smile = false;
        surprised = false;
        speak = false;

        winkleft = false;
        winkright = false;

        gazeup = false;
        gazedown = false;
        gazeleft = false;
        gazeright = false;
    }

    private void SetAnimation(string ani)
    {
        animator.CrossFade(ani, 0.0f);
        Clear();
    }


    // Update is called once per frame
    void Update ()
    {
        if (normal)     SetAnimation("normal");
        if (happy)      SetAnimation("happy");
        if (angry)      SetAnimation("angry");
        if (fear)       SetAnimation("fear");
        if (sad)        SetAnimation("sad");
        if (smile)      SetAnimation("smile");
        if (surprised)  SetAnimation("surprised");
        if (speak)      SetAnimation("speak");
        if (winkleft)   SetAnimation("winkleft");
        if (winkright)  SetAnimation("winkright");
        if (gazeup)     SetAnimation("gazeup");
        if (gazedown)   SetAnimation("gazedown");
        if (gazeleft)   SetAnimation("gazeleft");
        if (gazeright)  SetAnimation("gazeright");

    }

}
