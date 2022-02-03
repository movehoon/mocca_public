using UnityEngine;

namespace REEL.D2EEditor
{
    // 작성자: 장세윤.
    // 노드 정렬할 때 왼쪽/상단 지점을 기준으로 정렬하기위한 위치를 저장하는 컴포넌트.
    // 월드 좌표 기준으로 TopLeft 지점 / 왼쪽 상단에서 블록 위치까지의 오프셋을 저장 및 계산함.
    public class MCNodePosition : MonoBehaviour
    {
        private RectTransform refRect;
        public RectTransform RefRect
        {
            get
            {
                if (refRect == null)
                {
                    refRect = GetComponent<RectTransform>();
                }

                return refRect;
            }
        }

        public Vector3 TopLeftWorldPosition
        {
            get
            {
                Vector3[] corners = new Vector3[4];

                // array of 4 vertices is clockwise starts from bottom left.
                RefRect.GetWorldCorners(corners);

                // index 0 -> bottom left.
                // index 1 -> top left.
                // index 2 -> top right.
                // index 3 -> bottom right.
                return corners[1];
            }
        }

        public Vector2 OffsetPoint
        {
            get
            {
                return transform.position - TopLeftWorldPosition;
            }
        }
    }
}