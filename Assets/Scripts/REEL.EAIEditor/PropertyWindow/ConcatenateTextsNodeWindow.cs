using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace REEL.D2EEditor
{
    public class ConcatenateTextsNodeWindow : MCNodeWindow
    {
        [Header("References for Instantiation")]
        public RectTransform content;
        public GameObject elementPrefab;

        private readonly int minCount = 2;
        private readonly int maxCount = 5;
        private readonly float itemHeight = 50f;
        private readonly float defaultHeight = 50f;

        private MCStrcatNode strcatNode;
        private List<ConcatenatesTextsElementBlock> concatElements = new List<ConcatenatesTextsElementBlock>();

        private readonly string valueString = "Value";

        protected override void OnEnable()
        {
            foreach (var element in concatElements)
            {
                element.SetPropertyWindow(this);
            }
        }

        protected override void OnDisable()
        {
            // 편의상 InputField의 모든 리스너 제거.
            RemoveAllListeners();

            // 노드의 원래 리스너는 복구.
            for (int ix = 0; ix < strcatNode.inputFields.Count; ++ix)
            {
                strcatNode.inputFields[ix].onValueChanged.AddListener(
                    strcatNode.GetNodeInputWithIndex(ix).SetAlterData
                );
            }

            strcatNode = null;
            base.OnDisable();
        }

        public override void ShowProperty(MCNode node)
        {
            base.ShowProperty(node);

            strcatNode = node as MCStrcatNode;
            InitializeElements(strcatNode.inputFields.Count);
        }

        private void ClearElements()
        { 
            foreach (var element in concatElements)
            {
                Destroy(element.gameObject);
            }

            concatElements.Clear();
            content.sizeDelta = new Vector2(content.sizeDelta.x, defaultHeight);
        }

        private void InitializeElements(int count)
        {
            ClearElements();
            for (int ix = 0; ix < count; ++ix)
            {
                AddElement();
            }

            SyncInputFields();
            ArrangeElementNames();
        }

        private void AddElement()
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + itemHeight);

            GameObject element = Instantiate(elementPrefab, content);
            ConcatenatesTextsElementBlock newBlock = element.GetComponent<ConcatenatesTextsElementBlock>();
            newBlock.SetPropertyWindow(this);
            concatElements.Add(newBlock);
        }
        
        private void ArrangeElementNames()
        {
            for (int ix = 0; ix < concatElements.Count; ++ix)
            {
                concatElements[ix].SetElementName(valueString + (ix + 1).ToString());
            }
        }

        private void RemoveAllListeners()
        {
            for (int ix = 0; ix < concatElements.Count; ++ix)
            {
                concatElements[ix].inputField.onValueChanged.RemoveAllListeners();
                strcatNode.inputFields[ix].onValueChanged.RemoveAllListeners();
            }
        }

        private void SyncInputFields()
        {
            // 편의상 모든 리스너 삭제.
            RemoveAllListeners();

            for (int ix = 0; ix < concatElements.Count; ix++)
            {
                concatElements[ix].inputField.text = strcatNode.inputFields[ix].text;

                // 삭제한 리스너 복구.
                strcatNode.inputFields[ix].onValueChanged.AddListener(strcatNode.GetNodeInputWithIndex(ix).SetAlterData);

                // Node의 입력에 선이 연결돼 있으면 InputField 입력 안되게 설정.
                if (strcatNode.GetNodeInputWithIndex(ix).HasLine == true)
                {
                    concatElements[ix].inputField.interactable = false;
                }

                // Node의 입력에 선 연결 안되어 있으면 InputField 동기화.
                else
                {
                    SyncTwoInputFields(concatElements[ix].inputField, strcatNode.inputFields[ix]);
                }
            }
        }

        private void SyncTwoInputFields(TMP_InputField left, TMP_InputField right)
        {
            left.onValueChanged.AddListener((message) =>
            {
                right.text = message;
            });

            right.onValueChanged.AddListener((message) =>
            {
                left.text = message;
            });
        }

        public void AddButtonClicked()
        {
            if (concatElements.Count == maxCount)
            {
                return;
            }

            AddElement();
            strcatNode.AddNewConcatElement();
            SyncInputFields();
            ArrangeElementNames();

            refNode.GetComponent<MCNodeDrag>().ExecuteOnChanged();
        }

        public void RemoveButtonClicked(ConcatenatesTextsElementBlock element)
        {
            if (concatElements.Count == minCount)
            {
                return;
            }

            content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y - itemHeight);
            strcatNode.RemoveConcatElement(concatElements.IndexOf(element));
            concatElements.Remove(element);
            Destroy(element.gameObject);

            SyncInputFields();
            ArrangeElementNames();

            refNode.GetComponent<MCNodeDrag>().ExecuteOnChanged();
        }
    }
}