using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public enum LogLevel
    {
        Log, Warning, Error
    }

    public class MCLogLevel : MonoBehaviour
    {
        public LogLevel logLevel;

        [SerializeField]
        private MCLogNode refNode;

        private void OnEnable()
        {
            if (refNode == null)
            {
                refNode = GetComponentInParent<MCLogNode>();
            }
        }

        public void ChangeLogLevel(bool isActive)
        {
            if (isActive)
            {
                refNode.ChangeLogLevel(isActive, this);
            }
        }
    }
}