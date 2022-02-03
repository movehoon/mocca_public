using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCVariableContextPopup : MCPopup
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            Utils.GetGraphPane().SubscribeOnPointDown(HidePopup);
        }

        public void OnGetClicked()
        {
            //MCNode node = MCWorkspaceManager.Instance.PaneObject
            //    .AddNode(Input.mousePosition, PROJECT.NodeType.GET);

            int nodeID = Utils.NewGUID;
            string currentVariableName = MCEditorManager.Instance.DragInfoText;
            MCUndoRedoManager.Instance.AddCommand(new MCAddNodeCommand(
                Input.mousePosition, PROJECT.NodeType.GET, nodeID, false, false, false, 
                currentVariableName,
                (node) =>
                {
                    if (node != null && node is MCGetNode)
                    {
                        //Utils.LogRed("[ContextPopup Callback in GetClicked]");

                        //LeftMenuVariableItem
                        MCGetNode getNode = node as MCGetNode;
                        getNode.IsLocalVariable = MCEditorManager.Instance.isLocalVariable;

                        //string currentVariableName = MCEditorManager.Instance.DragInfoText;
                        //int dropdownIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(currentVariableName);
                        int dropdownIndex = -1;
                        if (getNode.IsLocalVariable == true)
                        {
                            MoccaTMPDropdownHelper helper = getNode.GetComponentInChildren<MoccaTMPDropdownHelper>();
                            helper.type = MoccaTMPDropdownHelper.Type.LocalVariableList;
                            helper.InitializeDropdownOptionData();
                            dropdownIndex = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(node.nodeData.body.name);
                        }
                        else
                        {
                            dropdownIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(node.nodeData.body.name);
                        }

                        // Todo : Change to set proper index.
                        if (dropdownIndex != -1)
                        {
                            getNode.SetDropdownIndex(dropdownIndex);

                            if (getNode.IsLocalVariable == true)
                            {
                                MCWorkspaceManager.Instance.VariableFunctionManager.AdjustLocalVariableContentHeight();
                            }
                            else
                            {
                                // 변수 추가 후 content 높이 값 조정.
                                MCWorkspaceManager.Instance.VariableFunctionManager.AdjustVariableContentHeight();
                            }
                        }
                    }
                }));

            //MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(nodeID);
            //if (node != null && node is MCGetNode)
            //{
            //    //LeftMenuVariableItem
            //    MCGetNode getNode = node as MCGetNode;

            //    string currentVariableName = MCEditorManager.Instance.DragInfoText;
            //    int dropdownIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(currentVariableName);

            //    // Todo : Change to set proper index.
            //    getNode.SetDropdownIndex(dropdownIndex);

            //    // 변수 추가 후 content 높이 값 조정.
            //    MCWorkspaceManager.Instance.VariableFunctionManager.AdjustVariableContentHeight();
            //}

            HidePopup();
        }

        public void OnSetClicked()
        {
            //MCNode node = MCWorkspaceManager.Instance.PaneObject
            //    .AddNode(Input.mousePosition, PROJECT.NodeType.SET);

            int nodeID = Utils.NewGUID;
            string currentVariableName = MCEditorManager.Instance.DragInfoText;
            MCUndoRedoManager.Instance.AddCommand(new MCAddNodeCommand(
                Input.mousePosition, PROJECT.NodeType.SET, nodeID, false, false, false,
                currentVariableName,
                (node) =>
                {
                    if (node != null && node is MCSetNode)
                    {
                        MCSetNode setNode = node as MCSetNode;
                        setNode.IsLocalVariable = MCEditorManager.Instance.isLocalVariable;

                        //int dropdownIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(currentVariableName);
                        int dropdownIndex = -1;
                        if (setNode.IsLocalVariable == true)
                        {
                            MoccaTMPDropdownHelper helper = setNode.GetComponentInChildren<MoccaTMPDropdownHelper>();
                            helper.type = MoccaTMPDropdownHelper.Type.LocalVariableList;
                            helper.InitializeDropdownOptionData();
                            dropdownIndex = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(node.nodeData.body.name);
                        }
                        else
                        {
                            dropdownIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(node.nodeData.body.name);
                        }

                        setNode.SetDropdownIndex(dropdownIndex);

                        LeftMenuVariableItem variable = setNode.IsLocalVariable ?
                        MCWorkspaceManager.Instance.GetLocalVariable(dropdownIndex) :
                        MCWorkspaceManager.Instance.GetVariable(dropdownIndex);

                        if (variable.dataType == PROJECT.DataType.FACIAL ||
                            variable.dataType == PROJECT.DataType.MOTION ||
                            variable.dataType == PROJECT.DataType.MOBILITY)
                        {
                            string variableValue = variable.value;
                            int changeDropdownIndex = Constants.GetDropdownValueIndex(variableValue, variable.dataType);
                            setNode.SetChangeDropdownIndex(changeDropdownIndex);
                        }
                    }
                }));

            //MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(nodeID);
            //if (node != null && node is MCSetNode)
            //{
            //    MCSetNode setNode = node as MCSetNode;

            //    string currentVariableName = MCEditorManager.Instance.DragInfoText;
            //    int dropdownIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(currentVariableName);

            //    setNode.SetDropdownIndex(dropdownIndex);

            //    LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariable(dropdownIndex);
            //    if (variable.dataType == PROJECT.DataType.FACIAL || 
            //        variable.dataType == PROJECT.DataType.MOTION ||
            //        variable.dataType == PROJECT.DataType.MOBILITY)
            //    {
            //        string variableValue = variable.value;
            //        int changeDropdownIndex = Constants.GetDropdownValueIndex(variableValue, variable.dataType);
            //        setNode.SetChangeDropdownIndex(changeDropdownIndex);
            //    }
            //}

            HidePopup();
        }
    }
}