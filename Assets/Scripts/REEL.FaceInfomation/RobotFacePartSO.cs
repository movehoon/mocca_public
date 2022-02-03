using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.FaceInfomation
{
    public class RobotFacePartSO : ScriptableObject
    {
        public string faceName;
        public List<RobotFacePart> faceParts = new List<RobotFacePart>();
        public List<FaceAnimation> animDemensions = new List<FaceAnimation>();
    }
}