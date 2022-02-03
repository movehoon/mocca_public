using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    [Serializable]
    public class FunctionUpdateEvent : UnityEvent<PROJECT.FunctionData>
    {
    }

    public class MCVariableFunctionManager : MonoBehaviour
	{
        [SerializeField]
        public MCTables tables;

        [SerializeField]
        private GameObject variablePrefab = null;

        [SerializeField]
        private GameObject functionPrefab = null;

        [SerializeField]
        private Transform variableParentTransform = null;

        [SerializeField]
        private Transform localVariableParentTransform = null;

        [SerializeField]
        private Transform functionParentTransform = null;

        private UnityEvent OnVariableUpdate = null;

        private UnityEvent OnLocalVariableUpdate = null;

        private FunctionUpdateEvent OnFunctionUpdate = null;

        private int variableID = -100;

        private void OnEnable()
        {
            if (tables == null)
            {
                tables = GetComponent<MCTables>();
            }

            if (OnVariableUpdate is null)
            {
                OnVariableUpdate = new UnityEvent();
            }

            if (OnLocalVariableUpdate is null)
            {
                OnLocalVariableUpdate = new UnityEvent();
            }

            if (OnFunctionUpdate is null)
            {
                OnFunctionUpdate = new FunctionUpdateEvent();
            }
        }

        public bool AddVariable(VariableInfomation variableInformation)
        {
            return AddVariable(
                variableInformation.name,
                variableInformation.type,
                variableInformation.nodeType,
                variableInformation.value);
        }

        public bool AddVariable(string name, REEL.PROJECT.DataType type, PROJECT.NodeType nodeType, string value)
        {
            if (tables.CanAddVariable(name))
            {
                LeftMenuVariableItem newVariable = CreateNewVariable(variablePrefab, variableParentTransform)
                    .GetComponent<LeftMenuVariableItem>();
                newVariable.SetName(name);
                newVariable.SetNodeType(nodeType);
                newVariable.SetDataType(type);
                newVariable.SetValue(value, type == PROJECT.DataType.EXPRESSION ? "EXPRESSION" : "");
                newVariable.VariableID = Utils.NewGUID;

                tables.AddVariable(newVariable);
                UpdateVariable();

                ResizeVariableContent();

                return true;
            }

            //MessageBox.Show("[ID_SAME_VARIABLE]같은(동일한) 이름을 가진 변수가 이미 존재합니다.\n다른 이름을 입력해주세요.");  //local 추가완료

            return false;
        }

        public bool AddLocalVariable(VariableInfomation variableInformation)
        {
            return AddLocalVariable(
                variableInformation.name,
                variableInformation.type,
                variableInformation.nodeType,
                variableInformation.value,
                variableInformation.functionName);
        }

        public bool AddLocalVariable(string name, REEL.PROJECT.DataType type, PROJECT.NodeType nodeType, string value, string functionName)
        {
            if (tables.CanAddLocalVariable(name))
            {
                LeftMenuVariableItem newVariable = CreateNewVariable(variablePrefab, localVariableParentTransform)
                    .GetComponent<LeftMenuVariableItem>();
                newVariable.SetName(name);
                newVariable.SetNodeType(nodeType);
                newVariable.SetDataType(type);
                newVariable.SetValue(value, type == PROJECT.DataType.EXPRESSION ? "EXPRESSION" : "");
                newVariable.VariableID = Utils.NewGUID;
                newVariable.isLocalVariable = true;
                newVariable.functionName = functionName;

                //// 지역 변수 정보.
                //PROJECT.Node variableInfo = new PROJECT.Node();
                //variableInfo.type = nodeType;
                //variableInfo.body = new PROJECT.Body();
                //variableInfo.body.isLocalVariable = true;
                //variableInfo.body.name = name;
                //variableInfo.body.type = type;
                //variableInfo.body.value = value;

                //LeftMenuFunctionItem function = MCFunctionTable.Instance.GetFunctionItemWithName(functionName);
                //function.FunctionData.AddLocalVariable(variableInfo);

                tables.AddLocalVariable(newVariable);
                UpdateVariable();
                UpdateLocalVariable();

                ResizeLocalVariableContent();

                return true;
            }

            //Utils.LogRed("local variable with the same name");
            //MessageBox.Show("[ID_SAME_LOCAL_VARIABLE]같은(동일한) 이름을 가진 지역 변수가 이미 존재합니다.\n다른 이름을 입력해주세요."); // local 추가 완료

            return false;
        }

        public bool CanAddVariable(string name, LeftMenuVariableItem variableItem = null)
        {
            return tables.CanAddVariable(name, variableItem);
        }

        public bool CanAddLocalVariable(string name, LeftMenuVariableItem variableItem = null)
        {
            return tables.CanAddLocalVariable(name, variableItem);
        }

        public void AdjustVariableContentHeight()
        {
            VariableList variableList = variableParentTransform.GetComponentInParent<VariableList>();
            if (variableList != null)
            {
                variableList.AdjustContentHeight();
            }
        }

        public void AdjustLocalVariableContentHeight()
        {
            VariableList variableList = localVariableParentTransform.GetComponentInParent<VariableList>();
            if (variableList != null)
            {
                variableList.AdjustContentHeight();
            }
        }

        public bool AddFunctionFromData(PROJECT.FunctionData data)
        {
            if (MCFunctionTable.Instance.CanAddFunction(data.name))
            {
                LeftMenuFunctionItem newFunction = CreateNewFunction(functionPrefab, functionParentTransform)
                    .GetComponent<LeftMenuFunctionItem>();
                newFunction.SetName(data.name);
                newFunction.FunctionData = data;
                newFunction.FunctionID = data.functionID == 0 ? Utils.NewGUID : data.functionID;

                newFunction.SetInputOuput(data.inputs, data.outputs);

                MCFunctionTable.Instance.AddFunction(newFunction);
                UpdateFunction(data);
                ResizeFunctionContent();

                return true;
            }

            return false;
        }

        public bool AddFunction(string name, PROJECT.Input[] inputs, PROJECT.Output[] outputs, string description = "")
        {
            if (MCFunctionTable.Instance.CanAddFunction(name))
            {
                PROJECT.FunctionData data = new PROJECT.FunctionData();
                data.name = name;
                data.functionID = Utils.NewGUID;
                data.description = description;
                data.SetInputData(inputs);
                data.SetOutputData(outputs);

                LeftMenuFunctionItem newFunction = CreateNewFunction(functionPrefab, functionParentTransform)
                    .GetComponent<LeftMenuFunctionItem>();
                newFunction.SetName(name);
                newFunction.SetInputOuput(inputs, outputs);
                newFunction.FunctionID = data.functionID;
                newFunction.FunctionData.description = description;

                MCFunctionTable.Instance.AddFunction(newFunction);
                UpdateFunction(data);
                ResizeFunctionContent();

                return true;
            }

            //LogWindow.Instance.PrintWarning("MOCCA", "같은(동일한) 이름을 가진 함수가 이미 존재합니다.<br>다른 이름을 입력해주세요.");
            //MessageBox.Show("[ID_SAME_FUNCTION]같은(동일한) 이름을 가진 함수가 이미 존재합니다.\n다른 이름을 입력해주세요."); // local 추가 완료
            return false;
        }

        public void ResizeAllContents()
        {
            ResizeVariableContent();
            ResizeLocalVariableContent();
            ResizeFunctionContent();
        }

        private void ResizeVariableContent()
        {
            // 변수 Content Y 크기 설정.
            Vector2 size = variableParentTransform.GetComponent<RectTransform>().sizeDelta;
            size.y = tables.variables.Count * 40f + ((tables.variables.Count - 1) * 10f);
            variableParentTransform.GetComponent<RectTransform>().sizeDelta = size;
        }

        private void ResizeLocalVariableContent()
        {
            // 변수 Content Y 크기 설정.
            Vector2 size = localVariableParentTransform.GetComponent<RectTransform>().sizeDelta;
            size.y = tables.localVariables.Count * 40f + ((tables.localVariables.Count - 1) * 10f);
            localVariableParentTransform.GetComponent<RectTransform>().sizeDelta = size;
        }

        private void ResizeFunctionContent()
        {
            Vector2 size = functionParentTransform.GetComponent<RectTransform>().sizeDelta;
            int functionCount = MCFunctionTable.Instance.FunctionCount;
            size.y = functionCount * 40f + ((functionCount - 1) * 10f);
            functionParentTransform.GetComponent<RectTransform>().sizeDelta = size;
        }

        public void UpdateVariable()
        {
            if (OnVariableUpdate is null)
            {
                OnVariableUpdate = new UnityEvent();
            }

            OnVariableUpdate?.Invoke();
        }

        public void UpdateLocalVariable()
        {
            if (OnLocalVariableUpdate is null)
            {
                OnLocalVariableUpdate = new UnityEvent();
            }

            OnLocalVariableUpdate?.Invoke();
        }

        public void UpdateFunction(PROJECT.FunctionData data)
        {
            if (OnFunctionUpdate is null)
            {
                OnFunctionUpdate = new FunctionUpdateEvent();
            }

            OnFunctionUpdate?.Invoke(data);
        }

        public bool RemoveVariable(LeftMenuVariableItem variable)
        {
            if (tables.RemoveVariable(variable))
            {
                Destroy(variable.gameObject);
                UpdateVariable();
                ResizeVariableContent();
                return true;
            }

            return false;
        }

        public void RemoveAllVariables()
        {
            tables.DeleteAllVariables();
            OnVariableUpdate?.Invoke();
        }

        public bool RemoveLocalVariable(LeftMenuVariableItem localVariable)
        {
            if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                return false;
            }

            if (tables.RemoveLocalVariable(localVariable))
            {
                Destroy(localVariable.gameObject);
                UpdateLocalVariable();
                ResizeLocalVariableContent();

                return true;
            }

            //LeftMenuFunctionItem function = MCWorkspaceManager.Instance.GetFunctionItemWithName(Utils.CurrentTabName);
            //if (function.FunctionData.DeleteLocalVariable(localVariable.VariableName) == true)
            //{
            //    Destroy(localVariable.gameObject);
            //    UpdateLocalVariable();
            //    ResizeLocalVariableContent();

            //    return true;
            //}

            return false;
        }

        public void RemoveAllLocalVariables()
        {
            tables.DeleteAllLocalVariables();

            OnVariableUpdate?.Invoke();
            OnLocalVariableUpdate?.Invoke();
        }

        public bool RemoveFuncion(LeftMenuFunctionItem function)
        {
            // 삭제할 함수와 같은 이름의 함수 탭이 열려있는지 찾아보고,
            // 찾으면(null이 아니면) 탭닫기.
            Utils.GetTabManager().GetFunctionTabWithName(function.FunctionData.name)?.CloseTab();

            // 함수 삭제.
            if (MCFunctionTable.Instance.RemoveFunction(function))
            {
                Destroy(function.gameObject);
                UpdateFunction(null);
                ResizeFunctionContent();
                return true;
            }

            return false;
        }

        private GameObject CreateNewVariable(GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent);
        }

        private GameObject CreateNewLocalVariable(GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent);
        }

        private GameObject CreateNewFunction(GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent);
        }

        public void SubscribeVariableUpdate(UnityAction update)
        {
            OnVariableUpdate.AddListener(update);
        }

        public void UnsubscribeVariableUpdate(UnityAction update)
        {
            OnVariableUpdate.RemoveListener(update);
        }

        public void SubscribeLocalVariableUpdate(UnityAction update)
        {
            OnLocalVariableUpdate.AddListener(update);
        }

        public void UnsubscribeLocalVariableUpdate(UnityAction update)
        {
            OnLocalVariableUpdate.RemoveListener(update);
        }

        public void SubscribeFunctionUpdate(UnityAction<PROJECT.FunctionData> update)
        {
            OnFunctionUpdate.AddListener(update);
        }

        public void UnsubscribeFunctionUpdate(UnityAction<PROJECT.FunctionData> update)
        {
            OnFunctionUpdate.RemoveListener(update);
        }

        public string[] GetVariableNameListWithType(PROJECT.DataType targetType)
        {
            List<string> variables = new List<string>();
            foreach (var variable in tables.variables)
            {
                if (variable.dataType == targetType)
                {
                    variables.Add(variable.VariableName);
                }
            }

            return variables.ToArray();
        }

        public string[] GetLocalVariableNameListWithType(PROJECT.DataType targetType)
        {
            if (tables.localVariables == null || tables.localVariables.Count == 0)
            {
                return null;
            }

            List<string> variables = new List<string>();
            foreach (var variable in tables.localVariables)
            {
                if (variable.dataType == targetType)
                {
                    variables.Add(variable.VariableName);
                }
            }

            return variables.ToArray();
        }

        public string[] VariableNameList
        {
            get
            {
                return tables.VariableNameList;
            }
        }

        public string[] LocalVariableNameList
        {
            get
            {
                return tables.LocalVariableNameList;
            }
        }

        public string[] FunctionNameList
        {
            get
            {
                return MCFunctionTable.Instance.FunctionNameList;
            }
        }

        public int NewVariableID
        {
            get
            {
                return variableID--;
            }
        }
    }
}