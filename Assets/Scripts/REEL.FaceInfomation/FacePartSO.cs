using System.Collections.Generic;
using System;
using UnityEngine;
using REEL.Animation;

using Type = REEL.Animation.Demension.Type;

namespace REEL.FaceInfomation
{
    public class FacePartSO : ScriptableObject
    {
        public string faceName;
        public List<FacePart> faceParts = new List<FacePart>();
        public List<FaceAnimation> animDemensions = new List<FaceAnimation>();
    }

    [Serializable]
    public class FaceAnimation
    {
        public EFacePart facePart;
        public Type type;
        public Paramters parameter;
    }
}