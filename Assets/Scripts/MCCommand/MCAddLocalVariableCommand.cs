using REEL.PROJECT;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCAddLocalVariableCommand : MCCommand
    {
        private string name = string.Empty;
        private DataType type = DataType.NONE;
        private NodeType nodeType = NodeType.START;
        private string value = string.Empty;
        private int variableID = 0;
        private string functionName = "";

        public MCAddLocalVariableCommand(string name, DataType type, NodeType nodeType, string value, string functionName)
        {
            this.name = name;
            this.type = type;
            this.nodeType = nodeType;
            this.value = value;
            this.functionName = functionName;
        }

        public void Execute()
        {
            MCVariableFunctionManager manager = GameObject.FindObjectOfType<MCVariableFunctionManager>();
            if (manager == null)
            {
                Utils.LogRed("[AddLocalVariableCommand Error] Can't find MCVariableFunctionManager");
                return;
            }

            bool success = manager.AddLocalVariable(name, type, nodeType, value, functionName);
            if (success)
            {
                int variableIndex = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(name);
                variableID = MCWorkspaceManager.Instance.GetLocalVariable(variableIndex).VariableID;
                MCWorkspaceManager.Instance.SubscribeLocalVariableUpdate(OnLocalVariableUpdate);
            }
            else
            {
                Utils.LogRed("[AddLocalVariableCommand Error] Can't Add Local Variable");
                MessageBox.Show("[ID_SAME_LOCAL_VARIABLE]같은(동일한) 이름을 가진 지역 변수가 이미 존재합니다.\n다른 이름을 입력해주세요."); // local 추가 완료
                return;
            }
        }

        public void Undo()
        {
            int variableIndex = MCWorkspaceManager.Instance.GetLocalVariableIndexWithName(name);
            MCWorkspaceManager.Instance.RequestLocalVariableDelete(MCWorkspaceManager.Instance.GetLocalVariable(variableIndex));
        }

        // 변수가 변경된 경우 처리.
        private void OnLocalVariableUpdate()
        {
            LeftMenuVariableItem variable = MCWorkspaceManager.Instance.GetLocalVariableWithID(variableID);
            if (variable == null)
            {
                //Utils.LogRed("[AddLocalVariableCommand Error] can't find local variable");
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