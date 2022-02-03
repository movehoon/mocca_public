using REEL.PROJECT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.D2EEditor
{
    public class MCPlayStateManager : Singleton<MCPlayStateManager>
    {
        public Button[] pauseButtons;

        public Sprite pauseButtonImage;

        public Sprite resumeButtonImage;

        public SimulationWindow simulationWindow;

        private bool isSimulation = false;
        public bool IsSimulation
        {
            get { return isSimulation; }
            set
            {
                if (Utils.GetTabManager() == null || Utils.GetTabManager().CurrentTab == null)
                {
                    return;
                }

                //Debug.LogWarning(string.Format("isSimulation {0}", isSimulation));
                isSimulation = value;
                MCWorkspaceManager.Instance.SetWorkspaceActive(!isSimulation);
                SetAllUnSelected();
                PropertyWindowManager.Instance.TurnOffAll();

                OnSimulationStateChanged?.Invoke(isSimulation);
            }
        }

        [SerializeField]
        private DialogTMPInputField dialogInputField;

        [SerializeField]
        private DialogTMPInputField fullscreenDialogInputField;

        private Action<bool> OnSimulationStateChanged;
        private Action<bool> OnPauseStateChanged;

        private bool isPaused = false;
        public bool PauseState { get { return isPaused; } }

        private WaitForSeconds waitForHighlight = new WaitForSeconds(0.1f);
        private WaitForSeconds waitMillisecond = new WaitForSeconds(0.01f);
        private WaitForSeconds wait = new WaitForSeconds(0.2f);
        private WaitForSeconds waitForOnSecond = new WaitForSeconds(1f);

        private void OnEnable()
        {
            IsSimulation = false;

            if (simulationWindow == null)
            {
                simulationWindow = FindObjectOfType<SimulationWindow>();
            }
        }

        private void Update()
        {
            // 실행 상태가 아닌데 입력 창이 켜져있는 경우 끄도록 처리.
            if (IsSimulation == false)
            {
                SetDialogState(false);
            }
        }

        private List<Process> processes = new List<Process>();
        public void AddNewProcess(Process process)
        {
            if (processes.Count == 0)
            {
                processes.Add(process);
                return;
            }

            for (int ix = 0; ix < processes.Count; ++ix)
            {
                if (processes[ix].GetPID() == process.GetPID())
                {
                    return;
                }
            }

            processes.Add(process);
        }

        private int processEndCount = 0;
        public void OnProcessEnd(int processID)
        {
            //Debug.LogWarning($"OnProcessEnd; processes.Count: {processes.Count}; processEndCount: {processEndCount}");

            if (processes.Count == 1)
            {
                //Debug.LogWarning($"processes.Count == 1");

                MCPlayStateManager.Instance.IsSimulation = false;
                processEndCount = 0;
            }
            else
            {
                //Debug.LogWarning($"processes.Count Else");
                ++processEndCount;
                //Debug.LogWarning($"processEndCount: {processEndCount }, processes.Count: {processes.Count}");
                if (processEndCount >= processes.Count)
                {
                    IsSimulation = false;
                    processEndCount = 0;
                }
            }
        }

        public void ResetProcessInfomation()
        {
            processEndCount = 0;
            processes.Clear();
        }

        public void StartCheckDialogueState()
        {
            StartCoroutine(CheckDialogState());
            StartCoroutine(CheckHighlightState());
        }

        public void SetSimulationMode(bool isSimulation)
        {
            this.isSimulation = isSimulation;
        }

        private void PauseProject()
        {
            isPaused = true;
            Player.Instance.Pause();
        }

        private void ResumeProject()
        {
            isPaused = false;
            Player.Instance.Resume();
            OnPauseStateChanged?.Invoke(false);
        }

        public void StopProject()
        {
            isPaused = false;
            foreach (Button button in pauseButtons)
            {
                button.transform.parent.gameObject.SetActive(false);
                if (button.GetComponent<Image>().sprite != null)
                {
                    button.GetComponent<Image>().sprite = resumeButtonImage;
                }

                button.transform.parent.GetComponentInChildren<Text>().text = resumeString;
            }

            OnPauseStateChanged?.Invoke(false);
        }

        private readonly string pauseString = "일시정지";
        private readonly string resumeString = "다시실행";
        public void OnPauseOrResumeButtonClicked()
        {
            if (isPaused)
            {
                foreach (Button button in pauseButtons)
                {
                    button.transform.parent.gameObject.SetActive(false);
                    if (button.GetComponent<Image>().sprite != null)
                    {
                        button.GetComponent<Image>().sprite = resumeButtonImage;
                    }

                    button.transform.parent.GetComponentInChildren<Text>().text = resumeString;
                }

                ResumeProject();
                OnPauseStateChanged?.Invoke(false);
            }

            else
            {
                foreach (Button button in pauseButtons)
                {
                    button.transform.parent.gameObject.SetActive(true);
                    if (button.GetComponent<Image>().sprite != null)
                    {
                        button.GetComponent<Image>().sprite = pauseButtonImage;
                    }

                    button.transform.parent.GetComponentInChildren<Text>().text = pauseString;
                }

                PauseProject();
                OnPauseStateChanged?.Invoke(true);
            }
        }

        private IEnumerator CheckHighlightState()
        {
            while (MCPlayStateManager.Instance.IsSimulation)
            {
                //yield return waitForHighlight;

                int processCount = processes.Count;
                if (processes.Count == 0)
                {
                    processes = FindObjectsOfType<Process>().ToList();
                }

                foreach (Process process in processes)
                {
                    // 현재 프로젝트가 카메라를 사용하는지 확인
                    // 현재 실행 중인 노드가 카메라 사용 노드가 아닌 경우에만.
                    ShowCamPopupWhenNextNodeIsCamUsingNode(process.CurrentNode);

                    if (process.CurrentNode == null || process.CurrentNode.type == NodeType.START)
                    {
                        continue;
                    }

                    MCNode node = MCWorkspaceManager.Instance.FindNodeWithID(process.CurrentNode.id, process.GetPID());
                    if (node != null && !node.IsSelected)
                    {
                        Utils.GetTables().SetAllUnSelected(process.GetPID());
                        StartCoroutine(DelayFunctionCall(() =>
                        {
                            if (MCPlayStateManager.Instance.IsSimulation == false)
                            {
                                return;
                            }

                            node = MCWorkspaceManager.Instance.FindNodeWithID(process.CurrentNode.id, process.GetPID());
                            if (node != null)
                            {
                                node.IsSelected = true;
                            }
                        }, 0.1f));
                    }
                }

                //yield return waitForHighlight;
                yield return waitMillisecond;
            }

            StartCoroutine(DelayFunctionCall(SetAllUnSelected, 0.2f));

            // 프로젝트 실행 종료하고 일정시간(1초) 후에 웹캠 창 닫기.
            yield return waitForOnSecond;
            MCEditorManager.Instance.CloseAllPopups();
        }

        private IEnumerator CheckDialogState()
        {
            // 프로젝트 실행 시작할 때 로그 메시지 출력.
            //LogWindow.Instance.PrintWarning("MOCCA Studio", $"<프로젝트 [{CurrentTabName}] 시작>");

            // 튜토리얼 도중 프로젝트 실행중에 강제로 튜토리얼 종료 되면 
            // CurrentTabName 이 날아 가는경우가 있어서 미리 tabName에 지정함 - kjh
            string tabName = Utils.CurrentTabName;

            LogWindow.Instance.PrintLog("MOCCA Studio", $"<프로젝트 [{tabName}] 시작>");

            while (MCPlayStateManager.Instance.IsSimulation)
            {
                yield return waitForHighlight;

                if (processes.Count == 1)
                {
                    bool shouldOn = processes[0].CurrentNode != null &&
                        (processes[0].CurrentNode.type == NodeType.SPEECH_REC ||
                        processes[0].CurrentNode.type == NodeType.DIALOGUE ||
                        processes[0].CurrentNode.type == NodeType.CHATBOT_PIZZA_ORDER);

                    SetDialogState(shouldOn);
                }

                else if (processes.Count > 1)
                {
                    bool shouldOn = false;
                    foreach (Process process in processes)
                    {
                        if (process.CurrentNode != null &&
                            (process.CurrentNode.type == NodeType.SPEECH_REC ||
                            process.CurrentNode.type == NodeType.DIALOGUE
                            || process.CurrentNode.type == NodeType.CHATBOT_PIZZA_ORDER))
                        {
                            shouldOn = true;
                        }
                    }

                    SetDialogState(shouldOn);
                }
            }

            // 프로젝트 종료할 때 로그 메시지 출력.
            //LogWindow.Instance.PrintWarning("MOCCA Studio", $"<프로젝트 [{CurrentTabName}] 종료>");
            LogWindow.Instance.PrintLog("MOCCA Studio", $"<프로젝트 [{tabName}] 종료>");

            yield return waitForOnSecond;
            TutorialManager.SendEvent(Tutorial.CustomEvent.ProjectStopped, "Project Stopped");
        }

        private void SetDialogState(bool shouldOn)
        {
            if (shouldOn == true)
            {
                // 전체화면 모드일 때.
                if (simulationWindow.IsFullScreen)
                {
                    fullscreenDialogInputField.interactable = true;
                    fullscreenDialogInputField.Select();
                }

                else
                {
                    dialogInputField.interactable = true;
                    dialogInputField.Select();
                }
            }
            else
            {
                if (dialogInputField.interactable == true)
                {
                    dialogInputField.interactable = false;
                }

                if (fullscreenDialogInputField.interactable == true)
                {
                    fullscreenDialogInputField.interactable = false;
                }
            }
        }

        IEnumerator DelayFunctionCall(Action function, float time)
        {
            yield return new WaitForSeconds(time);
            function();
        }

        private void ProcessDialogState(int processID, int nodeID, MCNode node)
        {
            // 현재 실행 블록이 Speech_Rec인 경우에만 입력 받도록 설정.
            if (node.nodeData.type == NodeType.SPEECH_REC)
            {
                // 전체화면 모드일 때.
                if (simulationWindow.IsFullScreen)
                {
                    fullscreenDialogInputField.interactable = true;
                    fullscreenDialogInputField.Select();
                }

                else
                {
                    dialogInputField.interactable = true;
                    dialogInputField.Select();
                }
            }
            else
            {
                if (dialogInputField.interactable == true)
                {
                    dialogInputField.interactable = false;
                }

                if (fullscreenDialogInputField.interactable == true)
                {
                    fullscreenDialogInputField.interactable = false;
                }
            }
        }

        private void ShowCamPopupWhenNextNodeIsCamUsingNode(Node node)
        {
            // 현재 프로젝트가 카메라를 사용하는지 확인
            // 현재 실행 중인 노드가 카메라 사용 노드가 아닌 경우에만.
            if (node == null)
            {
                return;
            }

            if (MCWorkspaceManager.Instance.isCurrentProjectUsingCameraNode == true && Utils.IsCameraUsingType(node.type) == false)
            {
                // Get Next Node.
                int nextNodeID = node.nexts[0].next;
                MCNode nextNode = MCWorkspaceManager.Instance.FindNodeWithID(nextNodeID);

                // 다음번 실행 노드가 카메라 사용 노드인 경우, 팜업 열기. 
                if (nextNode != null && Utils.IsCameraUsingType(nextNode.nodeData.type) == true)
                {
                    if (simulationWindow.IsFullScreen == true)
                    {
                        MCEditorManager.Instance.GetPopup(MCEditorManager.PopupType.WebcamPopupFullScreen).ShowPopup();
                    }
                    else
                    {
                        MCEditorManager.Instance.GetPopup(MCEditorManager.PopupType.WebcamPopup).ShowPopup();
                    }
                }
            }
        }

        public void SubscribeSimulationStateChanged(Action<bool> function)
        {
            OnSimulationStateChanged += function;
        }

        public void UnSubscribeSimulationStateChanged(Action<bool> function)
        {
            OnSimulationStateChanged -= function;
        }

        public void SubscribeOnPauseStateChanged(Action<bool> listener)
        {
            OnPauseStateChanged += listener;
        }

        public void UnSubscribeOnPauseStateChanged(Action<bool> listener)
        {
            OnPauseStateChanged -= listener;
        }

        public void SetAllUnSelected()
        {
            if (IsProjectNull)
            {
                return;
            }

            Utils.GetTables().SetAllUnSelected();
        }

        public static bool IsProjectNull { get { return MCProjectManager.ProjectDescription == null; } }
    }
}