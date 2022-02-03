//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace REEL.D2EEditor
//{
//	public class VariableNodeWindow : MonoBehaviour/*, IShowProperty*/
//	{
//        [SerializeField] private Text nodeTypeText = null;
//        [SerializeField] private Text nodeIDText = null;
//        [SerializeField] private InputField nodeTitleInput = null;
//        [SerializeField] private InputField nodeNameInput = null;
//        [SerializeField] private Dropdown nodeOperatorDropdown = null;
//        [SerializeField] private InputField nodeValueInput = null;
//        [SerializeField] private Text nextIDText = null;

//        [SerializeField] private VariableItem variableNode = null;

//        private void OnEnable()
//        {
//            ResetDropdownList();
//        }

//        private void OnDisable()
//        {
//            variableNode = null;
//            nodeTitleInput.onValueChanged.RemoveAllListeners();
//            nodeNameInput.onValueChanged.RemoveAllListeners();
//            nodeOperatorDropdown.onValueChanged.RemoveAllListeners();
//            nodeValueInput.onValueChanged.RemoveAllListeners();
//        }

//        public void ShowProperty(GraphItem node)
//        {
//            gameObject.SetActive(true);

//            variableNode = node as VariableItem;
//            nodeTitleInput.text = variableNode.GetBlockTitle;
//            nodeNameInput.text = variableNode.GetBlockName;

//            nodeTitleInput.onValueChanged.AddListener(variableNode.SetBlockTitle);
//            nodeNameInput.onValueChanged.AddListener(variableNode.SetBlockName);

//            nodeOperatorDropdown.value = (int)variableNode.GetOperatorType;
//            nodeOperatorDropdown.onValueChanged.AddListener(variableNode.SetOperatorType);

//            nodeTypeText.text = node.GetNodeType.ToString();
//            nodeIDText.text = node.BlockID.ToString();

//            nodeValueInput.text = node.GetItemData() != null ? node.GetItemData().ToString() : "";
//            nodeValueInput.onValueChanged.AddListener(variableNode.SetItemData);

//            nextIDText.text = node.GetNextBlockID.ToString();
//        }

//        private void ResetDropdownList()
//        {
//            nodeOperatorDropdown.ClearOptions();
//            List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();
//            for (int ix = 0; ix < (int)XMLVariableOperatorType.Length; ++ix)
//            {
//                optionData.Add(new Dropdown.OptionData() { text = ((XMLVariableOperatorType)(ix)).ToString() });
//            }

//            nodeOperatorDropdown.AddOptions(optionData);
//        }
//    }
//}