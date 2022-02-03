using TMPro;

namespace REEL.D2EEditor
{
    public class FacialNodeWindow : OneDropdownWindow
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            LocalizationManager.OnLanguageChanged += OnLanguageChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            if (refNode != null)
            {
                UpdateDropdownList();
            }
        }

        private void UpdateDropdownList()
        {
            targetNodeDropdown = refNode.GetComponentInChildren<TMP_Dropdown>();
            if (targetNodeDropdown != null)
            {
                nodeValueDropdown.interactable = true;
                SetDropdownList(refNode);
                nodeValueDropdown.value = targetNodeDropdown.value;
                //nodeValueDropdown.onValueChanged.AddListener(SyncDropdown);
                SyncTwoDropdowns();
            }
            else
            {
                nodeValueDropdown.interactable = false;
                int connectedNodeID = refNode.GetNodeInputWithIndex(0).input.source;

                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
                if (connectedNode is MCGetNode)
                {
                    MCGetNode getNode = connectedNode as MCGetNode;
                    string connectedNodeValue = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                    PROJECT.DataType connectedNodeDataType = MCWorkspaceManager.Instance.GetVariableDataType(getNode.CurrentVariableIndex);
                    nodeValueDropdown.GetComponentInChildren<TMP_Text>().text = Constants.GetValueForKorean(connectedNodeValue, connectedNodeDataType);
                }
                else
                {
                    onOffBlocks[0].SetActive(false);
                }
            }
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            UpdateDropdownList();

            //targetNodeDropdown = refNode.GetComponentInChildren<TMP_Dropdown>();
            //if (targetNodeDropdown != null)
            //{
            //    nodeValueDropdown.interactable = true;
            //    SetDropdownList(node);
            //    nodeValueDropdown.value = targetNodeDropdown.value;
            //    //nodeValueDropdown.onValueChanged.AddListener(SyncDropdown);
            //    SyncTwoDropdowns();
            //}
            //else
            //{
            //    nodeValueDropdown.interactable = false;
            //    int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;

            //    MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
            //    if (connectedNode is MCGetNode)
            //    {
            //        MCGetNode getNode = connectedNode as MCGetNode;
            //        string connectedNodeValue = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
            //        PROJECT.DataType connectedNodeDataType = MCWorkspaceManager.Instance.GetVariableDataType(getNode.CurrentVariableIndex);
            //        nodeValueDropdown.GetComponentInChildren<TMP_Text>().text = Constants.GetValueForKorean(connectedNodeValue, connectedNodeDataType);
            //    }
            //    else
            //    {
            //        onOffBlocks[0].SetActive(false);
            //    }
            //}
        }
    }
}