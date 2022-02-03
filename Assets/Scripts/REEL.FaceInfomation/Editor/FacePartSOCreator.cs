using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace REEL.FaceInfomation
{
    public class FacePartSOCreator : MonoBehaviour
    {
        private static readonly string filePath = "Assets/Resources/PartData/FaceData.asset";
        private static readonly string robotFilePath = "Assets/Resources/PartData/MoccaData/FaceData.asset";

        [MenuItem("FacePart/Create Face Infomation SO")]
        static void CreateFacePartSO()
        {
            FacePartSO so = ScriptableObject.CreateInstance<FacePartSO>();
            AssetDatabase.CreateAsset(so, filePath);
        }

        [MenuItem("FacePart/Create Robot Face Infomation SO")]
        static void CreateRobotFacePartSO()
        {
            RobotFacePartSO so = ScriptableObject.CreateInstance<RobotFacePartSO>();
            SetInfo(so);
            AssetDatabase.CreateAsset(so, robotFilePath);
        }

        static void SetInfo(RobotFacePartSO so)
        {
            Transform[] gos = Selection.activeGameObject.GetComponentsInChildren<Transform>();
            EFacePart eFacePart;
            for (int ix = 0; ix < gos.Length; ++ix)
            {
                if (Enum.TryParse<EFacePart>(gos[ix].tag, out eFacePart))
                {
                    Debug.Log(gos[ix].tag);
                    RobotFacePart rfp = new RobotFacePart();
                    rfp.facialPartEnum = eFacePart;
                    rfp.partPosition = gos[ix].localPosition;
                    rfp.partScale = gos[ix].localScale;
                    rfp.partSprite = gos[ix].GetComponent<SpriteRenderer>().sprite;

                    so.faceParts.Add(rfp);
                }
            }
        }
    }
}
