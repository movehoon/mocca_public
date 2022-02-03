using REEL.FaceInfomation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Animation
{
    public class RobotFacialRenderer : MonoBehaviour, Renderrer
    {
		public string startFaceName = "surprised";
        public RobotFacialData robotFacialData;
        public RobotFacialAnimator robotFacialAnimator;

        public RobotFacialAnimation robotFacialAnimation;   //kjh


        void Awake()
        {
            if (robotFacialData == null) robotFacialData = GetComponent<RobotFacialData>();
            if (robotFacialAnimator == null) robotFacialAnimator = GetComponent<RobotFacialAnimator>();

            if (robotFacialAnimation == null) robotFacialAnimation = GetComponent<RobotFacialAnimation>(); //kjh

            //Play(startFaceName);
            //StartCoroutine("PlayAllForTest");
        }

        public void Init()
        {

        }

        // 테스트용. 표정 정보 전체를 설정해가면서 이상 없는지 확인할 때 사용.
        IEnumerator PlayAllForTest()
        {
            foreach (RobotFacePartSO data in robotFacialData.partData)
            {
                yield return new WaitForSeconds(2f);

                Play(data.faceName);
            }
        }

        public void Play(string name)
        {
            string[] splitString = name.Split('(');
            SetFacialModel(splitString[0]);
            robotFacialAnimator.PlayFacialAnim(splitString[0], 1f);

            if(robotFacialAnimation != null)
            {
                robotFacialAnimation.PlayFacialAnim(splitString[0], 1f);
            }
        }

        public void Stop()
        {

        }

        public bool IsRunning()
        {
            return false;
        }

        // 표정 이름으로 캔버스에 있는 표정 설정하는 함수.
        private void SetFacialModel(string faceName)
        {
            for (int ix = 0; ix < robotFacialData.partData.Count; ++ix)
            {
                if (CompareTwoStrings(faceName, robotFacialData.partData[ix].faceName))
                {
                    SetFacePart(robotFacialData.partData[ix]);
                    break;
                }

                SetFacePart(robotFacialData.partData[0]);
            }
        }

        // 대문자, 소문자 관계없이 두 문자열을 비교하는 함수.
        bool CompareTwoStrings(string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }

        // 캔버스에 있는 각 표정 이미지에 표정 정보 파일에서 불러온 정보를 설정하는 함수.
        void SetFacePart(RobotFacePartSO model)
        {
            TurnOffAllFacialPart();

            for (int ix = 0; ix < model.faceParts.Count; ++ix)
            {
				SpriteRenderer partRenderer = robotFacialData.partDictionary[model.faceParts[ix].facialPartEnum];
                partRenderer.gameObject.SetActive(true);
                SetSprite(partRenderer, model.faceParts[ix]);
            }
        }

        // 캔버스 이미지에 스프라이트 설정하는 함수.
        void SetSprite(SpriteRenderer partRenderer, RobotFacePart facePart)
        {
            partRenderer.sprite = facePart.partSprite;
            SetSpriteSize(partRenderer, facePart.partScale);
            SetSpritePosition(partRenderer, facePart.partPosition);
        }

        // 캔버스 이미지 위치 설정 함수.
        void SetSpritePosition(SpriteRenderer partRenderer, Vector2 partPosition)
        {
            partRenderer.transform.localPosition = partPosition;
        }

        // 캔버스 이미지 크기 설정 함수.
        void SetSpriteSize(SpriteRenderer partRenderer, Vector2 partScale)
        {
            partRenderer.transform.localScale = partScale;
        }

        // 스프라이트가 속한 텍스처에서 크기 정보 구하는 함수.
        Vector2 GetSpriteSize(Sprite sprite)
        {
            return sprite.textureRect.size;
        }

        // 표정 파트 전체를 끄는 함수.
        // 표정 변경할 때 일단 파트 전체를 끈 다음 필요한 게임 오브젝트를 활성화해 설정함.
        void TurnOffAllFacialPart()
        {
            for (int ix = 0; ix < robotFacialData.partSprites.Length ; ++ix)
            {
                robotFacialData.partSprites[ix].gameObject.SetActive(false);
                robotFacialData.partSprites[ix].transform.localScale = Vector3.one;
            }
        }
    }
}