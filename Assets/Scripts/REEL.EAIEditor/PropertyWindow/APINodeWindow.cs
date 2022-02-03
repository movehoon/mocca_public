//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace REEL.D2EEditor
//{
//	public class APINodeWindow : MonoBehaviour/*, IShowProperty*/
//	{
//        [SerializeField] private Text nodeTypeText = null;
//        [SerializeField] private Text nodeIDText = null;
//        [SerializeField] private InputField nodeTitleInput = null;
//        [SerializeField] private InputField requestInput = null;
//        [SerializeField] private Dropdown switchTypeDropdown = null;
//        [SerializeField] private InputField switchNameInput = null;
//        [SerializeField] private ScrollRect scrollRect = null;

//        [SerializeField] private RectTransform content = null;
//        [SerializeField] private float defaultHeight = 0f;
//        [SerializeField] private float itemHeight = 60f;

//        [SerializeField] private CaseBlock[] caseBlocks = null;            // case block.
//        [SerializeField] private DefaultBlock defaultBlock = null;         // default block.

//        //private SwitchBranchItem switchNode = null;
//        private APIBlockItem apiNode = null;

//        private void OnEnable()
//        {
//            ResetTypeDropdown();
//        }

//        private void OnDisable()
//        {   
//            content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight);
//            scrollRect.verticalScrollbar.value = 1f;
//            nodeTitleInput.onValueChanged.RemoveAllListeners();
//            requestInput.onValueChanged.RemoveAllListeners();
//            switchTypeDropdown.onValueChanged.RemoveAllListeners();

//            foreach (CaseBlock block in caseBlocks)
//            {
//                block.RemoveAllListeners();
//            }

//            apiNode = null;
//        }

//        public void ShowProperty(GraphItem node)
//        {
//            gameObject.SetActive(true);

//            nodeTitleInput.text = node.GetBlockTitle;

//            apiNode = node as APIBlockItem;
//            //nodeTitleInput.onValueChanged.AddListener(switchNode.SetBlockTitle);
//            nodeTitleInput.text = apiNode.GetBlockTitle;
//            requestInput.text = apiNode.GetRequestURL;

//            nodeTypeText.text = node.GetNodeType.ToString();
//            nodeIDText.text = node.BlockID.ToString();

//            switchTypeDropdown.value = (int)apiNode.GetAPIType;
//            switchTypeDropdown.onValueChanged.AddListener(apiNode.SetSwitchType);

//            switchNameInput.text = apiNode.GetAPIName;

//            TurnOffAll();
//            int blockCount = apiNode.GetBlockCount;
//            TurnOnWithCount(blockCount);
//            scrollRect.content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight + blockCount * itemHeight * 3);
//            scrollRect.verticalScrollbar.value = 1f;

//            // 0번은 ExecutePointLeft.
//            // ExecutePointRight에 대해서만 처리하면 되기 때문에 index 1번 부터 순회.
//            for (int ix = 0; ix < blockCount; ++ix)
//            {
//                ExecutePoint point = node.executePoints[ix + 1];
//                int nextID = point.GetLineData != null ? point.GetLineData.GetRightExecutePointInfo.blockID : -1;

//                if (point is ExecuteCasePoint)
//                {
//                    ExecuteCasePoint casePoint = point as ExecuteCasePoint;
                    
//                    caseBlocks[ix].SetProperty(casePoint.GetCaseValue, nextID.ToString());
//                    caseBlocks[ix].valueInput.onValueChanged.AddListener(casePoint.SetCaseValue);
//                }
//            }

//            ExecuteDefaultPoint defaultPoint = node.executePoints[node.executePoints.Length - 1] as ExecuteDefaultPoint;
//            int defaultNextID = defaultPoint.GetLineData != null ? defaultPoint.GetLineData.GetRightExecutePointInfo.blockID : -1;
//            defaultBlock.SetProperty(defaultNextID.ToString());
//        }

//        private void TurnOffAll()
//        {
//            content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight);

//            foreach (CaseBlock block in caseBlocks)
//            {
//                block.ShowBlocks(false);
//            }
//        }

//        private void TurnOnWithCount(int count)
//        {
//            for (int ix = 0; ix < count; ++ix)
//            {
//                caseBlocks[ix].ShowBlocks(true);
//            }
//        }

//        private void ResetTypeDropdown()
//        {
//            switchTypeDropdown.ClearOptions();
//            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
//            for (int ix = 0; ix < (int)Constants.APIBranchType.LENGTH; ++ix)
//            {
//                string optionString = ((Constants.APIBranchType)(ix)).ToString();
//                options.Add(new Dropdown.OptionData(optionString));
//            }
//            switchTypeDropdown.AddOptions(options);
//        }

//        [System.Serializable]
//        public class CaseBlock
//        {
//            [SerializeField] public GameObject block;
//            [SerializeField] public InputField valueInput;
//            [SerializeField] public Text nextIDText;

//            public void RemoveAllListeners()
//            {
//                valueInput.onValueChanged.RemoveAllListeners();
//            }

//            public void ShowBlocks(bool isShow)
//            {
//                block.SetActive(isShow);
//                if(valueInput) valueInput.transform.parent.gameObject.SetActive(isShow);
//                nextIDText.transform.parent.gameObject.SetActive(isShow);
//            }

//            public void SetProperty(string valueText, string nextIDText)
//            {
//                if (valueInput != null)
//                    valueInput.text = valueText;

//                this.nextIDText.text = nextIDText;
//            }
//        }

//        [System.Serializable]
//        public class DefaultBlock
//        {
//            [SerializeField] GameObject block;
//            [SerializeField] Text nextIDText;

//            public void ShowBlocks(bool isShow)
//            {
//                block.SetActive(isShow);
//                nextIDText.transform.parent.gameObject.SetActive(isShow);
//            }

//            public void SetProperty(string nextIDText)
//            {   
//                this.nextIDText.text = nextIDText;
//            }
//        }
//    }
//}