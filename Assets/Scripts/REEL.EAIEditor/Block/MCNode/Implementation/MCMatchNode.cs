using REEL.PROJECT;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace REEL.D2EEditor
{
    public class MCMatchNode : MCNode
    {
        [SerializeField] private float blockOriginYSize = 110f;
        [SerializeField] private float ySizeOffset = 24f;
        [SerializeField] private float socketOriginYPos = -8f;
        private bool isInputConnected = false;

        private List<TMP_InputField> inputFieldPrefabs = new List<TMP_InputField>();
        private List<MCNodeNext> nextSocketPrefabs = new List<MCNodeNext>();

        [SerializeField] private GameObject inputPrefab;
        [SerializeField] private GameObject nextSocketPrefab;
        [SerializeField] private RectTransform matchElems;
        [SerializeField] private RectTransform[] lastSockets;
        [SerializeField] private RectTransform epLeft;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            nodeData.body = null;
            nodeData.outputs = null;

            InitMatchBlock();

            GetNodeInputWithIndex(0).SubscribeOnLineConnected(OnLineConnected);
            GetNodeInputWithIndex(0).SubscribeOnLineDelete(OnLineDeleted);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GetNodeInputWithIndex(0).UnSubscribeOnLineConnected(OnLineConnected);
            GetNodeInputWithIndex(0).UnSubscribeOnLineDelete(OnLineDeleted);
        }

        private void OnLineConnected(MCBezierLine line) { isInputConnected = true; }

        private void OnLineDeleted() { isInputConnected = false; }

        protected override void UpdateNodeData()
        {
            nodeData.inputs = new PROJECT.Input[inputs.Length];
            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                nodeData.inputs[ix] = new PROJECT.Input();
                nodeData.inputs[ix].id = inputs[ix].input.id;
                nodeData.inputs[ix].type = inputs[ix].input.type;
                nodeData.inputs[ix].source = inputs[ix].input.source;
                nodeData.inputs[ix].subid = inputs[ix].input.subid;
                nodeData.inputs[ix].default_value = inputs[ix].input.default_value;

                if (inputs[ix].HasLine)
                {
                    nodeData.inputs[ix].default_value = string.Empty;
                }
            }

            nodeData.nexts = new Next[nexts.Length];
            for (int i = 0; i < nexts.Length; i++)
            {
                nodeData.nexts[i] = nexts[i].next;
                if (i != nexts.Length - 1)
                    nodeData.nexts[i].value = inputFieldPrefabs[i].text;
                else
                    nodeData.nexts[i].value = "DEFAULT";
            }

            // Test.
            // BreakPoint 옵션 설정.
            // 0:설정 안됨, 1:설정됨.
            nodeData.hasBreakPoint = hasBreakPoint;
        }

        private void InitMatchBlock()
        {
            for (int i = 0; i < 2; i++) //처음 기본 생성 2개.
                AddMatchElem();
        }

        private void SetBlockSizeAndSocketPos(int count)
        {
            Vector2 blockSize = SetBlockSize(count - 2); //블록사이즈 설정
            SetFirstSocketPos(blockSize); //첫번째 소켓 위치 설정
        }

        private Vector2 SetBlockSize(int inputCount)
        {
            Vector2 size = GetComponent<RectTransform>().sizeDelta;

            size.y = blockOriginYSize + inputCount * ySizeOffset;

            //if (!isInputConnected)
            //    size.y = blockOriginYSize + (inputCount - 1) * ySizeOffset;
            //else
            //    size.y = blockOriginYSize + inputCount * ySizeOffset;

            GetComponent<RectTransform>().sizeDelta = size;

            Vector2 selectedSize = selectedGameObject.GetComponent<RectTransform>().sizeDelta;
            selectedSize = size + new Vector2(2f, 2f);
            selectedGameObject.GetComponent<RectTransform>().sizeDelta = selectedSize;

            return size;
        }

        private void SetFirstSocketPos(Vector2 blockSize)
        {
            float yOffset = (blockSize.y - blockOriginYSize) * 0.5f + socketOriginYPos;
            Vector2 newPos = epLeft.anchoredPosition;
            newPos.y = yOffset;

            epLeft.anchoredPosition = newPos;
        }

        private void SetLastSockets(Vector2 yPos)
        {
            int count = InputFieldPrefabCount;
            for (int ix = 0; ix < lastSockets.Length; ix++)
            {
                Vector2 position = lastSockets[ix].localPosition;
                MCNodeInput nodeInput = lastSockets[ix].GetComponent<MCNodeInput>();
                if (nodeInput != null)
                {
                    position.y = yPos.y - count * ySizeOffset - 12f;
                    lastSockets[ix].localPosition = position;
                    continue;
                }
                TMP_InputField inputField = lastSockets[ix].GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    //position.x = 14f;
                    position.x = 0f;
                    position.y = yPos.y - count * ySizeOffset - 12f;
                    lastSockets[ix].localPosition = position;
                    continue;
                }
                MCNodeNext nodeNext = lastSockets[ix].GetComponent<MCNodeNext>();
                if (nodeNext != null)
                {
                    position.y = yPos.y - (count - 1) * ySizeOffset;
                    lastSockets[ix].localPosition = position;
                    continue;
                }

                //Vector2 pos = lastSockets[ix].anchoredPosition;
                //if (!isInputConnected)
                //{
                //    if (ix == 0)
                //        pos.y = yPos.y - (inputFieldPrefabs.Count - 1) * ySizeOffset;
                //    else
                //        pos.y = yPos.y - inputFieldPrefabs.Count * ySizeOffset;
                //}
                //else
                //{
                //    if (ix == 0)
                //        pos.y = yPos.y - inputFieldPrefabs.Count * ySizeOffset;
                //    else
                //        pos.y = yPos.y - (inputFieldPrefabs.Count + 1) * ySizeOffset;
                //}

                //lastSockets[ix].anchoredPosition = pos;
            }
        }

        private void SetNexts()
        {
            nexts = GetComponentsInChildren<MCNodeNext>();
        }

        private void SetInputFields()
        {
            inputFieldPrefabs = GetComponentsInChildren<TMP_InputField>().ToList();
        }

        private void SetMatchElem()
        {
            SetInputFields();
            SetNexts();
        }

        public void RemoveMatchElem(int index)
        {
            DestroyImmediate(inputFieldPrefabs[index].gameObject);
            inputFieldPrefabs.RemoveAt(index);

            DestroyImmediate(nextSocketPrefabs[index].gameObject);
            nextSocketPrefabs.RemoveAt(index);

            ChangedMatchElem();
        }

        public void AddMatchElem()
        {
            GameObject inputValue = Instantiate(inputPrefab, matchElems); //inputfield 생성
            inputFieldPrefabs.Add(inputValue.GetComponent<TMP_InputField>()); //inputfield List에 추가

            GameObject nextSocket = Instantiate(nextSocketPrefab, matchElems); //nextsocket 생성
            nextSocketPrefabs.Add(nextSocket.GetComponent<MCNodeNext>()); //nextSockets List에 추가

            ChangedMatchElem();
        }

        private void ChangedMatchElem()
        {
            SetMatchElem(); //inputfields, nexts 초기화
            SetBlockSizeAndSocketPos(InputFieldPrefabCount); //블록사이즈, 첫번째 소켓 위치 설정
            SetMatchElemsPos(); //MatchElem 위치 설정
            SetLastSockets(epLeft.anchoredPosition); //마지막 줄 위치 설정
        }

        private void SetMatchElemsPos()
        {
            SetInputFieldsPos();
            SetNextSocketsPos();
        }

        private void SetInputFieldsPos()
        {
            for (int i = 0; i < inputFieldPrefabs.Count; i++)
            {
                Vector2 Pos = new Vector2(10f, epLeft.anchoredPosition.y - i * ySizeOffset);
                inputFieldPrefabs[i].GetComponent<RectTransform>().anchoredPosition = Pos;
            }
        }

        private void SetNextSocketsPos()
        {
            for (int i = 0; i < nextSocketPrefabs.Count; i++)
            {
                float xPos = -epLeft.anchoredPosition.x;
                Vector2 Pos = new Vector2(xPos, epLeft.anchoredPosition.y - i * ySizeOffset);
                nextSocketPrefabs[i].GetComponent<RectTransform>().anchoredPosition = Pos;
            }
        }

        private readonly int minCount = 2;
        public override void SetData(Node node)
        {
            base.SetData(node);

            // 필요한 만큼의 입력 추가 생성.
            int instantiateCount = node.nexts.Length - minCount - 1;
            for (int ix = 0; ix < instantiateCount; ++ix)
            {
                AddMatchElem();
            }

            for (int ix = 0; ix < inputFieldPrefabs.Count; ++ix)
            {
                inputFieldPrefabs[ix].text = node.nexts[ix].value;
            }

            if (!isInputConnected)
            {
                lastSockets[2].GetComponent<TMP_InputField>().text = nodeData.inputs[0].default_value;
            }
        }

        private int InputFieldPrefabCount
        {
            get
            {
                return isInputConnected == true ? inputFieldPrefabs.Count + 1 : inputFieldPrefabs.Count;
            }
        }
    }
}