using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Animation
{
    [Serializable]
    public class Demension
    {
        public enum Type
        {
            None = 0,
            Scale,
            RotationZ,
            MovementX,
            MovementY,
            Image
        }

        // Fields
        [SerializeField]
        private Image image;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private Type type;
        [SerializeField]
        private Paramters parameters;

		public Demension(Image image, Type type, Paramters parameters)
		{
			this.image = image;
			this.type = type;
			this.parameters = parameters;
		}

        public Demension(SpriteRenderer spriteRenderer, Type type, Paramters parameters)
        {
            this.spriteRenderer = spriteRenderer;
            this.type = type;
            this.parameters = parameters;
        }

        public void SetImage (Image img)
		{
            image = img;
        }

        // Methods
        public IEnumerator Run(float period)
        {
            switch (type)
            {
                case Type.Scale: yield return LoopScale(period);break;
                case Type.RotationZ: yield return LoopRotation(period); break;
                case Type.MovementX: yield return LoopMovementX(period); break;
                case Type.MovementY: yield return LoopMovementY(period); break;
                case Type.Image: yield return LoopChangeImage(period); break;
                default: Debug.LogWarning("애니메이션 타입이 설정되지 않았어요!"); break;
            }
        }

        private IEnumerator LoopScale(float period)
        {
            // 조작할 대상 Transform
            //RectTransform rt = image.rectTransform;
            Transform animTransform = image == null ? spriteRenderer.transform : image.transform;

            // 반바퀴 주기
            float halfPeriod = period * 0.5f;

            while (true)
            {
                #region 반바퀴 (정방향)
                // 정방향 초기값 세팅
                float time = 0f;
                animTransform.localScale = Vector3.one * parameters.DefaultValue;

                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 값을 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.DefaultValue, parameters.GoalValue, normalTime);

                    // X,Y-축 스케일 대입 (로컬 좌표계 기준)
                    animTransform.localScale = new Vector3(value, value, 1f);
                }
                #endregion

                #region 반바퀴 (역방향)
                // 역방향 초기값 세팅
                time = 0f;
                animTransform.localScale = Vector3.one * parameters.GoalValue;

                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 값을 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.GoalValue, parameters.DefaultValue, normalTime);

                    // X,Y-축 스케일 대입 (로컬 좌표계 기준)
                    animTransform.localScale = new Vector3(value, value, 1f);
                }
                #endregion
            }
        }

        private IEnumerator LoopRotation(float period)
        {
            // 조작할 대상 Transform
            //RectTransform rt = image.rectTransform;
            Transform animTransform = image == null ? spriteRenderer.transform : image.transform;

            // 반바퀴 주기
            float halfPeriod = period * 0.5f;

            while (true)
            {
                float time = 0f;
                animTransform.localRotation = Quaternion.Euler(0f, 0.0f, parameters.DefaultValue);

                #region 반바퀴 (정방향)
                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 값을 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.DefaultValue, parameters.GoalValue, normalTime); // <-- 선형보간함수 (Lerp)

                    // Z-축 회전 각도 대입 (로컬 좌표계 기준)
                    animTransform.localRotation = Quaternion.Euler(0f, 0f, value);
                }
                #endregion

                #region 반바퀴 (역방향)
                time = 0f;
                animTransform.localRotation = Quaternion.Euler(0f, 0.0f, parameters.GoalValue);

                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 각도를 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.GoalValue, parameters.DefaultValue, normalTime);

                    // Z-축 회전 각도 대입 (로컬 좌표계 기준)
                    animTransform.localRotation = Quaternion.Euler(0f, 0f, value);
                }
                #endregion
            }
        }

        private IEnumerator LoopMovementY(float period)
        {
            // 조작할 대상 Transform
            //RectTransform rt = image.rectTransform;
            Transform animTransform = image == null ? spriteRenderer.transform : image.transform;
            Vector3 targetPos = animTransform.localPosition;

            // 반바퀴 주기
            float halfPeriod = period * 0.5f;

            while (true)
            {
                float time = 0f;
                animTransform.localPosition = targetPos;

                #region 반바퀴 (정방향)
                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 값을 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.DefaultValue, parameters.GoalValue, normalTime); // <-- 선형보간함수 (Lerp)
                    targetPos = animTransform.localPosition;
                    targetPos.y = value;

                    // Y-축 이동 위치 대입.
                    animTransform.localPosition = targetPos;
                }
                #endregion

                #region 반바퀴 (역방향)
                time = 0f;
                targetPos = animTransform.localPosition;
                targetPos.y = parameters.GoalValue;
                animTransform.localPosition = targetPos;

                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 각도를 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.GoalValue, parameters.DefaultValue, normalTime);
                    targetPos = animTransform.localPosition;
                    targetPos.y = value;

                    // Y-축 이동 위치 대입.
                    animTransform.localPosition = targetPos;
                }
                #endregion
            }
        }

        private IEnumerator LoopMovementX(float period)
        {
            // 조작할 대상 Transform
            //RectTransform rt = image.rectTransform;
            Transform animTransform = image == null ? spriteRenderer.transform : image.transform;
            Vector3 targetPos = animTransform.localPosition;

            // 반바퀴 주기
            float halfPeriod = period * 0.5f;

            while (true)
            {
                float time = 0f;
                animTransform.localPosition = targetPos;

                #region 반바퀴 (정방향)
                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 값을 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.DefaultValue, parameters.GoalValue, normalTime); // <-- 선형보간함수 (Lerp)
                    targetPos = animTransform.localPosition;
                    targetPos.x = value;

                    // X-축 이동 위치 대입.
                    animTransform.localPosition = targetPos;
                }
                #endregion

                #region 반바퀴 (역방향)
                time = 0f;
                targetPos = animTransform.localPosition;
                targetPos.x = parameters.GoalValue;
                animTransform.localPosition = targetPos;

                while (time < halfPeriod)
                {
                    yield return new WaitForEndOfFrame();

                    // 시간값 누적 (1프레임이 갱신되는데 소요된 시간 값을 더해줌)
                    time += Time.deltaTime;

                    // normalization time을 계산하여, 목표 각도를 선형보간함.
                    float normalTime = time / halfPeriod;
                    float value = Mathf.Lerp(parameters.GoalValue, parameters.DefaultValue, normalTime);
                    targetPos = animTransform.localPosition;
                    targetPos.x = value;

                    // X-축 이동 위치 대입.
                    animTransform.localPosition = targetPos;
                }
                #endregion
            }
        }

        private IEnumerator LoopChangeImage(float period)
        {
            Sprite originSprite = image != null ? image.sprite : spriteRenderer.sprite;
            float halfPeriod = period * 0.5f;

            yield return new WaitForSeconds(halfPeriod);

            SetSprite(parameters.ChangeSprite);

            yield return new WaitForSeconds(halfPeriod);

            SetSprite(originSprite);
        }

        private void SetSprite(Sprite sprite)
        {
            if (image)
            {
                image.gameObject.SetActive((sprite != null));
                image.sprite = sprite;
            }
            else
            {
                spriteRenderer.gameObject.SetActive((sprite != null));
                spriteRenderer.sprite = sprite;
            }
        }
    }
}