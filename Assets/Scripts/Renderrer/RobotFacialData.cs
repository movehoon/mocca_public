using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using REEL.FaceInfomation;
using System;

namespace REEL.Animation
{
    public class RobotFacialData : MonoBehaviour
    {
		public SpriteRenderer[] partSprites;
        public List<RobotFacePartSO> partData = new List<RobotFacePartSO>();

        public Dictionary<EFacePart, SpriteRenderer> partDictionary
            = new Dictionary<EFacePart, SpriteRenderer>();

        void Awake()
        {
            InitPartDictionary();
        }

        // EFacePart 열거형으로 표정 정보 파일 검색 쉽게 하도록 만든 딕셔너리 설정 함수.
        void InitPartDictionary()
        {
            for (int ix = 0; ix < partSprites.Length; ++ix)
            {
				for (int jx = 0; jx < (int)EFacePart.Length; ++jx)
                {
                    if (partSprites[ix].CompareTag(((EFacePart)(jx)).ToString()))
                    {
						partDictionary.Add(((EFacePart)(jx)), partSprites[ix]);
                        break;
                    }
                }
            }
        }

        // 대문자, 소문자 관계없이 두 문자열을 비교하는 함수.
        bool CompareTwoStrings(string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }

        // EFacePart 열거형으로 FacialPart SpriteRenderer 반환해주는 함수.
        SpriteRenderer GetFacialPartSpriteRenderer(EFacePart facePart)
        {
            return partDictionary[facePart];
        }

        // 표정 이름으로 표정 정보가 저장된 파일 검색하는 함수.
        public RobotFacePartSO GetFacePartSO(string faceName)
        {
            //if (IsLatestFace(faceName)) faceName = facialData[GetFacialIndex(faceName)].name;
            for (int ix = 0; ix < partData.Count; ++ix)
            {
                if (partData[ix].faceName.Equals(faceName))
                    return partData[ix];
            }
            return null;
        }
    }
}