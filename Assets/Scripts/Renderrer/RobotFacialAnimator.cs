using REEL.FaceInfomation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.Animation
{
    public class RobotFacialAnimator : MonoBehaviour
    {
        public RobotFacialData robotFacialData;

        public List<Demension> animationDemensions = new List<Demension>();

        private List<IEnumerator> animCoroutine = new List<IEnumerator>();

        private void Awake()
        {
            if (robotFacialData == null) robotFacialData = GetComponent<RobotFacialData>();
        }

		void Start()
		{
		}

        // 표정 정보 파일에 설정된 애니메이션 정보 읽어와서 애니메이션 재생 배열 재설정 함수.
        void SetFacialAnimData(RobotFacePartSO model)
        {
            animationDemensions.Clear();
            AddToAnimDemensions(model);
        }

        // 애니메이션 재생 배열에 정보 설정하는 함수.
        void AddToAnimDemensions(RobotFacePartSO model)
        {
            for (int ix = 0; ix < model.animDemensions.Count; ++ix)
            {
                animationDemensions.Add(new Demension(
                    robotFacialData.partDictionary[model.animDemensions[ix].facePart],
                    model.animDemensions[ix].type,
                    model.animDemensions[ix].parameter)
                );
            }
        }

        // 애니메이션 재생 함수.
        public void PlayFacialAnim(string faceName, float animPeriod)
        {
            RobotFacePartSO animData = robotFacialData.GetFacePartSO(faceName);
            if (animData)
            {
                SetFacialAnimData(animData);
                PlayFacialAnim(animPeriod);
            }
        }

        // 애니메이션 정지 함수.
        // 애니메이션을 바꾸거나 표정을 바꿀 때 사용함.
        void StopAllPartAnimation()
        {
            for (int ix = 0; ix < animCoroutine.Count; ix++)
            {
                StopCoroutine(animCoroutine[ix]);
            }
        }

        // 애니메이션 배열에 저장된 코루틴 재생 함수.
        void PlayFacialAnim(float animPeriod)
        {
            StopAllPartAnimation();
            animCoroutine.Clear();

            for (int ix = 0; ix < animationDemensions.Count; ix++)
            {
                animCoroutine.Add(animationDemensions[ix].Run(animPeriod));
            }

            for (int ix = 0; ix < animCoroutine.Count; ++ix)
            {
                StartCoroutine(animCoroutine[ix]);
            }
        }
    }
}