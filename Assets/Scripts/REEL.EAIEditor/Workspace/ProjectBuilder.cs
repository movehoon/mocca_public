using UnityEngine;
using System.Collections.Generic;

using REEL.PROJECT;
using System.IO;
using System;

namespace REEL.D2EEditor
{
    public class ProjectBuilder : MonoBehaviour
    {
        public string BuildToJson(List<MCNode> locatedNodes,
            List<LeftMenuVariableItem> locatedVariables,
            List<LeftMenuFunctionItem> locatedFunctions,
            LocalizationManager.Language language = LocalizationManager.Language.DEFAULT)
        {
            ProjectData project = new ProjectData();
            project.language = language;
            project.title = MCProjectManager.ProjectDescription.title;
            project.owner = MCProjectManager.ProjectDescription.ownerName;

            List<PROJECT.Process> processes = new List<PROJECT.Process>();
            //List<Node> nodes = new List<Node>();
            List<Node> variables = new List<Node>();
            List<FunctionData> functions = new List<FunctionData>();

            if (locatedVariables != null && locatedVariables.Count > 0)
            {
                // Add Variables or LIST.
                foreach (LeftMenuVariableItem variable in locatedVariables)
                {
                    Node node = new Node();
                    node.body = new Body();

                    node.type = variable.nodeType;
                    node.id = MCWorkspaceManager.Instance.NewVariableID;
                    node.inputs = null;

                    node.body.name = variable.VariableName;
                    node.body.type = variable.dataType;
                    node.body.value = variable.value;

                    node.outputs = null;
                    node.nexts = null;

                    variables.Add(node);
                }
            }

            if (locatedFunctions != null && locatedFunctions.Count > 0)
            {
                List<VariableInfomation> variableReferences = new List<VariableInfomation>();
                // Add Functions.
                foreach (LeftMenuFunctionItem function in locatedFunctions)
                {
                    //Utils.LogGreen($"function.Inputs.Count: {function.Inputs.Count}");
                    // 함수에서 변수를 참조하는 지 확인.
                    foreach (var node in function.FunctionData.nodes)
                    {
                        if (node.type == NodeType.GET || node.type == NodeType.SET)
                        {
                            // GET/SET 노드가 지역 변수를 참조하는 경우에는 건너뜀.
                            if (node.body.isLocalVariable == true)
                            {
                                continue;
                            }

                            var variable = MCWorkspaceManager.Instance.GetVariable(node.body.name);
                            VariableInfomation variableInfo = null;
                            if (variable != null)
                            {
                                variableInfo = new VariableInfomation(variable);
                            }

                            else // 프로젝트에서 노드를 복사한 경우에는 클립보드에 복사된 정보에서 변수 찾기.
                            {
                                ClipBoardManager.ClipBoardContent content = ClipBoardManager.Instance.PopContent();
                                foreach (var info in content.copiedVariables)
                                {
                                    if (info.name.Equals(node.body.name))
                                    {
                                        variableInfo = new VariableInfomation(info);
                                        break;
                                    }
                                }
                            }

                            if (variableInfo != null && variableReferences.Contains(variableInfo) == false)
                            {
                                variableReferences.Add(variableInfo);
                            }
                        }
                    }

                    // 함수에서 변수를 참조하는 경우, 변수 정보 추가.
                    if (variableReferences.Count > 0)
                    {
                        function.FunctionData.SetVariableReferences(variableReferences.ToArray());
                    }

                    // 함수 데이터 추가.
                    functions.Add(function.FunctionData);
                }
            }

            Dictionary<int, PROJECT.Process> processesByID = new Dictionary<int, PROJECT.Process>();

            if (locatedNodes != null && locatedNodes.Count > 0)
            {
                // Add Nodes.
                // 프로세스 ID 값을 기준으로 프로세스 별로 노드 추가.
                foreach (MCNode node in locatedNodes)
                {
                    if (processesByID.TryGetValue(node.OwnedProcess.id, out PROJECT.Process outProcess))
                    {
                        Node[] temp = new Node[outProcess.nodes.Length];
                        for (int ix = 0; ix < temp.Length; ++ix)
                        {
                            temp[ix] = outProcess.nodes[ix];
                        }

                        outProcess.nodes = new Node[outProcess.nodes.Length + 1];
                        for (int ix = 0; ix < temp.Length; ++ix)
                        {
                            outProcess.nodes[ix] = temp[ix];
                        }

                        outProcess.nodes[outProcess.nodes.Length - 1] = node.MakeNode();
                    }
                    else
                    {
                        outProcess = new PROJECT.Process();
                        outProcess.name = node.OwnedProcess.name;
                        outProcess.id = node.OwnedProcess.id;
                        outProcess.priority = node.OwnedProcess.priority;
                        outProcess.nodes = new Node[1] { node.MakeNode() };

                        processesByID.Add(outProcess.id, outProcess);
                    }
                }
            }
            
            if (processesByID.Count > 0)
            {
                // 프로세스 추가.
                foreach (var process in processesByID)
                {
                    processes.Add(process.Value);
                }

                // 시작 노드만 있는 프로세스는 제거.
                // 멀티 프로세스일 때만 확인. 
                // Start 노드가 끊어지는 문제가 있어 비활성화함.
                //if (processes.Count > 1)
                //{
                //    List<int> deleteProcessKeys = new List<int>();
                //    foreach (var process in processesByID)
                //    {
                //        if (process.Value.nodes.Length == 1 && process.Value.nodes[0].type == NodeType.START)
                //        {
                //            deleteProcessKeys.Add(process.Key);
                //            continue;
                //        }
                //    }

                //    foreach (var key in deleteProcessKeys)
                //    {
                //        processes.Remove(processesByID[key]);
                //    }
                //}
            }

            // 프로젝트에 데이터 설정.
            project.variables = variables.ToArray();
            project.processes = processes.ToArray();
            project.functions = functions.ToArray();

            // 주석 정보 추가.
            if (MCCommentManager.Instance.ProjectCommentInfo != null 
                && MCCommentManager.Instance.ProjectCommentInfo.Length > 0)
            {
                project.comments = MCCommentManager.Instance.ProjectCommentInfo;
            }

            string jsonData = string.Empty;
            try
            {
                // JSON 변환.
                //jsonData = JsonUtility.ToJson(project, false);
                jsonData = Utils.CompileToJson(project);
            }
            catch (Exception ex)
            {
                Utils.LogRed($"[ProjectBuilder.BuildToJson] Failed to Compile: {ex.ToString()}");
                MessageBox.ShowYesNo("[ID_LOGIC_DATA_COMPILE_FAILED_REQUEST]", (res) => //local 추가 완료
                {
                    if (res == true)
                    {
                        Slack.SendException("[ID_LOGIC_DATA_COMPILE_FAILED]", MCProjectManager.ProjectDescription, ex); //local 추가 완료
                        MessageBox.Show("[ID_SEND_MSG_TO_DEVELOPER]"); // local 추가 완료
                    }
                });
            }

            return jsonData;
        }
    }
}