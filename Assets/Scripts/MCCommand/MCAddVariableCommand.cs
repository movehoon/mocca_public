using REEL.PROJECT;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCAddVariableCommand : MCCommand
    {
        private string name = string.Empty;
        private DataType type = DataType.NONE;
        private NodeType nodeType = NodeType.START;
        private string value = string.Empty;
        private int variableID = 0;

        public MCAddVariableCommand(string name, DataType type, NodeType nodeType, string value)
        {
            this.name = name;
            this.type = type;
            this.nodeType = nodeType;
            this.value = value;
        }

        public void Execute()
        {
            MCVariableFunctionManager manager = GameObject.FindObjectOfType<MCVariableFunctionManager>();
            if (manager == null)
            {
                Utils.LogRed("[AddVariableCommand Error] Can't find MCVariableFunctionManager");
                return;
            }

            bool success = manager.AddVariable(name, type, nodeType, value);
            if (success)
            {
                int variableIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(name);
                variableID = MCWorkspaceManager.Instance.GetVariable(variableIndex).VariableID;
                MCWorkspaceManager.Instance.SubscribeVariableUpdate(OnVariableUpdate);
            }
            else
            {
                Utils.LogRed("[AddVariableCommand Error] Can't Add Variable");
                return;
            }
        }

        public void Undo()
        {
            int variableIndex = MCWorkspaceManager.Instance.GetVariableIndexWithName(name);
            MCWorkspaceManager.Instance.RequestVariableDelete(MCWorkspaceManager.Instance.GetVariable(variableIndex));
        }

        // 변수가 변경된 경우 처리.
        private void OnVariableUpdate()
        {
            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetVariableWithID(variableID);
            if (variable == null)
            {
                //Utils.LogRed("[AddVariableCommand Error] can't find variable");
                return;
            }
            else
            {
                UpdateProperties(variable);
            }
        }

        // 변수 속성이 변경되면 내용 업데이트.
        private void UpdateProperties(LeftMenuVariableItem variable)
        {
            name = variable.nameText.text;
            type = variable.dataType;
            nodeType = variable.nodeType;
            value = variable.value;
        }
    }
}