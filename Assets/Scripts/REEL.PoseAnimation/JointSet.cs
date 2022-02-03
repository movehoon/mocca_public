using System.Collections;
using UnityEngine;

namespace REEL.PoseAnimation
{
    public enum JointAxis
    {
        RotFX, RotFY, RotFZ, RotRX, RotRY, RotRZ
    }

    [System.Serializable]
    public class JointSet
    {
        public Transform joint;
        public Vector3 baseRotation;
        public JointAxis jointRotAxis;
        public bool isFixed = false;
        private float angle;

        public void SetAngle(float angle)
        {
            this.angle = angle;
            Vector3 rot = isFixed ? GetFixedEulerAngle(angle) : GetEulerAngle(angle);
            joint.localRotation = Quaternion.Euler(rot);
        }

        public IEnumerator SetAngleLerp(float angle, float duration, bool isDebug = false)
        {
            float elapsedTime = 0f;
            this.angle = angle;
            Vector3 rot = isFixed ? GetFixedEulerAngle(angle) : GetEulerAngle(angle);
            Quaternion startRot = joint.localRotation;
            Quaternion targetRot = Quaternion.Euler(rot);

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                float normalTime = elapsedTime / duration;
                normalTime = float.IsInfinity(normalTime) ? 0f : normalTime;
                joint.localRotation = Quaternion.Lerp(startRot, targetRot, normalTime);

                if (isDebug) Debug.Log("elapsedTime: " + elapsedTime + " ,normalTime: " + normalTime);
                yield return new WaitForEndOfFrame();
            }
        }

        public float GetAngle()
        {
            return angle;
        }

        public void SetBaseAngle()
        {
            joint.localRotation = Quaternion.Euler(baseRotation);
        }

        public float GetTargetAngleAxis(float angle)
        {
            Vector3 currentRot = joint.localRotation.eulerAngles;
            Vector3 targetRot = GetTargetAngle(angle);

            switch (jointRotAxis)
            {
                case JointAxis.RotFX: return targetRot.x - currentRot.x;
                case JointAxis.RotRX: return currentRot.x - targetRot.x;
                case JointAxis.RotFY: return targetRot.y - currentRot.y;
                case JointAxis.RotRY: return currentRot.y - targetRot.y;
                case JointAxis.RotFZ: return targetRot.z - currentRot.z;
                case JointAxis.RotRZ: return currentRot.z - targetRot.z;
                default: return -1f;
            }
        }

        public Vector3 GetTargetAngle(float angle)
        {
            return isFixed ? GetFixedEulerAngle(angle) : GetEulerAngle(angle);
        }

        Vector3 GetEulerAngle(float angle)
        {
            Vector3 rot = joint.localEulerAngles;

            switch (jointRotAxis)
            {
                case JointAxis.RotFX: rot.x = baseRotation.x + angle; break;
                case JointAxis.RotRX: rot.x = baseRotation.x - angle; break;
                case JointAxis.RotFY: rot.y = baseRotation.y + angle; break;
                case JointAxis.RotRY: rot.y = baseRotation.y - angle; break;
                case JointAxis.RotFZ: rot.z = baseRotation.z + angle; break;
                case JointAxis.RotRZ: rot.z = baseRotation.z - angle; break;

                default: break;
            }

            return rot;
        }

        Vector3 GetFixedEulerAngle(float angle)
        {
            Vector3 rot = baseRotation;

            switch (jointRotAxis)
            {
                case JointAxis.RotFX: rot.x = baseRotation.x + angle; break;
                case JointAxis.RotRX: rot.x = baseRotation.x - angle; break;
                case JointAxis.RotFY: rot.y = baseRotation.y + angle; break;
                case JointAxis.RotRY: rot.y = baseRotation.y - angle; break;
                case JointAxis.RotFZ: rot.z = baseRotation.z + angle; break;
                case JointAxis.RotRZ: rot.z = baseRotation.z - angle; break;

                default: break;
            }

            return rot;
        }
    }
}