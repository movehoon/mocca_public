using REEL.PROJECT;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

namespace REEL.D2EEditor
{
    public class MCStrcatNode : MCNode
    {
        [Header("Input Socket/Inpupt Field Prefabs")]
        public MCNodeInput inputSocketPrefab;
        public TMP_InputField inputFieldPrefab;

        [Header("Block Assemble Parameters")]
        public float socketOriginYPos = -20f;
        public float blockOriginYSize = 90f;
        public float ySizeOffset = 22.5f;

        public List<TMP_InputField> inputFields = new List<TMP_InputField>();

        private readonly int minInputNumber = 2;
        private RectTransform refRect;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            if (refRect == null)
            {
                refRect = GetComponent<RectTransform>();
            }

            // 블록 초기화.
            InitializeBlock();

            nodeData.body = null;
            nodeData.outputs = null;
        }

        public override void SetData(Node node)
        {
            // 필요한 만큼의 입력 추가.
            int instantiateCount = node.inputs.Length - minInputNumber;
            for (int ix = 0; ix < instantiateCount; ++ix)
            {
                AddNewConcatElement();
            }

            for (int ix = 0; ix < node.inputs.Length; ++ix)
            {
                inputFields[ix].text = node.inputs[ix].default_value;
            }

            base.SetData(node);

            //Debug.Log($"inputFields.Count: {inputFields.Count} / node.inputs.Length: {node.inputs.Length}");

            //firstInput.text = node.inputs[0].default_value;
            //secondInput.text = node.inputs[1].default_value;
        }

        private void InitializeBlock()
        {
            // 입력 최소개수(2개) 만큼 생성.
            for (int ix = 0; ix < minInputNumber; ++ix)
            {
                AddConcatElement();
            }

            // 블록 크기 및 소켓/입력 위치 재설정.
            ArrangeSocketAndInputFieldPositions();
        }

        private void AddConcatElement()
        {
            // 소켓 생성.
            MCNodeInput inputsocket = Instantiate(inputSocketPrefab, refRect);
            inputsocket.input.id = inputs.Length;
            //Debug.Log($"inputsocket.input.id: {inputsocket.input.id}");
            Array.Resize(ref inputs, inputs.Length + 1);
            inputs[inputs.Length - 1] = inputsocket;

            // InputField 생성 및 입력 이벤트 연결.
            TMP_InputField inputField = Instantiate(inputFieldPrefab, refRect);
            inputFields.Add(inputField);
            inputField.onValueChanged.AddListener(inputsocket.SetAlterData);
            inputField.onEndEdit.AddListener(inputsocket.OnSubmitInputfield);

            // AltImage 참조 값 설정.
            inputsocket.altImage = inputField.GetComponent<Image>();


            //노드 이름 지정 - kjh 
            inputsocket.name = "Input_" + inputs.Length.ToString();
            inputField.name = "InputField_" + inputs.Length.ToString();

        }

        // Property Window의 +버튼 클릭되면 실행되는 메소드.
        public void AddNewConcatElement()
        {
            // 소켓/입력 추가.
            AddConcatElement();

            // 위치 재설정.
            ArrangeSocketAndInputFieldPositions();
        }

        // Property Window의 -버튼 클릭되면 실행되는 메소드.
        public void RemoveConcatElement(int index)
        {
            // 소켓 제거.
            List<MCNodeInput> inputList = inputs.ToList();
            Destroy(inputList[index].gameObject);
            inputList.RemoveAt(index);
            inputs = inputList.ToArray();

            // 입력 제거.
            Destroy(inputFields[index].gameObject);
            inputFields.RemoveAt(index);

            // 블록 크기 및 소켓/입력 위치 재설정.
            ArrangeSocketAndInputFieldPositions();
        }

        private Vector2 SetTotalSizeOfBlock()
        {
            // Block 크기 설정.
            Vector2 size = refRect.sizeDelta;
            size.y = blockOriginYSize + (inputs.Length - 1) * ySizeOffset;
            refRect.sizeDelta = size;

            // Select GO 크기 설정.
            RectTransform selectRect = selectedGameObject.GetComponent<RectTransform>();
            Vector2 selectSize = size + new Vector2(2f, 2f);
            selectRect.sizeDelta = selectSize;

            return size;
        }

        private void ArrangeSocketAndInputFieldPositions()
        {
            // 전체 블록크기 설정.
            Vector2 size = SetTotalSizeOfBlock();

            // 첫번째 소켓의 위치 계산.
            float positionOfTheFirstSocket = (size.y - blockOriginYSize) * 0.5f + socketOriginYPos;

            // 개별 소켓/입력 위치 설정.
            for (int ix = 0; ix < inputs.Length; ++ix)
            {
                // Socket Y위치 설정.
                RectTransform socketRect = inputs[ix].GetComponent<RectTransform>();
                Vector2 socketPosition = socketRect.anchoredPosition;
                socketPosition.y = positionOfTheFirstSocket - ySizeOffset * (ix);
                socketRect.anchoredPosition = socketPosition;

                // InputField Y위치 설정.
                RectTransform inputFieldRect = inputFields[ix].GetComponent<RectTransform>();
                Vector2 inputFieldPosition = inputFieldRect.anchoredPosition;
                inputFieldPosition.y = socketPosition.y;
                inputFieldRect.anchoredPosition = inputFieldPosition;
            }
        }
    }
}