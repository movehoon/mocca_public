using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using REEL.FaceInfomation;

namespace REEL.Animation
{
	public class FacialRenderrer : MonoBehaviour, Renderrer
	{
        public class FacialData
		{
            //public String name, speed, mouth;
            public string name;
            public float mouthPos, mouthSize, eyeSize, tearSize, speed;

			public FacialData(string name = "normal", float mouthPos = 0f,
                float mouthSize = 0f, float eyeSize = 0f, float tearSize = 0f, float speed = 100f)
			{
				this.name = name;				
				this.mouthPos = mouthPos;
                this.mouthSize = mouthSize;
                this.eyeSize = eyeSize;
                this.tearSize = tearSize;
                this.speed = speed;
            }
		}

		public Transform FacePanel;

		public List<Image> partImages;
        public List<FacePartSO> partData = new List<FacePartSO>();

        private float _period = 100f;
		public List<Demension> animationDemensions = new List<Demension>();
		List<IEnumerator> _cos = new List<IEnumerator>();

		public Dictionary<EFacePart, Image> partDictionary = new Dictionary<EFacePart, Image>();

		// To record Facial Model.
		private int arrayMaxLength = 5;

        public List<string> facialModelHistories = new List<string>();
		private List<FacialData> facialData = new List<FacialData>();

		int mouthPos = 0;
        int mouthSize = 0;
        int eyeSize = 0;
        int tearSize = 0;

		void Awake()
		{            
            InitPartDictionary();
            //StartCoroutine(PlayAllForTest());
            //Play("normal");
            //StartCoroutine(PlayAllForTest());    
        }

		public void OnFacialChange()
		{
			Play("latest(speed=40)");
		}

		public void FirstPlay()
		{
			Play("surprised");
		}

        // 두 표정 설정해서 변경해보는 함수 (테스트 용).
        IEnumerator PlayTest(string first, string second)
        {
            FacePartSO firstData = GetFacePartSO(first);
            Play(firstData.faceName);

            yield return new WaitForSeconds(5f);

            FacePartSO secondData = GetFacePartSO(second);
            Play(secondData.faceName);
        }

        // 테스트용. 표정 정보 전체를 설정해가면서 이상 없는지 확인할 때 사용.
		IEnumerator PlayAllForTest()
		{
			foreach (FacePartSO data in partData)
			{
				yield return new WaitForSeconds(2f);

				Play(data.faceName);
			}
		}

        public void Init()
		{
			
		}

        // EFacePart 열거형으로 표정 정보 파일 검색 쉽게 하도록 만든 딕셔너리 설정 함수.
        void InitPartDictionary()
        {            
            for (int ix = 0; ix < partImages.Count; ++ix)
            {
                for (int jx = 0; jx < (int)EFacePart.Length; ++jx)
                {
                    if (partImages[ix].CompareTag(((EFacePart)(jx)).ToString()))
                    {
                        partDictionary.Add(((EFacePart)(jx)), partImages[ix]);
                        break;
                    }
                }
            }            
        }       

        // 입이 이동할 수 있는지 확인하는 함수.
        bool CanMoveMouth(int movePos)
        {            
            if (movePos > 0 && mouthPos < 80) return true;
            else if (movePos < 0 && mouthPos > -80) return true;            
            return false;
        }

        // 표정 정보 기록하는 함수.
		private void SetHistory(string prevCommandString)
		{            
            facialModelHistories.Add(prevCommandString);
			//Debug.Log(facialModelHistories.Count);
			//Debug.Log(facialModelHistories[0]);

			if (facialModelHistories.Count > arrayMaxLength)
			{
				facialModelHistories.RemoveAt(0);
				//Debug.Log("remove : " + facialModelHistories[4]);
			}            
        }

        // 표정 기록 확인하는 함수.
        // 전달된 표정 정보가 latest 일 때 사용.
		private string GetHistroy()
		{            
            string retString = string.Empty;

            if (facialModelHistories.Count > 0)
			{
                retString = facialModelHistories[facialModelHistories.Count - 1];
                //facialModelHistories.RemoveAt(facialModelHistories.Count - 1);
                //string[] splitted_retString = retString.Split('('); //GetFacialIndex에서 faceName이 split된 상태라서
                //retString = splitted_retString[0];                  //스플릿 안해도 될것같은데 변화 없어서 그냥둠
            }            
            return retString;
		}

        // 표정이 지난 표정(latest) 인지 확인하는 함수.
		string _latestString = "latest";
		bool IsLatestFace(string faceParam)
		{
            return faceParam.Equals(_latestString);
		}

        // facialData에서 인덱스 검색하는 함수.
        int GetFacialIndex(string faceName)
        {            
            int retIndex = -1;
            if (IsLatestFace(faceName)) //lastest가 들어왔을때
                retIndex =  FindIndex(GetHistroy());

            else //lastest가 아닐때
            {   
                retIndex = FindIndex(faceName);
                if (retIndex == -1)
                {   
                    facialData.Add(new FacialData(faceName));
                    SetHistory(faceName);
                    retIndex = FindIndex(faceName);
                }
            }            
            return retIndex;
        }

        //입위치, 크기, 눈크기, 눈물크기 0으로
        void SetOrigin()
        {          
            mouthPos = 0;
            mouthSize = 0;
            eyeSize = 0;
            tearSize = 0;
        }

        //저장된 정보 불러오기
        void CallEditData(int facialIndex, string faceName)
        {
            MouthPosUp((int)facialData[facialIndex].mouthPos);
            MouthSize((int)facialData[facialIndex].mouthSize);
            EyeSize((int)facialData[facialIndex].eyeSize);
            TearSize((int)facialData[facialIndex].tearSize);
            PlayFacialAnim(faceName, facialData[facialIndex].speed * 0.01f);
        }

        // 속도 조절 함수
        public void Speed(string parameters, int facialIndex, string faceName)
        {
            facialData[facialIndex].speed -= (float.Parse(parameters));
            // 애니메이션 주기가 너무 짧아지면 프로그램이 멈추기 때문에 
            // 최소 간격을 10으로 고정.
            facialData[facialIndex].speed =
                Mathf.Clamp(facialData[facialIndex].speed, 10, float.MaxValue);
            PlayFacialAnim(faceName, facialData[facialIndex].speed * 0.01f);
        }

        // 입크기 조절 함수.
        public void MouthSize(int size)
        {
            Image mouth = partDictionary[EFacePart.Mouth];
            mouth.rectTransform.sizeDelta += new Vector2(size, size);
            mouthSize += size;
        }

        // 눈크기 조절 함수
        public void EyeSize(int size)
        {
            Image[] eye = { partDictionary[EFacePart.EyeLeft],
                partDictionary[EFacePart.EyeRight],
                partDictionary[EFacePart.Eyeball_Left],
                partDictionary[EFacePart.Eyeball_Right]};
            for (int ix = 0; ix < eye.Length; ix++)
            {
                eye[ix].rectTransform.sizeDelta += new Vector2(size, size);
            }
            eyeSize += size;
        }

        // 눈물크기 조절 함수
        public void TearSize(int size)
        {
            Image[] tear = { partDictionary[EFacePart.Tear_Left],
                partDictionary[EFacePart.Tear_Right] };
            for (int ix = 0; ix < tear.Length; ix++)
            {
                tear[ix].rectTransform.sizeDelta += new Vector2(size, size);
            }
            tearSize += size;
        }

        // 입 이동 함수.
        public void MouthPosUp(int size)
        {
            Image mouth = partDictionary[EFacePart.Mouth];
            if (mouth != null && mouth.IsActive() && CanMoveMouth(size))
            {
                Vector3 newPos = mouth.transform.localPosition + new Vector3(0f, size, 0f);
                mouth.transform.localPosition = newPos;
                mouthPos += size;
            }
        }

        // 표정 재생 함수.
        // 표정 변경/애니메이션 재생 등에 사용함.
        public void Play(string name)
		{
            string[] splitted_command = name.Split('(');

            int facialIndex = GetFacialIndex(splitted_command[0]);
            if (!IsLatestFace(splitted_command[0]))
            {
                SetFacialModel(splitted_command[0]);
                SetOrigin();                
                CallEditData(facialIndex, splitted_command[0]);                
            }

            if (splitted_command.Length > 1)
            {
                string[] detail = splitted_command[1].Substring(0, splitted_command[1].Length - 1).Split('=');
                ProcessParameters(detail, facialIndex, splitted_command[0]);
            }


            Debug.Log("FFFFFFFFFFFF + " + name);

        }        
        
        private void ProcessParameters(string[] parameters, int facialIndex, string faceName)
        {
            switch (parameters[0])
            {
                case "speed":
                    {
                        Speed(parameters[1], facialIndex, faceName);
                    }
                    break;
                case "mouth":
                    {
                        MouthPosUp(int.Parse(parameters[1]));
                        facialData[facialIndex].mouthPos = mouthPos;
                    }
                    break;
                case "mouthsize":
                    {
                        MouthSize(int.Parse(parameters[1]));
                        facialData[facialIndex].mouthSize = mouthSize;
                    }
                    break;
                case "eyesize":
                    {
                        EyeSize(int.Parse(parameters[1]));
                        facialData[facialIndex].eyeSize = eyeSize;
                    }
                    break;
                case "tearsize":
                    {
                        TearSize(int.Parse(parameters[1]));
                        facialData[facialIndex].tearSize = tearSize;
                    }
                    break;
                default: break;
            }
        }           
             
        // 표정 이름으로 표정 정보가 저장된 파일 검색하는 함수.
        public FacePartSO GetFacePartSO(string faceName)
        {            
            if (IsLatestFace(faceName)) faceName = facialData[GetFacialIndex(faceName)].name;
            for (int ix = 0; ix < partData.Count; ++ix)
            {
                if (partData[ix].faceName.Equals(faceName))
                    return partData[ix];
            }
            return null;
        }

        // 표정 이름으로 facialData 인덱스 검색하는 함수.
		public int FindIndex(string face)
		{            
            int result = -1;
			for (int ix = 0; ix < facialData.Count; ix++)
			{
                if (CompareTwoStrings(face, facialData[ix].name))
				{
					result = ix;
				}
			}            
            return result;
		}

        // 표정 이름으로 캔버스에 있는 표정 설정하는 함수.
        private void SetFacialModel(string faceName)
		{            
            for (int ix = 0; ix < partData.Count; ++ix)
			{
                if (CompareTwoStrings(faceName, partData[ix].faceName))
				{
					SetFacePart(partData[ix]);
					break;
				}

				SetFacePart(partData[0]);
			}            
        }

		public void Stop()
		{

		}

		public bool IsRunning()
		{
			return false;
		}

        // 대문자, 소문자 관계없이 두 문자열을 비교하는 함수.
        bool CompareTwoStrings(string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }

        // 캔버스에 있는 각 표정 이미지에 표정 정보 파일에서 불러온 정보를 설정하는 함수.
        void SetFacePart(FacePartSO model)
		{            
            TurnOffAllFacialPart();

			for (int ix = 0; ix < model.faceParts.Count; ++ix)
			{
				Image partImage = partDictionary[model.faceParts[ix].facePartEnum];
				partImage.gameObject.SetActive(true);
				SetSprite(partImage, model.faceParts[ix]);
			}
        }

        // 캔버스 이미지에 스프라이트 설정하는 함수.
		void SetSprite(Image partImage, FacePart facePart)
		{            
            partImage.sprite = facePart.partSprite;
			SetSpriteSize(partImage, facePart.partSprite);
			SetSpritePosition(partImage, facePart.partPosition);            
        }

        // 캔버스 이미지 위치 설정 함수.
		void SetSpritePosition(Image partImage, Vector2 partPosition)
		{            
            partImage.GetComponent<RectTransform>().localPosition = partPosition;            
        }

        // 캔버스 이미지 크기 설정 함수.
		void SetSpriteSize(Image partImage, Sprite partSprite)
		{            
            partImage.GetComponent<RectTransform>().sizeDelta = GetSpriteSize(partSprite);            
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
            for (int ix = 0; ix < partImages.Count; ++ix)
			{
				partImages[ix].gameObject.SetActive(false);
                partImages[ix].GetComponent<RectTransform>().localScale = Vector3.one;
			}            
        }

        // 표정 정보 파일에 설정된 애니메이션 정보 읽어와서 애니메이션 재생 배열 재설정 함수.
        void SetFacialAnimData(FacePartSO model)
		{            
            animationDemensions.Clear();
			AddToAnimDemensions(model);            
        }

        // 애니메이션 재생 배열에 정보 설정하는 함수.
		void AddToAnimDemensions(FacePartSO model)
		{            
            for (int ix = 0; ix < model.animDemensions.Count; ++ix)
			{
				animationDemensions.Add(new Demension(
					partDictionary[model.animDemensions[ix].facePart],
					model.animDemensions[ix].type,
					model.animDemensions[ix].parameter)
				);
			}            
        }

        // 애니메이션 재생 함수.
        void PlayFacialAnim(string faceName, float animPeriod)
        {
            



            FacePartSO animData = GetFacePartSO(faceName);
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
            for (int ix = 0; ix < _cos.Count; ix++)
            {
                StopCoroutine(_cos[ix]);
            }            
        }

        // 애니메이션 배열에 저장된 코루틴 재생 함수.
        void PlayFacialAnim(float animPeriod)
		{            
            StopAllPartAnimation();
			_cos.Clear();

			for (int ix = 0; ix < animationDemensions.Count; ix++)
			{
				_cos.Add(animationDemensions[ix].Run(animPeriod));
			}

			for (int ix = 0; ix < _cos.Count; ++ix)
			{
				StartCoroutine(_cos[ix]);
			}            
        }

		// Update is called once per frame
		void Update()
		{

		}

		private static FacialRenderrer _instance = null;
		public static FacialRenderrer Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType(typeof(FacialRenderrer)) as FacialRenderrer;
				}
				return _instance;
			}
		}
	}
}