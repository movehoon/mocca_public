using TMPro;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class ForNodeWindow : MCNodeWindow
    {
        public TMP_InputField initialValueInput;
        public TMP_InputField targetNodeInitialValueInput;

        public TMP_InputField conditionValueInput;
        public TMP_InputField targetNodeConditionValueInput;

        public TMP_InputField incrementValueInput;
        public TMP_InputField targetNodeIncrementValueInput;

        protected override void OnDisable()
        {
            base.OnDisable();

            SafeRemoveAllListeners(initialValueInput, targetNodeInitialValueInput);
            SafeRemoveAllListeners(conditionValueInput, targetNodeConditionValueInput);
            SafeRemoveAllListeners(incrementValueInput, targetNodeIncrementValueInput);

            #region Backup
            //initialValueInput.onValueChanged.RemoveAllListeners();
            //if (targetNodeInitialValueInput != null)
            //{
            //    targetNodeInitialValueInput.onValueChanged.RemoveAllListeners();
            //    targetNodeInitialValueInput = null;
            //}

            //conditionValueInput.onValueChanged.RemoveAllListeners();
            //if (targetNodeConditionValueInput != null)
            //{
            //    targetNodeConditionValueInput.onValueChanged.RemoveAllListeners();
            //    targetNodeConditionValueInput = null;
            //}

            //incrementValueInput.onValueChanged.RemoveAllListeners();
            //if (targetNodeIncrementValueInput != null)
            //{
            //    targetNodeIncrementValueInput.onValueChanged.RemoveAllListeners();
            //    targetNodeIncrementValueInput = null;
            //}
            #endregion
        }

        void SafeRemoveAllListeners(TMP_InputField windowInputField, TMP_InputField targetNodeInputField)
        {
            windowInputField.onValueChanged.RemoveAllListeners();
            if (targetNodeInputField != null)
            {
                targetNodeInputField.onValueChanged.RemoveAllListeners();
            }
        }

        private void SyncTwoInputFields(TMP_InputField input1, TMP_InputField input2)
        {
            input1.onValueChanged.AddListener((text) =>
            {
                //Utils.LogGreen($"[SyncTwoInputField] targetNodeInput.text: {targetNodeInput.text} / text: {text}");
                input2.text = text;
            });

            input2.onValueChanged.AddListener((text) =>
            {
                //Utils.LogGreen($"[SyncTwoInputField] nodeValueInput.text: {nodeValueInput.text} / text: {text}");
                input1.text = text;
            });
        }

        protected void SyncInputFields()
        {
            //nodeValueInput.text = targetNodeInput.text;
            initialValueInput.onValueChanged.AddListener((text) =>
            {
                //Utils.LogGreen($"[SyncTwoInputField] targetNodeInput.text: {targetNodeInput.text} / text: {text}");
                targetNodeInitialValueInput.text = text;
            });

            targetNodeInitialValueInput.onValueChanged.AddListener((text) =>
            {
                //Utils.LogGreen($"[SyncTwoInputField] nodeValueInput.text: {nodeValueInput.text} / text: {text}");
                initialValueInput.text = text;
            });
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            MCLoopNode loopNode = node as MCLoopNode;
            ConnectNodeInputFieldToPropertyInputField(
                node, initialValueInput, targetNodeInitialValueInput, loopNode.initialInput, 0
            );

            ConnectNodeInputFieldToPropertyInputField(
                node, conditionValueInput, targetNodeConditionValueInput, loopNode.conditionInput, 1
            );

            ConnectNodeInputFieldToPropertyInputField(
                node, incrementValueInput, targetNodeIncrementValueInput, loopNode.incrementInput, 2
            );

            #region Backup
            //targetNodeInitialValueInput = node.GetComponentInChildren<TMP_InputField>();
            //if (targetNodeInitialValueInput != null)
            //{
            //    initialValueInput.interactable = true;
            //    initialValueInput.text = targetNodeInitialValueInput.text;
            //    //nodeValueInput.onValueChanged.AddListener(SyncInputField);
            //    SyncInputFields();
            //}
            //else
            //{
            //    initialValueInput.interactable = false;
            //    int connectedNodeID = node.GetNodeInputWithIndex(0).input.source;

            //    MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
            //    if (connectedNode is MCGetNode)
            //    {
            //        MCGetNode getNode = connectedNode as MCGetNode;
            //        initialValueInput.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
            //    }
            //    else
            //        onOffBlocks[0].SetActive(false);
            //}
            #endregion
        }

        private void ConnectNodeInputFieldToPropertyInputField(
            MCNode node, 
            TMP_InputField propertyInputField, TMP_InputField targetNodeInputField, TMP_InputField nodeInputField, 
            int blockIndex)
        {
            targetNodeInputField = nodeInputField;
            if (nodeInputField.gameObject.activeSelf == true)
            {
                propertyInputField.interactable = true;
                propertyInputField.text = targetNodeInputField.text;
                SyncTwoInputFields(propertyInputField, nodeInputField);
            }
            else
            {
                propertyInputField.interactable = false;
                int connectedNodeID = node.GetNodeInputWithIndex(blockIndex).input.source;

                MCNode connectedNode = MCWorkspaceManager.Instance.FindNodeWithID(connectedNodeID);
                if (connectedNode is MCGetNode)
                {
                    MCGetNode getNode = connectedNode as MCGetNode;
                    propertyInputField.text = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
                }
                else
                {
                    onOffBlocks[blockIndex].SetActive(false);
                }
            }
        }
    }
}
//else if (inputNode is MCCountNode)
//{
//    nodeValueInput.interactable = false;
//    MCCountNode countNode = inputNode as MCCountNode;
//    int connectedWithCountID = countNode.GetNodeInputWithIndex(0).input.source;
//    MCNode inputNodeOfCount = MCWorkspaceManager.Instance.FindNodeWithID(connectedWithCountID);
//    if (inputNodeOfCount is MCGetNode)
//    {
//        MCGetNode getNode = inputNodeOfCount as MCGetNode;
//        string jsonString = MCWorkspaceManager.Instance.GetVariableValue(getNode.CurrentVariableIndex);
//        PROJECT.ListValue list = JsonConvert.DeserializeObject<PROJECT.ListValue>(jsonString);
//        nodeValueInput.text = list.listValue.Length.ToString();
//    }
//}