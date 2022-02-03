using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.FaceInfomation
{
    [System.Serializable]
    public class RobotFacePart
    {
        public EFacePart facialPartEnum;
        public Sprite partSprite;
        public Vector2 partPosition;
        public Vector2 partScale;
    }
}