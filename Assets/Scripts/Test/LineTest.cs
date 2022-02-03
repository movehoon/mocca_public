using UnityEngine;

namespace REEL.Test
{
    public class LineTest : MonoBehaviour
    {
        public Transform[] points;
        public Transform[] controlPoints;

        void Update()
        {
            CalculateControlPoints();
        }

        private void CalculateControlPoints()
        {
            Vector3 startToEnd = points[0].position - points[1].position;
            Vector3 endToStart = points[1].position - points[0].position;

            Quaternion rot = Quaternion.Euler(0f, 0f, 90f);
            Vector3 controlPoint1 = rot * startToEnd;
            controlPoint1 = points[0].position + controlPoint1;
            Vector3 controlPoint2 = rot * endToStart;
            controlPoint2 = points[1].position + controlPoint2;

            controlPoints[0].position = controlPoint1;
            controlPoints[1].position = controlPoint2;
        }
    }
}