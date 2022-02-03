using UnityEngine;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public class MCTables : MonoBehaviour
    {
        public List<MCNode> locatedNodes = new List<MCNode>();
        public List<MCBezierLine> locatedLines = new List<MCBezierLine>();
        public List<LeftMenuVariableItem> variables = new List<LeftMenuVariableItem>();
        public List<LeftMenuVariableItem> localVariables = new List<LeftMenuVariableItem>();
        //public MCFunctionTable functionTables;
        //public List<LeftMenuFunctionItem> functions = new List<LeftMenuFunctionItem>();

        //private void OnEnable()
        //{
        //}

        private void LateUpdate()
        {
            if (shouldCheckSocketState == true)
            {
                shouldCheckSocketState = false;
                CheckSocketState();

                // 디버깅용
                //Invoke("CheckSocketState", 2f);
            }

            if (shouldCheckIsLineIsValid == true)
            {
                shouldCheckIsLineIsValid = false;
                CheckLineIsValid();

                // 디버깅용
                //Invoke("CheckLineIsValid", 2f);
            }
        }

        public void AddNode(MCNode node)
        {
            locatedNodes.Add(node);
        }

        public bool RemoveNode(MCNode node)
        {
            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                return locatedNodes.Remove(node);
            }
            else
            {
                MCWorkspaceManager manager = MCWorkspaceManager.Instance;
                LeftMenuFunctionItem function = manager.GetFunctionItemWithName(Utils.CurrentTabName);
                return function.RemoveLogicNode(node);
            }
        }

        //// 특정 함수에 로직 노드를 추가하는 함수.
        //// 함수 이름은 고유해야 함.
        //public bool AddFunctionNode(string functionName, MCNode node)
        //{
        //    return functionTables.AddFunctionNode(functionName, node);
        //}

        //// 특정 함수의 로직 노드를 제거하는 함수.
        //// 함수 이름은 고유해야 함.
        //public bool RemoveFunctionNode(string functionName, MCNode node)
        //{
        //    return functionTables.RemoveFunctionNode(functionName, node);
        //}

        public void AddLine(MCBezierLine line)
        {
            locatedLines.Add(line);
        }

        public bool RemoveLine(MCBezierLine line)
        {
            return locatedLines.Remove(line);
        }

        public bool RemoveLine(int lineID)
        {
            foreach (MCBezierLine line in locatedLines)
            {
                if (line.LineID.Equals(lineID))
                {
                    return locatedLines.Remove(line);
                }
            }

            return false;
        }

        public void UpdateAllLinesPosition()
        {
            int index = 0;
            while (index < locatedLines.Count)
            {
                var line = locatedLines[index];
                LinePoint linePoint = new LinePoint();
                linePoint.start = line.left.GetSocketPosition;
                linePoint.end = line.right.GetSocketPosition;

                locatedLines[index].UpdateLinePoint(linePoint);

                ++index;
            }
        }

        public void AddVariable(LeftMenuVariableItem variable)
        {
            variables.Add(variable);
        }

        public bool RemoveVariable(LeftMenuVariableItem variable)
        {
            return variables.Remove(variable);
        }

        public void AddLocalVariable(LeftMenuVariableItem variable)
        {
            localVariables.Add(variable);
        }

        public bool RemoveLocalVariable(LeftMenuVariableItem localVariable)
        {
            return localVariables.Remove(localVariable);
        }

        //public void AddFunction(LeftMenuFunctionItem function)
        //{
        //    functionTables.AddFunction(function);
        //}

        //public bool RemoveFunction(LeftMenuFunctionItem function)
        //{
        //    return functionTables.RemoveFunction(function);
        //}

        public void SetOneSelected(MCNode node)
        {
            SetAllUnSelected();
            node.IsSelected = true;
        }

        public void SetAllSelected()
        {
            Constants.OwnerGroup ownerGroup = MCWorkspaceManager.Instance.CurrentTabOwnerGroup;
            if (ownerGroup == Constants.OwnerGroup.PROJECT)
            {
                foreach (MCNode node in locatedNodes)
                {
                    node.IsSelected = true;
                }
            }

            else if (ownerGroup == Constants.OwnerGroup.FUNCTION)
            {
                MCFunctionTable.Instance.SetAllSelected();
            }
        }

        public void SetAllUnSelected()
        {
            Constants.OwnerGroup ownerGroup = MCWorkspaceManager.Instance.CurrentTabOwnerGroup;
            List<MCNode> selected = new List<MCNode>();
            if (ownerGroup == Constants.OwnerGroup.PROJECT)
            {
                selected = SelectedNodesInProject;
                foreach (MCNode node in selected)
                {
                    node.IsSelected = false;
                }

                selected = null;
            }

            else if (ownerGroup == Constants.OwnerGroup.FUNCTION)
            {
                MCFunctionTable.Instance.SetAllUnSelected();
            }
        }

        // 같은 프로세스에 있는 모든 노드 선택 해제.
        // 프로젝트 탭에서만 사용.
        public void SetAllUnSelected(int processID)
        {
            Constants.OwnerGroup ownerGroup = MCWorkspaceManager.Instance.CurrentTabOwnerGroup;
            if (ownerGroup != Constants.OwnerGroup.PROJECT)
            {
                return;
            }

            List<MCNode> selected = new List<MCNode>();
            selected = SelectedNodesInProject;
            foreach (MCNode node in selected)
            {
                if (node.OwnedProcess.id.Equals(processID))
                {
                    node.IsSelected = false;
                }
            }

            selected = null;
        }

        public void RequestNodeDelete(MCNode node)
        {
            if (node == null || node.DontDestroy)
            {
                return;
            }

            // Test.
            // 노드 추가하기 전에 현 상태 저장.
            //MCUndoRedoManager.Instance.RecordProject();

            RequestLineDelete(node.NodeID, node.OwnedProcess.id);
            RemoveNode(node);
            Destroy(node.gameObject);
        }

        private bool AnyNodeSelected
        {
            get
            {
                if (SelectedNodesInProject.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void DeleteSelected()
        {
            // Test.
            // 노드 추가하기 전에 현 상태 저장.
            //MCUndoRedoManager.Instance.RecordProject();

            List<MCNode> selected = SelectedNodesInProject;
            if (AnyNodeSelected == false)
            {
                selected = null;
                return;
            }
            else
            {
                if (selected.Count == 1)
                {
                    MCDeleteNodeCommand command = new MCDeleteNodeCommand(
                        selected[0]
                    );
                    MCUndoRedoManager.Instance.AddCommand(command);
                }
                else if (selected.Count > 1)
                {
                    MCDeleteMultipleNodeCommand command = new MCDeleteMultipleNodeCommand(selected);
                    MCUndoRedoManager.Instance.AddCommand(command);
                }

                selected = null;
            }
        }

        public void RequestLineDelete(int nodeID, int processID)
        {
            if (locatedLines.Count == 0)
            {
                return;
            }

            List<MCBezierLine> deleteLines = new List<MCBezierLine>();
            foreach (MCBezierLine line in locatedLines)
            {
                //if (line.left.Node.NodeID.Equals(nodeID) || line.right.Node.NodeID.Equals(nodeID))
                if (line.left.Node.OwnedProcess.id.Equals(processID) && line.left.Node.NodeID.Equals(nodeID) ||
                    line.right.Node.OwnedProcess.id.Equals(processID) && line.right.Node.NodeID.Equals(nodeID))
                {
                    deleteLines.Add(line);
                }
            }

            foreach (MCBezierLine line in deleteLines)
            {
                RemoveLine(line);
                Destroy(line.gameObject);
            }

            deleteLines = null;
        }

        public void DeleteAllNodes()
        {
            //Constants.OwnerGroup ownerGroup = MCWorkspaceManager.Instance.CurrentTabOwnerGroup;
            DeleteAllProjectNodes();
        }

        private void DeleteAllProjectNodes()
        {
            while (locatedNodes.Count > 0)
            {
                MCNode node = locatedNodes[locatedNodes.Count - 1];
                RemoveNode(node);
                Destroy(node.gameObject);
            }
            locatedNodes = new List<MCNode>();
        }

        public void DeleteAllLines()
        {
            while (locatedLines.Count > 0)
            {
                MCBezierLine line = locatedLines[locatedLines.Count - 1];
                RemoveLine(line);
                Destroy(line.gameObject);
            }

            locatedLines = new List<MCBezierLine>();
        }

        public void DeleteAllVariables()
        {
            while (variables.Count > 0)
            {
                LeftMenuVariableItem variable = variables[variables.Count - 1];
                RemoveVariable(variable);
                Destroy(variable.gameObject);
            }

            variables = new List<LeftMenuVariableItem>();
        }

        public void DeleteAllLocalVariables()
        {
            while (localVariables.Count > 0)
            {
                LeftMenuVariableItem localVariable = localVariables[localVariables.Count - 1];
                RemoveLocalVariable(localVariable);
                Destroy(localVariable.gameObject);
            }

            localVariables = new List<LeftMenuVariableItem>();
        }

        public bool CanAddVariable(string variableName, LeftMenuVariableItem variableItem = null)
        {
            foreach (LeftMenuVariableItem variable in variables)
            {
                if (variableItem != null && variable.GetInstanceID().Equals(variableItem.GetInstanceID()))
                {
                    continue;
                }

                if (variable.VariableName.Equals(variableName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanAddLocalVariable(string localVariableName, LeftMenuVariableItem variableItem = null)
        {
            foreach (LeftMenuVariableItem localVariable in localVariables)
            {
                if (variableItem != null && localVariable.GetInstanceID().Equals(variableItem.GetInstanceID()))
                {
                    continue;
                }

                if (localVariable.VariableName.Equals(localVariableName))
                {
                    return false;
                }
            }

            return true;
        }

        public List<MCNode> SelectedNodesInProject
        {
            get
            {
                List<MCNode> selected = new List<MCNode>();

                foreach (MCNode node in locatedNodes)
                {
                    if (node.IsSelected)
                        selected.Add(node);
                }

                return selected;
            }
        }

        //public List<MCNode> GetSelectedNodesInFunction(string functionName)
        //{
        //    return mcfunctiontable.GetSelectedNodesInFunction(functionName);
        //}

        bool shouldCheckSocketState = false;
        public void DelayCheckSocketState()
        {
            shouldCheckSocketState = true;
        }

        bool shouldCheckIsLineIsValid = false;
        public void DelayCheckLineIsValid()
        {
            shouldCheckIsLineIsValid = true;
        }

        public void CheckLineIsValid()
        {
            for (int ix = locatedLines.Count - 1; ix >= 0; --ix)
            {
                var line = locatedLines[ix];
                int lineID = line.LineID;
                var left = line.left;
                var right = line.right;

                if (left == null || right == null)
                {
                    // 연결된 양 소켓에 선 삭제 알림.
                    if (line.left != null)
                    {
                        line.left.RemoveLine(lineID);
                        line.left.LineDeleted();
                    }

                    if (line.right != null)
                    {
                        line.right.RemoveLine(lineID);
                        line.right.LineDeleted();
                    }

                    line.UnsubscribeAllDragNodes();
                    Destroy(line.gameObject);
                    RemoveLine(line);
                }
            }
        }

        public void CheckSocketState()
        {
            // 선 연결이 끊겼는데, 연결됐다고 나오는 경우에 대한 처리.
            foreach (var node in locatedNodes)
            {
                // input.
                for (int ix = 0; ix < node.NodeInputCount; ++ix)
                {
                    MCNodeInput input = node.GetNodeInputWithIndex(ix);
                    if (input.HasLine == true)
                    {
                        if (locatedLines.Count == 0 || input.line == null)
                        {
                            input.RemoveLine();
                            input.LineDeleted();
                            continue;
                        }

                        if (input.line != null)
                        {
                            var line = FindLineWithID(input.line.LineID);
                            if (line == null)
                            {
                                input.RemoveLine();
                                input.LineDeleted();
                                continue;
                            }

                            var left = line.left;
                            var right = line.right;

                            if (left == null || right == null)
                            {
                                line.UnsubscribeAllDragNodes();
                                RemoveLine(line);
                                Destroy(line.gameObject);
                                
                                input.RemoveLine();
                                input.LineDeleted();
                                continue;
                            }
                        }
                    }
                }

                // output.
                for (int ix = 0; ix < node.NodeOutputCount; ++ix)
                {
                    MCNodeOutput output = node.GetNodeOutputWithIndex(ix);
                    if (output.HasLine == true)
                    {
                        if (locatedLines.Count == 0 || output.line == null && output.currentLine == null)
                        {
                            output.RemoveLine();
                            output.LineDeleted();
                            continue;
                        }

                        if (output.line != null || output.currentLine != null)
                        {
                            var outputLine = output.line == null ? output.currentLine : output.line;
                            var line = FindLineWithID(outputLine.LineID);
                            if (line == null)
                            {
                                output.RemoveLine();
                                output.LineDeleted();
                                continue;
                            }

                            var left = outputLine.left;
                            var right = outputLine.right;

                            if (left == null || right == null)
                            {
                                line.UnsubscribeAllDragNodes();
                                RemoveLine(line);
                                Destroy(line.gameObject);

                                output.RemoveLine();
                                output.LineDeleted();
                                continue;
                            }
                        }
                    }
                }
            }

            if (locatedLines.Count > 0)
            {
                // 선이 연결돼 있는데, 안됐다고 나오는 경우에 대한 처리.
                foreach (var line in locatedLines)
                {
                    MCNodeOutput left = line.left as MCNodeOutput;
                    MCNodeInput right = line.right as MCNodeInput;

                    if (left != null && left.HasLine == false)
                    {
                        left.SetLine(line);
                        left.LineSet();
                    }

                    if (right != null && right.HasLine == false)
                    {
                        right.SetLine(line);
                        right.LineSet();
                    }
                }
            }
        }

        public bool CheckIfBlockSelected(MCNode node)
        {
            List<MCNode> selected = new List<MCNode>();
            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                selected = SelectedNodesInProject;
            }

            else if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                selected = MCFunctionTable.Instance
                    .GetSelectedNodesInFunction(Utils.CurrentTabName);
            }

            else
            {
                return false;
            }

            for (int ix = 0; ix < selected.Count; ++ix)
            {
                if (selected[ix].OwnedProcess.id.Equals(node.OwnedProcess.id)
                    && selected[ix].NodeID.Equals(node.NodeID))
                {
                    return true;
                }
            }

            return false;
        }

        public MCBezierLine FindLineWithID(int lineID)
        {
            foreach (MCBezierLine line in locatedLines)
            {
                if (line.LineID.Equals(lineID))
                {
                    return line;
                }
            }

            return null;
        }

        public MCNode FindNodeWithID(int nodeID, int processID = -1)
        {
            List<MCNode> targetNodes = new List<MCNode>();
            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                targetNodes = locatedNodes;
            }

            else if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                //string functionName = Utils.CurrentTabName;
                //int functionIndex = GetFunctionIndexWithName(functionName);
                //targetNodes = functions[functionIndex].LogicNodes;

                targetNodes = MCFunctionTable.Instance.LogicNodesOfCurrentFunction;
            }

            else
            {
                return null;
            }

            foreach (MCNode node in targetNodes)
            {
                if (processID != -1)
                {
                    if (node.OwnedProcess.id.Equals(processID) && node.NodeID.Equals(nodeID))
                    {
                        return node;
                    }
                }

                else
                {
                    if (node.NodeID.Equals(nodeID))
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        //public bool CanAddFunction(string functionName)
        //{
        //    return functionTables.CanAddFunction(functionName);
        //}

        //public LeftMenuFunctionItem GetFunctionItemWithName(string functionName)
        //{
        //    return functionTables.GetFunctionItemWithName(functionName);
        //}

        //public LeftMenuFunctionItem GetFunctionItemWithIndex(int index)
        //{
        //    return functionTables.GetFunctionItemWithIndex(index);
        //}

        //public int FunctionCount { get { return functionTables.FunctionCount; } }

        //public string[] FunctionNameList
        //{
        //    get
        //    {
        //        return functionTables.FunctionNameList;
        //    }
        //}

        //public List<MCNode> LogicNodesOfCurrentFunction
        //{
        //    get
        //    {
        //        return functionTables.LogicNodesOfCurrentFunction;
        //    }
        //}

        public string[] VariableNameList
        {
            get
            {
                List<string> names = new List<string>();
                foreach (LeftMenuVariableItem variable in variables)
                {
                    names.Add(variable.VariableName);
                }

                return names.ToArray();
            }
        }

        public int GetVariableIndexWithName(string name)
        {
            string[] names = VariableNameList;
            for (int ix = 0; ix < names.Length; ++ix)
            {
                if (names[ix].Equals(name))
                {
                    return ix;
                }
            }

            return -1;
        }

        public string[] LocalVariableNameList
        {
            get
            {
                List<string> names = new List<string>();
                foreach (LeftMenuVariableItem variable in localVariables)
                {
                    names.Add(variable.VariableName);
                }

                return names.ToArray();
            }
        }

        public int GetLocalVariableIndexWithName(string name)
        {
            string[] names = LocalVariableNameList;
            for (int ix = 0; ix < names.Length; ++ix)
            {
                if (names[ix].Equals(name))
                {
                    return ix;
                }
            }

            return -1;
        }

        //public int GetFunctionIndexWithName(string name)
        //{
        //    string[] names = FunctionNameList;
        //    for (int ix = 0; ix < names.Length; ++ix)
        //    {
        //        if (names[ix].Equals(name))
        //        {
        //            return ix;
        //        }
        //    }

        //    return -1;
        //}
    }
}