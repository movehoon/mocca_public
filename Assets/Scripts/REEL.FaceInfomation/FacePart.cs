using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.FaceInfomation
{
    public enum EFacePart
    {
        EyeLeft,
        Eyeball_Left,
        EyeRight,
        Eyeball_Right,
        Chick_Left,
        Chick_Right,
        Mouth,
        Nose,
        Dark_Left,
        Dark_Right,
        Tear_Left,
        Tear_Right,
        Eyebrow_Left,
        Eyebrow_Right,
        Length
    }

    [System.Serializable]
    public class FacePart
    {
        //public bool isTurnOff = false;
        public EFacePart facePartEnum;
        public Sprite partSprite;
        public Vector2 partPosition;
    }
}