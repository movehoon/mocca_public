using UnityEngine;
using UnityEngine.UI;
using REEL.PROJECT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;

namespace REEL.D2EEditor
{
    [RequireComponent(typeof(MCNodeDrag), typeof(MCNodeSelect), typeof(MCNodeTooltip))]
    [RequireComponent(typeof(MCNodePosition))]
    public class MCNode : MonoBehaviour
    {
        public Node nodeData;
        public GameObject selectedGameObject;

        [SerializeField]
        protected MCNodeInput[] inputs;

        [SerializeField]
        protected MCNodeOutput[] outputs;

        [SerializeField]
        protected MCNodeNext[] nexts;

        protected bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                selectedGameObject.SetActive(value);
            }
        }

        [SerializeField]
        protected bool dontDestroy = false;
        public bool DontDestroy
        {
            get { return dontDestroy; }
            set { dontDestroy = value; }
        }

        // 이 노드가 소속된 프로세스.
        public PROJECT.Process OwnedProcess;

        [SerializeField]
        protected Image breakPoint;

        // Break Point 설정 여부.
        protected bool hasBreakPoint = false;

        protected bool hasInitialized = false;

        // 선이 연결되거나 해제되는 등 블록의 상태가 변경되면 호출되는 이벤트.
        protected UnityEvent OnNodeStateChanged;

        //// 이 노드가 소속된 프로세스 ID.
        //public int OwnedProcessID { get; set; }

        //// 이 노드가 소속된 프로세스 우선순위.
        //public int OwnedProcessPriority { get; set; }

        private bool dontUnlink = false;
        public bool DontUnlink
        {
            get { return dontUnlink; }
            set { dontUnlink = value; }
        }


        private RectTransform rectTransform;
        public RectTransform RefRect
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                return rectTransform;
            }
        }

        public MCNodePosition mcNodePosition = null;

        protected virtual void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            if (NodeID == 0)
            {
                NodeID = Utils.NewGUID;
                nodeData.id = NodeID;
            }

            if (nodeData.type == NodeType.START)
            {
                NodeID = 0;
                nodeData.id = 0;
            }

            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                inputs[ix].SetInputIndex(ix);
            }

            for (int ix = 0; ix < outputs.Length; ++ix)
            {
                outputs[ix].SetOutputIndex(ix);
            }

            if (OnNodeStateChanged is null)
            {
                OnNodeStateChanged = new UnityEvent();
            }

            if (mcNodePosition == null)
            {
                mcNodePosition = GetComponent<MCNodePosition>();
                if (mcNodePosition == null)
                {
                    mcNodePosition = gameObject.AddComponent<MCNodePosition>();
                }
            }

            OwnedProcess = new PROJECT.Process();
            hasInitialized = true;


            //블록이 생성 될때 타이틀 글자번역을 위해 추가 - kjh
            //Title Text 노드가 없거나 이미 추가 되어 있으면 추가 안함
            var t = transform.Find("Title Area/Title Text (TMP)");
            if (t != null)
            {
                LocalizationOnEnable component = transform.GetComponent<LocalizationOnEnable>();
                if (component == null)
                    transform.gameObject.AddComponent<LocalizationOnEnable>();

            }

            var tmpInputFields = GetComponentsInChildren<TMPro.TMP_InputField>();
            if (tmpInputFields.Length > 0)
            {
                foreach (var tmpInputField in tmpInputFields)
                {
                    tmpInputField.onSelect.AddListener(OnInputFieldSelected);
                }
            }

            //positionOffetToTopLeft = TopLeftWorldPosition - transform.position;

            // 에디터 모드에서 노드 스크린샷 찍을 떄 사용하는 컴포넌트 추가.
#if UNITY_EDITOR
            if (gameObject.GetComponent<MCNodeImageCreator>() == null)
            {
                gameObject.AddComponent<MCNodeImageCreator>();
            }
#endif
        }

        private EventSystem eventSystem;
        private PointerEventData data;
        protected virtual void OnInputFieldSelected(string input)
        {
            if (isSelected == false)
            {
                MCWorkspaceManager.Instance.SetOneSelected(this);
                if (eventSystem == null)
                {
                    eventSystem = FindObjectOfType<EventSystem>();
                }

                if (data == null)
                {
                    data = new PointerEventData(eventSystem);
                }

                data.position = UnityEngine.Input.mousePosition;
                GetComponent<MCNodeSelect>().OnPointerDown(data);
            }
        }

        protected virtual void OnDisable()
        {
            var tmpInputFields = GetComponentsInChildren<TMPro.TMP_InputField>();
            if (tmpInputFields.Length > 0)
            {
                foreach (var tmpInputField in tmpInputFields)
                {
                    if (tmpInputField != null)
                    {
                        tmpInputField.onSelect.RemoveListener(OnInputFieldSelected);
                    }
                }
            }
        }

        protected virtual void Update()
        {
            if (breakPoint != null && isSelected && UnityEngine.Input.GetKeyDown(KeyCode.F9))
            {
                hasBreakPoint = !hasBreakPoint;
                breakPoint.gameObject.SetActive(hasBreakPoint);
            }
        }

        public virtual Node MakeNode()
        {
            //GetComponent<RectTransform>().position;
            //저장시 transform.localPosition 으로 저장함
            nodeData.nodePosition = transform.localPosition;

            UpdateNodeData();
            return nodeData;
        }

        public virtual void SetData(Node node)
        {
            // nodeData.inputs.
            nodeData.inputs = new PROJECT.Input[node.inputs.Length];
            if (node.inputs != null && node.inputs.Length == inputs.Length)
            {
                for (int ix = 0; ix < inputs.Length; ++ix)
                {
                    nodeData.inputs[ix] = new PROJECT.Input();
                    nodeData.inputs[ix].id = node.inputs[ix].id;
                    nodeData.inputs[ix].type = node.inputs[ix].type;
                    nodeData.inputs[ix].source = node.inputs[ix].source;
                    nodeData.inputs[ix].subid = node.inputs[ix].subid;
                    nodeData.inputs[ix].default_value = node.inputs[ix].default_value;

                    //Debug.Log($"[MCSayNode.SetData] node.inputs[ix].default_value: {node.inputs[ix].default_value}");

                    if (inputs[ix].HasLine)
                    {
                        nodeData.inputs[ix].default_value = string.Empty;
                    }
                }
            }

            // nodeData.outputs.
            if (node.outputs != null && node.outputs.Length > 0)
            {
                nodeData.outputs = new Output[node.outputs.Length];
                for (int ix = 0; ix < node.outputs.Length; ++ix)
                {
                    nodeData.outputs[ix] = new Output();
                    nodeData.outputs[ix].id = node.outputs[ix].id;
                    nodeData.outputs[ix].type = node.outputs[ix].type;
                    nodeData.outputs[ix].value = node.outputs[ix].value;

                    //outputs[ix].SetOutputData(node.outputs[ix]);
                }
            }

            // nodeData.nexts
            if (node.nexts != null && node.nexts.Length > 0)
            {
                nodeData.nexts = new Next[node.nexts.Length];
                for (int ix = 0; ix < node.nexts.Length; ++ix)
                {
                    nodeData.nexts[ix] = new Next();
                    nodeData.nexts[ix].next = node.nexts[ix].next;
                    nodeData.nexts[ix].value = node.nexts[ix].value;

                    //if (ix < nexts.Length && nexts[ix] != null)
                    //{
                    //    nexts[ix].next.next = node.nexts[ix].next;
                    //    nexts[ix].next.value = node.nexts[ix].value;
                    //}
                }
            }

            if (node.body != null)
            {
                nodeData.body = new Body();
                nodeData.body.name = node.body.name;
                nodeData.body.isLocalVariable = node.body.isLocalVariable;
                nodeData.body.type = node.body.type;
                nodeData.body.value = node.body.value;
            }

            // Test.
            // 중단점(BreakPoint) 설정 옵션.
            hasBreakPoint = node.hasBreakPoint;
            if (hasBreakPoint == true && breakPoint != null)
            {
                breakPoint.gameObject.SetActive(hasBreakPoint);
            }
        }

        // should be implemented in derived class.
        //protected abstract void UpdateNodeData();
        protected virtual void UpdateNodeData()
        {
            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                nodeData.inputs[ix] = new PROJECT.Input();
                nodeData.inputs[ix].id = inputs[ix].input.id;
                nodeData.inputs[ix].type = inputs[ix].input.type;
                nodeData.inputs[ix].source = inputs[ix].input.source;
                nodeData.inputs[ix].subid = inputs[ix].input.subid;
                nodeData.inputs[ix].name = inputs[ix].input.name;

                // Test.
                //if (inputs[ix].input.type == DataType.NUMBER)
                //{
                //    double retNumber = 0;
                //    double.TryParse(inputs[ix].input.default_value, out retNumber);
                //    nodeData.inputs[ix].default_value = retNumber.ToString();
                //}
                //else
                //{
                //    nodeData.inputs[ix].default_value = inputs[ix].input.default_value;
                //}

                nodeData.inputs[ix].default_value = inputs[ix].input.default_value;

                if (inputs[ix].HasLine)
                {
                    nodeData.inputs[ix].default_value = string.Empty;
                }
            }

            nodeData.outputs = new Output[outputs.Length];
            for (int ix = 0; ix < outputs.Length; ++ix)
            {
                nodeData.outputs[ix] = new Output();
                nodeData.outputs[ix].id = outputs[ix].output.id;
                nodeData.outputs[ix].type = outputs[ix].output.type;
                nodeData.outputs[ix].value = outputs[ix].output.value;
                nodeData.outputs[ix].name = outputs[ix].output.name;
            }

            nodeData.nexts = new Next[nexts.Length];
            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                nodeData.nexts[ix] = new Next();
                nodeData.nexts[ix].value = nexts[ix].next.value;
                nodeData.nexts[ix].next = nexts[ix].next.next;
            }

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;
        }

        // 프로세스 정보가 초기화되면 호출되는 메소드.
        // ex) Start 노드에 연결됐던 노드가 Start 노드에서 연결이 해제됐을 때.
        public static Dictionary<int, MCNode> visitedNode = new Dictionary<int, MCNode>();
        public void OnProcessInformationReset()
        {
            OwnedProcess = new PROJECT.Process();
            visitedNode.Add(NodeID, this);
            foreach (MCNodeNext next in nexts)
            {
                if (next.next.next > -1)
                {
                    MCNode nextNode = MCWorkspaceManager.Instance.FindNodeWithID(next.next.next);

                    // 노드가 null이 아니고, 이미 방문한 노드가 아닐 경우에만 함수 실행하도록.
                    // 블록이 순환 형태로 연결되면, Stackoverflow 발생해서 예외처리함.
                    if (nextNode != null && visitedNode.ContainsKey(nextNode.NodeID) == false)
                    {
                        nextNode.OnProcessInformationReset();
                    }
                }
            }
        }

        public int NodeID { get; set; }

        public List<MCNode> NextNodes
        {
            get
            {
                List<MCNode> nextNodes = new List<MCNode>();
                foreach (MCNodeNext nodeNext in nexts)
                {
                    int id = nodeNext.next.next;
                    nextNodes.Add(MCWorkspaceManager.Instance.FindNodeWithID(id));
                }

                return nextNodes;
            }
        }

        public IEnumerable<MCNode> NextMCNodes
        {
            get
            {
                return nexts.Where(n => n.Node != null).Select(x => x.Node);
            }
        }

        public MCNodeOutput GetNodeOutputWithIndex(int index)
        {
            if (outputs == null || outputs.Length == 0)
            {
                return null;
            }

            if (index < 0 || index >= outputs.Length)
            {
                return null;
            }

            return outputs[index];
        }

        public int GetNodeOutputIndexWithSocket(MCNodeSocket socket)
        {
            if (outputs == null || outputs.Length == 0)
            {
                return -1;
            }

            for (int ix = 0; ix < outputs.Length; ++ix)
            {
                MCNodeOutput output = outputs[ix];
                if (output.gameObject.GetInstanceID().Equals(socket.gameObject.GetInstanceID()))
                {
                    return ix;
                }
            }

            return -1;
        }

        public int NodeOutputCount
        {
            get { return outputs.Length; }
        }

        public int NodeInputCount
        {
            get { return inputs.Length; }
        }

        public MCNodeInput GetNodeInputWithIndex(int index)
        {
            if (inputs == null || inputs.Length == 0)
            {
                return null;
            }

            if (index < 0 || index >= inputs.Length)
            {
                return null;
            }

            return inputs[index];
        }

        public int GetNodeInputIndexWithSocket(MCNodeSocket socket)
        {
            if (inputs == null || inputs.Length == 0)
            {
                return -1;
            }

            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                MCNodeInput input = inputs[ix];
                if (input.gameObject.GetInstanceID().Equals(socket.gameObject.GetInstanceID()))
                {
                    return ix;
                }
            }

            return -1;
        }

        public int NodeNextCount
        {
            get { return nexts.Length; }
        }

        public MCNodeNext GetNodeNextWithIndex(int index)
        {
            if (index < 0 || index >= nexts.Length)
            {
                return null;
            }

            return nexts[index];
        }

        public int GetNodeNextIndexWithSocket(MCNodeSocket socket)
        {
            if (nexts == null || nexts.Length == 0)
            {
                return -1;
            }

            for (int ix = 0; ix < nexts.Length; ++ix)
            {
                MCNodeNext next = nexts[ix];
                if (next.gameObject.GetInstanceID().Equals(socket.gameObject.GetInstanceID()))
                {
                    return ix;
                }
            }

            return -1;
        }

        public void SubscribeOnNodeStateChanged(UnityAction func)
        {
            OnNodeStateChanged.AddListener(func);
        }

        public void UnsubscribeOnNodeStateChanged(UnityAction func)
        {
            OnNodeStateChanged.RemoveListener(func);
        }

        public virtual void NodeStateChanged()
        {
            OnNodeStateChanged?.Invoke();
        }

        public virtual void OnDoubleClicked()
        {
            //튜토리얼 진행때는 더블클릭 안먹히도록 처리 -kjh
            if(TutorialManager.IsPlaying == true) return;

            if(OwnedProcess.id != 0 && Utils.IsNullOrEmptyOrWhiteSpace(OwnedProcess.name) == false)
            {
                MCWorkspaceManager.Instance.ExecuteProjectFromNode(NodeID);
            }
        }

        public void HighlightLine()
        {

            if (outputs != null)
            {
                foreach (var output in outputs)
                {
                    if (output.HasLine == true)
                    {
                        output.HighlightOn();
                    }
                }
            }

            if (nexts != null)
            {
                foreach (var next in nexts)
                {
                    if (next.HasLine)
                    {
                        next.HighlightOn();
                    }
                }
            }
        }



        public void DisableUserControl()
        {
            EnableInputfield(false);
            EnableDropDown(false);
            EnableSocket(false);
            EnableUnlink(false);
        }


        public void EnableUnlink(bool isOn)
        {
            DontUnlink = !isOn;
		}

        public void EnableInputfield(bool isOn)
        {
            var inputFields = gameObject.GetComponentsInChildren<TMPro.TMP_InputField>();
           
            foreach( var field in inputFields)
            {
                field.enabled = isOn;
            }
		}


        public void EnableDropDown(bool isOn)
        {
            var dropDowns = gameObject.GetComponentsInChildren<TMPro.TMP_Dropdown>();

            foreach(var dropDown in dropDowns)
            {
                dropDown.enabled = isOn;
            }
        }


        public void EnableSocketType(REEL.D2EEditor.MCNodeSocket.SocketType socketType)
        {
            var sockets = gameObject.GetComponentsInChildren<MCNodeSocket>();

            foreach(var socket in sockets)
            {
				socket.enabled = socket.socketType == socketType;

                var mbs = socket.GetComponentsInChildren<MagneticBound>();

                foreach(var mb in mbs)
                {
                    mb.enabled = socket.enabled;
                }
            }
        }

        public void EnableSocket(bool isOn)
        {
            var sockets = gameObject.GetComponentsInChildren<MCNodeSocket>();

            foreach(var socket in sockets)
            {
                socket.enabled = isOn;
            }

            var mbs = gameObject.GetComponentsInChildren<MagneticBound>();

            foreach(var mb in mbs)
            {
                mb.enabled = isOn;
            }
        }

        public void EnableSocket(bool isOnLeft,bool isOnRight)
        {
            var sockets = gameObject.GetComponentsInChildren<MCNodeSocket>();

            foreach(var socket in sockets)
            {
                socket.enabled = isOnLeft;
            }

            var mbs = gameObject.GetComponentsInChildren<MagneticBound>();

            foreach(var mb in mbs)
            {
                mb.enabled = isOnLeft;
            }
        }


    }
}