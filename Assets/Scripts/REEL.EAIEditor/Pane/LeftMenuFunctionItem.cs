using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class LeftMenuFunctionItem : LeftMenuListItem, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Button button;
        public Button removeButton;

        private List<MCNode> locatedFunctionNodes = new List<MCNode>();

        public PROJECT.Input[] Inputs { get { return FunctionData.inputs; } }
        public PROJECT.Output[] Outputs { get { return FunctionData.outputs; } }
        public List<MCNode> LogicNodes
        {
            get
            {
                if (locatedFunctionNodes == null || locatedFunctionNodes.Count == 0)
                {
                    return null;
                }

                return locatedFunctionNodes;
            }
        }

        public PROJECT.FunctionData FunctionData;
        public int FunctionID
        {
            get { return FunctionData.functionID; }
            set
            {
                if (FunctionData == null)
                {
                    FunctionData = new PROJECT.FunctionData();
                }

                FunctionData.functionID = value;
            }
        }

        private MCPopup functionPopup = null;

        List<PROJECT.Node> inputBypass = new List<PROJECT.Node>();
        List<PROJECT.Node> outputBypass = new List<PROJECT.Node>();

        // For Debugging.
        //private void Awake()
        //{
        //    Utils.LogGreen($"LeftMenuFunctionItem StartID: {FunctionData.startID} / FunctionStartID: {FunctionStartID}");
        //}

        protected override void OnEnable()
        {
            base.OnEnable();

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (functionPopup == null)
            {
                functionPopup = MCEditorManager.Instance.GetPopup(MCEditorManager.PopupType.FunctionList);
            }

            if (FunctionData == null)
            {
                FunctionData = new PROJECT.FunctionData();
            }

            removeButton.onClick.AddListener(OnFunctionRemoveClicked);
        }

        public void CompileFunctionData()
        {
            //Utils.LogBlue($"CompileFunctionData, function name: {name}");

            if (FunctionData == null)
            {
                //Utils.LogRed($"CompileFunctionData FunctionData is null: {name}");
                return;
            }

            // 지역변수 추가.
            MCTables tables = Utils.GetTables();
            if (tables != null)
            {
                List<PROJECT.Node> localVariables = new List<PROJECT.Node>();
                if (tables.localVariables.Count > 0)
                {
                    foreach (LeftMenuVariableItem localVariable in tables.localVariables)
                    {
                        if (localVariable == null)
                        {
                            continue;
                        }

                        PROJECT.Node varInfo = new PROJECT.Node();
                        varInfo.id = MCWorkspaceManager.Instance.NewVariableID;
                        varInfo.body = new PROJECT.Body();
                        varInfo.type = localVariable.nodeType;
                        varInfo.body.name = localVariable.VariableName;
                        varInfo.body.type = localVariable.dataType;
                        varInfo.body.value = localVariable.value;
                        varInfo.body.isLocalVariable = true;

                        localVariables.Add(varInfo);
                    }

                    FunctionData.variables = new PROJECT.Node[localVariables.Count];
                    Array.Copy(localVariables.ToArray(), FunctionData.variables, localVariables.Count);
                }
            }


            // 노드.
            //FunctionData.nodes = new PROJECT.Node[locatedFunctionNodes.Count];
            List<PROJECT.Node> nodes = new List<PROJECT.Node>();
            //for (int ix = 0; ix < FunctionData.nodes.Length; ++ix)
            for (int ix = 0; ix < locatedFunctionNodes.Count; ++ix)
            {
                if (locatedFunctionNodes[ix] == null)
                {
                    continue;
                }

                locatedFunctionNodes[ix].MakeNode();

                // 노드의 입력이 함수 입력을 참조하는지 확인.
                foreach (var input in locatedFunctionNodes[ix].nodeData.inputs)
                {
                    // 노드가 함수의 입력을 참조하는 경우의 처리.
                    // -> 함수의 입력을 참조할 때는 Bypass를 참조하도록 처리하기 위해.
                    if (CheckIFInputHasReferenceToFunctionInput(input.source) == true)
                    {
                        // 기존 입력의 source id를 bypass id로 변경.
                        PROJECT.Node bypass = FindInputBypass(input.subid);
                        if (bypass != null)
                        {
                            input.source = bypass.id;
                        }

                        //Utils.LogGreen($"input.subid: {input.subid} / bypass.id: {bypass.id}");
                    }
                }

                // Todo.
                // 출력 Next 처리(함수의 최종 노드 id는 -2).
                // Process.cs에서 Function_Ouput 타입을 처리하도록 해서 아래 로직 필요 없음 (2020.07.08)
                //foreach (var next in locatedFunctionNodes[ix].nodeData.nexts)
                //{
                //    if (CheckIFNextHasReferenctToFunctionOutput(next.next) == true)
                //    {
                //        next.next = -2;
                //        break;
                //    }
                //}

                //FunctionData.nodes[ix] = locatedFunctionNodes[ix].nodeData;
                nodes.Add(locatedFunctionNodes[ix].nodeData);
            }

            // 입력 bypass 추가.
            if (inputBypass != null && inputBypass.Count > 0)
            {
                foreach (var bypass in inputBypass)
                {
                    if (bypass != null)
                    {
                        nodes.Add(bypass);
                    }
                }
            }

            // 출력 bypass 추가 및 처리.
            if (outputBypass != null && outputBypass.Count > 0)
            {
                for (int ix = 0; ix < outputBypass.Count; ++ix)
                {
                    PROJECT.Node bypass = outputBypass[ix];
                    if (bypass == null)
                    {
                        continue;
                    }

                    bypass.inputs[0].source = RefFunctionOutputNode.nodeData.inputs[ix].source;
                    bypass.inputs[0].subid = RefFunctionOutputNode.nodeData.inputs[ix].subid;

                    // 함수의 Output 값에 Bypass ID값 저장.
                    FunctionData.outputs[ix].value = bypass.id.ToString();
                    nodes.Add(bypass);
                }
            }
            

            FunctionData.nodes = new PROJECT.Node[nodes.Count];
            for (int ix = 0; ix < FunctionData.nodes.Length; ++ix)
            {
                if (nodes[ix] == null)
                {
                    continue;
                }

                FunctionData.nodes[ix] = nodes[ix];
            }
        }

        private MCNode RefFunctionInputNode
        {
            get
            {
                foreach (var node in locatedFunctionNodes)
                {
                    if (node.nodeData.type == PROJECT.NodeType.FUNCTION_INPUT)
                    {
                        return node;
                    }
                }

                return null;
            }
        }

        private MCNode RefFunctionOutputNode
        {
            get
            {
                foreach (var node in locatedFunctionNodes)
                {
                    if (node.nodeData.type == PROJECT.NodeType.FUNCTION_OUTPUT)
                    {
                        return node;
                    }
                }

                return null;
            }
        }

        private PROJECT.Node FindInputBypass(int subID)
        {
            foreach (var bypass in inputBypass)
            {
                if (bypass.inputs[0].subid.Equals(subID))
                {
                    return bypass;
                }
            }

            return null;
        }

        private bool CheckIFInputHasReferenceToFunctionInput(int sourceNodeID)
        {
            foreach (var node in locatedFunctionNodes)
            {
                if (!node.NodeID.Equals(sourceNodeID))
                {
                    continue;
                }

                if (node.nodeData.type == PROJECT.NodeType.FUNCTION_INPUT)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckIFNextHasReferenceToFunctionOutput(int nextNodeID)
        {
            foreach (var node in locatedFunctionNodes)
            {
                if (!node.NodeID.Equals(nextNodeID))
                {
                    continue;
                }

                if (node.nodeData.type == PROJECT.NodeType.FUNCTION_OUTPUT)
                {
                    return true;
                }
            }

            return false;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            removeButton.onClick.RemoveListener(OnFunctionRemoveClicked);
        }

        public override void SetName(string name)
        {
            base.SetName(name);
            FunctionData.name = name;
        }

        public void SetLogic(List<MCNode> nodes)
        {
            if (nodes == null)
            {
                locatedFunctionNodes.Clear();
                return;
            }

            foreach (MCNode node in nodes)
            {
                //logicNodes.Add(node);
                locatedFunctionNodes.Add(node);
            }
        }

        // 로직 노드를 추가하는 함수.
        public void AddLogicNode(MCNode node)
        {
            locatedFunctionNodes.Add(node);
        }

        public MCNode FindNodeWithID(int nodeID)
        {
            for (int ix = locatedFunctionNodes.Count - 1; ix >= 0; --ix)
            {
                if (locatedFunctionNodes[ix].NodeID.Equals(nodeID))
                {
                    return locatedFunctionNodes[ix];
                }
            }

            return null;
        }

        public bool RemoveLogicNode(MCNode node)
        {
            return locatedFunctionNodes.Remove(node);
        }
        
        public void SetInputOuput(PROJECT.Input[] inputs = null, PROJECT.Output[] outputs = null, bool shouldReset = true)
        {
            //if (inputs != null && inputs.Length > 0)
            if (inputs != null)
            {
                FunctionData.inputs = new PROJECT.Input[inputs.Length];

                // Bypass 리스트 초기화.
                for (int ix = 0; ix < inputBypass.Count; ++ix)
                {
                    inputBypass[ix] = null;
                }

                inputBypass.Clear();

                for (int ix = 0; ix < inputs.Length; ++ix)
                {
                    FunctionData.inputs[ix] = new PROJECT.Input();
                    FunctionData.inputs[ix].id = inputs[ix].id;
                    FunctionData.inputs[ix].type = inputs[ix].type;
                    FunctionData.inputs[ix].source = inputs[ix].source;
                    FunctionData.inputs[ix].subid = inputs[ix].subid;
                    FunctionData.inputs[ix].name = inputs[ix].name;

                    // 입력마다 Bypass 추가.
                    inputBypass.Add(MakeBypass(0, ix, PROJECT.DataType.FUNCTION));

                    if (inputs[ix].type == PROJECT.DataType.NUMBER)
                    {
                        int.TryParse(inputs[ix].default_value, out int retNumber);
                        FunctionData.inputs[ix].default_value = retNumber.ToString();
                    }

                    else
                    {
                        FunctionData.inputs[ix].default_value = inputs[ix].default_value;
                    }
                }
            }

            //if (outputs != null && outputs.Length > 0)
            if (outputs != null)
            {
                FunctionData.outputs = new PROJECT.Output[outputs.Length];

                // Bypass 리스트 초기화.
                for (int ix = 0; ix < outputBypass.Count; ++ix)
                {
                    outputBypass[ix] = null;
                }
                outputBypass.Clear();

                for (int ix = 0; ix < outputs.Length; ++ix)
                {
                    FunctionData.outputs[ix] = new PROJECT.Output();
                    FunctionData.outputs[ix].id = outputs[ix].id;
                    FunctionData.outputs[ix].type = outputs[ix].type;
                    FunctionData.outputs[ix].value = outputs[ix].value;
                    FunctionData.outputs[ix].name = outputs[ix].name;

                    // 출력마다 Bypass 추가.
                    outputBypass.Add(MakeBypass(0, ix, outputs[ix].type));
                }
            }
        }

        private PROJECT.Node MakeBypass(int sourceID, int subID, PROJECT.DataType dataType)
        {
            // Bypass 노드 생성 및 추가.
            PROJECT.Node bypass = new PROJECT.Node();
            bypass.id = Utils.NewGUID;
            bypass.type = PROJECT.NodeType.BYPASS;
            bypass.body = null;

            PROJECT.Input bypassInput = new PROJECT.Input();
            bypassInput.id = 0;
            bypassInput.type = dataType;
            bypassInput.source = sourceID;
            bypassInput.subid = subID;
            bypassInput.default_value = "";
            bypass.inputs = new PROJECT.Input[] { bypassInput };

            PROJECT.Output bypassOutput = new PROJECT.Output();
            bypassOutput.id = 0;
            bypassOutput.type = dataType;
            bypassOutput.value = "";
            bypass.outputs = new PROJECT.Output[] { bypassOutput };
            bypass.nexts = null;

            return bypass;
        }

        public void OnFunctionSelected()
        {
            // 다른 팝업이 열려있으면 진행 안함.
            if (MCEditorManager.Instance.IsAnyPopupActive == true)
            {
                return;
            }

            MCFunctionListPopup popup = functionPopup.GetComponent<MCFunctionListPopup>();

            popup.SetFunctionItem(this);
            popup.ShowPopup();
        }

        public void OpenFunctionTab()
        {
            // 다른 팝업이 열려있으면 진행 안함.
            if (MCEditorManager.Instance.IsAnyPopupActive == true)
            {
                return;
            }

            MCFunctionListPopup popup = functionPopup.GetComponent<MCFunctionListPopup>();
            Utils.GetTabManager().CreateTab(FunctionData.name, Constants.OwnerGroup.FUNCTION);
            popup.HideAllPopups();
        }

        public void OnFunctionRemoveClicked()
        {
            MessageBox.ShowYesNo("[ID_MSG_WANT_DELETE_FUNCTION]함수를 삭제하시겠습니까? ", (bool isYes) => 
            {
                if (isYes)
                {
                    MCWorkspaceManager.Instance.RequestFunctionDelete(this);
                }
                else
                {
                    MessageBox.Close();
                }
            });     //local 추가완료

            //Debug.Log("OnFunctionRemoveClicked Clicked");
            //MCWorkspaceManager.Instance.RequestFunctionDelete(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            MCEditorManager.Instance.DragInfoSetActive(true);
            MCEditorManager.Instance.DragInfoSetSprite(null);
            MCEditorManager.Instance.DragInfoSetText(FunctionData.name);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (MCEditorManager.Instance.DragInfoIsActive == false)
            {
                MCEditorManager.Instance.DragInfoSetActive(true);
            }

            MCEditorManager.Instance.DragInfoSetPosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Workspace 작업창에 드래그 하는지 확인.
            List<RaycastResult> results = new List<RaycastResult>();
            MCEditorManager.Instance.UIRaycaster.Raycast(eventData, results);
            if (MCEditorManager.Instance.IsOnGraphPane(results) == false)
            {
                MCEditorManager.Instance.DragInfoSetActive(false);
                return;
            }

            //// 현재 함수 탭에 열린 함수와 동일한 함수인지 비교.
            //if (FindObjectOfType<TabManager>().CurrentOwnerGroup == Constants.OwnerGroup.FUNCTION
            //    && Utils.CurrentTabName.Equals(nameText.text) == true)
            //{
            //    MCEditorManager.Instance.DragInfoSetActive(false);
            //    MessageBox.Show("같은(동일한) 함수는 배치할 수 없습니다.");
            //    return;
            //}


            if (Utils.GetTabManager().CurrentOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                MCEditorManager.Instance.DragInfoSetActive(false);
                MessageBox.Show("[ID_CAN_NOT_USE_FUNCTION_IN_FUNCTION_SPACE]");
                return;
            }

            MCEditorManager.Instance.DragInfoSetActive(false);
            MCNode node = Utils.GetGraphPane().AddNode(Input.mousePosition, PROJECT.NodeType.FUNCTION);

            if (node is MCFunctionNode)
            {
                MCFunctionNode functionNode = node as MCFunctionNode;
                functionNode.FunctionName = FunctionData.name;
                functionNode.StartID = FunctionData.startID;
                functionNode.SetInput(FunctionData.inputs);
                functionNode.SetOutput(FunctionData.outputs);

                // 함수 ID 설정.
                functionNode.FunctionID = FunctionID;

                functionNode.InitializeBlock();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            // 다른 팝업이 열려있으면 진행 안함.
            if (MCEditorManager.Instance.IsAnyPopupActive == true)
            {
                return;
            }

            functionPopup.HideAllPopupWithout(MCEditorManager.PopupType.FunctionList);
            //GetPopup(MCEditorManager.PopupType.VariableList)?.HidePopup();
            //GetPopup(MCEditorManager.PopupType.VariableContext)?.HidePopup();
            //functionPopup.HidePopup();
        }
    }
}