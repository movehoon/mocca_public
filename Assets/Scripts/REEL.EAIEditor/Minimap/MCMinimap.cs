using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCMinimap : MonoBehaviour
    {
        // 마커 이동 범위 설정용 클래스.
        [System.Serializable]
        public class RangeF
        {
            public RangeF()
            {
                min = max = 0f;
            }

            public RangeF(float min, float max)
            {
                this.min = min;
                this.max = max;
            }

            public float min;
            public float max;
        }

        [Header("MCScrollRect 참조")]
        public WorkspaceScrollRect workspaceScroll = null;

        [Header("미니맵 조정을 위한 참조")]
        public RectTransform marker = null;
        public RectTransform workspaceBG = null;
        public RectTransform miniMapRect = null;

        private Scrollbar horizontalScrollbar = null;
        private Scrollbar verticalScrollbar = null;

        private RangeF moveRangeX;
        private RangeF moveRangeY;

        private void Awake()
        {
            // 0 ~ 1.
            horizontalScrollbar = workspaceScroll.horizontalScrollbar;

            // 1 ~ 0 -> 1 - x.
            verticalScrollbar = workspaceScroll.verticalScrollbar;
        }

        private void LateUpdate()
        {
            // 스케일에 따라 이동 가능한 최대/최소 범위가 달라져 매 프레임 연산해야 함.
            moveRangeX = new RangeF(0f, miniMapRect.sizeDelta.x - (marker.sizeDelta.x * marker.localScale.x));
            moveRangeY = new RangeF(0f, - (miniMapRect.sizeDelta.y - (marker.sizeDelta.y * marker.localScale.y)));

            // 마커 스케일을 작업창 스케일에 맞추기.
            marker.localScale = workspaceBG.localScale;

            // Lerp를 통해 스크롤바 값으로 마커 위치 구하기.
            float xPosition = Lerpf(moveRangeX, horizontalScrollbar.value);
            float yPosition = Lerpf(moveRangeY, 1f - verticalScrollbar.value);

            // 마커 위치 설정.
            marker.anchoredPosition = new Vector2(xPosition, yPosition);
        }

        // 범위 값을 이용한 Lerp 연산.
        private float Lerpf(RangeF range, float t)
        {
            return ((1 - t) * range.min) + (t * range.max);
        }
    }
}