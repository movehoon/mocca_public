using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCFunctionTable : Singleton<MCFunctionTable>
    {
        public List<LeftMenuFunctionItem> functions = new List<LeftMenuFunctionItem>();

        // 특정 함수에 로직 노드를 추가하는 함수.
        // 함수 이름은 고유해야 함.
        public bool AddFunctionNode(string functionName, MCNode node)
        {
            // Find function.
            int index = GetFunctionIndexWithName(functionName);
            if (index == -1)
            {
                return false;
            }

            functions[index].AddLogicNode(node);
            return true;
        }

        // 특정 함수의 로직 노드를 제거하는 함수.
        // 함수 이름은 고유해야 함.
        public bool RemoveFunctionNode(string functionName, MCNode node)
        {
            int index = GetFunctionIndexWithName(functionName);
            if (index == -1)
            {
                return false;
            }

            return functions[index].RemoveLogicNode(node);
        }

        public void AddFunction(LeftMenuFunctionItem function)
        {
            functions.Add(function);
        }

        public bool RemoveFunction(LeftMenuFunctionItem function)
        {
            return functions.Remove(function);
        }

        public void SetAllSelected()
        {
            string functionName = Utils.CurrentTabName;
            int functionIndex = GetFunctionIndexWithName(functionName);
            foreach (MCNode node in functions[functionIndex].LogicNodes)
            {
                node.IsSelected = true;
            }
        }

        public void SetAllUnSelected()
        {
            //Constants.OwnerGroup ownerGroup = MCWorkspaceManager.Instance.CurrentTabOwnerGroup;
            List<MCNode> selected = new List<MCNode>();
            selected = GetSelectedNodesInFunction(Utils.CurrentTabName);

            foreach (MCNode node in selected)
            {
                node.IsSelected = false;
            }

            selected = null;
        }

        public void DeleteSelected()
        {
            //MCUndoRedoManager.Instance.RecordProject();

            string currentFunctionName = Utils.CurrentTabName;
            int functionIndex = MCWorkspaceManager.Instance.GetFunctionIndexWithName(currentFunctionName);

            List<MCNode> selected = GetSelectedNodesInFunction(currentFunctionName);
            for (int ix = 0; ix < selected.Count; ++ix)
            {
                if (selected[ix].DontDestroy == true)
                {
                    continue;
                }

                FindObjectOfType<MCTables>().RequestLineDelete(selected[ix].NodeID, selected[ix].OwnedProcess.id);

                functions[functionIndex].RemoveLogicNode(selected[ix]);
                Destroy(selected[ix].gameObject);
            }

            selected = null;
        }

        public void DeleteAllFunctions()
        {
            while (functions.Count > 0)
            {
                LeftMenuFunctionItem function = functions[functions.Count - 1];
                RemoveFunction(function);
                Destroy(function.gameObject);
            }

            functions = new List<LeftMenuFunctionItem>();
        }

        public void DeleteAllFunctionNodes(LeftMenuFunctionItem function)
        {
            if (function.LogicNodes == null)
            {
                ////Utils.LogRed($"DeleteAllFunctionNodes function.LogicNodes == null");
                //TabManager tabManager = FindObjectOfType<TabManager>();
                ////Utils.LogRed($"tabManager.GetTabIndex(function.name): {tabManager.GetTabIndex(function.name)} / function.name: {function.name}");
                //int tabIndex = tabManager.GetTabIndex(function.name);
                //if (tabIndex != -1)
                //{
                //    MCFunctionTab functionTab = tabManager.currentTabs[tabManager.GetTabIndex(function.name)] as MCFunctionTab;
                //    functionTab.DeleteInputOutputNode();
                //}

                return;
            }

            foreach (MCNode node in function.LogicNodes)
            {
                //Utils.LogRed($"DeleteAllFunctionNodes FunctionName: {function.FunctionData.name} / nodeName: {node.name}");
                Destroy(node.gameObject);
            }

            function.SetLogic(null);
        }

        public void DeleteAllFunctionNodes()
        {
            foreach (LeftMenuFunctionItem function in functions)
            {
                DeleteAllFunctionNodes(function);
            }
        }

        public int GetFunctionIndexWithName(string name)
        {
            string[] names = FunctionNameList;
            for (int ix = 0; ix < names.Length; ++ix)
            {
                //Debug.LogWarning($"Compare Function Names: {names[ix]} / {name}");

                if (names[ix].Equals(name))
                {
                    return ix;
                }
            }

            return -1;
        }

        public int GetFunctionIndexWithID(int id)
        {
            for (int ix = 0; ix < functions.Count; ++ix)
            {
                if (functions[ix].FunctionID == id)
                {
                    return ix;
                }
            }

            return -1;
        }

        public string[] FunctionNameList
        {
            get
            {
                List<string> names = new List<string>();
                foreach (LeftMenuFunctionItem function in functions)
                {
                    names.Add(function.FunctionData.name);
                }

                return names.ToArray();
            }
        }

        public List<MCNode> LogicNodesOfCurrentFunction
        {
            get
            {
                string functionName = Utils.CurrentTabName;
                int functionIndex = GetFunctionIndexWithName(functionName);
                return functions[functionIndex].LogicNodes;
            }
        }

        public List<MCNode> GetSelectedNodesInFunction(string functionName)
        {
            List<MCNode> selected = new List<MCNode>();
            int index = GetFunctionIndexWithName(functionName);
            if (index == -1)
            {
                return null;
            }

            foreach (MCNode node in functions[index].LogicNodes)
            {
                if (node.IsSelected)
                {
                    selected.Add(node);
                }
            }

            return selected;
        }

        public bool CanAddFunction(string functionName, LeftMenuFunctionItem functionItem = null)
        {
            foreach (LeftMenuFunctionItem function in functions)
            {
                if (functionItem != null && function.GetInstanceID().Equals(functionItem.GetInstanceID()))
                {
                    continue;
                }

                if (function.FunctionData.name.Equals(functionName))
                {
                    return false;
                }
            }

            return true;
        }

        public LeftMenuFunctionItem GetFunctionItemWithName(string functionName)
        {
            int index = GetFunctionIndexWithName(functionName);
            if (index == -1)
            {
                return null;
            }

            return functions[index];
        }

        public LeftMenuFunctionItem GetFunctionItemWithIndex(int index)
        {
            if (index == -1)
            {
                Debug.LogWarning($"GetFunctionItemWithIndex: {index} / Array Length: {functions.Count}");
                return null;
            }
            
            return functions[index];
        }

        public LeftMenuFunctionItem GetFunctionItemWithID(int id)
        {
            int index = GetFunctionIndexWithID(id);
            if (index == -1)
            {
                return null;
            }

            return functions[index];
        }

        public int FunctionCount { get { return functions.Count; } }
    }
}