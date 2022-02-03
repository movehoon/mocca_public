using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using REEL.PROJECT;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;

namespace REEL.D2EEditor
{
    public class MCWorkspaceManager : Singleton<MCWorkspaceManager>
    {
        //private bool isSimulation = false;
        //public bool IsSimulation
        //{
        //    get { return isSimulation; }
        //    set
        //    {
        //        if (tabManager == null || tabManager.CurrentTab == null)
        //        {
        //            return;
        //        }

        //        //Debug.LogWarning(string.Format("isSimulation {0}", isSimulation));
        //        isSimulation = value;
        //        SetWorkspaceActive(!isSimulation);
        //        SetAllUnSelected();
        //        PropertyWindowManager.Instance.TurnOffAll();

        //        OnSimulationStateChanged?.Invoke(isSimulation);
        //    }
        //}

        [SerializeField]
        private MCTables tables = null;

        [SerializeField]
        private MCVariableFunctionManager variableFunctionManager = null;

        public MCVariableFunctionManager VariableFunctionManager { get { return variableFunctionManager; } }

        public WorkspaceScrollRect workspaceScrollRect = null;

        [SerializeField]
        private Vector2 startPos = Vector2.zero;

        [SerializeField]
        private GameObject disableScreen = null;

        //[SerializeField]
        //private RectTransform lineParentTransform = null;

        [SerializeField]
        private RectTransform canvasRectTransform = null;

        [SerializeField]
        private TabManager tabManager = null;

        public RectTransform CanvasRectTransform { get { return canvasRectTransform; } }

        private event Action LineUpdate = null;

        [SerializeField]
        private string compiledProjectString = string.Empty;

        private Action OnToolbarButtonClicked;

        private Action OnProcessUpdate = null;

        public GameObject[] editorPanesExceptWorkspace;

        [Header("로봇 뷰 관련 옵션")]
        public ProgramSettingWindow.RobotType currentRobotType = ProgramSettingWindow.RobotType.MOCCA;
        public RenderTexture moccatRT = null;
        public RenderTexture robotDudeRT = null;
        public RawImage robotViewRawImage = null;
        public RawImage fullscreenRoibotViewRawImage = null;

        public bool IsDirty { get; set; }

        public void UpdateProcess()
        {
            OnProcessUpdate?.Invoke();
        }

        public void SubscribeProcessUpdate(Action update)
        {
            OnProcessUpdate += update;
        }

        public void UnsubscribeProcessUpdate(Action update)
        {
            OnProcessUpdate -= update;
        }

        public void SubscribeOnToolbarButtonClicked(Action listener)
        {
            OnToolbarButtonClicked += listener;
        }

        public void UnSubscribeOnToolbarButtonClicked(Action listener)
        {
            OnToolbarButtonClicked -= listener;
        }

        private void OnEnable()
        {
            if (tables == null)
            {
                tables = GetComponent<MCTables>();
            }

            if (variableFunctionManager == null)
            {
                variableFunctionManager = GetComponent<MCVariableFunctionManager>();
            }
            SetWorkspaceActive(false);

            //SubscribeSimulationStateChanged(ChangeWorkspacePaneState);

            // 화면 해상도 비율 설정 (16:9).
            //Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, true);
        }

        // For Test.
        private void Update()
        {
            //if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    //ExecuteProject();
            //    LoadTestProject();
            //}
        }

        private void LateUpdate()
        {
            if (shouldValidateProject == true)
            {
                shouldValidateProject = false;
                ValidateProject();
            }
        }

        public void ChangeWorkspacePaneState(bool isSimulation)
        {
            if (isSimulation == true)
            {
                MaxizeWorkspacePane();
            }
            else
            {
                ResetWorkspacePanes();
            }
        }

        public void MaxizeWorkspacePane()
        {
            foreach (GameObject pane in editorPanesExceptWorkspace)
            {
                pane.SetActive(false);
            }
        }

        public void ResetWorkspacePanes()
        {
            foreach (GameObject pane in editorPanesExceptWorkspace)
            {
                pane.SetActive(true);
            }
        }

        // 노드 더블클릭했을 때, 더블 클릭한 노드부터 실행하는 기능.
        public void ExecuteProjectFromNode(int nodeID)
        {
            if (Utils.IsProjectNullOrOnSimulation)
            {
                return;
            }

            // 프로젝트 실행할 때 모든 팝업창 닫기.
            MCEditorManager.Instance.CloseAllPopups();

            SetAllUnSelected();

            CompileProject();
            MCPlayStateManager.Instance.IsSimulation = true;

            // 프로세스 정보 초기화.
            //processEndCount = 0;
            //processes.Clear();
            MCPlayStateManager.Instance.ResetProcessInfomation();

            Player.Instance.Play(compiledProjectString, nodeID);

            //processEndCount = 0;
            //processes.Clear();
            //foreach (GameObject go in Player.Instance.goProcesses)
            //{
            //    processes.Add(go.GetComponent<Process>());
            //}

            //Test.
            MCPlayStateManager.Instance.StartCheckDialogueState();
            //StartCoroutine(CheckDialogState());
            //StartCoroutine(CheckHighlightState());
        }

        public bool isCurrentProjectUsingCameraNode = false;
        //private readonly string notValidProjectMessage = "스타트 노드만 배치되어 프로젝트가 실행되지 않습니다.\n스타트 노드에 다른 실행 노드를 연결해주세요.";
        //private void ExecuteProject()
        private bool ExecuteProject()
        {
            if (Utils.IsProjectNullOrOnSimulation)
            {
                return false;
            }

            // 프로젝트 실행할 때 모든 팝업창 닫기.
            MCEditorManager.Instance.CloseAllPopups();

            // Test.
            bool shouldPlay = false;
            List<MCStartNode> startNodes = StartNodes;
            //foreach (MCStartNode startNode in StartNodes)
            foreach (MCStartNode startNode in startNodes)
            {
                if (startNode.NodeNextCount > 0 && startNode.GetNodeNextWithIndex(0).next.next > 0)
                {
                    shouldPlay = true;
                }
            }

            if (shouldPlay == false)
            {
                MessageBox.Show("[ID_MSG_CONNECT_START]");
                return false;
            }

            // Test.
            // 프로젝트 실행 시작할 때 로그 메시지 출력.
            //LogWindow.Instance.PrintWarning("MOCCA Studio", $"<프로젝트 [{CurrentTabName}] 시작>");

            SetAllUnSelected();

            // 프로젝트 컴파일.
            //CompileProject();

            // 카메라 블록 사용 여부 확인.
            // 프로젝트 컴파일.
            isCurrentProjectUsingCameraNode = IsCameraNodeUsed(CompileProject());

            MCPlayStateManager.Instance.IsSimulation = true;

            // 프로세스 정보 초기화.
            MCPlayStateManager.Instance.ResetProcessInfomation();
            
            Player.Instance.Play(compiledProjectString);
            MCPlayStateManager.Instance.StartCheckDialogueState();

            return true;
        }

        public void StopProject()
        {
            // Test.
            // 프로젝트 종료할 때 로그 메시지 출력.
            //LogWindow.Instance.PrintWarning("MOCCA Studio", $"<프로젝트 [{CurrentTabName}] 종료>");

            Player.Instance.Stop();
            MCPlayStateManager.Instance.IsSimulation = false;

            // 캠뷰 메시지 상태 초기화.
            Webcam.Instance.HideProcessMessageWindow();
            MCPlayStateManager.Instance.StopProject();
        }

        public void OnPlayOrStopButtonClicked()
        {
            if (MCPlayStateManager.Instance.IsSimulation)
            {
                MCPlayStateManager.Instance.IsSimulation = false;
                StopProject();
            }
            else
            {
                if (CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
                {
                    FindObjectOfType<MCProjectTab>().OnTabClicked();
                    ExecuteProject();

                    //MessageBox.Show("현재 선택된 탭은 함수 탭입니다.\n프로젝트 탭을 선택하고 Play 버튼을 눌러야 정상적으로 실행이 됩니다."); // local 추가 완료
                    return;
                }

                ExecuteProject();

                TutorialManager.SendEvent(Tutorial.CustomEvent.ProjectPlayed, "Project Played");
            }

            OnToolbarButtonClicked?.Invoke();
        }

        public void OnSettingsClicked()
        {
            ProgramSettingWindow.Show();

            OnToolbarButtonClicked?.Invoke();
        }

        public void OnShutdownClicked()
        {
            OnToolbarButtonClicked?.Invoke();

            MessageBox.ShowYesNo("[ID_MSG_CAUTION_END]저장하지 않은 데이터는 사라집니다.\n정말 종료합니까?", res =>
            {
                if (res == true)
                {
                    if (Application.isEditor == true)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif
                    }
                    else
                    {
                        Application.Quit();
                    }
                }
            }); //local 추가완료
        }



        public void OnAlignXClicked()
        {
            if (LocatedNodes == null || LocatedNodes.Count == 0)
            {
                return;
            }

            var selectedNodes = LocatedNodes.Where(x => x.IsSelected);

            if (selectedNodes == null || selectedNodes.Count() <= 1)
            {
                MessageBox.Show("정렬할 블록을 두 개 이상 선택해주세요."); // local 추가 완료
                return;
            }

            //MCUndoRedoManager.Instance.RecordProject();

            //float minXPos = selectedNodes.Min( node=> node.transform.localPosition.x);
            float minXPos = selectedNodes.Min(node => node.mcNodePosition.TopLeftWorldPosition.x);

            List<ChangePositionInfo> changePositionInfos = new List<ChangePositionInfo>();
            foreach (MCNode node in selectedNodes)
            {
                //Vector2 pos = node.transform.localPosition;
                //pos.x = minXPos;
                Vector2 pos = node.transform.position;
                pos.x = minXPos + node.mcNodePosition.OffsetPoint.x;
                //node.transform.localPosition = pos;

                // 정렬 동작 시 Command 기반으로 동작시킴.
                // 정렬 버튼을 잘못 눌렀을 때 되돌리기 가능하도록.
                changePositionInfos.Add(new ChangePositionInfo()
                {
                    nodeID = node.NodeID,
                    position = pos
                });
            }

            MCUndoRedoManager.Instance.AddCommand(new MCChangeNodePositionsCommand(changePositionInfos));

            //UpdateAllLineUpdate();
        }

        public void OnAlignYClicked()
        {
            if (LocatedNodes == null || LocatedNodes.Count == 0)
            {
                return;
            }

            var selectedNodes = LocatedNodes.Where(x => x.IsSelected);

            if (selectedNodes == null || selectedNodes.Count() <= 1)
            {
                MessageBox.Show("정렬할 블록을 두 개 이상 선택해주세요."); // local 추가 완료
                return;
            }

            //MCUndoRedoManager.Instance.RecordProject();

            //float minYPos = selectedNodes.Max(node => node.transform.localPosition.y);
            float minYPos = selectedNodes.Max(node => node.mcNodePosition.TopLeftWorldPosition.y);


            // 정렬 동작 시 Command 기반으로 동작시킴.
            // 정렬 버튼을 잘못 눌렀을 때 되돌리기 가능하도록.
            List<ChangePositionInfo> changePositionInfos = new List<ChangePositionInfo>();
            foreach (MCNode node in selectedNodes)
            {
                //Vector2 pos = node.transform.localPosition;
                Vector2 pos = node.transform.position;
                pos.y = minYPos + node.mcNodePosition.OffsetPoint.y;
                //node.transform.localPosition = pos;

                changePositionInfos.Add(new ChangePositionInfo()
                {
                    nodeID = node.NodeID,
                    position = pos
                });
            }

            MCUndoRedoManager.Instance.AddCommand(new MCChangeNodePositionsCommand(changePositionInfos));

            //UpdateAllLineUpdate();
        }

        /*
                public void OnAlignXClicked()
                {
                    if (LocatedNodes == null || LocatedNodes.Count == 0)
                    {
                        return;
                    }

                    List<MCNode> nodes = LocatedNodes;

                    float minXPos = nodes[0].transform.localPosition.x;
                    for (int ix = 1; ix < nodes.Count; ++ix)
                    {
                        if (nodes[ix].transform.localPosition.x < minXPos)
                        {
                            minXPos = nodes[ix].transform.localPosition.x;
                        }
                    }

                    foreach (MCNode node in nodes)
                    {
                        Vector2 pos = node.transform.localPosition;
                        pos.x = minXPos;
                        node.transform.localPosition = pos;
                    }

                    UpdateAllLineUpdate();
                }


                public void OnAlignYClicked()
                      {
                          if (LocatedNodes == null || LocatedNodes.Count == 0)
                          {
                              return;
                          }

                          List<MCNode> nodes = LocatedNodes;

                          float minYPos = nodes[0].transform.localPosition.y;
                          for (int ix = 1; ix < nodes.Count; ++ix)
                          {
                              if (nodes[ix].transform.localPosition.y > minYPos)
                              {
                                  minYPos = nodes[ix].transform.localPosition.y;
                              }
                          }

                          foreach (MCNode node in nodes)
                          {
                              Vector2 pos = node.transform.localPosition;
                              pos.y = minYPos;
                              node.transform.localPosition = pos;
                          }

                          UpdateAllLineUpdate();
                      }
        */

        public bool RequestVariableDelete(LeftMenuVariableItem variable)
        {
            // 삭제할 변수를 사용하는 Get 노드 검사.
            int deleteRequestedIndex = GetVariableIndexWithName(variable.VariableName);
            List<MCNode> willBeDeleted = new List<MCNode>();
            for (int ix = 0; ix < tables.locatedNodes.Count; ++ix)
            {
                MCNode node = tables.locatedNodes[ix];
                if (node is MCGetNode)
                {
                    MCGetNode getNode = node as MCGetNode;
                    if (getNode.CurrentVariableIndex.Equals(deleteRequestedIndex))
                    {
                        willBeDeleted.Add(node);
                    }
                }
                else if (node is MCSetNode)
                {
                    MCSetNode setNode = node as MCSetNode;
                    if (setNode.CurrentVariableIndex.Equals(deleteRequestedIndex))
                    {
                        willBeDeleted.Add(node);
                    }
                }
            }

            // 변수 삭제 전 프로젝트 저장.
            //MCUndoRedoManager.Instance.RecordProject();

            // Get 노드 삭제.
            foreach (MCNode node in willBeDeleted)
            {
                RequestNodeDelete(node);

                //tables.RemoveNode(node);
                //Destroy(node.gameObject);
            }

            // 변수 삭제.
            return variableFunctionManager.RemoveVariable(variable);
        }

        public bool RequestLocalVariableDelete(LeftMenuVariableItem variable)
        {
            // 삭제할 변수를 사용하는 Get 노드 검사.
            int deleteRequestedIndex = GetLocalVariableIndexWithName(variable.VariableName);
            List<MCNode> willBeDeleted = new List<MCNode>();
            for (int ix = 0; ix < tables.locatedNodes.Count; ++ix)
            {
                MCNode node = tables.locatedNodes[ix];
                if (node is MCGetNode)
                {
                    MCGetNode getNode = node as MCGetNode;
                    if (getNode.CurrentVariableIndex.Equals(deleteRequestedIndex))
                    {
                        willBeDeleted.Add(node);
                    }
                }
                else if (node is MCSetNode)
                {
                    MCSetNode setNode = node as MCSetNode;
                    if (setNode.CurrentVariableIndex.Equals(deleteRequestedIndex))
                    {
                        willBeDeleted.Add(node);
                    }
                }
            }

            // 변수 삭제 전 프로젝트 저장.
            //MCUndoRedoManager.Instance.RecordProject();

            // Get 노드 삭제.
            foreach (MCNode node in willBeDeleted)
            {
                RequestNodeDelete(node);

                //tables.RemoveNode(node);
                //Destroy(node.gameObject);
            }

            // 변수 삭제.
            return variableFunctionManager.RemoveVariable(variable);
        }

        // 현재 탭이 프로젝트인지, 함수 탭인지에 따라 배치된 노드 리스트를 반환하는 함수.
        List<MCNode> LocatedNodesOnCurrentTab
        {
            get
            {
                if (tabManager.CurrentOwnerGroup == Constants.OwnerGroup.PROJECT)
                {
                    return tables.locatedNodes;
                }

                return GetFunction(GetFunctionIndexWithName(tabManager.CurrentTab.TabName)).LogicNodes;
            }
        }


        public bool RequestFunctionDelete(LeftMenuFunctionItem function)
        {
            // 삭제할 함수를 사용하는 Function 노드 검사.
            int deleteRequestedIndex = GetFunctionIndexWithName(function.FunctionData.name);

            List<MCNode> willBeDeleted = new List<MCNode>();
            List<MCNode> locatedNodes = LocatedNodesOnCurrentTab;

            //for (int ix = 0; ix < tables.locatedNodes.Count; ++ix)
            for (int ix = 0; ix < locatedNodes.Count; ++ix)
            {
                MCNode node = locatedNodes[ix];
                if (node is MCFunctionNode)
                {
                    MCFunctionNode functionNode = node as MCFunctionNode;
                    if (functionNode.FunctionIndex.Equals(deleteRequestedIndex))
                    {
                        willBeDeleted.Add(node);
                    }
                }
            }

            // Function 노드 삭제.
            foreach (MCNode node in willBeDeleted)
            {
                tables.RequestLineDelete(node.NodeID, node.OwnedProcess.id);
                locatedNodes.Remove(node);
                Destroy(node.gameObject);
            }

            // 함수 제거.
            return variableFunctionManager.RemoveFuncion(function);
        }

        public void ChangeProjectName(string newName)
        {
            if (tabManager.CurrentTab is MCProjectTab)
            {
                tabManager.ChangeTabName(newName);
            }
        }

        public FunctionDesc[] GetFunctionDescs()
        {
            List<FunctionDesc> functionDescs = new List<FunctionDesc>();
            foreach (LeftMenuFunctionItem function in MCFunctionTable.Instance.functions)
            {
                functionDescs.Add(function.FunctionData.GetFunctionDesc());
            }

            return functionDescs.ToArray();
        }

        public bool IsCameraNodeUsed(string compiledProjectJson)
        {
            try
            {
                ProjectData data = JsonUtility.FromJson<ProjectData>(compiledProjectJson);
                foreach (var node in data.processes[0].nodes)
                {
                    if (Utils.IsCameraUsingType(node.type) == true)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string CompileProject()
        {
            UpdateProcessInfo();

            if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                if (MCProjectManager.ProjectBuilder != null && MCFunctionTable.Instance != null)
                {
                    compiledProjectString = MCProjectManager.Build(
                                                    tables.locatedNodes,
                                                    tables.variables,
                                                    MCFunctionTable.Instance.functions,
                                                    MCProjectManager.ProjectDescription.language
                                                );

                    return compiledProjectString;

                    //try
                    //{
                    //    compiledProjectString = MCProjectManager.Build(
                    //                                tables.locatedNodes,
                    //                                tables.variables,
                    //                                MCFunctionTable.Instance.functions,
                    //                                MCProjectManager.ProjectDescription.language
                    //                            );

                    //    return compiledProjectString;
                    //}
                    //catch (Exception ex)
                    //{
                    //    Utils.LogRed($"[CompileProject] Compile Error: {ex.ToString()}");
                    //    MessageBox.ShowYesNo("[ID_LOGIC_DATA_COMPILE_FAILED_REQUEST]", (res) => //local 추가 완료
                    //    {
                    //        if (res == true)
                    //        {
                    //            Slack.SendException("[ID_LOGIC_DATA_COMPILE_FAILED]", MCProjectManager.ProjectDescription, ex); //local 추가 완료
                    //            MessageBox.Show("[ID_SEND_MSG_TO_DEVELOPER]"); // local 추가 완료
                    //        }
                    //    });
                    //}
                }
            }

            else if (CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                ProjectData projectData = null;
                try
                {
                    // 현재 프로젝트 데이터 불러오기.
                    projectData = JsonUtility.FromJson<ProjectData>(compiledProjectString);
                }
                catch (Exception ex)
                {
                    Utils.LogRed($"[CompileProject] Failed to compile function data. {ex.ToString()}");
                    MessageBox.ShowYesNo("[ID_LOGIC_DATA_COMPILE_FAILED_REQUEST]", (res) => //local 추가 완료
                    {
                        if (res == true)
                        {
                            Slack.SendException("[ID_LOGIC_DATA_COMPILE_FAILED]", MCProjectManager.ProjectDescription, ex); //local 추가 완료
                            MessageBox.Show("[ID_SEND_MSG_TO_DEVELOPER]"); // local 추가 완료
                        }
                    });
                }

                MCFunctionTab tab = tabManager.CurrentTab as MCFunctionTab;
                tab.CompileFunctionData();

                List<FunctionData> functions = new List<FunctionData>();

                // 함수 추가.
                foreach (LeftMenuFunctionItem function in MCFunctionTable.Instance.functions)
                {
                    FunctionData functionData = new FunctionData();
                    functionData.inputs = new PROJECT.Input[function.FunctionData.inputs.Length];
                    for (int ix = 0; ix < functionData.inputs.Length; ++ix)
                    {
                        functionData.inputs[ix] = new PROJECT.Input();
                        functionData.inputs[ix] = function.FunctionData.inputs[ix];
                    }

                    functionData.outputs = new Output[function.FunctionData.outputs.Length];
                    for (int ix = 0; ix < functionData.outputs.Length; ++ix)
                    {
                        functionData.outputs[ix] = new Output();
                        functionData.outputs[ix] = function.FunctionData.outputs[ix];
                    }

                    functionData.nodes = new Node[function.FunctionData.nodes.Length];
                    for (int ix = 0; ix < functionData.nodes.Length; ++ix)
                    {
                        functionData.nodes[ix] = new Node();
                        functionData.nodes[ix] = function.FunctionData.nodes[ix];
                    }

                    functionData.name = function.FunctionData.name;
                    functionData.startID = function.FunctionData.startID;

                    if (functionData.lastNodes == null)
                    {
                        functionData.lastNodes = new List<FunctionLastNodeInfo>();
                    }

                    foreach (FunctionLastNodeInfo info in function.FunctionData.lastNodes)
                    {
                        FunctionLastNodeInfo lastNodeInfo = new FunctionLastNodeInfo()
                        {
                            nodeID = info.nodeID,
                            socketIndex = info.socketIndex,
                        };

                        //lastNodeInfo.socketIndex = info.socketIndex;
                        functionData.lastNodes.Add(lastNodeInfo);
                    }

                    //functionData.lastNode.nodeID = function.FunctionData.lastNode.nodeID;
                    //functionData.lastNode.socketIndex = function.FunctionData.lastNode.socketIndex;
                    //functionData.lastID = function.FunctionData.lastID;

                    //functions.Add(function.FunctionData);
                    functions.Add(functionData);
                }

                // 프로젝트 데이터 갱신.
                projectData.functions = functions.ToArray();

                try
                {
                    // JSON 빌드.
                    //compiledProjectString = JsonUtility.ToJson(projectData);
                    compiledProjectString = Utils.CompileToJson(projectData);
                    return compiledProjectString;
                }
                catch (Exception ex)
                {
                    Utils.LogRed($"[CompileProject] Failed to convert function compiled data to json. {ex.ToString()}");
                    MessageBox.ShowYesNo("[ID_LOGIC_DATA_COMPILE_FAILED_REQUEST]", (res) => //local 추가 완료
                    {
                        if (res == true)
                        {
                            Slack.SendException("[ID_LOGIC_DATA_COMPILE_FAILED]", MCProjectManager.ProjectDescription, ex); //local 추가 완료
                            MessageBox.Show("[ID_SEND_MSG_TO_DEVELOPER]"); // local 추가 완료
                        }
                    });
                }
            }

            //Debug.LogWarning($"CompileProject: {compiledProjectString}");
            //return compiledProjectString;
            return string.Empty;
        }

        public void SaveProject(string filename = "")
        {
            if (string.IsNullOrEmpty(compiledProjectString))
            {
                return;
            }

            string baseProjectPath = "Assets/Data";
            string filePath = string.IsNullOrEmpty(filename) ?
                "Assets/Data/testproject.json" : Path.Combine(baseProjectPath, filename + ".json");

            File.WriteAllText(filePath, compiledProjectString);
        }

        private void CreateNodesFromProcess(PROJECT.Process process, FunctionData[] functions, int processIndex = -1)
        {
            //GraphPane graphPane = PaneObject;
            GraphPane graphPane = Utils.GetGraphPane();

            // 노드 생성.
            foreach (Node node in process.nodes)
            {
                //if (PaneObject == null)
                //{
                //    Debug.Log(MCEditorManager.Instance.GetPane(MCEditorManager.PaneType.Graph_Pane));
                //    //return;
                //}

                // 함수 탭에서 변수를 삭제한 다음, 
                // 프로젝트 탭으로 넘어오면 Get/Set 노드인 경우에 삭제된 변수인지 확인해야함.
                if (node.type == NodeType.GET || node.type == NodeType.SET)
                {
                    int index = GetVariableIndexWithName(node.body.name);
                    // 변수가 삭제돼 검색 안되는 경우에는 GET/SET 노드 생성 안함.
                    if (index.Equals(Utils.FIND_ERROR_CODE))
                    {
                        continue;
                    }
                }

                // 함수 탭에서 함수를 삭제한 다음,
                // 프로젝트 탭으로 넘어오면 배치된 함수가 삭제된 함수인지 확인해야함.
                if (node.type == NodeType.FUNCTION)
                {
                    int functionID = GetFunctionIndexWithName(node.body.name);
                    if (functionID.Equals(Utils.FIND_ERROR_CODE))
                    {
                        continue;
                    }
                }

                MCNode newNode = graphPane.AddNode(node.nodePosition, node.type, node.id, true, true);
                if (newNode is null)
                {
                    Debug.Log(node.body.name + " : " + node.type + " : " + node.body.type);
                    continue;
                }

                // 첫번째 프로세스의 StartNode의 경우에는 DontDestroy 활성화.
                if (newNode is MCStartNode && processIndex == 0)
                {
                    newNode.DontDestroy = true;
                }

                // 함수의 경우 FunctionID 설정.
                if (newNode is MCFunctionNode)
                {
                    foreach (var function in functions)
                    {
                        // 함수 이름으로 검색해서 ID 설정.
                        if (function.name.Equals(node.body.name))
                        {
                            MCFunctionNode functionNode = newNode as MCFunctionNode;
                            functionNode.FunctionID = function.functionID;
                        }
                    }
                }

                newNode.OwnedProcess.name = process.name;
                newNode.OwnedProcess.id = process.id;
                newNode.OwnedProcess.priority = process.priority;
                newNode.SetData(node);
                //Debug.Log(node.id);
            }
        }

        private void CreateNodeInputLines(PROJECT.Process process, Node node)
        {
            MCNode mcNode = FindNodeWithID(node.id);
            if (mcNode != null && mcNode.nodeData.inputs != null)
            {
                foreach (PROJECT.Input input in node.inputs)
                {
                    if (input == null)
                    {
                        continue;
                    }

                    CreateInputLine(process, node.id, input);

                    // 작성자: 장세윤.
                    // MCLogNode의 경우 특수한 input이 있어서 예외 처리함
                    // Log Option의 경우 기존의 input과 다름.
                    if (input == null)
                    {
                        continue;
                    }

                    if (input.source != -1)
                    {
                        if (FindNodeWithID(input.source) == null)
                        {
                            continue;
                        }

                        CreateNodeInputLines(process, FindNodeWithID(input.source).nodeData);
                    }
                }
            }
        }

        private void CreateNodeInputLinesFromList(List<MCNode> createdList, Node node)
        {
            MCNode mcNode = FindNodeWithIDFromList(createdList, node.id);
            if (mcNode != null && mcNode.nodeData.inputs != null)
            {
                foreach (PROJECT.Input input in node.inputs)
                {
                    CreateInputLineFromList(createdList, node.id, input);

                    // 작성자: 장세윤.
                    // MCLogNode의 경우 특수한 input이 있어서 예외 처리함
                    // Log Option의 경우 기존의 input과 다름.
                    if (input == null)
                    {
                        continue;
                    }

                    if (input.source != -1)
                    {
                        if (FindNodeWithIDFromList(createdList, input.source) == null)
                        {
                            continue;
                        }

                        CreateNodeInputLinesFromList(createdList, FindNodeWithIDFromList(createdList, input.source).nodeData);
                    }
                }
            }
        }

        private MCNode CreateInputLineFromList(List<MCNode> createdList, int nodeID, PROJECT.Input input)
        {
            if (input == null)
            {
                //Utils.LogRed("input == null");
                return null;
            }

            MCNode leftNode = FindNodeWithIDFromList(createdList, input.source);
            MCNode rightNode = FindNodeWithIDFromList(createdList, nodeID);

            if (leftNode != null && rightNode != null)
            {
                if (!leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id))
                {
                    return null;
                }

                MCNodeOutput left = leftNode.GetNodeOutputWithIndex(input.subid);
                MCNodeInput right = rightNode.GetNodeInputWithIndex(input.id);

                if (left != null && right != null && !right.HasLine)
                {
                    MCBezierLine line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();

                    // 왼쪽 노드가 GetNode인 경우 라인 색상 설정.
                    if (leftNode is MCGetNode)
                    {
                        MCGetNode getNode = leftNode as MCGetNode;
                        LeftMenuVariableItem variable;
                        if (getNode.IsLocalVariable == true)
                        {
                            variable = GetLocalVariable(getNode.CurrentVariableIndex);
                        }
                        else
                        {
                            variable = GetVariable(getNode.CurrentVariableIndex);
                        }

                        bool isList = variable.nodeType == NodeType.LIST;

                        //line.SetLineColor(Utils.GetParameterColor(isList ? DataType.LIST : variable.dataType));
                        line.SetLineColor(Utils.GetParameterColor(isList ? DataType.LIST : right.parameterType));
                    }
                    // 그 외의 경우 라인 색상 설정.
                    else
                    {
                        // 변수 타입이 Boolean인 경우 None으로 저장되는 경우가 발생함.
                        // 기존에 None으로 저장되어 있는 프로젝트는 Bool로 설정되도록 변경.
                        // 참고: 새로 저장하는 프로젝트에서는 None으로 저장되는 문제가 발생하지 않음.
                        if (input.type == DataType.NONE)
                        {
                            input.type = DataType.BOOL;
                        }

                        //line.SetLineColor(Utils.GetParameterColor(input.type));
                        line.SetLineColor(Utils.GetParameterColor(right.parameterType));
                    }

                    line.SetLinePoint(left, right);
                    AddLine(line);

                    left.LineSet();
                    right.LineSet();
                }
            }

            return leftNode;
        }

        private MCNode CreateInputLine(PROJECT.Process process, int nodeID, PROJECT.Input input)
        {
            // 왼쪽/오른쪽 노드의 NodeID와 ProcessID가 같아야 하지만,
            // ProcessID가 다른 경우에도 일단 연결.
            // 아래에서 왼쪽/오른쪽 노드의 processID 값이 다르지만 둘 중 하나가 0인 경우에는
            // 예외적으로 연결. 이 경우에는 실행선 연결 전에 작업 중인 노드일 가능성이 높기 때문에 허용.
            MCNode leftNode = FindNodeWithID(input.source, process.id);
            leftNode = leftNode == null ? FindNodeWithID(input.source) : leftNode;

            MCNode rightNode = FindNodeWithID(nodeID, process.id);
            rightNode = rightNode == null ? FindNodeWithID(nodeID) : rightNode;

            if (leftNode != null && rightNode != null)
            {
                // 2020.11.05.
                // 작성자: 장세윤 
                // 원칙적으로 왼쪽/오른쪽 노드의 프로세스 id 값이 같아야하는데,
                // 둘 중 하나의 id가 0인 경우는 실행선을 연결하기 전 상태에서 작업해둔 경우일 가능성이 높기때문에
                // 프로세스 id == 0인 경우에는 예외적으로 연결.
                //if (leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id) == false)
                if (leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id) == false
                    && leftNode.OwnedProcess.id.Equals(0) == false
                    && rightNode.OwnedProcess.id.Equals(0) == false)
                {
                    return null;
                }

                

                MCNodeOutput left = leftNode.GetNodeOutputWithIndex(input.subid);

                // 2021.12.02
                // For 루프 선 연결 예외처리.
                // For 루프의 입력이 기존 1개에서 3개로 변경되면서 이전 버전 호환성을 위한 처리.
                MCNodeInput right = null;
                if (rightNode is MCLoopNode && rightNode.nodeData.inputs != null && rightNode.nodeData.inputs.Length == 1)
                {
                    right = rightNode.GetNodeInputWithIndex(1);
                }
                else
                {
                    right = rightNode.GetNodeInputWithIndex(input.id);
                }
                //MCNodeInput right = rightNode.GetNodeInputWithIndex(input.id);

                if (left != null && right != null && !right.HasLine)
                {
                    MCBezierLine line = Utils.CreateNewLine();

                    // 왼쪽 노드가 GetNode인 경우 라인 색상 설정.
                    if (leftNode is MCGetNode)
                    {
                        //Utils.LogRed($"GetNode in Left id: {leftNode.NodeID}/ rightNode.NodeID: {rightNode.NodeID}");

                        MCGetNode getNode = leftNode as MCGetNode;
                        LeftMenuVariableItem variable;
                        if (getNode.IsLocalVariable == true)
                        {
                            variable = GetLocalVariable(getNode.CurrentVariableIndex);
                        }
                        else
                        {
                            variable = GetVariable(getNode.CurrentVariableIndex);
                        }

                        bool isList = variable.nodeType == NodeType.LIST;

                        //line.SetLineColor(Utils.GetParameterColor(isList ? DataType.LIST : variable.dataType));
                        line.SetLineColor(Utils.GetParameterColor(isList ? DataType.LIST : right.parameterType));
                    }
                    // 그 외의 경우 라인 색상 설정.
                    else
                    {
                        // 변수 타입이 Boolean인 경우 None으로 저장되는 경우가 발생함.
                        // 기존에 None으로 저장되어 있는 프로젝트는 Bool로 설정되도록 변경.
                        // 참고: 새로 저장하는 프로젝트에서는 None으로 저장되는 문제가 발생하지 않음.
                        if (input.type == DataType.NONE)
                        {
                            input.type = DataType.BOOL;
                        }

                        //line.SetLineColor(Utils.GetParameterColor(input.type));
                        line.SetLineColor(Utils.GetParameterColor(right.parameterType));
                    }

                    line.SetLinePoint(left, right);
                    AddLine(line);

                    left.LineSet();
                    right.LineSet();

                }
            }

            return leftNode;
        }

        private void CreateLinesFromProcess(PROJECT.Process process)
        {
            // 라인 생성.
            foreach (Node node in process.nodes)
            {
                if (node.nexts != null && node.nexts.Length > 0)
                {
                    for (int ix = 0; ix < node.nexts.Length; ++ix)
                    {
                        Next next = node.nexts[ix];

                        MCNode leftNode = FindNodeWithID(node.id, process.id);
                        MCNode rightNode = FindNodeWithID(next.next, process.id);
                        if (leftNode != null && rightNode != null)
                        {
                            if (!leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id))
                            {
                                continue;
                            }

                            MCNodeNext left = leftNode.GetNodeNextWithIndex(ix);
                            MCNodeStart right = rightNode.GetComponentInChildren<MCNodeStart>();

                            //if (left != null && right != null && !left.HasLine && !right.HasLine)
                            if (left != null && right != null && !left.HasLine)
                            {
                                MCBezierLine line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();

                                line.SetLinePoint(left, right);
                                AddLine(line);

                                left.LineSet();
                                right.LineSet();
                            }
                        }
                    }
                }

                // input/output 라인.
                CreateNodeInputLines(process, node);
            }
        }

        private MCNode FindNodeWithIDFromList(List<MCNode> createdList, int id)
        {
            foreach (var node in createdList)
            {
                if (node.NodeID.Equals(id))
                {
                    return node;
                }
            }

            return null;
        }

        private void CreateLineWithCreatedList(List<MCNode> createdList)
        {
            // 라인 생성.
            foreach (var node in createdList)
            {
                if (node.nodeData.nexts != null && node.nodeData.nexts.Length > 0)
                {
                    for (int ix = 0; ix < node.nodeData.nexts.Length; ++ix)
                    {
                        Next next = node.nodeData.nexts[ix];

                        MCNode leftNode = FindNodeWithIDFromList(createdList, node.NodeID);
                        MCNode rightNode = FindNodeWithIDFromList(createdList, next.next);
                        if (leftNode != null && rightNode != null)
                        {
                            if (!leftNode.OwnedProcess.id.Equals(rightNode.OwnedProcess.id))
                            {
                                continue;
                            }

                            MCNodeNext left = leftNode.GetNodeNextWithIndex(ix);
                            MCNodeStart right = rightNode.GetComponentInChildren<MCNodeStart>();

                            if (left != null && right != null && !left.HasLine && !right.HasLine)
                            {
                                MCBezierLine line = Utils.CreateNewLineGO().GetComponent<MCBezierLine>();

                                line.SetLinePoint(left, right);
                                AddLine(line);

                                left.LineSet();
                                right.LineSet();
                            }
                        }
                    }
                }

                // input/output 라인.
                CreateNodeInputLinesFromList(createdList, node.nodeData);
                //CreateNodeInputLines(process, node);
            }
        }

        public void LoadOnlyProjectLogic(ProjectDesc projectDesc, string projectData)
        {
            SetWorkspaceActive(true);

            ProjectData project = JsonUtility.FromJson<ProjectData>(projectData);

            // 변수 생성.
            //CreateVariableFromProject(project.variables);

            for (int ix = 0; ix < project.processes.Length; ++ix)
            {
                PROJECT.Process process = project.processes[ix];

                // 노드 생성.
                CreateNodesFromProcess(process, project.functions, ix);

                // 라인 생성.
                CreateLinesFromProcess(process);
            }

            // 주석 생성.
            CreateCommentsFromProject(project.comments);
        }

        public void CreateVariableFromProject()
        {
            if (HasCompiled == false)
            {
                Utils.LogRed("There's no compiled project data.");
                return;
            }

            ProjectData project = JsonUtility.FromJson<ProjectData>(compiledProjectString);
            CreateVariableFromProject(project.variables);
        }

        private void CreateVariableFromProject(Node[] variables)
        {
            foreach (Node variable in variables)
            {
                AddVariable(variable.body.name, variable.body.type, variable.type, variable.body.value, true);
            }
        }

        private void CreateCommentsFromProject(Comment[] comments)
        {
            for (int ix = 0; ix < comments.Length; ++ix)
            {
                MCCommentManager.Instance.AddNewComment(
                    comments[ix].title,
                    comments[ix].position,
                    comments[ix].size
                );
            }
        }

        // Test.
        public void LoadTestProject()
        {
            string path = "Rock_Paper_Scissors.json";
            string filepath = Application.dataPath + "/" + path;

            string jsonString = File.ReadAllText(filepath);

            MCProjectManager.ProjectDescription = ProjectDesc.CreateEmpty();
            MCProjectManager.ProjectDescription.title = "OX QUIZ";
            MCProjectManager.ProjectDescription.email = "ronniej@naver.com";
            MCProjectManager.ProjectDescription.description = "OX QUIZ Test";
            MCProjectManager.ProjectDescription.ownerName = FirebaseManager.CurrentUserName;
            MCProjectManager.ProjectDescription.ownerUuid = FirebaseManager.CurrentUserID;
            MCProjectManager.ProjectDescription.updateDate = DateTime.Now.ToString("yyyy-MM-dd");

            LoadProject(MCProjectManager.ProjectDescription, jsonString);
        }

        public ProjectData loadedProject;
        public void LoadProject(ProjectDesc projectDesc, string projectData)
        {
            ProjectData project = null;

            try
            {
                project = JsonUtility.FromJson<ProjectData>(projectData);
                project.language = projectDesc.language;
            }
            catch
            {
                throw new Exception("Json 데이터 오류");
            }

            // Test code.
            Mocca.MoccaSenarioPanel panel = FindObjectOfType<Mocca.MoccaSenarioPanel>();
            if (panel != null)
            {
                panel.SetName(projectDesc.title);
            }

            tabManager.AddProjectTab(projectDesc == null ? "Test" : projectDesc.title, TabManager.TabAddType.Load);
            SetWorkspaceActive(true);
            loadedProject = project;

            compiledProjectString = projectData;

            //Debug.Log($"<color=green> Loaded Project: {projectData} </color>");


            // 변수 생성.
            foreach (Node variable in project.variables)
            {
                AddVariable(variable.body.name, variable.body.type, variable.type, variable.body.value, true);
            }

            // 함수 생성.
            foreach (FunctionData data in project.functions)
            {
                variableFunctionManager.AddFunctionFromData(data);
            }

            //foreach (PROJECT.Process process in project.processes)
            for (int ix = 0; ix < project.processes.Length; ++ix)
            {
                PROJECT.Process process = project.processes[ix];

                // 프로세스 이름이 없는 경우에는 프로세스 이름 설정.
                // -> 예전에 저장된 프로세스의 경우 이름을 지정하지 않았기 때문에 저장되지 않았음.
                if (Utils.IsNullOrEmptyOrWhiteSpace(process.name) == true)
                {
                    process.name = "Process_" + (ix + 1).ToString();
                }

                // 노드 생성.
                CreateNodesFromProcess(process, project.functions, ix);

                // 라인 생성.
                CreateLinesFromProcess(process);
            }

            // 주석 생성.
            if (project.comments != null)
            {
                CreateCommentsFromProject(project.comments);
            }

            MCProjectManager.ProjectDescription = projectDesc;

            OnProcessUpdate?.Invoke();

            // Test code.
            //ResetWorkspaceState();
        }

        public void LoadFunctionFromLibrary(ProjectDesc projectDesc, string projectData, List<FunctionDesc> selectedFunctions)
        {
            if (selectedFunctions.Count == 0)
            {
                //Debug.Log("selectedFunctions.Count == 0");
                return;
            }

            ProjectData project = null;

            try
            {
                project = JsonUtility.FromJson<ProjectData>(projectData);
            }
            catch
            {
                throw new Exception("Json 데이터 오류");
            }


            // 선택한 함수 생성.
            foreach (FunctionData data in project.functions)
            {
                // Todo: 함수의 Private 변수 추가.
                foreach (VariableInfomation variableInfo in data.variableReferences)
                {
                    variableFunctionManager.AddVariable(
                        variableInfo.name,
                        variableInfo.type,
                        variableInfo.nodeType,
                        variableInfo.value
                    );

                    //Debug.Log($"variableInfo: {variableInfo}");
                }

                foreach (var selectedFunction in selectedFunctions)
                {
                    //Debug.Log($"data.name: {data.name} / selectedFunction.name: {selectedFunction.name}");
                    if (data.name.Equals(selectedFunction.name) == true)
                    {
                        variableFunctionManager.AddFunctionFromData(data);
                    }
                }
            }
        }

        public void UpdateFunction(PROJECT.FunctionData data)
        {
            variableFunctionManager.UpdateFunction(data);
            tables.UpdateAllLinesPosition();
        }

        //public void UpdateFunctionData(FunctionData data)
        //{
        //    if (IsFunctionExist(data.name))
        //    {
        //        int index = GetFunctionIndexWithName(data.name);
        //         GetFunction(index);
        //    }
        //    else
        //    {

        //    }
        //}

        //private bool IsFunctionExist(string functionName)
        //{
        //    foreach (string name in variableFunctionManager.FunctionNameList)
        //    {
        //        if (name.Equals(functionName))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //public GameObject CreateNewLineGO()
        //{
        //    GameObject newLine = Instantiate(MCEditorManager.Instance.linePrefab);
        //    newLine.transform.SetParent(LineParentTransform, false);
        //    newLine.transform.localPosition = Vector3.zero;

        //    return newLine;
        //}

        public MCNode FindNodeWithID(int nodeID, int processID = -1)
        {
            return tables.FindNodeWithID(nodeID, processID);
        }

        public GraphPane PaneObject
        {
            get
            {
                GameObject paneObject = MCEditorManager.Instance.GetPane(MCEditorManager.PaneType.Graph_Pane);
                //Debug.Log("paneObject null " + paneObject + " : " + gameObject);
                return paneObject.GetComponent<GraphPane>();
            }
        }

        // 함수 탭 열때 함수 시작 노드 만드는 함수 작성중.
        public void AddFunctionEntryNode(MCFunctionTab tab)
        {
            if (tab == null)
            {
                Debug.LogWarning("tab is null");
                return;
            }
            //Debug.LogWarning($"tabName: {tab.TabName}");

            //LeftMenuFunctionItem functionItem = MCFunctionTable.Instance.GetFunctionItemWithName(tab.TabName);
            //tab.CreateInputOutputNode(functionItem.Inputs, functionItem.Outputs);
        }

        public void AddEntryNode(Vector2 newPosition, int nodeID = 0)
        {
            // 프로젝트 새로 생성할 때 프로세스 ID 부여.
            PROJECT.Process newProcess = new PROJECT.Process();
            newProcess.name = "Process_" + NewProcessIndex.ToString();
            newProcess.id = Utils.NewGUID;
            newProcess.priority = 5;

            Vector3 position = new Vector3(newPosition.x, newPosition.y, 0f);
            MCNode startNode = PaneObject.AddNode(position, NodeType.START, nodeID, false, false, false);

            startNode.OwnedProcess = newProcess;

            UpdateProcess();

            //startNode.OwnedProcess.id = Utils.NewGUID;
            //startNode.OwnedProcess.priority = 5;
        }

        public int NewProcessIndex
        {
            get
            {
                int maxIndex = 0;
                var processes = Processes;
                foreach (var process in processes)
                {
                    int parsedIndex = int.Parse(process.name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    if (maxIndex < parsedIndex)
                    {
                        maxIndex = parsedIndex;
                    }
                }

                return maxIndex + 1;
            }
        }

        public void AddEntryNode()
        {
            AddEntryNode(startPos);

            ////Vector3 position = new Vector3(80f, -80f, 0f);
            //Vector3 position = new Vector3(startPos.x, startPos.y, 0f);
            //MCNode startNode = PaneObject.AddNode(position, NodeType.START);

            //// 프로젝트 새로 생성할 때 프로세스 ID 부여.
            //startNode.OwnedPocessID = Utils.NewGUID;
            //startNode.OwnedProcessPriority = 5;
        }

        public void AddNode(MCNode node, int nodeID = 0)
        {
            if (nodeID != 0)
            {
                node.NodeID = nodeID;
                node.nodeData.id = nodeID;
            }

            // 현재 탭이 프로젝트 탭인 경우.
            if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                tables.AddNode(node);
            }

            // 현재 탭이 함수 탭인 경우.
            else if (CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                // 현재 탭 정보 받아오기.
                MCFunctionTable.Instance.AddFunctionNode(tabManager.CurrentTab.TabName, node);
            }
        }

        public List<MCNode> SelectedNodes
        {
            get
            {
                if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
                {
                    return tables.SelectedNodesInProject;
                }
                else if (CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
                {
                    return MCFunctionTable.Instance.GetSelectedNodesInFunction(Utils.CurrentTabName);
                }

                return null;
            }
        }

        public int CurrentSelectedBlockCount
        {
            get
            {
                if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
                {
                    return tables.SelectedNodesInProject.Count;
                }
                else if (CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
                {
                    return MCFunctionTable.Instance.GetSelectedNodesInFunction(Utils.CurrentTabName).Count;
                }

                return 0;
            }
        }

        // 주석(Comment)가 눌렸을 때 Ctrl/Shift키가 눌리지 않은 상태인 경우,
        // 노드의 선택을 해제하기 위해 호출.
        public void OnCommentPointDown()
        {
            if (!KeyInputManager.Instance.shouldMultiSelect)
            {
                SetAllUnSelected();
            }
        }

        public void SetOneSelected(MCNode node)
        {
            if (IsProjectNull)
            {
                return;
            }

            tables.SetOneSelected(node);
        }

        public void SetAllSelected()
        {
            if (IsProjectNull)
            {
                return;
            }

            tables.SetAllSelected();
        }

        public void SetAllUnSelected()
        {
            if (IsProjectNull)
            {
                return;
            }

            tables.SetAllUnSelected();
        }

        private void DeleteSelectedProjectNodes()
        {
            if (IsProjectNull)
            {
                return;
            }

            tables.DeleteSelected();
        }

        private void DeleteSelectedFunctionNodes()
        {
            MCFunctionTable.Instance.DeleteSelected();
        }

        public void DeleteSelected()
        {
            if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            {
                DeleteSelectedProjectNodes();
            }
            else if (CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                DeleteSelectedFunctionNodes();
            }
        }

        public void SetDragOffset(Vector3 pointerPosition)
        {
            var nodeDrags = from drag in GetNodeDrags
                            where drag.GetComponent<MCNode>().IsSelected == true
                            select drag;

            foreach (MCNodeDrag drag in nodeDrags)
            {
                drag.SetDragOffset(pointerPosition);
            }

            // 노드 이동 명령 생성.
            MCCommandHelper.nodeMoveCommand = new MCMultiNodeMoveCommand(nodeDrags.ToList());
        }

        //public void RequestLineDelete(int blockID, int )

        public void SetScrollbarValue(float horizontalValue, float verticalValue)
        {
            workspaceScrollRect.horizontalScrollbar.value = horizontalValue;
            workspaceScrollRect.verticalScrollbar.value = verticalValue;
        }

        public void ResetWorkspace()
        {
            workspaceScrollRect.ResetState();
            workspaceScrollRect.GetComponent<ContentScaler>().ResetAll();
        }

        public void ResetWorkspaceState()
        {
            workspaceScrollRect.ResetState();
        }

        public void SetWorkspaceActive(bool isActive)
        {
            //Debug.LogWarning(string.Format("SetWorkspaceActive {0}", !isActive));
            disableScreen.SetActive(!isActive);
        }

        public void SubscribeLineUpdate(Action lineUpdate)
        {
            LineUpdate += lineUpdate;
        }

        public void UnsubscribeLineUpdate(Action lineUpdate)
        {
            LineUpdate -= lineUpdate;
        }

        public void UpdateAllLineUpdate()
        {
            LineUpdate?.Invoke();
        }

        private bool IsNoneExecuteType(MCNode node)
        {
            if (node is MCGetNode
                || node is MCGetElementNode
                || node is MCInsertNode
                || node is MCRemoveNode
                || node is MCRemoveAllNode
                || node is MCAddNode
                || node is MCSubstractNode
                || node is MCMultiplyNode
                || node is MCDivideNode
                || node is MCAndNode
                || node is MCOrNode
                || node is MCEqualNode
                || node is MCNotEqualNode
                || node is MCGreaterNode
                || node is MCGreaterEqualNode
                || node is MCLessNode
                || node is MCLessEqualNode)
            {
                return true;
            }

            return false;
        }

        public List<MCStartNode> StartNodes
        {
            get
            {
                List<MCStartNode> startNodes = new List<MCStartNode>();
                foreach (MCNode node in tables.locatedNodes)
                {
                    if (node is MCStartNode)
                    {
                        startNodes.Add(node as MCStartNode);
                    }
                }

                return startNodes;
            }
        }

        // 프로세스 정보를 업데이트하는 과정에서 이미 처리된 노드 기록을 위한 리스트.
        // StackOverflow 방지용.
        List<MCNode> processedNodes = new List<MCNode>();

        // 이미 처리된 노드인지 확인하는 함수.
        private bool HasProcessed(MCNode node)
        {
            if (processedNodes == null || processedNodes.Count == 0)
            {
                return false;
            }

            foreach (MCNode processedNode in processedNodes)
            {
                if (processedNode.NodeID.Equals(node.NodeID))
                {
                    return true;
                }
            }

            return false;
        }

        // 노드 순회를 위한 함수.
        //private void GoNextNode(MCNode current, int processID, int priority)
        private void GoNextNode(MCNode current, PROJECT.Process process)
        {
            // 이미 처리한 노드인지 확인.
            if (current != null && HasProcessed(current))
            {
                return;
            }

            if (current != null)
            {
                current.OwnedProcess.name = process.name;
                current.OwnedProcess.id = process.id;
                current.OwnedProcess.priority = process.priority;

                // 처리된 노드를 리스트에 기록.
                processedNodes.Add(current);
                GoNextInputNode(current, process);
            }

            if (current != null && current.NextMCNodes != null && current.NextNodes.Count > 0)
            {
                foreach (MCNode node in current.NextNodes)
                {
                    GoNextNode(node, process);
                    GoNextInputNode(node, process);
                }
            }
        }

        // 입력 노드 순회를 위한 함수.
        //private void GoNextInputNode(MCNode current, int processID, int priority)
        private void GoNextInputNode(MCNode current, PROJECT.Process process)
        {
            if (current == null)
            {
                return;
            }

            for (int ix = 0; ix < current.NodeInputCount; ++ix)
            {
                int sourceID = current.GetNodeInputWithIndex(ix).input.source;
                MCNode sourceNode = FindNodeWithID(sourceID);
                if (sourceNode)
                {
                    sourceNode.OwnedProcess.name = process.name;
                    sourceNode.OwnedProcess.id = process.id;
                    sourceNode.OwnedProcess.priority = process.priority;

                    for (int jx = 0; jx < sourceNode.NodeInputCount; ++jx)
                    {
                        int secondSourceID = sourceNode.GetNodeInputWithIndex(jx).input.source;
                        MCNode secondSourceNode = FindNodeWithID(secondSourceID);
                        if (secondSourceNode)
                        {
                            secondSourceNode.OwnedProcess.name = process.name;
                            secondSourceNode.OwnedProcess.id = process.id;
                            secondSourceNode.OwnedProcess.priority = process.priority;
                        }
                    }
                }

                if (sourceNode != null && sourceNode.NodeInputCount > 0)
                {
                    GoNextInputNode(sourceNode, process);
                }
            }
        }

        public void UpdateProcessInfo()
        {
            processedNodes = new List<MCNode>();
            List<MCStartNode> startNodes = StartNodes;
            foreach (MCStartNode startNode in startNodes)
            {
                MCNode node = startNode.NextNodes.Count > 0 ? startNode.NextNodes[0] : null;
                if (node != null)
                {
                    GoNextNode(node, startNode.OwnedProcess);
                }
            }
        }

        #region UpdateProcessInfo 코드 백업
        //private void UpdateProcessInfo()
        //{
        //    List<MCStartNode> startNodes = GetStartNodes;
        //    foreach (MCStartNode startNode in startNodes)
        //    {
        //        int index = 0;
        //        MCNode prevNode = startNode;
        //        List<MCNode> nextNodes = prevNode.NextNodes;
        //        MCNode current = nextNodes.Count == 0 ? null : nextNodes[index];
        //        while (current != null)
        //        {
        //            // node.
        //            current.OwnedPocessID = prevNode.OwnedPocessID;
        //            current.OwnedProcessPriority = prevNode.OwnedProcessPriority;

        //            // input.
        //            for (int ix  = 0; ix < current.NodeInputCount; ++ix)
        //            {
        //                //Debug.Log(string.Format("ProcessInput NodeID: {0} , processID: {1}", current.GetNodeInputWithIndex(ix).Node.NodeID, prevNode.OwnedPocessID));

        //                int sourceID = current.GetNodeInputWithIndex(ix).input.source;
        //                MCNode sourceNode = FindNodeWithID(sourceID);
        //                if (sourceNode)
        //                {
        //                    sourceNode.OwnedPocessID = prevNode.OwnedPocessID;
        //                    sourceNode.OwnedProcessPriority = prevNode.OwnedProcessPriority;

        //                    for (int jx = 0; jx < sourceNode.NodeInputCount; ++jx)
        //                    {
        //                        int secondSourceID = sourceNode.GetNodeInputWithIndex(jx).input.source;
        //                        MCNode secondSourceNode = FindNodeWithID(secondSourceID);
        //                        if (secondSourceNode)
        //                        {
        //                            secondSourceNode.OwnedPocessID = prevNode.OwnedPocessID;
        //                            secondSourceNode.OwnedProcessPriority = prevNode.OwnedProcessPriority;
        //                        }
        //                    }
        //                }
        //            }

        //            if (index < (nextNodes.Count - 1))
        //            {
        //                ++index;
        //                current = nextNodes[index];
        //            }

        //            else
        //            {
        //                index = 0;
        //                nextNodes = prevNode.NextNodes;
        //                prevNode = current;
        //                current = nextNodes.Count == 0 ? null : nextNodes[index];
        //            }
        //        }
        //    }
        //}
        #endregion

        public void AddLine(MCBezierLine line)
        {
            // Test.
            // 노드 추가하기 전에 현 상태 저장.
            //MCUndoRedoManager.Instance.RecordProject();

            UpdateProcessInfo();

            //새로 연결된 노드에 프로세스 ID 설정.
            if (IsNoneExecuteType(line.left.Node))
            {
                line.left.Node.OwnedProcess.id = line.right.Node.OwnedProcess.id;
                line.left.Node.OwnedProcess.priority = line.right.Node.OwnedProcess.priority;
            }
            else
            {
                line.right.Node.OwnedProcess.id = line.left.Node.OwnedProcess.id;
                line.right.Node.OwnedProcess.priority = line.left.Node.OwnedProcess.priority;
            }

            tables.AddLine(line);
        }

        public void DeleteAllLines()
        {
            tables.DeleteAllLines();
        }

        public void RemoveLine(MCBezierLine line)
        {
            tables.RemoveLine(line);
        }

        public void RequestNodeDelete(MCNode node)
        {
            tables.RequestNodeDelete(node);
        }

        public bool RequestLineDelete(int lineID)
        {
            foreach (MCBezierLine line in tables.locatedLines)
            {
                if (line.LineID.Equals(lineID))
                {
                    // 연결된 양 소켓에 선 삭제 알림.
                    line.left.RemoveLine(lineID);
                    line.left.LineDeleted();
                    line.right.RemoveLine(lineID);
                    line.right.LineDeleted();

                    line.UnsubscribeAllDragNodes();
                    Destroy(line.gameObject);
                    tables.RemoveLine(line);

                    // Test.
                    // 노드 추가하기 전에 현 상태 저장.
                    //MCUndoRedoManager.Instance.RecordProject();

                    //----------------------------------------
                    // 선연결 해제시 발생하는 튜토리얼이벤트 -kjh
                    //----------------------------------------
                    //string msg = string.Format($"{targetSocket.Node.nodeData.type},{this.Node.nodeData.type}");
                    TutorialManager.SendEvent(Tutorial.CustomEvent.NodeUnlinked);

                    return true;
                }
            }

            //Utils.LogRed("[MCWorkSpaceManager.RequestLineDelete] Hey?");
            return false;
        }

        // 현재는 변수 유형이 바뀐 경우, 입출력의 파라미터 타입이 같은 경우만 기존 선 유지함.
        // Number-Boolean/Number-String과 같이 타입이 달라도 연결돼야하는 경우도 반영해야 함.
        private bool EqualsParameterType(MCBezierLine line)
        {
            MCNodeOutput output = line.left.GetComponent<MCNodeOutput>();
            MCNodeInput input = line.right.GetComponent<MCNodeInput>();

            return output.CheckTargetSocketType(MCNodeSocket.SocketType.Input, input);

            #region 기존 코드 백업
            //if (output.parameterType == input.parameterType)
            //{
            //    return true;
            //}

            //else if (output.parameterType == DataType.STRING && input.parameterType == DataType.NUMBER
            //    || output.parameterType == DataType.NUMBER && input.parameterType == DataType.STRING)
            //{
            //    return true;
            //}

            //else if (output.parameterType == DataType.STRING && input.parameterType == DataType.BOOL
            //    || output.parameterType == DataType.BOOL && input.parameterType == DataType.STRING)
            //{
            //    return true;
            //}

            //else if (output.parameterType == DataType.BOOL && input.parameterType == DataType.NUMBER
            //    || output.parameterType == DataType.NUMBER && input.parameterType == DataType.BOOL)
            //{
            //    return true;
            //}

            //else if (output.Node.nodeData.type == NodeType.GET && input.parameterType == DataType.LIST)
            //{
            //    return true;
            //}

            //return false;
            #endregion
        }

        // 노드 정보가 바뀌었을 때 
        // 기존에 연결된 노드의 Input/Output 간의 데이터 타입이 달라졌을 경우 
        // 라인 삭제하는 로직.
        public void ValidateNodeInputOutput(MCNode node)
        {
            if (MCNode.visitedNode.TryGetValue(node.NodeID, out MCNode testNode) == true)
            {
                return;
            }

            MCNode.visitedNode.Add(node.NodeID, node);

            List<int> willBeDeleted = new List<int>();

            for (int ix = 0; ix < tables.locatedLines.Count; ++ix)
            {
                MCBezierLine line = tables.locatedLines[ix];
                if (line.left == null || line.right == null)
                {
                    willBeDeleted.Add(line.LineID);
                    continue;
                }

                for (int jx = 0; jx < node.NodeOutputCount; ++jx)
                {
                    if (line.left.GetInstanceID().Equals(node.GetNodeOutputWithIndex(jx).GetInstanceID()))
                    {
                        if (EqualsParameterType(line) == false)
                        {
                            willBeDeleted.Add(line.LineID);
                            continue;
                        }
                    }
                }

                for (int jx = 0; jx < node.NodeInputCount; ++jx)
                {
                    if (line.right.GetInstanceID().Equals(node.GetNodeInputWithIndex(jx).GetInstanceID()))
                    {
                        if (EqualsParameterType(line) == false)
                        {
                            willBeDeleted.Add(line.LineID);
                            continue;
                        }
                    }
                }
            }

            foreach (int id in willBeDeleted)
            {
                RequestLineDelete(id);
            }

            // 위에서 배치된 라인을 기준으로 검증하고 나서 각 노드의 입출력에 대한 라인 검증도 진행.
            foreach (var locatedNode in tables.locatedNodes)
            {
                for (int ix = 0; ix < locatedNode.NodeInputCount; ++ix)
                {
                    MCNodeInput input = locatedNode.GetNodeInputWithIndex(ix);
                    if (input != null && input.HasLine && input.line == null)
                    {
                        input.LineDeleted();
                    }
                }

                for (int ix = 0; ix < locatedNode.NodeOutputCount; ++ix)
                {
                    MCNodeOutput output = locatedNode.GetNodeOutputWithIndex(ix);
                    if (output != null)
                    {
                        if (output.lines.Count == 0 && output.HasLine == true 
                            && output.currentLine == null && output.line == null)
                        {
                            output.LineDeleted();
                            output.HasLine = false;
                            continue;
                        }
                    }

                }
            }

            // 라인 색상을 항상 라인의 오른쪽 소켓의 색상으로 설정.
            foreach (var line in LocatedLines)
            {
                MCNodeInputOutputBase ioBase = line.right.GetComponent<MCNodeInputOutputBase>();
                if (ioBase != null)
                {
                    line.SetLineColor(Utils.GetParameterColor(ioBase.parameterType));
                }
            }
        }

        public void NewProject()
        {
            LoginCheck.Instance.ClickButton_NewProject();
            MCEditorManager.Instance.CloseAllPopups();

            OnToolbarButtonClicked?.Invoke();

        }

        public bool HasCompiled
        {
            get
            {
                return !string.IsNullOrEmpty(compiledProjectString);
            }
        }

        public void DelayValidateProjectCall(float time = 0.05f)
        {
            //Invoke("ValidateProject", time);
            StartCoroutine(WaitForFrames(2));
        }

        private WaitForEndOfFrame oneFrame = null;
        private IEnumerator WaitForFrames(int frameCount)
        {
            if (oneFrame == null)
            {
                oneFrame = new WaitForEndOfFrame();
            }

            while (frameCount > 0)
            {
                yield return oneFrame;
                --frameCount;
            }

            shouldValidateProject = true;
        }

        bool shouldValidateProject = false;
        public void DelayValidateProject()
        {
            shouldValidateProject = true;
        }

        // 배치된 노드가 유효한지 확인하는 메소드.
        // 변수의 Get/Set, 함수 노드가 유효한지 확인.
        public bool ValidateProject()
        {
            List<MCNode> deletedNodes = new List<MCNode>();
            List<DeleteLineInfo> deletedLines = new List<DeleteLineInfo>();
            tables.locatedNodes.ForEach((node) =>
            {
                if (node is MCGetNode)
                {
                    MCGetNode getNode = node as MCGetNode;

                    bool shouldDelete = true;
                    // 변수 확인.
                    foreach (var variable in tables.variables)
                    {
                        if (variable.VariableID.Equals(getNode.CurrentVariableID) == true
                        || variable.VariableName.Equals(getNode.nodeData.body.name) == true)
                        {
                            //deletedNodes.Add(getNode);
                            //MCCommandHelper.CheckIfDeleteLines(
                            //    node.nodeData.id, node.OwnedProcess.id, ref deletedLines);
                            shouldDelete = false;
                            break;
                        }
                    }

                    if (shouldDelete == true)
                    {
                        deletedNodes.Add(getNode);
                        MCCommandHelper.CheckIfDeleteLines(
                            node.nodeData.id, node.OwnedProcess.id, ref deletedLines
                        );
                    }
                }
                else if (node is MCSetNode)
                {
                    MCSetNode setNode = node as MCSetNode;

                    bool shouldDelete = true;
                    // 변수 확인.
                    foreach (var variable in tables.variables)
                    {
                        if (variable.VariableID.Equals(setNode.CurrentVariableID) == true
                        || variable.VariableName.Equals(setNode.nodeData.body.name) == true)
                        {
                            //deletedNodes.Add(setNode);
                            //MCCommandHelper.CheckIfDeleteLines(
                            //    node.nodeData.id, node.OwnedProcess.id, ref deletedLines);
                            shouldDelete = false;
                            break;
                        }
                    }

                    if (shouldDelete == true)
                    {
                        deletedNodes.Add(setNode);
                        MCCommandHelper.CheckIfDeleteLines(
                            node.nodeData.id, node.OwnedProcess.id, ref deletedLines
                        );
                    }
                }

                else if (node is MCFunctionNode)
                {
                    MCFunctionNode funcNode = node as MCFunctionNode;
                    bool shouldDelete = true;
                    // 함수 확인.
                    foreach (var function in MCFunctionTable.Instance.functions)
                    {
                        if (function.FunctionID.Equals(funcNode.FunctionID) == true)
                        {
                            //deletedNodes.Add(funcNode);
                            //MCCommandHelper.CheckIfDeleteLines(
                            //    node.nodeData.id, node.OwnedProcess.id, ref deletedLines);
                            shouldDelete = false;
                            break;
                        }
                    }

                    if (shouldDelete == true)
                    {
                        deletedNodes.Add(funcNode);
                        MCCommandHelper.CheckIfDeleteLines(
                            node.nodeData.id, node.OwnedProcess.id, ref deletedLines
                        );
                    }

                    // 삭제된 함수가 아닌 경우에는 입출력 변경 여부 확인.
                    else
                    {
                        // Todo: 함수탭 열린 상태에서 입출력 추가/삭제/변경이 발생했는지 확인.
                        var functionItem = GetFunctionItemWithID(funcNode.FunctionID);
                        if (functionItem != null)
                        {
                            // 함수 업데이트 호출.
                            UpdateFunction(functionItem.FunctionData);

                            //// 함수 노드와 연결된 다른 노드와의 입출력 검증.
                            //ValidateNodeInputOutput(funcNode);
                        }
                    }
                }
            });

            // 노드 검증 후 라인 검증.
            foreach (var node in tables.locatedNodes)
            {
                ValidateNodeInputOutput(node);
            }

            if (deletedNodes.Count == 0)
            {
                return true;
            }

            // 제거된 변수나 함수의 노드가 배치됐으면 제거.
            foreach (var node in deletedNodes)
            {
                tables.RemoveNode(node);
                Destroy(node.gameObject);
            }

            deletedNodes.Clear();
            deletedNodes = null;

            // 앞서 제거된 노드와 연결된 라인 삭제.
            foreach (var lineInfo in deletedLines)
            {
                MCBezierLine line = tables.FindLineWithID(lineInfo.lineID);
                tables.RemoveLine(line);
                Destroy(line.gameObject);
            }

            deletedLines.Clear();
            deletedLines = null;

            return false;
        }

        public void ReloadFromCompiledProject()
        {
            //Debug.Log(compiledProjectString);

            //if (!string.IsNullOrEmpty(compiledProjectString))
            if (HasCompiled == true)
            {
                //Debug.Log("Reload Project");
                //Debug.Log(compiledProjectString);
                LoadOnlyProjectLogic(MCProjectManager.ProjectDescription, compiledProjectString);
            }
        }

        // 디버깅용.
        // 작성자: 장세윤 (20211118).
        // 프로젝트 string 데이터를 MCWorkspaceManager의 compiledProjectString에 입력한 다음,
        // Alt + F10으로 프로젝트를 강제 로드하는 데 사용하는 메소드.
        public void LoadProjectFromText()
        {
            ProjectDesc desc = new ProjectDesc();
            desc.language = LocalizationManager.Language.KOR;
            LoadProject(desc, compiledProjectString);
        }

        public void LoadProject()
        {
            if (TutorialManager.IsPlaying == true)
                return;

            if (MCPlayStateManager.Instance.IsSimulation == false)
            {
                LoginCheck.Instance.ClickButton_DownloadProject();
                MCEditorManager.Instance.CloseAllPopups();

                OnToolbarButtonClicked?.Invoke();
            }
            else
            {
                MessageBox.Show("[ID_MSG_FAILE_LOAD_PROJECT_DUETO_PROJECT_ISSIMULATING]");
            }
        }

        public void LoadLibrary()
        {
            LoginCheck.Instance.ClickButton_DownloadLibrary();
        }

        public void SaveProject()
        {
            // 함수 탭에서 저장을 시도할 때 메시지를 보여주도록 처리함.
            // 프로젝트 저장은 프로젝트 탭에서만 가능하도록.
            if (Utils.GetTabManager().CurrentOwnerGroup == Constants.OwnerGroup.FUNCTION)
            {
                MessageBox.Show("[ID_Project_Save_In_Function]");
                return;
            }

            LoginCheck.Instance.ClickButton_UploadProject();
            MCEditorManager.Instance.CloseAllPopups();

            OnToolbarButtonClicked?.Invoke();

        }

        public void OnCloseCurrentProjectClicked()
        {
            if (tabManager.CurrentTab == null)
                return;

            if (IsDirty == true)
            {
                MessageBox.ShowYesNo("[ID_MSG_CAUTION_END_PROJECT]저장되지 않은 정보는 사라집니다.\n 현재 프로젝트를 닫습니까?", res =>
                {
                    if (res == true)
                    {
                        CloseCurrentProject();
                    }
                }); // local 추가완료

                OnToolbarButtonClicked?.Invoke();
            }

            else
            {
                CloseCurrentProject();
            }
        }

        public void CloseCurrentProject()
        {
            if (IsProjectNull)
            {
                return;
            }

            ReleaseAllLogic();
            //tabManager.RemoveTab(tabManager.CurrentTab);
            tabManager.RemoveAllTabs();
            SetWorkspaceActive(false);

            // 주석 삭제.
            MCCommentManager.Instance.DeleteAll();

            Mocca.MoccaSenarioPanel panel = FindObjectOfType<Mocca.MoccaSenarioPanel>();
            if (panel != null)
            {
                panel.SetName(string.Empty);
                //panel.SetActive(false);
            }

            // UndoRedo 초기화.
            MCUndoRedoManager.Instance.ResetAllRecord();

            // 모든 프로퍼티 창 닫기.
            PropertyWindowManager.Instance.TurnOffAll();

            // 변수/로컬변수/함수탭 Content 크기조정.
            variableFunctionManager.ResizeAllContents();

            // 게임 오브젝트 이름 설정을 위한 노드 넘버 초기화.
            Utils.ResetNodeNumber();

            IsDirty = false;
        }

        public void ReleaseOnlyNodeLogic()
        {
            tables.DeleteAllNodes();
            tables.DeleteAllLines();

            // 배치된 변수 모두 제거.
            //tables.DeleteAllVariables();

            // 배치된 주석도 모두 제거.
            MCCommentManager.Instance.DeleteAll();
        }

        public void ReleaseAllLogic()
        {
            tables.DeleteAllNodes();
            tables.DeleteAllLines();
            //tables.DeleteAllVariables();
            variableFunctionManager.RemoveAllVariables();
            //tables.DeleteAllLocalVariables();
            variableFunctionManager.RemoveAllLocalVariables();
            MCFunctionTable.Instance.DeleteAllFunctionNodes();
            MCFunctionTable.Instance.DeleteAllFunctions();

            MCProjectManager.ProjectDescription = null;
            compiledProjectString = string.Empty;
        }


        private void DuplicateNodes(
            List<Node> selectedNodes,
            List<VariableInfomation> copiedVariables,
            List<FunctionData> copiedFunction)
        {
            // 노드 복사 처리 전에 Undo 기록.
            //MCUndoRedoManager.Instance.RecordProject();

            // 기존 노드 선택 해제.
            SetAllUnSelected();

            // 작성자: 장세윤.
            // Command 패턴을 적용.
            //MCAddNodeByCopyPasteCommand command = new MCAddNodeByCopyPasteCommand(selectedNodes);
            //MCUndoRedoManager.Instance.AddCommand(command);

            // 작성자: kjh 
            // MCNode를 Node 로 변경 (2020-07-23)
            //MCAddNodeByCopyPasteCommand command = new MCAddNodeByCopyPasteCommand(selectedNodes);
            MCAddNodeByCopyPasteCommand command = new MCAddNodeByCopyPasteCommand(
                selectedNodes,
                copiedVariables,
                copiedFunction);
            MCUndoRedoManager.Instance.AddCommand(command);


            // 작성자: 장세윤.
            // 기존 복/붙 로직.
            //Vector3 offset = new Vector3(120f, 120f);

            //// 기존 ID - 새로 생성된 ID 테이블.
            //Dictionary<int, int> idTable = new Dictionary<int, int>();

            //// 생성된 블록 리스트.
            //List<MCNode> createdNodes = new List<MCNode>();

            //// 노드 복사 생성.
            //foreach (MCNode node in selectedNodes)
            //{
            //    MCNode newNode = PaneObject.AddNode(node.transform.localPosition - offset, node.nodeData.type, Utils.NewGUID, false, true, true);
            //    node.MakeNode();
            //    newNode.SetData(node.nodeData);
            //    createdNodes.Add(newNode);
            //    idTable.Add(node.NodeID, newNode.NodeID);

            //    // 새로 생성된 노드 선택.
            //    newNode.IsSelected = true;
            //}

            //// 참조 ID 정보 갱신.
            //foreach (var node in createdNodes)
            //{
            //    // 다음 실행선 정보 갱신.
            //    for (int ix = 0; ix < node.NodeNextCount; ++ix)
            //    {
            //        Next next = node.nodeData.nexts[ix];
            //        if (next.next != -1)
            //        {
            //            int newID;
            //            if (idTable.TryGetValue(next.next, out newID))
            //            {
            //                next.next = newID;
            //            }
            //        }
            //    }

            //    // 입력 정보 갱신.
            //    for (int ix = 0; ix < node.NodeInputCount; ++ix)
            //    {
            //        PROJECT.Input input = node.nodeData.inputs[ix];
            //        if (input.source != 0)
            //        {
            //            int newID;
            //            if (idTable.TryGetValue(input.source, out newID))
            //            {
            //                input.source = newID;
            //            }
            //        }
            //    }
            //}

            //CreateLineWithCreatedList(createdNodes);

            // 선 추가.
            //foreach (var node in createdNodes)
            //{
            //    CreateLineWithCreatedList(createdNodes);
            //}
        }


        Node NodeCopy(Node node)
        {
            string json = JsonUtility.ToJson(node);
            return JsonUtility.FromJson<Node>(json);
        }


        public void CopySelectedNodes()
        {
            if (SelectedNodes == null || SelectedNodes.Count == 0)
            {
                return;
            }

            ClipBoardManager.ClipBoardContent content = new ClipBoardManager.ClipBoardContent();
            content.clipboardType = ClipBoardManager.ClipBoardContent.ClipboardType.Copy;

            List<PROJECT.Node> nodes = new List<Node>();
            List<VariableInfomation> variables = new List<VariableInfomation>();
            List<FunctionData> functions = new List<FunctionData>();

            foreach (var node in SelectedNodes)
            {
                if (node.nodeData.type == NodeType.START)
                {
                    continue;
                }

                if (node.nodeData.type == NodeType.FUNCTION_OUTPUT ||
                    node.nodeData.type == NodeType.FUNCTION_INPUT)
                {
                    continue;
                }

                node.MakeNode();

                if (node.nodeData.type == NodeType.GET || node.nodeData.type == NodeType.SET)
                {
                    // 이미 추가된 변수인지 확인 (중복 방지를 위해).
                    bool hasAlreadyAdded = false;
                    foreach (VariableInfomation info in variables)
                    {
                        if (info.name.Equals(node.nodeData.body.name))
                        {
                            hasAlreadyAdded = true;
                            break;
                        }
                    }

                    // 추가되지 않은 변수 정보만 추가.
                    if (hasAlreadyAdded == false)
                    {
                        int index = GetVariableIndexWithName(node.nodeData.body.name);
                        LeftMenuVariableItem variable = GetVariable(index);
                        if (variable != null && variable.isLocalVariable == false)
                        {
                            variables.Add(new VariableInfomation()
                            {
                                name = variable.VariableName,
                                nodeType = variable.nodeType,
                                type = variable.dataType,
                                value = variable.value
                            });
                        }
                    }

                    // Todo: 지역 변수 추가.
                }

                if (node.nodeData.type == NodeType.FUNCTION)
                {
                    // 이미 추가된 함수 정보인지 확인 (중복 방지위해).
                    bool hasAlreadyAdded = false;
                    foreach (FunctionData info in functions)
                    {
                        if (info.name.Equals(node.nodeData.body.name))
                        {
                            hasAlreadyAdded = true;
                            break;
                        }
                    }

                    // 추가되지 않은 함수 정보만 추가.
                    if (hasAlreadyAdded == false)
                    {
                        LeftMenuFunctionItem function = GetFunctionItemWithName(node.nodeData.body.name);
                        FunctionData data = new FunctionData(function.FunctionData);
                        functions.Add(data);
                    }
                }

                nodes.Add(NodeCopy(node.nodeData));
            }

            content.copiedNodes = nodes;

            //content.copiedVariables = variables;
            foreach (VariableInfomation info in variables)
            {
                content.copiedVariables.Add(info);
            }
            //content.copiedFunction = functions;
            foreach (FunctionData data in functions)
            {
                content.copiedFunction.Add(data);
            }

            ClipBoardManager.Instance.PushContent(content);
        }


        public void PasteSelectedNodes()
        {
            ClipBoardManager.ClipBoardContent content = ClipBoardManager.Instance.PopContent();
            if (content == null)
            {
                return;
            }

            //DuplicateNodes(content.copiedNodes);
            DuplicateNodes(content.copiedNodes, content.copiedVariables, content.copiedFunction);
        }

        public void AddVariable(string name, DataType type, NodeType nodeType, string value, bool isLoaded = false)
        {
            // 프로젝트 로드 과정에서 변수 추가하는 경우에는 Undo 처리 명령에서 제외.
            if (isLoaded)
            {
                variableFunctionManager.AddVariable(name, type, nodeType, value);
                return;
            }

            // 변수 추가 팝업을 통해 변수를 추가하는 경우에는 Undo 처리 명령으로 간주.
            MCAddVariableCommand command = new MCAddVariableCommand(name, type, nodeType, value);
            MCUndoRedoManager.Instance.AddCommand(command);
        }

        public LeftMenuVariableItem GetVariable(string name)
        {
            int index = GetVariableIndexWithName(name);
            if (index.Equals(-1))
            {
                return null;
            }

            return GetVariable(index);
        }

        public LeftMenuVariableItem GetVariable(int index)
        {
            if (index < 0 || index >= tables.variables.Count)
            {
                return null;
            }

            //Debug.LogWarning($"index check {index} / variable count: {tables.variables.Count}");

            return tables.variables[index];
        }

        public LeftMenuVariableItem GetVariableWithID(int variableID)
        {
            foreach (var variable in tables.variables)
            {
                if (variable.VariableID.Equals(variableID))
                {
                    return variable;
                }
            }

            return null;
        }

        public void UpdateVariable()
        {
            variableFunctionManager.UpdateVariable();
            tables.UpdateAllLinesPosition();
        }

        public string[] VariableNameList
        {
            get
            {
                return variableFunctionManager.VariableNameList;
            }
        }

        public int GetVariableIndexWithName(string name)
        {
            return tables.GetVariableIndexWithName(name);
        }

        public string GetVariableValue(int index)
        {
            return tables.variables[index].value;
        }

        public DataType GetVariableDataType(int index)
        {
            return tables.variables[index].dataType;
        }

        public void AddLocalVariable(string name, DataType type, NodeType nodeType, string value, string functionName, bool isLoaded = false)
        {
            // 프로젝트 로드 과정에서 변수 추가하는 경우에는 Undo 처리 명령에서 제외.
            if (isLoaded)
            {
                variableFunctionManager.AddLocalVariable(name, type, nodeType, value, functionName);
                return;
            }

            // 변수 추가 팝업을 통해 변수를 추가하는 경우에는 Undo 처리 명령으로 간주.
            MCAddLocalVariableCommand command = new MCAddLocalVariableCommand(name, type, nodeType, value, functionName);
            MCUndoRedoManager.Instance.AddCommand(command);
        }

        public LeftMenuVariableItem GetLocalVariable(string name)
        {
            int index = GetLocalVariableIndexWithName(name);
            if (index.Equals(-1))
            {
                return null;
            }

            return GetLocalVariable(index);
        }

        public LeftMenuVariableItem GetLocalVariable(int index)
        {
            if (index < 0 || index >= tables.localVariables.Count)
            {
                return null;
            }

            //Debug.LogWarning($"index check {index} / variable count: {tables.variables.Count}");

            return tables.localVariables[index];
        }

        public LeftMenuVariableItem GetLocalVariableWithID(int variableID)
        {
            foreach (var localVariable in tables.localVariables)
            {
                if (localVariable.VariableID.Equals(variableID))
                {
                    return localVariable;
                }
            }

            return null;
        }

        public void UpdateLocalVariable()
        {
            variableFunctionManager.UpdateLocalVariable();
        }

        public string[] LocalVariableNameList
        {
            get
            {
                return variableFunctionManager.LocalVariableNameList;
            }
        }

        public string[] GetVariableNameListWithType(DataType targetType)
        {
            return variableFunctionManager.GetVariableNameListWithType(targetType);
        }

        public string[] GetLocalVariableNameListWithType(DataType targetType)
        {
            return variableFunctionManager.GetLocalVariableNameListWithType(targetType);
        }

        public int GetLocalVariableIndexWithName(string name)
        {
            return tables.GetLocalVariableIndexWithName(name);
        }

        public string GetLocalVariableValue(int index)
        {
            return tables.localVariables[index].value;
        }

        public DataType GetLocalVariableDataType(int index)
        {
            LeftMenuFunctionItem function = GetFunctionItemWithName(Utils.CurrentTabName);
            return function.FunctionData.variables[index].body.type;
        }


        public void AddFunction(string functionName, PROJECT.Input[] inputs, PROJECT.Output[] outputs, string description = "")
        {
            bool success = variableFunctionManager.AddFunction(functionName, inputs, outputs, description);
            if (success)
            {
                // Test.
                // 노드 추가하기 전에 현 상태 저장.
                //MCUndoRedoManager.Instance.RecordProject();
            }
        }

        public LeftMenuFunctionItem GetFunction(int index)
        {
            return MCFunctionTable.Instance.GetFunctionItemWithIndex(index);
        }

        public int GetFunctionIndexWithName(string name)
        {
            return MCFunctionTable.Instance.GetFunctionIndexWithName(name);
        }

        public LeftMenuFunctionItem GetFunctionItemWithName(string functionName)
        {
            return MCFunctionTable.Instance.GetFunctionItemWithName(functionName);
        }

        public LeftMenuFunctionItem GetFunctionItemWithID(int id)
        {
            return MCFunctionTable.Instance.GetFunctionItemWithID(id);
        }

        public int NewVariableID
        {
            get
            {
                return variableFunctionManager.NewVariableID;
            }
        }

        public void SubscribeVariableUpdate(UnityAction listener)
        {
            variableFunctionManager.SubscribeVariableUpdate(listener);
        }

        public void UnsubscribeVariableUpdate(UnityAction listener)
        {
            variableFunctionManager.UnsubscribeVariableUpdate(listener);
        }

        public void SubscribeLocalVariableUpdate(UnityAction listener)
        {
            variableFunctionManager.SubscribeLocalVariableUpdate(listener);
        }

        public void UnsubscribeLocalVariableUpdate(UnityAction listener)
        {
            variableFunctionManager.UnsubscribeLocalVariableUpdate(listener);
        }

        public void SubscribeFunctionUpdate(UnityAction<PROJECT.FunctionData> listener)
        {
            variableFunctionManager.SubscribeFunctionUpdate(listener);
        }

        public void UnsubscribeFunctionUpdate(UnityAction<PROJECT.FunctionData> listener)
        {
            variableFunctionManager.UnsubscribeFunctionUpdate(listener);
        }

        public void OnRobotTypeChanged(ProgramSettingWindow.RobotType robotType)
        {
            currentRobotType = robotType;

            // 로봇 뷰에 보여줄 RT 변경.
            robotViewRawImage.texture = robotType == ProgramSettingWindow.RobotType.MOCCA ? moccatRT : robotDudeRT;
            fullscreenRoibotViewRawImage.texture = robotType == ProgramSettingWindow.RobotType.MOCCA ? moccatRT : robotDudeRT;
        }

        public WorkspaceScrollRect GetWorkspaceScrollRect { get { return workspaceScrollRect; } }

        public DragPane GetDragPane { get { return workspaceScrollRect.GetComponent<DragPane>(); } }

        //public RectTransform LineParentTransform { get { return lineParentTransform; } }

        private List<MCNodeDrag> GetNodeDrags
        {
            get
            {
                List<MCNodeDrag> drags = new List<MCNodeDrag>();
                if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
                {
                    foreach (MCNode node in tables.locatedNodes)
                    {
                        drags.Add(node.GetComponent<MCNodeDrag>());
                    }
                }

                else if (CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
                {
                    LeftMenuFunctionItem function = GetFunctionItemWithName(Utils.CurrentTabName);
                    if (function != null)
                    {
                        foreach (MCNode node in function.LogicNodes)
                        {
                            drags.Add(node.GetComponent<MCNodeDrag>());
                        }
                    }
                }

                return drags;
            }
        }

        public List<MCNode> LocatedNodes
        {
            get
            {
                if (tabManager.currentTabs == null || tabManager.currentTabs.Count == 0)
                {
                    return null;
                }

                if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
                {
                    return tables.locatedNodes;
                }
                else
                {
                    MCFunctionTab tab = tabManager.CurrentTab as MCFunctionTab;
                    return tab.CurrentFunctionItem.LogicNodes;
                }
            }
        }

        public List<MCBezierLine> LocatedLines
        {
            get
            {
                if (CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
                {
                    return tables.locatedLines;
                }
                else
                {
                    return variableFunctionManager.tables.locatedLines;
                }
            }
        }

        public List<int> ProcessIDs
        {
            get
            {
                List<int> processIDs = new List<int>();
                var startNodes = StartNodes;
                foreach (MCNode start in startNodes)
                {
                    processIDs.Add(start.OwnedProcess.id);
                }

                return processIDs;
            }
        }

        public List<PROJECT.Process> Processes
        {
            get
            {
                List<PROJECT.Process> processes = new List<PROJECT.Process>();
                var startNodes = StartNodes;
                foreach (MCNode start in startNodes)
                {
                    processes.Add(start.OwnedProcess);
                }

                //Debug.Log($"Process Count: {processes.Count}");
                return processes;
            }
        }

        public Constants.OwnerGroup CurrentTabOwnerGroup { get { return tabManager.CurrentOwnerGroup; } }
        //public string CurrentTabName { get { return tabManager.CurrentTab.TabName; } }
        public static bool IsProjectNull { get { return MCProjectManager.ProjectDescription == null; } }



        internal void BuildMovementFollow()
        {
            var nodes = FindObjectsOfType<MCNode>();
            if (nodes == null || nodes.Length == 0) return;

            List<MCNode> checkNodes = new List<MCNode>();

            foreach (var node in nodes)
            {
                if (node.IsSelected == true)
                {
                    BuildMovementFollow(node, checkNodes);
                }
            }
        }

        private void BuildMovementFollow(MCNode parent, List<MCNode> checkNodes)
        {
            //자식노드 추가
            var nextNodes = parent.NextNodes;
            foreach (var child in nextNodes)
            {
                if (child == null) continue;

                if (checkNodes.Contains(child)) continue;
                checkNodes.Add(child);

                var movement = child.GetComponent<MCNodeMovementFollow>();
                if (movement == null) continue;
                if (child.IsSelected)
                {
                    movement.SetParent(null);
                    continue;
                }

                movement.SetParent(parent);

                BuildMovementFollow(child, checkNodes);
            }


            //인풋노드 추가
            int inputCount = parent.NodeInputCount;

            for (int i = 0; i < inputCount; i++)
            {
                var inputNode = parent.GetNodeInputWithIndex(i);
                if (inputNode == null) continue;

                if (inputNode.input == null) continue;
                if (inputNode.input.source == -1) continue;

                int sourceId = inputNode.input.source;

                var inputMcNode = FindNodeWithID(sourceId);
                if (inputMcNode == null) continue;

                if (checkNodes.Contains(inputMcNode)) continue;
                checkNodes.Add(inputMcNode);

                var movement = inputMcNode.GetComponent<MCNodeMovementFollow>();
                if (movement == null) continue;
                if (inputMcNode.IsSelected)
                {
                    movement.SetParent(null);
                    continue;
                }

                movement.SetParent(parent);

                BuildMovementFollow(inputMcNode, checkNodes);
            }

        }

    }
}