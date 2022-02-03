using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    [CreateAssetMenu(menuName = "Create NodeTooltipData")]
    public class MCNodeTooltipData : ScriptableObject
    {
        public PROJECT.NodeType nodeType;
        public Sprite tooltipImageSprite;
        public string nodeName;
        public List<string> inputTexts = new List<string>();
        public List<string> outputTexts = new List<string>();
        public string descTexts = string.Empty;
    }
}